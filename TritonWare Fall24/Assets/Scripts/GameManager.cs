using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public static List<Unit> AllUnits = new List<Unit>();
    public static List<ResourcePickup> AllResourcesPickups = new List<ResourcePickup>();

    public ResourceManager SupplyResource;
    public ResourceManager CureResource;

    public float waveInterval = 30f;       // average seconds between waves
    [Range(0f, 1f)] public float waveIntervalVariance = 0.3f;  // randomness multiplier
    private float waveTimer;
    [Range(0f, 1f)] public float waveAggroChance = 0.8f;
    [Range(0f, 1f)] public float rushChance = 0.4f;

    public Doctor DoctorUnit;
    public List<HospitalBed> AvailableBeds;

    public EnemyUnit TurnedZombiePrefab;
    public Soldier SoldierPrefab;
    public Medic MedicPrefab;
    public Scientist ScientistPrefab;

    public float InfectionProgressSpeed = 0.015f;
    [Range(0f, 1f)] public float InfectionTurnChance;
    [Range(0f, 1f)] public float MinInfectionToTurn = 0.3f;
    [SerializeField] private float infectionWaveInterval;
    [Range(0f, 1f)][SerializeField] private float infectionWaveIntervalRandomness = 0.2f;
    private float infectionWaveTimer;

    private float startTime;
    [SerializeField] private float setupTime;
    public bool isSettingUp;

    public float InitialDifficultyScaling = 1f;

    public static float DifficultyScaling;
    public static float ReducedDifficultyScaling => Mathf.Sqrt(DifficultyScaling);
    public float TimeToMaxDifficuly = 300f;
    private float difficultyScaleRate => (MaxDifficultyScaling - 1) / TimeToMaxDifficuly;
    public float MaxDifficultyScaling = 1.5f;

    public List<Unit> AllUnitPrefabs() => new() { SoldierPrefab, MedicPrefab, ScientistPrefab };

    public int PatientsCured = 0;

    public Dictionary<UnitType, List<Sprite>> spriteVariants = new();

    [Header("Sprite Variants")]
    public List<Sprite> doctorSprites;
    public List<Sprite> patientSprites;
    public List<Sprite> scientistSprites;
    public List<Sprite> soldierSprites;
    public List<Sprite> medicSprites;
    public List<Sprite> zombieSprites;

    private void Awake()
    {
        Instance = this;
        DifficultyScaling = InitialDifficultyScaling;
        waveTimer = waveInterval;
        infectionWaveTimer = infectionWaveInterval;
        DoctorUnit = FindObjectOfType<Doctor>();
        AvailableBeds = FindObjectsOfType<HospitalBed>().ToList();
        SupplyResource.Init();
        CureResource.Init();

        startTime = Time.time;
        isSettingUp = true;

        // Setup spriteVariants HashMap
        spriteVariants.Add(UnitType.Doctor, doctorSprites);
        spriteVariants.Add(UnitType.Patient, patientSprites);
        spriteVariants.Add(UnitType.Scientist, scientistSprites);
        spriteVariants.Add(UnitType.Solider, soldierSprites);
        spriteVariants.Add(UnitType.Medic, medicSprites);
        spriteVariants.Add(UnitType.Zombie, zombieSprites);
    }

    private void Update()
    {
        if (Time.time - startTime > setupTime)
        {
            isSettingUp = false;
        }

        if (waveTimer < 0 && !isSettingUp)
        {
            TriggerWave();
            Debug.LogWarning("Wave Incoming!");
            waveTimer = waveInterval * Random.Range(1 - waveIntervalVariance, 1 + waveIntervalVariance);
        }
        else
        {
            waveTimer -= Time.deltaTime;
        }

        if (infectionWaveTimer < 0f && !isSettingUp)
        {
            TriggerInfectionWave();
            Debug.LogWarning("Infection Wave!");
            infectionWaveTimer = infectionWaveInterval * Random.Range(1 - infectionWaveIntervalRandomness, 1 + infectionWaveIntervalRandomness);
        }
        else if (!isSettingUp)
        {
            infectionWaveTimer -= Time.deltaTime;
        }

        if (DifficultyScaling <= MaxDifficultyScaling)
        {
            DifficultyScaling += difficultyScaleRate * Time.deltaTime;
        }


    }

    private void TriggerWave()
    {
        foreach (Unit unit in AllUnits)
        {
            if (unit is EnemyUnit e)
            {
                if (e.CurrentState == EnemyState.Idle && Random.value < waveAggroChance)
                {
                    if (Random.value < rushChance) e.CurrentState = EnemyState.Rush;
                    else e.CurrentState = EnemyState.AttackClosest;
                }
            }
        }
    }

    public void TriggerInfectionWave(float multiplier = 1f)
    {
        multiplier *= ReducedDifficultyScaling;
        List<Unit> units = GetUnitsOfTeam(Team.Allied);
        units.AddRange(GetUnitsOfTeam(Team.Visitor));
        foreach (Unit unit in units)
        {
            unit.Infection?.TryTriggerTurn(multiplier);
        }
    }

    public static List<Unit> GetUnitsOfTeam(Team team)
    {
        List<Unit> units = new List<Unit>();
        foreach (Unit unit in AllUnits)
        {
            if (unit.Team == team)
            {
                units.Add(unit);
            }
        }
        return units;
    }

    public static bool OpposingTeams(Team team1, Team team2)
    {
        if (
            (team1 == Team.Allied && team2 == Team.Enemy) ||
            (team1 == Team.Enemy && team2 == Team.Allied) ||
            (team1 == Team.Visitor && team2 == Team.Enemy) ||
            (team1 == Team.Enemy && team2 == Team.Visitor)
            )
        {
            return true;
        }
        return false;
    }

    private int CalculateScore()
    {
        return (int)((Time.timeSinceLevelLoad + PatientsCured * 40) * InitialDifficultyScaling);
    }

    public void GameOver()
    {
        OverlayManager.Instance.ShowGameOverScreen(CalculateScore(), Time.timeSinceLevelLoad, PatientsCured);
        Time.timeScale = 0;
    }

}
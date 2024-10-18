using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public static List<Unit> AllUnits = new List<Unit>();
    public static List<Resource> AllResources = new List<Resource>();

    private float waveInterval = 30f;       // average seconds between waves
    private float waveIntervalVariance = 0.3f;  // randomness multiplier
    private float waveTimer;
    private float waveAggroChance = 0.8f;
    private float rushChance = 0.4f;

    public Doctor DoctorUnit;
    public List<HospitalBed> AvailableBeds;

    public EnemyUnit TurnedZombiePrefab;
    public Soldier SoldierPrefab;
    public Medic MedicPrefab;
    public Scientist ScientistPrefab;

    public List<Unit> AllUnitPrefabs() => new() { SoldierPrefab, MedicPrefab, ScientistPrefab };

    private void Awake()
    {
        Instance = this;
        waveTimer = waveInterval;
        DoctorUnit = FindObjectOfType<Doctor>();
        AvailableBeds = FindObjectsOfType<HospitalBed>().ToList();
    }

    private void Update()
    {
        if (waveTimer < 0)
        {
            TriggerWave();
            Debug.LogWarning("Wave Incoming!");
            waveTimer = waveInterval * Random.Range(1 - waveIntervalVariance, 1 + waveIntervalVariance);
        }
        else
        {
            waveTimer -= Time.deltaTime;
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


}
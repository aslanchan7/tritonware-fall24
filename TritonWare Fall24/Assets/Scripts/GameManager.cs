using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public static List<Unit> AllUnits = new List<Unit>();
    private float waveInterval = 30f;       // average seconds between waves
    private float waveIntervalVariance = 0.3f;  // randomness multiplier
    private float waveTimer;
    private float waveAggroChance = 0.8f;

    private void Awake()
    {
        Instance = this;
        waveTimer = waveInterval;
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
                    e.ChangeState(EnemyState.AttackClosest);
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
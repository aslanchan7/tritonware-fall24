using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Infection
{
    public Unit InfectedUnit;
    public float Progress = 0f;

    public Infection(Unit infectedUnit, float progress = 0)
    {
        InfectedUnit = infectedUnit;
        Progress = progress;
    }

    public void IncreaseInfection(float value)
    {
        Progress += value;
        if (Progress >= 1f) TriggerTurn();
        InfectedUnit.UnitDisplay.UpdateDisplay();
    }

    public void TriggerTurn()
    {
        InfectedUnit.Infection = null;
        if (InfectedUnit.InsideStructure != null)
        {
            OverlayManager.Instance.CreateTargetIndicator(InfectedUnit.InsideStructure.Pos, TargetIndicatorType.ZombieTurn);
        }
        else
        {
            OverlayManager.Instance.CreateTargetIndicator(InfectedUnit.Pos, TargetIndicatorType.ZombieTurn);
        }

        InfectedUnit.TurnIntoUnit(GameManager.Instance.TurnedZombiePrefab);


    }

    public void RemoveInfection()
    {
        InfectedUnit.Infection = null;
        InfectedUnit.UnitDisplay.UpdateDisplay();
        InfectedUnit = null;

    }

    public void TryTriggerTurn(float multiplier = 1f)
    {
        float rand = Random.Range(0f, 1f);
        float adjustedInfectionFactor = (Progress - GameManager.Instance.MinInfectionToTurn) / (1 - GameManager.Instance.MinInfectionToTurn);
        float chanceToTurn = GameManager.Instance.InfectionTurnChance * adjustedInfectionFactor * multiplier;

        if (rand < chanceToTurn)
        {
            TriggerTurn();
        }
    }
}
using System;
using UnityEngine;

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
        InfectedUnit.TurnIntoUnit(GameManager.Instance.TurnedZombiePrefab);
    }
}
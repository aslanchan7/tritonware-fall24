using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitDisplay : MonoBehaviour
{
    [HideInInspector] public Unit Unit;
    public ProgressBar HealthBar;
    public ProgressBar InfectionBar;
    public Color[] HealthColors = new Color[5];

    private void Awake()
    {
        Unit attachedUnit = GetComponentInParent<Unit>();
        if (attachedUnit != null)
        {
            Unit = attachedUnit;
        }
        else
        {
            Debug.LogWarning("UnitDisplay is not attached to an object of type Unit");
        }
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {

        int HealthPercent = (int)(Mathf.Clamp01((float)Unit.Health / Unit.MaxHealth) * 100);
        int color = (5 - HealthPercent / 20) > 4 ? 4 : (5 - HealthPercent / 20);
        // print(Health + " " + color + " " + colors[color]);
        Debug.Log(HealthPercent);
        HealthBar.SetColor(HealthColors[color]);
        if (Unit.Health == Unit.MaxHealth)
        {
            HealthBar.gameObject.SetActive(false);
        }
        else
        {
            HealthBar.gameObject.SetActive(true);
            HealthBar.SetProgress((float)HealthPercent / 100f);
        }
        if (Unit.Infection != null)
        {
            InfectionBar.gameObject.SetActive(true);
            InfectionBar.SetProgress(Unit.Infection.Progress);
        }
        else
        {
            InfectionBar.gameObject.SetActive(false);
        }
    }

}

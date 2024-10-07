using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitDisplay : MonoBehaviour
{
    public TMP_Text healthText; // temporary text display for testing
    public Unit Unit;
    public void UpdateDisplay()
    {
        healthText.text = Unit.Health.ToString();
    }
    
}

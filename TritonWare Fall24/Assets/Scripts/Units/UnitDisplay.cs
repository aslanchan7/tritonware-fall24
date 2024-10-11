using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitDisplay : MonoBehaviour
{
    [HideInInspector] public Unit Unit;
    public RectTransform HealthForeground;
    public SpriteRenderer HealthSprite;
    public Color[] colors = new Color[5];

    private void Awake()
    {
        Unit attachedUnit = GetComponentInParent<Unit>();
        Debug.Log(attachedUnit);
        if (attachedUnit != null)
        {
            Unit = attachedUnit;
        }
        else
        {
            Debug.LogWarning("UnitDisplay is not attached to an object of type Unit");
        }

        gameObject.SetActive(false);
    }

    public void UpdateDisplay()
    {
        int Health = Unit.Health >= 0 ? Unit.Health : 0;
        int color = (4 - Health / 20) > 4 ? 4 : (4 - Health / 20);
        print(Health + " " + color + " " + colors[color]);


        HealthSprite.color = colors[color];
        if (Unit.Health == 100)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            HealthForeground.localScale = new Vector3(Health / 100f, 1f, 1f);
        }
    }

}

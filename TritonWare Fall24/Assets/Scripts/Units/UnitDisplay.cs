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
        int HealthPercent = (int)(Mathf.Clamp01((float)Unit.Health / Unit.MaxHealth) * 100);
        int color = (4 - HealthPercent / 20) > 4 ? 4 : (4 - HealthPercent / 20);
        // print(Health + " " + color + " " + colors[color]);

        if (HealthSprite != null)
        {


            HealthSprite.color = colors[color];
            if (Unit.Health == Unit.MaxHealth)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                HealthForeground.localScale = new Vector3(HealthPercent / 100f, 1f, 1f);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VisionController : MonoBehaviour
{
    public SpriteRenderer fog;

    
    public static VisionController Instance;
    public bool VisionEnabled = false;
    public float FogAlpha = 0.95f;

    private void Awake()
    {
        Instance = this;
        fog.sharedMaterial.color = Color.black.WithAlpha(FogAlpha);
    }

    private void Update()
    {
        // deselect units that are out of vision range
        if (!VisionEnabled)
        {
            for (int i = UnitController.Instance.SelectedUnits.Count - 1; i >= 0; i--)
            {
                Unit unit = UnitController.Instance.SelectedUnits[i];
                if (!IsVisible(unit))
                {
                    UnitController.Instance.DeselectUnit(unit);
                }
            }
        }
    }

    public bool IsVisible(Unit unit)
    {
        return VisionEnabled || GameManager.Instance.DoctorUnit.CanSee(unit);
    }

    public void Start()
    {
        ToggleVision(false);
    }

    public void ToggleVision(bool toggle)
    {
        VisionEnabled = toggle;
        SceneViewMask.Instance.gameObject.SetActive(toggle);
        fog.gameObject.SetActive(!toggle);
    }
}

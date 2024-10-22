using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackoutController : MonoBehaviour
{
    public static BlackoutController Instance;
    public List<BlackoutFusebox> Fuseboxes = new();
    public Workstation ControlStation;
    public Renderer Fog;

    private bool blackoutActive = false;
    public float BlackoutInterval = 20f;
    public float BlackoutIntervalVariance = 0.5f;
    private float blackoutTimer;

    private void Awake()
    {
        Instance = this;
        blackoutTimer = 30 + BlackoutInterval * Random.Range(1 - BlackoutIntervalVariance, 1 + BlackoutIntervalVariance);   
        //blackoutTimer = 5f;

        foreach (var fusebox in Fuseboxes)
        {
            fusebox.SetState(false);
        }

    }

    private void Update()
    {
        if (blackoutTimer <= 0)
        {
            StartBlackout();
            blackoutTimer = BlackoutInterval * Random.Range(1 - BlackoutIntervalVariance, 1 + BlackoutIntervalVariance);   // todo add randomness
        }
        else if (!blackoutActive) 
        {
            blackoutTimer -= Time.deltaTime;
        }
    }

    public void StartBlackout()
    {
        blackoutActive = true;
        StartCoroutine(BlackoutCoroutine());
    }

    private IEnumerator BlackoutCoroutine()
    {
        bool startWithVision = VisionController.Instance.VisionEnabled;
        ControlStation.ToggleEnabled(false);
        if (startWithVision)
        {
            // flicker
            Fog.sharedMaterial.color = new(0,0,0,0.6f);
            yield return new WaitForSeconds(0.1f);
            VisionController.Instance.ToggleVision(true);
            yield return new WaitForSeconds(0.15f);
            VisionController.Instance.ToggleVision(false);
            yield return new WaitForSeconds(0.07f);
            VisionController.Instance.ToggleVision(true);
            yield return new WaitForSeconds(0.6f);
        }
        Fog.sharedMaterial.color = new(0, 0, 0, 0.995f);
        VisionController.Instance.ToggleVision(false);

        GameManager.Instance.TriggerInfectionWave(1.3f);
        BlackoutFusebox chosenFusebox = Fuseboxes.RandomElement();
        chosenFusebox.SetState(true);

        yield return null;
    }

    public void FixBlackout()
    {
        foreach (var fusebox in Fuseboxes)
        {
            fusebox.SetState(false);
        }
        ControlStation.ToggleEnabled(true);
        blackoutActive = false;
        Fog.sharedMaterial.color = new(0, 0, 0, VisionController.Instance.FogAlpha);
    }

}

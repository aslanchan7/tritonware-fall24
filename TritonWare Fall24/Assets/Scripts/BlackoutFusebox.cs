using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackoutFusebox : MonoBehaviour
{
    public Workstation Workstation;
    public MeshRenderer MeshRenderer;

    public void SetState(bool state)
    {
        Workstation.ToggleEnabled(state);
        MeshRenderer.enabled = state;
    }
}

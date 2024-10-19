using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int maxResourceValue = 20;
    public int ResourceValue;
    public Transform ResourceBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    void changeResourceLevel(int changeBy) {
        ResourceValue += changeBy;
        ResourceValue = Math.Clamp(ResourceValue, 0, maxResourceValue);
        ResourceBar.localScale = new Vector3((float)ResourceValue / maxResourceValue, 1, 1);
    }

    void setResourceLevel(int level) {
        ResourceValue = level;
        ResourceValue = Math.Clamp(ResourceValue, 0, maxResourceValue);
        ResourceBar.localScale = new Vector3((float)ResourceValue / maxResourceValue, 1, 1);
    }
}

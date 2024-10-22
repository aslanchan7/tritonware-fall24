using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int maxResourceValue = 20;
    public int ResourceValue;
    public TextMeshProUGUI ResourceAmountText;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void changeResourceLevel(int changeBy)
    {
        ResourceValue += changeBy;
        ResourceValue = Math.Clamp(ResourceValue, 0, maxResourceValue);
        ResourceAmountText.text = ResourceValue.ToString();
        // ResourceBar.localScale = new Vector3((float)ResourceValue / maxResourceValue, 1, 1);
    }

    public void setResourceLevel(int level)
    {
        ResourceValue = level;
        ResourceValue = Math.Clamp(ResourceValue, 0, maxResourceValue);
        ResourceAmountText.text = ResourceValue.ToString();
        // ResourceBar.localScale = new Vector3((float)ResourceValue / maxResourceValue, 1, 1);
    }

    public void Init()
    {
        Debug.Log(gameObject);
        ResourceAmountText.text = ResourceValue.ToString();
        // ResourceBar.localScale = new Vector3((float)ResourceValue / maxResourceValue, 1, 1);
    }
}

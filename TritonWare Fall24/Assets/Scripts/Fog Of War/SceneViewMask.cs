using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneViewMask : MonoBehaviour
{
    [SerializeField] GameObject fog;

    void Update()
    {
        // This game object needs to be active in order to be able to see the units.
        if (fog.activeSelf && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else if (!fog.activeSelf && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    void ToggleEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }
}

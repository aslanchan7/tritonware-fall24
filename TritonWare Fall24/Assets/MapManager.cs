using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap Tilemap;
    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}

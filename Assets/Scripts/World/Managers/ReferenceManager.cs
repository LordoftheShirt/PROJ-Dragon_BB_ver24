using StarterAssets;
using System;
using UnityEngine;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public MiniMapManager mapManager;
    public DayNightManager dayNightManager;
    public TerrainGenerator terrainGenerator;

    public GameObject player;

    public GameManager gameManager;
    public PlayerMeter playerMeter;
    public GameObject cabbin;

    public ResourceCluster resourceClusterPrefab;
    public ResourceNode[] resourceNodePrefabs;

    /* Singleton script does this automatically
    public static ReferenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
}


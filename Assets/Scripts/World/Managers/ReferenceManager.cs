using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public MiniMapManager mapManager;
    public DayNightManager dayNightManager;
    public PlayerResourceManager playerResourceManager;
    public PlayerMeter playerMeter;
    public GameObject cabbin;
    public ResourceCluster resourceClusterPrefab;
    public ResourceNode[] resourceNodePrefab;
    public ResourceType[] resourceType;

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
}


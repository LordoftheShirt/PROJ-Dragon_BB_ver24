using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public MiniMapManager mapManager;
    public GameObject cabbin;

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

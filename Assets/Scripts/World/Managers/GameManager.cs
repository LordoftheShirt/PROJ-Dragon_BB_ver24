using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ReferenceManager refManager;

    private void Awake()
    {
        refManager = ReferenceManager.Instance;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 90;
    } 
}

public enum ResourceTypeEnum { wood, scrap }
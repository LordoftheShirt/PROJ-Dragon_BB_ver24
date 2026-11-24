using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 90;
    }

    private void Start()
    {
        ResetWorld();
    }

    public void ResetPlayer()
    {
        GameObject cab = ReferenceManager.Instance.cabbin;
        ReferenceManager.Instance.playerMeter.ResetMeter();
        ReferenceManager.Instance.player.GetComponent<CharacterController>().enabled = false;
        ReferenceManager.Instance.player.gameObject.transform.position = new Vector3(cab.transform.position.x, cab.transform.position.y + cab.transform.lossyScale.y, cab.transform.position.z);
        ReferenceManager.Instance.player.GetComponent<CharacterController>().enabled = true;

        BuildingPlacer.Instance?.ResetBuildings();
    }

    public void ResetResources()
    {
        Inventory.Instance.SetResource(ResourceType.Scrap, 0);
        Inventory.Instance.SetResource(ResourceType.Wood, 0);
        Inventory.Instance.SetResource(ResourceType.Fabric, 0);
        Inventory.Instance.SetResource(ResourceType.Magnet, 0);
        Inventory.Instance.SetResource(ResourceType.Crystal, 0);
        Inventory.Instance.SetResource(ResourceType.Tech, 0);
    }

    public void ResetWorld()
    {
        // Reset PCGeneration from here.
        ReferenceManager.Instance.terrainGenerator.NewSeed();
        ReferenceManager.Instance.terrainGenerator.CreateNewTerrain();
        ReferenceManager.Instance.dayNightManager.Newday();

        StartCoroutine(CreateMapLate());
        //RESET PLAYER
        ReferenceManager.Instance.gameManager.ResetPlayer();
    }

    IEnumerator CreateMapLate()
    {
        yield return new WaitForSeconds(0.2f);
        ReferenceManager.Instance.mapManager.CreateMap();
    }

}
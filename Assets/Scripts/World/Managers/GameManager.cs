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

    public void ResetTemporaryResources()
    {
        Inventory.Instance.RemoveAllTemporaryResources();
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
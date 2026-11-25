using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniMapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cabbin;
    [SerializeField] private Transform playerMap;
    [Header("Prefabs")]
    [SerializeField] private GameObject cabbinTokenPrefab;
    [SerializeField] private GameObject[] mapObjectTokenPrefabs;
    [Header("Settings")]
    [Tooltip("The scale factor for the minimap. If its a larger value, the minimap will be more zoomed out. Ex: 5 means a map in 1:5 scale")]
    [SerializeField] private float scale = 110;
    [SerializeField] private float xRotation = 180;
    [SerializeField] private bool isUnlocked = false;


    private float size = 0.9f;

    private bool isOpen;
    private List<GameObject> mapObjects = new List<GameObject>();

    public void CreateMap()
    {
        ViewMap(false);

        ClearMapOfObjects();
        Instantiate(cabbinTokenPrefab, playerMap.position, playerMap.rotation, playerMap);


        foreach (GameObject mapObject in mapObjects)
        {
            Vector3 mapObjectPosition = new Vector3(CalculateObjectCordinate(playerMap.position.x, cabbin.position.x, mapObject.transform.position.x), playerMap.position.y + 0.1f, CalculateObjectCordinate(playerMap.position.z, cabbin.position.z, mapObject.transform.position.z));
            //Later implement instantiation for POIs.
            ResourceNode node = mapObject.GetComponent<ResourceNode>();
            //Debug.Log((int)node.GetResourceType());
            if(Vector3.Distance(playerMap.position, mapObjectPosition) < size)
                Instantiate(mapObjectTokenPrefabs[(int)node.GetResourceType()], mapObjectPosition, playerMap.rotation, playerMap);
        }
        ViewMap(false);
        mapObjects.Clear();
    }

    public void ActiveteOrDeactivateMap(InputAction.CallbackContext context)
    {
        if (!isUnlocked) return;
        Debug.Log("presed map");
        if (isOpen)
        {
            isOpen = false;
            ViewMap(isOpen);          
        }
        else
        {
            isOpen = true;
            ViewMap(isOpen);
        }
    }

    private void ViewMap(bool showMap)
    {
        if (showMap)
        {
            playerMap.localRotation = Quaternion.Euler(90, xRotation, playerMap.rotation.z);
            playerMap.gameObject.SetActive(showMap);
        }
        else
        {
            playerMap.gameObject.SetActive(showMap);
            playerMap.localRotation = Quaternion.Euler(0, 0, playerMap.rotation.z);
        }
    }

    public void AddMapObject(GameObject mapObject) { if (!mapObjects.Contains(mapObject)) mapObjects.Add(mapObject); }

    public void RemoveMapObject(GameObject mapObject)
    {
        if (mapObjects.Contains(mapObject)) { mapObjects.Remove(mapObject); }

        foreach (Transform child in playerMap)
        {
            if (child.gameObject == mapObject)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    private void ClearMapOfObjects() { foreach (Transform child in playerMap) Destroy(child.gameObject); }

    private float CalculateObjectCordinate(float mapCordinate, float cabbinCordinate, float objectCordinate) { return mapCordinate - ((cabbinCordinate - objectCordinate) / scale); }

    public void SetMapScale(float newScale) { scale = newScale; }

    public void UnlockMap() { isUnlocked = true; }

}

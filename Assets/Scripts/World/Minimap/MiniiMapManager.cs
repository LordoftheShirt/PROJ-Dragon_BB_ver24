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
    [SerializeField] private GameObject mapObjectTokenPrefab;
    [Header("Settings")]
    [Tooltip("The scale factor for the minimap. If its a larger value, the minimap will be more zoomed out. Ex: 5 means a map in 1:5 scale")]
    [SerializeField] private float scale = 5;
    [SerializeField] private float size = 5;
    [SerializeField] private float xRotation = 180;

    private List<GameObject> mapObjects = new List<GameObject>();
    private bool activated;

    public void CreateMap()
    {
        ViewMap(false);

        ClearMapOfObjects();
        Instantiate(cabbinTokenPrefab, playerMap.position, playerMap.rotation, playerMap);


        foreach (GameObject mapObject in mapObjects)
        {
            //Debug.Log("Creates: " + mapObject);
            Vector3 mapObjectPosition = new Vector3(CalculateObjectCordinate(playerMap.position.x, cabbin.position.x, mapObject.transform.position.x), playerMap.position.y + 0.1f, CalculateObjectCordinate(playerMap.position.z, cabbin.position.z, mapObject.transform.position.z));
            if(Vector3.Distance(playerMap.position, mapObjectPosition) < size)            
                Instantiate(mapObjectTokenPrefab, mapObjectPosition, playerMap.rotation, playerMap);                
        }

        mapObjects.Clear();
    }

    public void ActiveteOrDeactivateMap(InputAction.CallbackContext context)
    {
        Debug.Log("presed map");
        if (activated)
        {
            activated = false;
            ViewMap(activated);          
        }
        else
        {
            activated = true;
            ViewMap(activated);
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

}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    private List<GameObject> mapObjects = new List<GameObject>();

    public void AddMapObject(GameObject mapObject)
    {
        if (!mapObjects.Contains(mapObject)) { mapObjects.Add(mapObject); }        
    }

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

    private void ClearMapObjects()
    {
        //mapObjects.Clear();
        foreach (Transform child in playerMap) { Destroy(child.gameObject); }
    }

    public void CreateMap()
    {
        ClearMapObjects();
        Instantiate(cabbinTokenPrefab, playerMap.position, playerMap.rotation, playerMap);


        foreach (GameObject mapObject in mapObjects)
        {
            Debug.Log("Creates: " + mapObject);
            Vector3 mapObjectPosition = new Vector3(CalculateObjectCordinate(playerMap.position.x, cabbin.position.x, mapObject.transform.position.x), CalculateObjectCordinate(playerMap.position.y, cabbin.position.z, mapObject.transform.position.z), playerMap.localPosition.z);
            if(Vector3.Distance(playerMap.position, mapObjectPosition) < size)            
                Instantiate(mapObjectTokenPrefab, mapObjectPosition, playerMap.rotation, playerMap);                
        }

        mapObjects.Clear();
    }

    private float CalculateObjectCordinate(float mapLocalCordinate, float cabbinCordinate, float objectCordinate)
    {
        return mapLocalCordinate - ((cabbinCordinate - objectCordinate) / scale);
    }

}

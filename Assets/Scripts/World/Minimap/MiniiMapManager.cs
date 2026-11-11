using System.Collections.Generic;
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

    private List<GameObject> mapObjects = new List<GameObject>();
    private List<GameObject> placedMapObjects = new List<GameObject>();


    public void AddMapObject(GameObject mapObject)
    {
        if (!mapObjects.Contains(mapObject))        
            mapObjects.Add(mapObject);        
    }

    public void RemoveMapObject(GameObject mapObject)
    {
        if (mapObjects.Contains(mapObject))        
            mapObjects.Remove(mapObject);        
    }

    public void ClearMapObjects()
    {
        mapObjects.Clear();
        foreach (GameObject placedMapObject in placedMapObjects)        
            Destroy(placedMapObject);        
    }

    public void CreateMap()
    {
        placedMapObjects.Add(Instantiate(cabbinTokenPrefab, playerMap.localPosition, playerMap.rotation, playerMap));

        foreach (GameObject mapObject in mapObjects)
        {
            Vector3 mapObjectPosition = new Vector3(CalculateObjectCordinate(playerMap.localPosition.x, cabbin.position.x, mapObject.transform.position.x), CalculateObjectCordinate(playerMap.localPosition.y, cabbin.position.z, mapObject.transform.position.z), playerMap.localPosition.z);
            placedMapObjects.Add(Instantiate(mapObjectTokenPrefab, mapObjectPosition, playerMap.rotation, playerMap));
        }
    }

    private float CalculateObjectCordinate(float mapLocalCordinate, float cabbinCordinate, float objectCordinate)
    {
        return mapLocalCordinate - ((cabbinCordinate - objectCordinate) / scale);
    }

}

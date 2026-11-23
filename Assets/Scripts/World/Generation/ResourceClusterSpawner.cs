using System.Collections;
using UnityEngine;

public class ResourceClusterSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 30f;

    public void SpawnResourceClusterWithSpecificResource(ResourceType[] resourcesToSpawn, Vector3 centerPosition)
    {
        foreach(ResourceType type in resourcesToSpawn)
        {
            SpawnAResourceCluster(centerPosition, type);
        }
    }

    //Old way to spawn resourses 
    //public void SpawnResourceClusters(Vector3 centerPosition)
    //{
    //    for (int i = 0; i < numberOfClusters; i++)
    //    {
    //        currentResourceType = Random.Range(0, resourceType.Length);
    //        SpawnAResourceCluster(centerPosition, resourceType[currentResourceType]);
    //    }
    //}

    private void SpawnAResourceCluster(Vector3 centerPosition, ResourceType resourceType)
    { 
        //Selects position
        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;

        //Sets the spawn position
        Vector3 spawnPosition = centerPosition + new Vector3(randomPosition.x, 0f, randomPosition.y);
        ResourceCluster resourceCluster = Instantiate(ReferenceManager.Instance.resourceClusterPrefab, spawnPosition, Quaternion.identity, transform);

        resourceCluster.SpawnResources(ReferenceManager.Instance.resourceNodePrefabs[(int)resourceType], resourceType);
    }

    public void DestroyCildren()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
    }

}
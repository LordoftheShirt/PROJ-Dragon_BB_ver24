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

    private void SpawnAResourceCluster(Vector3 centerPosition, ResourceType resourceType)
    { 
        //Selects position
        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;

        //Sets the spawn position
        Vector3 spawnPosition = centerPosition + new Vector3(randomPosition.x, 0f, randomPosition.y);
        ResourceCluster resourceCluster = Instantiate(ReferenceManager.Instance.resourceClusterPrefab, spawnPosition, Quaternion.identity, transform);

        resourceCluster.SpawnResources(ReferenceManager.Instance.resourceNodePrefabs[(int)resourceType], resourceType);
    }
}
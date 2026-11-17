using System.Collections;
using UnityEngine;

public class ResourceClusterSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float centerOffset = 0f;
    [SerializeField] private int numberOfClusters = 5;
    [SerializeField] private ResourceCluster resourceClusterPrefab;
    [SerializeField] private ResourceNode[] resourceNodePrefab;
    [SerializeField] private ResourceType[] resourceType;

    private int currentResourceType;

    private void Start()
    {
        ReferenceManager refe = ReferenceManager.Instance;
        DefineClusterSpawner(30, 5, refe.resourceClusterPrefab, refe.resourceNodePrefab, refe.resourceType);
        SpawnResourceClusters(transform.position);
    }

    public void SpawnResourceClusters(Vector3 centerPosition)
    {
        for (int i = 0; i < numberOfClusters; i++)
        {
            currentResourceType = Random.Range(0, resourceType.Length);
            SpawnAResourceCluster(centerPosition, resourceType[currentResourceType]);
        }
    }

    private void SpawnAResourceCluster(Vector3 centerPosition, ResourceType resourceType)
    { 
        //Selects position
        Vector2 randomPosition = Random.insideUnitCircle * (spawnRadius + centerOffset);

        //Make a method to remove resources that are too close

        //Sets the spawn position with offset
        Vector3 spawnPosition = centerPosition + new Vector3(randomPosition.x, 0f, randomPosition.y);
        ResourceCluster resourceCluster = Instantiate(resourceClusterPrefab, spawnPosition, Quaternion.identity);

        resourceCluster.transform.SetParent(transform);

        resourceCluster.SpawnResources(resourceNodePrefab[currentResourceType], resourceType);
    }

    public void DefineClusterSpawner(float newSpawnRadius, int newNumberOfclusters, ResourceCluster newResourceClusterPrefab, ResourceNode[] newResourceNodePrefabsInOrder, ResourceType[] newResourceTypes)
    {
        spawnRadius = newSpawnRadius;
        numberOfClusters = newNumberOfclusters;
        resourceClusterPrefab = newResourceClusterPrefab;
        resourceNodePrefab = newResourceNodePrefabsInOrder;
        resourceType = newResourceTypes;
    }

    public void DestroyCildren()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
    }

}
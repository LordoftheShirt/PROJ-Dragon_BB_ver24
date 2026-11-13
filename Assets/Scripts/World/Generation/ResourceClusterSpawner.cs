using Unity.Hierarchy;
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
        SpawnResourceClusters(ReferenceManager.Instance.cabbin.transform.position);
    }

    public void SpawnResourceClusters(Vector3 centerPosition)
    {
        for (int i = 0; i < numberOfClusters; i++)
        {
            currentResourceType = Random.Range(0, rnum.Length);
            SpawnAResourceCluster(centerPosition, rnum[currentResourceType]);
        }
    }

    public void SpawnAResourceCluster(Vector3 centerPosition, ResourceType resourceType)
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

    public void DestroyCildren()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
    }

}
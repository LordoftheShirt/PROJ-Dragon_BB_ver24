using UnityEngine;

public class ResourceCluster : MonoBehaviour
{
    [SerializeField] private float clusterRadius = 5f;
    [SerializeField] private int maxResourceCount = 10;
    [SerializeField] private int minResourceCount = 5;
    [SerializeField] private int maxResourceAmount = 100;

    public void SpawnResources(ResourceNode resourcePrefab, ResourceType resourceType)

    {
        for (int i = 0; i < Random.Range(minResourceCount, maxResourceCount + 1); i++)
        {
            if (maxResourceAmount <= 0)
                break;

            //Selects position
            Vector2 randomPosition = Random.insideUnitCircle * clusterRadius;

            //Se till så att y matchar heightmappen
            //Sets the spawn position in relation to the cluster's position
            Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, 0f, randomPosition.y);

            // Se till så att rotationen matchar normalen i heightmappen, ev gör metod som ser till att normalen är realistisk
            ResourceNode resource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            resource.transform.SetParent(transform);

            int addedResourceAmount = Random.Range(1, maxResourceAmount);   
            resource.SetResourceType(resourceType, addedResourceAmount);
            maxResourceAmount -= addedResourceAmount;
        }
    }

    private void OnDestroy()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
    }
}


using Mono.Cecil;
using UnityEngine;

public class ResourceCluster : MonoBehaviour
{
    [SerializeField] private float clusterRadius = 5f;

    private int maxResourceAmount;

    public void SpawnResources(ResourceNode resourcePrefab, ResourceType resourceType)
    {
        Vector3 spawnPosition;
        float spawnXRotation;
        float spawnZRotation;
        maxResourceAmount = 0;

        do
        {
            spawnXRotation = spawnZRotation = 0;

            //Selects position
            Vector2 randomPosition = Random.insideUnitCircle * clusterRadius;

            spawnPosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);
            
            if (Physics.Raycast(new Vector3(spawnPosition.x, 50, spawnPosition.z), Vector3.down, out RaycastHit hitInfo, 150))
            {
                float dist = Vector3.Distance(hitInfo.point, spawnPosition);
                if (hitInfo.point.y <= spawnPosition.y)
                    spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y - dist, spawnPosition.z);
                else
                    spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + dist, spawnPosition.z);

                spawnXRotation = Vector3.SignedAngle(Vector3.down, hitInfo.normal, Vector3.forward);
                spawnZRotation = Vector3.SignedAngle(Vector3.down, hitInfo.normal, Vector3.right);
            }

            // Se till så att rotationen matchar normalen i heightmappen, ev gör metod som ser till att normalen är realistisk
            ResourceNode resource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity, transform);
            resource.transform.rotation = Quaternion.Euler(spawnXRotation, 0, spawnZRotation);

            if (maxResourceAmount == 0)
                maxResourceAmount = resource.GetMaxResourceAmountPerCluster();
            int amount = Random.Range(resource.GetResourceMin(), resource.GetResourceMax());
            maxResourceAmount -= amount;
            SetResourceStats(resource, resourceType, amount);
        }
        while (maxResourceAmount >= 0);

    }


    private void SetResourceStats(ResourceNode resource, ResourceType resourceType, int addedResourceAmount)
    {
        resource.SetResourceType(resourceType, addedResourceAmount);
        maxResourceAmount -= addedResourceAmount;
    }
}


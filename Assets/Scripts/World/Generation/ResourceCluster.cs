using UnityEngine;

public class ResourceCluster : MonoBehaviour
{
    [SerializeField] private float clusterRadius = 5f;
    [SerializeField] private int maxResourceCount = 10;
    [SerializeField] private int minResourceCount = 5;
    [SerializeField] private int maxResourceAmount = 30;

    public void SpawnResources(ResourceNode resourcePrefab, ResourceType resourceType)
    {
        for (int i = 0; i < Random.Range(minResourceCount, maxResourceCount + 1); i++)
        {
            if (maxResourceAmount <= 0)
                break;

            Vector3 spawnPosition;
            float spawnXRotation = 0;
            float spawnZRotation = 0;



            //Selects position
            Vector2 randomPosition = Random.insideUnitCircle * clusterRadius;

            spawnPosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);

            if (Physics.Raycast(new Vector3(spawnPosition.x, 30, spawnPosition.z), Vector3.down, out RaycastHit hitInfo, 100))
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
            ResourceNode resource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);
            resource.transform.rotation = Quaternion.Euler(spawnXRotation, 0, spawnZRotation);
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


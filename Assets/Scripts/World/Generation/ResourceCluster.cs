//using Mono.Cecil;
//using UnityEditor.PackageManager;
using UnityEngine;

public class ResourceCluster : MonoBehaviour
{
    [SerializeField] private float clusterRadius = 5f;

    private int maxResourceAmount;

    public void SpawnResources(ResourceNode resourcePrefab, ResourceType resourceType)
    {
        Vector3 spawnPosition;
        //float spawnXRotation;
        //float spawnZRotation;
        maxResourceAmount = -1;

        do
        {
            //spawnXRotation = spawnZRotation = 0;

            //Selects position
            Vector2 randomPosition = Random.insideUnitCircle * clusterRadius;

            spawnPosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);
            
            if (Physics.Raycast(new Vector3(spawnPosition.x, 100, spawnPosition.z), Vector3.down, out RaycastHit hitInfo, 200))
            {
                if (hitInfo.collider.tag == "ResourceSpawnIgnore")
                    break;

                float dist = Vector3.Distance(hitInfo.point, spawnPosition);

                if (hitInfo.point.y <= spawnPosition.y)
                    spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y - dist, spawnPosition.z);
                else
                    spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + dist, spawnPosition.z);

                //spawnXRotation = Vector3.SignedAngle(Vector3.down, hitInfo.normal, Vector3.forward);
                //spawnZRotation = Vector3.SignedAngle(Vector3.down, hitInfo.normal, Vector3.right);
            }

            // Se till så att rotationen matchar normalen i heightmappen, ev gör metod som ser till att normalen är realistisk
            ResourceNode resource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity, transform);

            //resource.transform.rotation = Quaternion.Euler(spawnXRotation, 0, spawnZRotation);
            // EDIT: byte ut med den ovan med den under; ger varje grej en random riktning.
            resource.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            if (maxResourceAmount == -1)
                maxResourceAmount = resource.GetMaxResourceAmountPerCluster();

            SetResourceStats(resource, resourceType, Random.Range(resource.GetResourceMin(), resource.GetResourceMax() + 1));
        }
        while (maxResourceAmount >= 0);

    }


    private void SetResourceStats(ResourceNode resource, ResourceType resourceType, int addedResourceAmount)
    {
        resource.SetResourceType(resourceType, addedResourceAmount);

        maxResourceAmount -= addedResourceAmount;
    }
}


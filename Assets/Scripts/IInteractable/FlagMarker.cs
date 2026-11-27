using UnityEngine;

public class FlagMarker : MonoBehaviour, IInteractable
{
    private static int TIME_OFFSET_PROP = Shader.PropertyToID("_TimeOffset");

    [SerializeField] private Renderer flagPlane;

    private void Awake()
    {
        flagPlane.material.SetFloat(TIME_OFFSET_PROP, Random.Range(0, 60));
    }

    public void Interact()
    {
        if (ConsumablesManager.Instance.AddFlag())
        {
            // Successfully picked up flag
            Destroy(gameObject);
        }
    }

    public string GetDescription()
    {
        return "Recollect Flag";
    }
}

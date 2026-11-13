using UnityEngine;

public class WarmthEmitter : MonoBehaviour
{
    [SerializeField] private int warmthRangeRadius = 3;
    [SerializeField] private int effectivateRange = 1;
    [SerializeField] private float warmthIncreaseRate = 1.0f;
    [SerializeField] private float effectivateWarmthIncreaseRate = 2.0f;
    private PlayerMeter warmthMeter;
    private SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = warmthRangeRadius;
        sphereCollider.isTrigger = true;
    }

    protected virtual void FixedUpdate()
    {
        if (warmthMeter)
            warmthMeter.IncreaseWarmth(CalculateWarmthIncreaseRate());
    }

    private float CalculateWarmthIncreaseRate()
    {
        float distance = Vector3.Distance(warmthMeter.transform.position, transform.position);
        if (distance <= effectivateRange)
            return effectivateWarmthIncreaseRate / distance;
        return warmthIncreaseRate / distance;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            warmthMeter = other.gameObject.GetComponent<PlayerMeter>();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            warmthMeter = null;
    }
}

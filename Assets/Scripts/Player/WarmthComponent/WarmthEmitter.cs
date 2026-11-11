using UnityEngine;

public class WarmthEmitter : MonoBehaviour
{
    [SerializeField] private int effectivateRange = 4;
    [SerializeField] private float warmthIncreaseRate = 5.0f;
    [SerializeField] private float effectivateWarmthIncreaseRate = 10.0f;
    private PlayerMeter warmthMeter;

    protected virtual void FixedUpdate()
    {
        if (warmthMeter != null)
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

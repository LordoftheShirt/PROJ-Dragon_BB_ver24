using UnityEngine;

public class ParticlesDieAfterDuration : MonoBehaviour
{
    private ParticleSystem particles;
    private float duration = 0;

    private void Start()
    {
        if (gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem comp))
        {
            particles = comp;
            duration = comp.main.duration;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

}

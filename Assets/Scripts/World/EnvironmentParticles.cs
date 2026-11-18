using UnityEngine;

public class EnvironmentParticles : Singleton<EnvironmentParticles>
{
    [SerializeField] private ParticleSystem stars;

    private Transform _player;

    private void Start()
    {
        stars.transform.SetParent(null);

        SetPlayer();

        transform.SetParent(_player);
        transform.localPosition = Vector3.zero;
    }

    private void LateUpdate()
    {
        stars.transform.position = _player.transform.position;
    }

    private void SetPlayer()
    {
        var refManager = ReferenceManager.Instance;
        if (refManager == null) return;
        if (refManager.player == null) return;

        _player = refManager.player.transform;
    }
}

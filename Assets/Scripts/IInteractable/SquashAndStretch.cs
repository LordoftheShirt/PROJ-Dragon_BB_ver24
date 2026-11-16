using DG.Tweening;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    Vector3 scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Animate()
    {
        transform.DOKill();
        transform.localScale *= 0.8f;
        transform.DOScale(scale, 2).SetEase(Ease.OutElastic);
        
    }
}

using Coffee.UIEffects;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MapPrompt : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UIEffect uiEffect;

    [Space]
    [SerializeField] private float timeUntilAnnoyance1 = 10;
    [SerializeField] private float timeUntilAnnoyance2 = 30;

    private Coroutine _coroutine;

    private void OnEnable()
    {
        transform.localScale = Vector2.one;

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1);

        uiEffect.transitionRate = 0;
        uiEffect.DOTransitionRate(1, 4f).SetEase(Ease.InOutCirc).SetDelay(2f).SetLoops(-1, LoopType.Restart);
        _coroutine = StartCoroutine(AnnoyanceCoroutine());
    }

    private void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        canvasGroup.DOKill();
        uiEffect.DOKill();
        transform.DOKill();
    }

    private IEnumerator AnnoyanceCoroutine()
    {
        yield return new WaitForSeconds(timeUntilAnnoyance1);

        DoAnnoyance1();

        yield return new WaitForSeconds(timeUntilAnnoyance2);

        DoAnnoyance2();
    }

    private void DoAnnoyance1()
    {
        transform.DOScale(1.25f, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void DoAnnoyance2()
    {
        transform.DORotate(new Vector3(0, 0, 5), 0.5f).SetEase(Ease.InOutSine).onComplete = () =>
        {
            transform.DORotate(new Vector3(0, 0, -5), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        };
    }
}

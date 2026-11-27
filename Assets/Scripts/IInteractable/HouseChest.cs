using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseChest : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform chestRoof;
    private PlayerUI _playerUI;
    private Canvas _canvas;
    private CanvasScaler _canvasScaler;

    private bool _puttingResources;

    private void Start()
    {
        _playerUI = PlayerUI.Instance;
        _canvas = _playerUI.GetComponentInParent<Canvas>(true);
        _canvasScaler = _playerUI.GetComponentInParent<CanvasScaler>(true);
    }

    public void Interact()
    {
        if (_puttingResources) return;

        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        Inventory inv = Inventory.Instance;

        List<RectTransform> rects = new();
        int amount = 0;

        foreach (var slot in _playerUI.ResourceUISlots)
        {
            if (inv.GetResourceTemporary(slot.Type) > 0)
            {
                rects.Add(slot.CreateCloneWithAnim());
                amount++;
            }
        }

        if (amount <= 0)
        {
            // NO RESOURCES TO PUT INTO CHEST
            yield break;
        }

        _puttingResources = true;

        inv.ConvertTemporaryResourcesToPermanent();

        chestRoof.DOKill();
        chestRoof.DOLocalRotateQuaternion(Quaternion.Euler(-180, 0, 0), 1).SetEase(Ease.OutSine);

        yield return new WaitForSeconds(0.5f);

        foreach (RectTransform rect in rects)
        {
            rect.DOKill();
        }

        int length = rects.Count;
        Vector3[] startPos = new Vector3[length];
        Quaternion[] startRot = new Quaternion[length];
        for (int i = 0; i < length; i++)
        {
            RectTransform rect = rects[i];
            startPos[i] = rect.localPosition;
            startRot[i] = rect.localRotation;
        }

        Camera cam = Camera.main;

        float time = 0;

        void Set(float t)
        {
            time = t;

            Vector2 pos = cam.WorldToViewportPoint(transform.position);

            Vector2 halfScreenSize = _canvasScaler.referenceResolution / 2f;
            Vector2 screenPoint = new(
                Mathf.LerpUnclamped(-halfScreenSize.x, halfScreenSize.x, pos.x),
                Mathf.LerpUnclamped(-halfScreenSize.y, halfScreenSize.y, pos.y)
                );

            for (int i = 0; i < length; i++)
            {
                RectTransform rect = rects[i];
                rect.localPosition = Vector3.Lerp(startPos[i], screenPoint, t);
                rect.localRotation = Quaternion.Slerp(startRot[i], Quaternion.identity, t);

                rect.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, Mathf.Max(0, t - 0.5f) * 2);
            }
        }

        float duration = 0.5f;
        DOTween.To(() => time, Set, 1, duration).SetEase(Ease.InSine);
        yield return new WaitForSeconds(duration);

        chestRoof.DOLocalRotateQuaternion(Quaternion.Euler(-90, 0, 0), 1).SetEase(Ease.OutBounce);

        foreach (RectTransform rect in rects)
        {
            Destroy(rect.gameObject);
        }

        _puttingResources = false;
    }

    public string GetDescription()
    {
        return "Store resources";
    }
}

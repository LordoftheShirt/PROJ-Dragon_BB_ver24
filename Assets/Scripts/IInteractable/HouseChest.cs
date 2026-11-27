using DebugTools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseChest : MonoBehaviour, IInteractable, IComponentDebugGUI
{
    [SerializeField] private Transform chestRoof;
    private PlayerUI _playerUI;
    private Canvas _canvas;
    private CanvasScaler _canvasScaler;
    private Collider _collider;

    private bool _puttingResources;
    private Vector2 _actualScreenPos;
    private Vector2 _screenPos;
    private bool _onScreen;

    private void Start()
    {
        _playerUI = PlayerUI.Instance;
        _canvas = _playerUI.GetComponentInParent<Canvas>(true);
        _canvasScaler = _playerUI.GetComponentInParent<CanvasScaler>(true);

        _collider = GetComponentInChildren<Collider>(true);
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
        chestRoof.DOLocalRotateQuaternion(Quaternion.Euler(-180, 0, 0), 1).SetEase(Ease.OutSine).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.5f);

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

        void Set(int i, float t, ref float time)
        {
            time = t;

            RectTransform rect = rects[i];
            rect.localPosition = Vector3.Lerp(startPos[i], _screenPos, t);
            rect.localRotation = Quaternion.Slerp(startRot[i], Quaternion.identity, t);

            rect.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, Mathf.Max(0, t - 0.5f) * 2);
        }

        float delayBetween = 0.05f;
        float duration = 0.5f;
        for (int i = 0; i < length; i++)
        {
            float time = 0;
            int index = i;
            DOTween.To(() => time, (t) => Set(index, t, ref time), 1, duration).SetDelay(index * delayBetween).SetEase(Ease.InSine).SetUpdate(true);
        }

        yield return new WaitForSeconds(duration + delayBetween * (length - 1));

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

    private void LateUpdate()
    {
        if (!_puttingResources) return;

        Camera cam = Camera.main;

        _onScreen = IsVisibleFrom(_collider.bounds, cam);

        if (!_onScreen) return;

        Vector2 pos = cam.WorldToViewportPoint(transform.position);

        Vector2 halfScreenSize = _canvasScaler.referenceResolution / 2f;
        _actualScreenPos = new(
            Mathf.LerpUnclamped(-halfScreenSize.x, halfScreenSize.x, pos.x),
            Mathf.LerpUnclamped(-halfScreenSize.y, halfScreenSize.y, pos.y)
            );

        _screenPos = Vector2.Lerp(_screenPos, _actualScreenPos, 25 * Time.unscaledDeltaTime);
    }

    public bool IsVisibleFrom(Bounds bounds, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    public void OnDebugGUI()
    {
        using (new DisableGUIScope(true))
        {
            DebugGUI.Property("V", Camera.main.WorldToViewportPoint(transform.position));
            DebugGUI.Property("S", Camera.main.WorldToScreenPoint(transform.position));
        }
    }
}

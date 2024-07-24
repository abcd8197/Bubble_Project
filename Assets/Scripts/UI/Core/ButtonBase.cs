using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonBase : EventTrigger
{
    [SerializeField] private float DestScale = 0.95f;
    [Space(10)]
    [SerializeField] private float PointerDown_Duration = 0.2f;
    [SerializeField] private Ease PointerDown_Ease = Ease.OutExpo;
    [Space(10)]
    [SerializeField] private float PointerUp_Duration = 0.4f;
    [SerializeField] private Ease PointerUp_Ease = Ease.OutBounce;

    private void OnDestroy()
    {
        if (DOTween.IsTweening(transform))
            transform.DOKill();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(DestScale, PointerDown_Duration).SetEase(PointerDown_Ease).SetRecyclable(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1, PointerUp_Duration).SetEase(PointerUp_Ease).SetRecyclable(true);
        base.OnPointerUp(eventData);
    }
}

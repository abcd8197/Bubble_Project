using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class DotweenMover : MonoBehaviour
{
    [SerializeField] Vector2 Start;
    [SerializeField] Vector2 Via;
    [SerializeField] Vector2 Dest;
    [Space(10)]
    [SerializeField] private float AppearDuration;
    [SerializeField] private Ease AppearEase;
    [Space(10)]
    [SerializeField] private float Delay;
    [Space(10)]
    [SerializeField] private float DisAppearDuration;
    [SerializeField] private Ease DisAppearEase;

    private RectTransform m_RT;
    private Coroutine m_Coroutine;

    private void Awake()
    {
        m_RT = GetComponent<RectTransform>();
        m_Coroutine = StartCoroutine(DirectionCoroutine());
    }

    private void OnDestroy()
    {
        if(m_Coroutine != null)
            StopCoroutine(m_Coroutine);
        if (DOTween.IsTweening(m_RT))
            m_RT.DOKill();
    }

    private System.Collections.IEnumerator DirectionCoroutine()
    {
        m_RT.DOAnchorPos(Via, AppearDuration).From(Start).SetEase(AppearEase);
        yield return new WaitForSeconds(AppearDuration + Delay);
        m_RT.DOAnchorPos(Dest, DisAppearDuration).From(Via).SetEase(DisAppearEase);
        yield return new WaitForSeconds(DisAppearDuration);
        m_Coroutine = null;
    }

}

using UnityEngine;
using DG.Tweening;

public class SceneLoadPopup : Codejay.UI.PopupBase
{
    [SerializeField] private float FadeDuration = 1f;

    private CanvasGroup m_CanvasGroup;
    private Coroutine m_Coroutine;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDestroy()
    {
        if(m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        if (DOTween.IsTweening(m_CanvasGroup))
            m_CanvasGroup.DOKill();
    }

    public void Active()
    {
        m_Coroutine = StartCoroutine(FadeDirectionCoroutine(true));
    }

    public void InActiveAndDestory()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
        m_Coroutine = StartCoroutine(FadeDirectionCoroutine(false));
    }

    private System.Collections.IEnumerator FadeDirectionCoroutine(bool enabled)
    {
        m_CanvasGroup.DOFade(enabled ? 1 : 0, FadeDuration * 0.8f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(FadeDuration);

        m_Coroutine = null;

        if (!enabled)
            Destroy(this.gameObject);
    }
}

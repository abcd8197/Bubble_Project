namespace Codejay.UI
{
    using UnityEngine;
    using DG.Tweening;

    [RequireComponent(typeof(Canvas))]
    public class PopupBase : MonoBehaviour, System.IDisposable
    {
        protected Canvas m_canvas = null;
        protected bool m_isDisposed = false;

        public void Initialize()
        {
            m_canvas = GetComponent<Canvas>();
        }

        // Sorting Order 조절
        public virtual void ShowPopup(int drawOrder)
        {
            m_canvas.sortingOrder = (drawOrder + 1) * 2;
            transform.DOScale(1, 0.5f).From(Vector3.one * 0.75f).SetEase(Ease.OutBounce);
        }

        // 팝업 삭제
        public virtual void HidePopup()
        {
            if (transform != null && DOTween.IsTweening(transform))
                transform.DOKill();
            Dispose();
            Destroy(this.gameObject);
        }

        public virtual void Dispose() 
        {
            m_canvas = null; 
        }
    }

}
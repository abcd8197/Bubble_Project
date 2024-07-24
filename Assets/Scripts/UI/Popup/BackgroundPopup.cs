using UnityEngine;
using Codejay.UI;
using UnityEngine.EventSystems;

public class BackgroundPopup : PopupBase, UnityEngine.EventSystems.IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.UI.Escapeable)
            Managers.UI.HideTopPopup();
    }

    public override void ShowPopup(int drawOrder)
    {
        m_canvas.sortingOrder = ((drawOrder + 1) * 2) - 1;
    }

}

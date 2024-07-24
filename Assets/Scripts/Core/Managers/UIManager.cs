using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Codejay.UI;

public class UIManager : MonoBehaviour
{
    public enum EPopupType
    {
        SceneLoadPopup, BackgroundPopup, QuitPopup, ClearPopup, FailPopup
    }

    private const string POPUP_PATH = "Prefabs/UI/Popups/";

    private Stack<(EPopupType key, PopupBase value)> m_StkPopups;
    private HashSet<EPopupType> m_HashActivatedPopups;
    private Transform m_PopupRootTransform;
    private int m_EscapeableCount = 0;

    public bool Escapeable { get => m_EscapeableCount <= 0; }

    #region ## Behaviours ##
    private void Update()
    {
        if (Managers.Game.Controlable && Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_HashActivatedPopups.Count > 0)
                HideTopPopup();
            else
                ShowPopup(EPopupType.QuitPopup);
        }
    }
    #endregion

    #region ## Public Methods ##
    public void Initialize()
    {
        if (m_StkPopups == null)
            m_StkPopups = new Stack<(EPopupType, PopupBase)>();
        if (m_HashActivatedPopups == null)
            m_HashActivatedPopups = new HashSet<EPopupType>();
    }

    public void RegisterRootCanvas(Transform rootTransform)
    {
        m_PopupRootTransform = rootTransform;
    }

    public PopupBase ShowPopup(EPopupType popupType)
    {
        PopupBase popup = null;
        if (m_StkPopups.Count == 0)
        {
            popup = CreatePopupOrNull(EPopupType.BackgroundPopup);
            popup.Initialize();
            popup.ShowPopup(0);
            m_StkPopups.Push((EPopupType.BackgroundPopup, popup));
            m_HashActivatedPopups.Add(EPopupType.BackgroundPopup);
        }

        popup = CreatePopupOrNull(popupType);

        if (popup == null)
        {
            throw new System.ArgumentException($"Invalid Call. {popupType} not exist.");
        }
        else
        {
            popup.Initialize();
            popup.ShowPopup(m_StkPopups.Count);
            m_StkPopups.Push((popupType, popup));
            m_HashActivatedPopups.Add(popupType);

            return popup;
        }
    }

    public void HideTopPopup()
    {
        if (m_StkPopups.Count > 0)
        {
            var pair = m_StkPopups.Pop();
            pair.value.HidePopup();

            if (m_HashActivatedPopups.Contains(pair.key))
                m_HashActivatedPopups.Remove(pair.key);

            if (m_StkPopups.Count == 1)
            {
                var temp = m_StkPopups.Pop();
                temp.value.HidePopup();

                if (m_HashActivatedPopups.Contains(temp.key))
                    m_HashActivatedPopups.Remove(temp.key);
            }
        }
    }

    public bool IsPopupOpend(EPopupType popupType)
    {
        return m_HashActivatedPopups.Contains(popupType);
    }

    public void EscapeableEnabled(bool b)
    {
        if (b)
        {
            m_EscapeableCount--;

            if (m_EscapeableCount < 0)
                m_EscapeableCount = 0;
        }
        else
            m_EscapeableCount++;
    }
    #endregion

    #region ## Private Methods ##

    private PopupBase CreatePopupOrNull(EPopupType popupType)
    {
        switch (popupType)
        {
            case EPopupType.BackgroundPopup:
                return Instantiate(Resources.Load<PopupBase>(POPUP_PATH + "BackgroundPopup"), m_PopupRootTransform);
            case EPopupType.QuitPopup:
                return Instantiate(Resources.Load<PopupBase>(POPUP_PATH + "QuitPopup"), m_PopupRootTransform);
            case EPopupType.ClearPopup:
                return Instantiate(Resources.Load<PopupBase>(POPUP_PATH + "ClearPopup"), m_PopupRootTransform);
            case EPopupType.FailPopup:
                return Instantiate(Resources.Load<PopupBase>(POPUP_PATH + "FailPopup"), m_PopupRootTransform);
            default:
                return null;
        }
    }
    #endregion
}

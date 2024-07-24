using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopup : Codejay.UI.PopupBase
{
    public System.Action ConfirmAction;
    public System.Action CancelAction;

    public override void ShowPopup(int drawOrder)
    {
        base.ShowPopup(drawOrder);
        Managers.Game.ControlEnabled(false);
    }

    public override void HidePopup()
    {
        base.HidePopup();
        Managers.Game.ControlEnabled(true);
    }

    private void OnDestroy()
    {
        ConfirmAction = null;
        CancelAction = null;
    }

    public void Confirm()
    {
        ConfirmAction?.Invoke();
        ConfirmAction = null;

        switch (Managers.Scene.CurrentSceneType)
        {
            case SceneTransitionManager.ESceneType.Title:
                Application.Quit();
                break;
            case SceneTransitionManager.ESceneType.Main:
                Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.Title);
                break;
            case SceneTransitionManager.ESceneType.InGame:
                Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.Main);
                break;
        }

        Managers.UI.HideTopPopup();
    }

    public void Cancel()
    {
        CancelAction?.Invoke();
        CancelAction = null;
        Managers.UI.HideTopPopup();
    }
}

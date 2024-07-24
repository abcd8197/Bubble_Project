using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailPopup : Codejay.UI.PopupBase
{
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

    public void Confirm()
    {
        Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.Main);
    }
}

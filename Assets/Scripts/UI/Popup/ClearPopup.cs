using UnityEngine;

public class ClearPopup : Codejay.UI.PopupBase
{
    public override void ShowPopup(int drawOrder)
    {
        base.ShowPopup(drawOrder);
        Managers.Game.ControlEnabled(false);

        Managers.DB.UpdateClearedMaxStage(Managers.Game.CurrentStage);
    }

    public override void HidePopup()
    {
        base.HidePopup();
        Managers.Game.ControlEnabled(true);
    }

    public void ConfirmButton()
    {
        Managers.UI.HideTopPopup();
        Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.Main);
    }
}

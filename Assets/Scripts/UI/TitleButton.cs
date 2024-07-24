using UnityEngine;

[RequireComponent(typeof(ButtonBase))]
public class TitleButton : MonoBehaviour
{
    private bool m_IsClicked = false;
    public void LoadScene()
    {
        if(!m_IsClicked && Managers.Instance != null)
        {
            m_IsClicked = true;
            Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.Main);
        }
    }

}

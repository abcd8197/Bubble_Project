using UnityEngine;

public class StageSlot : MonoBehaviour
{
    [SerializeField] private int Stage;
    [SerializeField] private UnityEngine.UI.Image BlockImage;
    private TMPro.TextMeshProUGUI m_TMP;

    private void Awake()
    {
        if (Managers.Instance == null)
            return;

        m_TMP = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        m_TMP.text = Stage.ToString();

        BlockImage.enabled = Managers.DB.ClearedMaxStage < Stage - 1;
    }

    public void ClickStageSlot()
    {
        if (BlockImage.enabled)
        {

        }
        else if (Managers.Instance != null)
        {
            Managers.Game.CurrentStage = Stage;
            Managers.Scene.LoadScene(SceneTransitionManager.ESceneType.InGame);
        }
    }
}

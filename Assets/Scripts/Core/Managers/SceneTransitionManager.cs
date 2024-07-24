using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public const string LOADPOPUP_PATH = "Prefabs/UI/Popups/SceneLoadPopup";
    public const float MINIMUM_LOAD_TIME = 2f;
    public enum ESceneType { Title, Main, InGame, Empty }

    #region ## Fields
    private ESceneType m_CurrentSceneType = ESceneType.Empty;
    private Coroutine m_LoadCoroutine = null;
    private float m_LoadCount;
    #endregion

    #region ## Properties ##
    public ESceneType CurrentSceneType { get { return m_CurrentSceneType; } }
    public bool IsLoading { get => m_LoadCoroutine != null; }
    #endregion

    public void Initialize()
    {
        m_CurrentSceneType = ESceneType.Title;
    }

    public void LoadScene(ESceneType sceneType)
    {
        if (IsLoading)
            return;

        m_LoadCoroutine = StartCoroutine(LoadSceneCoroutine(sceneType));
    }

    private IEnumerator LoadSceneCoroutine(ESceneType sceneType)
    {
        Managers.Game.ControlEnabled(false);
        Managers.Game.InitializeGameData();

        m_LoadCount = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync((int)ESceneType.Empty);
        async.allowSceneActivation = true;

        SceneLoadPopup loadPopup = Instantiate(Resources.Load<SceneLoadPopup>(LOADPOPUP_PATH), null);
        loadPopup.Active();
        DontDestroyOnLoad(loadPopup.gameObject);
        m_CurrentSceneType = ESceneType.Empty;
        while (!async.isDone)
        {
            m_LoadCount += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        async = SceneManager.LoadSceneAsync((int)sceneType);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            m_LoadCount += Time.deltaTime;
            yield return null;
        }

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            m_LoadCount += Time.deltaTime;
            yield return null;
        }

        while (m_LoadCount < MINIMUM_LOAD_TIME)
        {
            m_LoadCount += Time.deltaTime;
            yield return null;
        }

        m_CurrentSceneType = sceneType;
        loadPopup.InActiveAndDestory();
        yield return new WaitForSeconds(1f);

        m_LoadCoroutine = null;
        Managers.Game.ControlEnabled(true);
    }

}

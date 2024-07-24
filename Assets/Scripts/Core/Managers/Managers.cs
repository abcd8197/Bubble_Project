using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Managers : MonoBehaviour
{
    private static Managers sInstance = null;
    public static Managers Instance
    {
        get
        {
            // 종료 중이라면 null 반환
            if (m_IsApplicationQuitting)
                return null;

            return sInstance;
        }
    }

    #region ## Static Fields ##
    private static bool m_IsApplicationQuitting = false;
    #endregion

    #region ## Managers ##
    private DBManager m_DB;
    private UIManager m_UI;
    private SceneTransitionManager m_Scene;
    private GameManager m_Game;

    public static DBManager DB { get => Instance.m_DB; }
    public static UIManager UI { get => Instance.m_UI; }
    public static SceneTransitionManager Scene { get => Instance.m_Scene;}
    public static GameManager Game { get => Instance.m_Game;}
    #endregion

    // 씬 로드 전에 초기화 호출
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (sInstance != null)
            return;

        GameObject newObj = new GameObject("_Managers");
        sInstance = newObj.AddComponent<Managers>();
        DontDestroyOnLoad(newObj);
    }

    #region ## Behaviours ##
    private void Awake()
    {
        SetUpGame();
        StartCoroutine(CreateManagersCoroutine());
    }
    private void OnDestroy()
    {
        if (!m_IsApplicationQuitting)
            m_IsApplicationQuitting = true;
    }
    #endregion

    private void SetUpGame()
    {
        Application.targetFrameRate = 120;
        Input.multiTouchEnabled = false;
    }

    private System.Collections.IEnumerator CreateManagersCoroutine()
    {
        m_DB = new GameObject("_DB").AddComponent<DBManager>();
        m_DB.transform.SetParent(transform);
        DontDestroyOnLoad(m_DB);
        yield return new WaitUntil(() => m_DB.IsDataLoaded);

        m_UI = new GameObject("_UI").AddComponent<UIManager>();
        m_UI.transform.SetParent(transform);
        m_UI.Initialize();
        DontDestroyOnLoad(m_UI);

        m_Scene = new GameObject("_Scene").AddComponent<SceneTransitionManager>();
        m_Scene.transform.SetParent(transform);
        DontDestroyOnLoad(m_Scene);

        m_Game = new GameObject("_Game").AddComponent<GameManager>();
        m_Game.transform.SetParent(transform);
        DontDestroyOnLoad(m_Game);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private const string SAVE_KEY = "ClearMaxStage";

    public bool IsDataLoaded { get; private set; } = false;
    public int ClearedMaxStage { get; private set; } = 0;

    private void Awake()
    {
        StartCoroutine(LoadDataCoroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator LoadDataCoroutine()
    {
        // 로컬 데이터 로드
        ClearedMaxStage = PlayerPrefs.GetInt(SAVE_KEY, 0);

        yield return null;
        IsDataLoaded = true;
    }

    public void UpdateClearedMaxStage(int stage)
    {
        ClearedMaxStage = stage;
        PlayerPrefs.SetInt(SAVE_KEY, ClearedMaxStage);
    }
}

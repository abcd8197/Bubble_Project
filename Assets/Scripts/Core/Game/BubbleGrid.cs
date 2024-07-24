using System.Collections.Generic;
using UnityEngine;
using Codejay.Datas;

public class BubbleGrid : MonoBehaviour
{
    [SerializeField] private GameObject BubblePrefabs;
    [SerializeField] private Transform TopColliderTransform;
    [SerializeField] private float ShootSpeed = 5;
    private List<string[]> m_lstStageBubbleDatas;
    private List<Bubble> m_lstBubbles;

    private Coroutine m_MoveToDestinationCoroutine = null;

    #region ## Behaviours ##
    private void Awake()
    {
        m_lstBubbles = new List<Bubble>();
        m_lstStageBubbleDatas = new List<string[]>();

        LoadBubbleData();
        CreateBubble();
        UpdateAllBubbles();
        SetTopColliderPosition();

        if (Managers.Instance != null)
        {
            Managers.Game.ShootBullet += ShootCallback;
            Managers.Game.CheckFallableBubbles += CheckFallableBubbles;
        }
    }

    private void OnDestroy()
    {
        if (m_MoveToDestinationCoroutine != null)
            StopCoroutine(m_MoveToDestinationCoroutine);

        if (Managers.Instance != null)
        {
            Managers.Game.ShootBullet -= ShootCallback;
            Managers.Game.CheckFallableBubbles -= CheckFallableBubbles;
        }

        m_lstStageBubbleDatas.Clear();
        m_lstBubbles.Clear();
    }
    #endregion

    #region ## Private Methods ##
    private void UpdateAllBubbles()
    {
        for (int i = 0; i < m_lstBubbles.Count; i++)
        {
            if (m_lstBubbles[i].Data.IsActivated)
                m_lstBubbles[i].UpdateNearBubble();
        }
    }

    private void ShootCallback(EBubblePrefabType prefabType, Vector3[] positions, Bubble targetBubble)
    {
        Vector3[] pos = new Vector3[positions.Length];
        Vector3 destination = targetBubble.transform.position;
        bool ishigest = targetBubble.Data.IsHighest;

        for (int i = 0; i < positions.Length; i++)
            pos[i] = positions[i];

        Vector3 temp = positions[0];
        temp.z = 0;
        targetBubble.transform.position = temp;

        Vector2 dir = (pos[pos.Length - 1] - pos[pos.Length - 2]).normalized;
        Vector2Int lineNumber = GameManager.GetNearstBubbleLineNumber(pos[pos.Length - 1], dir);

        BubbleData data = new BubbleData();
        data.SetData(prefabType, lineNumber.x, lineNumber.y, ishigest);
        targetBubble.SetData(data);

        Managers.Game.AddColorType(data.BubbleColorType);
        m_MoveToDestinationCoroutine = StartCoroutine(MoveToDestinationCoroutine(targetBubble, pos, destination));
    }

    private void LoadBubbleData()
    {
        // Load From CSV
        TextAsset csvText = Resources.Load<TextAsset>($"Datas/StageData/Stage_{Managers.Game.CurrentStage}");
        string[] line = csvText.text.Split("\r\n");

        // 비어있는 맨 아랫줄 추가
        m_lstStageBubbleDatas.Add(new string[10] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" });
        if (line.Length > 1)
        {
            for (int i = 1; i < line.Length; i++)
            {
                m_lstStageBubbleDatas.Add(line[i].Split(','));
            }
        }
    }

    private void CreateBubble()
    {
        if (m_lstStageBubbleDatas != null && m_lstStageBubbleDatas.Count > 0)
        {
            for (int i = 0; i < m_lstStageBubbleDatas.Count; i++)
            {
                for (int j = 1; j < m_lstStageBubbleDatas[i].Length; j++)
                {
                    int temp = int.Parse(m_lstStageBubbleDatas[i][j]);

                    EBubblePrefabType prefabType;
                    Bubble bubble = Instantiate(BubblePrefabs, transform).GetComponent<Bubble>();
                    bubble.transform.position = GameManager.GetBubblePosition(j - 1, i);
                    BubbleData data = new BubbleData();

                    // Empty String Exception
                    if (string.IsNullOrEmpty(m_lstStageBubbleDatas[i][j]))
                        prefabType = EBubblePrefabType.Empty;
                    else
                        prefabType = (EBubblePrefabType)temp;

                    data.SetData(prefabType, i, j, i == m_lstStageBubbleDatas.Count - 1);

                    if (prefabType == EBubblePrefabType.Empty)
                        bubble.SetEmpty(data);
                    else
                        bubble.SetData(data);

                    bubble.gameObject.name = $"Bubble_{m_lstBubbles.Count}";
                    bubble.TMP.text = m_lstBubbles.Count.ToString();

                    if (Managers.Instance != null)
                        Managers.Game.AddColorType(bubble.Data.BubbleColorType);
                    m_lstBubbles.Add(bubble);
                }
            }
        }
    }

    private void SetTopColliderPosition()
    {
        float highest = 0;
        for (int i = 0; i < m_lstBubbles.Count; i++)
        {
            if (m_lstBubbles[i].transform.position.y > highest)
                highest = m_lstBubbles[i].transform.position.y;
        }

        TopColliderTransform.position = (Vector3.up * highest) + (Vector3.up * GameManager.BUBBLE_RADIUS);
    }

    private void CheckFallableBubbles()
    {
        Managers.Game.ControlEnabled(false);

        HashSet<Bubble> visited = UnityEngine.Pool.HashSetPool<Bubble>.Get();
        HashSet<Bubble> shoulFall = UnityEngine.Pool.HashSetPool<Bubble>.Get();
        visited.Clear();
        shoulFall.Clear();

        for (int i = 0; i < m_lstBubbles.Count; i++)
        {
            if (m_lstBubbles[i].Data.IsActivated)
            {
                if (ShouldFallBubble(m_lstBubbles[i], visited))
                {
                    foreach (var bubble in visited)
                    {
                        shoulFall.Add(bubble);
                    }
                }
                visited.Clear();
            }
        }

        foreach (var bubble in shoulFall)
        {
            bubble.Fall();
        }

        UnityEngine.Pool.HashSetPool<Bubble>.Release(visited);
        UnityEngine.Pool.HashSetPool<Bubble>.Release(shoulFall);

        UpdateAllBubbles();
        Managers.Game.ControlEnabled(true);
    }

    private bool ShouldFallBubble(Bubble bubble, HashSet<Bubble> visited)
    {
        if (bubble.Data.IsHighest)
            return false;

        if (bubble.NearBubbles.Count == 0 || visited.Contains(bubble))
            return true;

        visited.Add(bubble);

        for (int i = 0; i < bubble.NearBubbles.Count; i++)
        {
            if (!ShouldFallBubble(bubble.NearBubbles[i], visited))
            {
                return false;
            }
        }

        return true;
    }

    Vector3 testPos;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(testPos, 0.1f);
    }
    #endregion

    #region ## Coroutines ##
    private System.Collections.IEnumerator MoveToDestinationCoroutine(Bubble bubble, Vector3[] positions, Vector2 destination)
    {
        Managers.Game.ControlEnabled(false);
        Transform bubbleTransform = bubble.transform;

        // 0은 시작점이기 때문에 1부터 시작
        for (int i = 1; i < positions.Length; i++)
        {
            Vector3 dest = Vector3.Distance(positions[i], destination) <= GameManager.BUBBLE_RADIUS ? destination : positions[i];
            Vector3 dir = dest - bubbleTransform.position;
            dir.z = 0;
            while (Vector3.Distance(bubbleTransform.position, dest) > GameManager.BUBBLE_RADIUS)
            {
                bubbleTransform.position += dir.normalized * ShootSpeed * Time.deltaTime;
                yield return null;
            }
        }

        bubbleTransform.position = destination;
        bubble.UpdateNearBubble();
        for (int i = 0; i < bubble.NearBubbles.Count; i++)
            bubble.NearBubbles[i].UpdateNearBubble();

        yield return new WaitForSeconds(0.2f);
        m_MoveToDestinationCoroutine = null;
        bubble.ExplosionSameColorBubble(0);
        bubble.Data.IsActivated = true;

        UpdateAllBubbles();
        Managers.Game.ClearReveredExplosionContainer();
        Managers.Game.ControlEnabled(true);
    }
    #endregion
}

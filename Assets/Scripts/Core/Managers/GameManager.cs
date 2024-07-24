using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region ## Const / Statics ##
    /// <summary> 버블 폭발에 필요한 수</summary>
    public const int EXPLOSION_REQUIRE_BUBBLE_COUNT = 3;

    /// <summary> 버블 반지름</summary>
    public const float BUBBLE_RADIUS = 0.25f;
    /// <summary> 게임 영역 최소 위치</summary>
    public readonly static Vector2 MINIMUM_POSITION = new Vector2(-2.5f, -BUBBLE_RADIUS);
    /// <summary> 게임 영역 X 최대 위치</summary>
    public const float MAX_POS_X = 2.5f;
    /// <summary> 버블 배치 시작 간격 </summary>
    public static float BUBBLE_START_POS_Y_INTERVAL = 0;
    /// <summary> 라인 별 간격</summary>
    public static float BUBBLE_INTERVAL_BY_ROW = 0;
    /// <summary> 발사 경계선</summary>
    public static float SHOOT_BORDER = -1f;
    #endregion

    #region ## Publics Fields ##
    public System.Action<Codejay.Datas.EBubblePrefabType, Vector3[], Bubble> ShootBullet;
    public System.Action CheckFallableBubbles;
    public int CurrentStage = 1;
    #endregion

    #region ## Private Fields ##
    private System.Collections.Generic.List<int> m_lstExistColorType;
    private System.Collections.Generic.HashSet<int> m_hashExplosionBubbles;
    #endregion

    #region ## Properties
    public int ControlableCount { get; private set; } = 0;
    public bool Controlable { get => ControlableCount <= 0; }
    public float ExplosionBubbleHighestPosition { get; set; } = 0;
    #endregion

    #region ## Static Methods ##
    public static Vector2 GetBubblePosition(int x, int y)
    {
        Vector2 result = MINIMUM_POSITION;
        result.x += BUBBLE_RADIUS;
        result.y += BUBBLE_RADIUS;

        result.x += x * BUBBLE_RADIUS * 2;
        result.y += y * BUBBLE_INTERVAL_BY_ROW;
        if (y % 2 == 0)
            result.x += BUBBLE_RADIUS;

        return result;
    }

    /// <summary>
    /// 가장 가까운 그리드 행,열을 반환하는 함수
    /// </summary>
    /// <param name="point">return (Row, Column) (x,y)</param>
    public static Vector2Int GetNearstBubbleLineNumber(Vector2 point, Vector2 inDirection)
    {
        Vector2 offset = -inDirection.normalized * (BUBBLE_RADIUS / 2f);
        Vector2 pos = point + offset;

        // Y
        int Y_Line = Mathf.RoundToInt((pos.y) / BUBBLE_INTERVAL_BY_ROW);
        if (Y_Line < 0)
            Y_Line = 0;
        // X
        float temp = pos.x + Mathf.Abs(MINIMUM_POSITION.x);
        if (temp < 0)
            temp = 0;
        int X_Line = Mathf.Abs(Mathf.FloorToInt(temp / (BUBBLE_RADIUS * 2)));

        return new Vector2Int(X_Line, Y_Line);
    }

    public static Vector2 GetNearestBubblePosition(Vector2 point, Vector2 inDirection)
    {
        Vector2Int LineNumber = GetNearstBubbleLineNumber(point, inDirection);
        return GetBubblePosition(LineNumber.x, LineNumber.y);
    }
    #endregion

    #region ## Behaviours ##
    private void Awake()
    {
        SetUpData();
    }

    private void OnDestroy()
    {
        ShootBullet = null;
        m_lstExistColorType.Clear();
        m_hashExplosionBubbles.Clear();
    }
    #endregion

    #region ## Private Methods ##
    private void SetUpData()
    {
        BUBBLE_INTERVAL_BY_ROW = (BUBBLE_RADIUS * Mathf.Sqrt(3));
        BUBBLE_START_POS_Y_INTERVAL = BUBBLE_INTERVAL_BY_ROW * 7;

        m_lstExistColorType = new System.Collections.Generic.List<int>();
        for (int i = 0; i < (int)Codejay.Datas.EBubbleColorType.Transparent; i++)
            m_lstExistColorType.Add(0);

        m_hashExplosionBubbles = new System.Collections.Generic.HashSet<int>();
    }
    #endregion

    #region ## Public Methods ##
    public void InitializeGameData()
    {
        m_hashExplosionBubbles.Clear();

        for (int i = 0; i < m_lstExistColorType.Count; i++)
            m_lstExistColorType[i] = 0;

        ControlableCount = 0;
    }

    public void ControlEnabled(bool b)
    {
        if (b)
        {
            ControlableCount--;

            if (ControlableCount < 0)
                ControlableCount = 0;
        }
        else
            ControlableCount++;
    }

    public void AddColorType(Codejay.Datas.EBubbleColorType colorType)
    {
        if (colorType == Codejay.Datas.EBubbleColorType.Transparent)
            return;

        m_lstExistColorType[(int)colorType]++;
    }

    public void RemoveColorType(Codejay.Datas.EBubbleColorType colorType)
    {
        if (colorType == Codejay.Datas.EBubbleColorType.Transparent)
            return;

        m_lstExistColorType[(int)colorType]--;
    }

    public bool IsExistBubbleColorType(Codejay.Datas.EBubbleColorType colorType)
    {
        return m_lstExistColorType[(int)colorType] > 0;
    }

    public bool AddReservedExplosionBubble(int hash)
    {
        if (!m_hashExplosionBubbles.Contains(hash))
        {
            m_hashExplosionBubbles.Add(hash);
            return true;
        }
        else
            return false;
    }

    public void ClearReveredExplosionContainer()
    {
        m_hashExplosionBubbles.Clear();
    }

    public bool Explosionable()
    {
        return m_hashExplosionBubbles.Count >= EXPLOSION_REQUIRE_BUBBLE_COUNT;
    }

    public bool ClearCheck()
    {
        return m_lstExistColorType[0] <= 0 && m_lstExistColorType[1] <= 0 && m_lstExistColorType[2] <= 0;
    }

    #endregion
}

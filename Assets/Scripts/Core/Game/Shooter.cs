using UnityEngine;
using Codejay.Datas;

[RequireComponent(typeof(CircleCollider2D))]
public class Shooter : MonoBehaviour
{
    private readonly Vector3 RaycastStartPosition = new Vector3(0, -2.216875f, 0);

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Bubble PreviewBubble;
    [SerializeField] private BulletBubble bulletBubble;
    [SerializeField] private BulletBubble Sub_bulletBubble;
    [SerializeField] private TMPro.TextMeshPro TMP;

    private int m_ShootChance = 20;
    private int m_layerMask_Object;
    private int m_layerMask_Bubble;
    private int m_layerMask_EmptyBubble;

    private bool m_IsReadyToShoot = false;

    private Bubble m_PrevTargetBubble;
    private Vector2 m_Direction;
    private Vector3[] m_LineRendererPoints = new Vector3[4];
    private Collider2D[] m_Colliders = new Collider2D[6];
    private Vector2 m_PrevTouchPos = new Vector2(0, -100);

    #region ## Behaviours ## 
    private void Start()
    {
        m_layerMask_Object = 1 << LayerMask.NameToLayer("Objects");
        m_layerMask_Bubble = 1 << LayerMask.NameToLayer("Bubble");
        m_layerMask_EmptyBubble = 1 << LayerMask.NameToLayer("EmptyBubble");

        Initialize();

        TMP.text = m_ShootChance.ToString();
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Managers.Instance == null || !Managers.Game.Controlable || Managers.Game.ClearCheck() || m_ShootChance <= 0)
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            m_PrevTouchPos = worldPos;
            if (worldPos.y > GameManager.SHOOT_BORDER)
            {
                if (!m_IsReadyToShoot)
                    ActiveTrajectory();
                Vector2 dir = (worldPos - RaycastStartPosition);
                dir = dir.normalized;
                if (dir != m_Direction)
                {
                    m_Direction = dir;
                    UpdateLineRenderer(m_Direction);
                }
            }
            else if (m_IsReadyToShoot)
                InActiveActiveTrajectory();
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Managers.Instance == null || !Managers.Game.Controlable)
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            worldPos.z = 0;
            m_PrevTouchPos = worldPos;
            if (worldPos.y > GameManager.SHOOT_BORDER)
            {
                if (!m_IsReadyToShoot)
                    ActiveTrajectory();
                Vector2 dir = (worldPos - RaycastStartPosition);
                dir = dir.normalized;
                if (dir != m_Direction)
                {
                    m_Direction = dir;
                    UpdateLineRenderer(m_Direction);
                }
            }
            else if (m_IsReadyToShoot)
                InActiveActiveTrajectory();
        }
#endif
        else
        {
            if (m_ShootChance > 0 && m_PrevTouchPos.y >= GameManager.SHOOT_BORDER)
            {
                ShootBubble();
            }
            InActiveActiveTrajectory();
        }
    }
    #endregion

    private void Initialize()
    {
        m_LineRendererPoints[0] = RaycastStartPosition;
        m_IsReadyToShoot = true;
        InActiveActiveTrajectory();
        InitializeBulletBubbles();
    }

    private void InitializeBulletBubbles()
    {
        // 풀 리스트 사용
        System.Collections.Generic.List<EBubbleColorType> lstColor = UnityEngine.Pool.ListPool<EBubbleColorType>.Get();

        for (EBubbleColorType i = 0; i < EBubbleColorType.Transparent; i++)
        {
            if (Managers.Game.IsExistBubbleColorType(i))
                lstColor.Add(i);
        }

        EBubbleColorType result = lstColor[Random.Range(0, lstColor.Count - 1)];

        if (lstColor.Count == 1)
        {
            EBubblePrefabType prefabType = Codejay.Utility.Utility.ColorTypeToNormalPrefabType(result);
            bulletBubble.SetData(prefabType);
            Sub_bulletBubble.SetData(prefabType);
        }
        else
        {
            EBubblePrefabType prefabType = Codejay.Utility.Utility.ColorTypeToNormalPrefabType(result);
            bulletBubble.SetData(prefabType);

            lstColor.Remove(result);
            result = lstColor[Random.Range(0, lstColor.Count - 1)];
            prefabType = Codejay.Utility.Utility.ColorTypeToNormalPrefabType(result);
            Sub_bulletBubble.SetData(prefabType);
        }

        UnityEngine.Pool.ListPool<EBubbleColorType>.Release(lstColor);
    }

    // 발사 궤적 활성화
    private void ActiveTrajectory()
    {
        if (m_IsReadyToShoot)
            return;

        m_IsReadyToShoot = true;
        lineRenderer.enabled = true;
        bulletBubble.Enabled(true);
    }

    // 발사 궤적 비활성화
    private void InActiveActiveTrajectory()
    {
        if (!m_IsReadyToShoot)
            return;

        m_Direction = Vector2.down;
        m_PrevTouchPos.y = -100;
        m_PrevTargetBubble = null;

        for (int i = 0; i < m_LineRendererPoints.Length; i++)
            m_LineRendererPoints[i] = RaycastStartPosition;

        PreviewBubbleEnabled(false);
        lineRenderer.SetPositions(m_LineRendererPoints);
        lineRenderer.enabled = false;

        m_IsReadyToShoot = false;
        bulletBubble.Enabled(false);
    }

    // LineRenderer 위치 업데이트
    private void UpdateLineRenderer(Vector2 dir, int depth = 1)
    {
        float epsilon = dir.x > 0 ? 0.01f : -0.01f;
        Vector2 raycastStart = new Vector2(m_LineRendererPoints[depth - 1].x + epsilon, m_LineRendererPoints[depth - 1].y);
        var hit = Physics2D.Raycast(raycastStart, dir, 10, m_layerMask_Object | m_layerMask_Bubble);

        if (hit == default(RaycastHit2D) || hit.transform == null)
        {
            return;
        }

        if (hit.transform.CompareTag("Wall"))
        {
            m_LineRendererPoints[depth] = hit.point;
            if (depth < 3)
            {
                UpdateLineRenderer(new Vector2(-dir.x, dir.y).normalized, depth + 1);
            }
            else
            {
                PreviewBubbleEnabled(true);
                SetPreviewBubblePosition(hit.point, dir);
            }
        }
        else if (hit.transform.CompareTag("Bubble"))
        {
            m_LineRendererPoints[depth] = hit.point;
            PreviewBubbleEnabled(true);
            SetPreviewBubblePosition(hit.point, dir);

            for (int i = depth; i < m_LineRendererPoints.Length; i++)
            {
                m_LineRendererPoints[i] = m_LineRendererPoints[depth];
            }
        }
        else
        {
            m_LineRendererPoints[depth] = m_LineRendererPoints[depth - 1];
        }

        if (depth == 1)
        {
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;
            lineRenderer.SetPositions(m_LineRendererPoints);
        }
    }

    // 미리보기 방울 켜/끄기
    private void PreviewBubbleEnabled(bool b)
    {
        PreviewBubble.gameObject.SetActive(b);

    }

    // 미리보기 방울 포지션 셋팅
    private void SetPreviewBubblePosition(Vector2 point, Vector2 inDirection)
    {
        // 원 레이케스팅
        int collidedNumber = Physics2D.OverlapCircleNonAlloc(point, GameManager.BUBBLE_RADIUS * 2, m_Colliders, m_layerMask_EmptyBubble);

        // 가장 가까운 비어있는 방울 찾기
        float temp = float.MaxValue;
        int index = 0;
        for (int i = 0; i < collidedNumber; i++)
        {
            if (m_Colliders[i].CompareTag("EmptyBubble") && m_Colliders[i].TryGetComponent<Bubble>(out Bubble bubble))
            {
                if (bubble.Data.BubbleColorType == EBubbleColorType.Transparent && bubble.Data.IsActivated == false)
                {
                    float dist = Vector2.Distance(point, m_Colliders[i].transform.position);
                    if (dist < temp)
                    {
                        temp = dist;
                        index = i;
                    }
                }
            }
        }

        Bubble target = m_Colliders[index].GetComponent<Bubble>();

        if (m_PrevTargetBubble == null || m_PrevTargetBubble.GetInstanceID() != target.GetInstanceID())
            m_PrevTargetBubble = target;

        PreviewBubble.transform.position = m_Colliders[index].transform.position;
    }

    // 방울 쏘기
    private void ShootBubble()
    {
        if (m_PrevTargetBubble == null)
            Debug.LogError($"{nameof(m_PrevTargetBubble)} Never be null");
        else
            Managers.Game.ShootBullet?.Invoke(bulletBubble.PrefabType, m_LineRendererPoints, m_PrevTargetBubble);

        bulletBubble.SetData(EBubblePrefabType.Max);
        m_ShootChance--;
        TMP.text = m_ShootChance.ToString();
        StartCoroutine(SubBulletBubbleChangeCoroutine());
    }

    // 불릿 발사 후 서브 불릿 교체
    private void ChangeBulletBubbleAfterShoot()
    {
        bulletBubble.SetData(Sub_bulletBubble.PrefabType);

        if (m_ShootChance >= 2)
        {
            // 풀 리스트 사용
            System.Collections.Generic.List<EBubbleColorType> lstColor = UnityEngine.Pool.ListPool<EBubbleColorType>.Get();

            for (EBubbleColorType i = 0; i < EBubbleColorType.Transparent; i++)
            {
                if (Managers.Game.IsExistBubbleColorType(i))
                    lstColor.Add(i);
            }

            if (lstColor.Count <= 0)
                return;

            EBubbleColorType result = lstColor[Random.Range(0, lstColor.Count - 1)];
            EBubblePrefabType prefabType = Codejay.Utility.Utility.ColorTypeToNormalPrefabType(result);

            if (lstColor.Count != 3 && !lstColor.Contains(bulletBubble.ColorType))
                bulletBubble.SetData(prefabType);
            Sub_bulletBubble.SetData(prefabType);
            UnityEngine.Pool.ListPool<EBubbleColorType>.Release(lstColor);
        }
        else
        {
            Sub_bulletBubble.SetData(EBubblePrefabType.Max);
        }
    }


    #region ## Coroutines ##
    // 방울 검사가 끝나거나 모두 터진 후 체크해야 되기 때문에 기다리기
    private System.Collections.IEnumerator SubBulletBubbleChangeCoroutine()
    {
        Managers.Game.ControlEnabled(false);
        yield return new WaitUntil(() => Managers.Game.ControlableCount == 1);
        Managers.Game.ControlEnabled(true);

        if (!Managers.Game.ClearCheck())
        {
            if (m_ShootChance <= 0)
            {
                Managers.UI.ShowPopup(UIManager.EPopupType.FailPopup);
            }
            else
            {
                Managers.Game.CheckFallableBubbles?.Invoke();
                ChangeBulletBubbleAfterShoot();
            }
        }
    }
    #endregion
}

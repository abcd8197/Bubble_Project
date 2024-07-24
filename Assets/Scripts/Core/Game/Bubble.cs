using System.Collections.Generic;
using UnityEngine;
using Codejay.Datas;

[System.Serializable]
public class Bubble : MonoBehaviour
{
    [SerializeField] private ParticleSystem ExplosionParticle;
    [SerializeField] private SpriteRenderer SpRenderer;
    [SerializeField] private bool IsPreviewBubble = false;

    public TMPro.TextMeshPro TMP;
    public List<Bubble> NearBubbles = new List<Bubble>();
    private Collider2D[] m_colliders = new Collider2D[7];

    private int m_BubbleMask;
    private Coroutine m_ExplosionCoroutine;
    //public BubbleData Data;
    public BubbleData Data { get; private set; }
    #region ## Behaviours ##
    private void Awake()
    {
        m_BubbleMask = 1 << LayerMask.NameToLayer("Bubble");
        m_ExplosionCoroutine = null;
    }
    public void OnDisable()
    {
        if (!IsPreviewBubble)
        {
            NearBubbles.Clear();
            for (int i = 0; i < m_colliders.Length; i++)
                m_colliders[i] = null;
        }

        if (m_ExplosionCoroutine != null)
            StopCoroutine(m_ExplosionCoroutine);
    }

    public override int GetHashCode()
    {
        return gameObject.name.GetHashCode();
    }
    #endregion

    public void SetEmpty(BubbleData data)
    {
        TMP.enabled = false;
        Data = data;
        Data.IsActivated = false;
        SpRenderer.color = new Color(1, 1, 1, 0);
        transform.tag = "EmptyBubble";
        gameObject.layer = LayerMask.NameToLayer("EmptyBubble");
    }

    public void SetData(BubbleData data)
    {
        TMP.enabled = true;
        Data = data;
        transform.tag = "Bubble";
        gameObject.layer = LayerMask.NameToLayer("Bubble");

        switch (data.BubbleColorType)
        {
            case EBubbleColorType.Red:
                SpRenderer.color = Color.red;
                break;
            case EBubbleColorType.Green:
                SpRenderer.color = Color.green;
                break;
            case EBubbleColorType.Blue:
                SpRenderer.color = Color.blue;
                break;
            case EBubbleColorType.Transparent:
                IsPreviewBubble = true;
                SpRenderer.color = new Color(1, 1, 1, 0.5f);
                break;
        }
    }

    public void Explosion()
    {
        ExplosionParticle.Play();
    }

    public void Fall()
    {
        Explosion();
        StartCoroutine(ExplostionCoroutine(0));
    }

    public void UpdateNearBubble()
    {
        NearBubbles.Clear();
        int collidedNumber = Physics2D.OverlapCircleNonAlloc(transform.position, GameManager.BUBBLE_RADIUS * 2, m_colliders, m_BubbleMask);

        for (int i = 0; i < collidedNumber; i++)
        {
            if (m_colliders[i].TryGetComponent<Bubble>(out Bubble bubble))
            {
                if (bubble.GetHashCode() != GetHashCode() && bubble.Data.IsActivated)
                    NearBubbles.Add(m_colliders[i].GetComponent<Bubble>());
            }
        }
    }

    public void ExplosionSameColorBubble(int depth = 0)
    {
        if (m_ExplosionCoroutine != null)
            return;

        for (int i = 0; i < NearBubbles.Count; i++)
        {
            if (NearBubbles[i].Data.BubbleColorType == Data.BubbleColorType)
            {
                if (Managers.Game.AddReservedExplosionBubble(NearBubbles[i].gameObject.GetHashCode()))
                    NearBubbles[i].ExplosionSameColorBubble(depth + 1);
            }
        }

        if (Managers.Game.Explosionable())
        {
            StartCoroutine(ExplostionCoroutine(depth * 0.05f));
        }
    }

    private System.Collections.IEnumerator ExplostionCoroutine(float delay)
    {
        Managers.Game.ControlEnabled(false);
        Data.IsActivated = false;

        float cnt = delay;
        while (cnt > 0f)
        {
            cnt -= Time.deltaTime;
            yield return null;
        }

        Explosion();
        Managers.Game.RemoveColorType(Data.BubbleColorType);
        Data.BubbleColorType = EBubbleColorType.Transparent;
        SetEmpty(Data);
        m_ExplosionCoroutine = null;

        if (Managers.Game.ClearCheck() && !Managers.UI.IsPopupOpend(UIManager.EPopupType.ClearPopup))
        {
            yield return new WaitForSeconds(0.5f);
            Managers.UI.ShowPopup(UIManager.EPopupType.ClearPopup);
        }

        Managers.Game.ControlEnabled(true);
    }
}

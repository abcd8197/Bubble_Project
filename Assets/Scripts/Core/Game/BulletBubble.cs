using UnityEngine;

public class BulletBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spRenderer;
    [SerializeField] private ParticleSystem particle;
    public Codejay.Datas.EBubblePrefabType PrefabType { get; private set; }
    public Codejay.Datas.EBubbleColorType ColorType { get; private set; } = Codejay.Datas.EBubbleColorType.Max;

    public void SetData(Codejay.Datas.EBubblePrefabType prefabType)
    {
        PrefabType = prefabType;

        // Setup Particle
        var main = particle.main;
        switch (PrefabType)
        {
            case Codejay.Datas.EBubblePrefabType.Red_Normal:
            case Codejay.Datas.EBubblePrefabType.Red_Fairy:
                main.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                spRenderer.color = Color.red;
                ColorType = Codejay.Datas.EBubbleColorType.Red;
                break;
            case Codejay.Datas.EBubblePrefabType.Green_Normal:
            case Codejay.Datas.EBubblePrefabType.Green_Fairy:
                main.startColor = new ParticleSystem.MinMaxGradient(Color.green);
                spRenderer.color = Color.green;
                ColorType = Codejay.Datas.EBubbleColorType.Green;
                break;
            case Codejay.Datas.EBubblePrefabType.Blue_Normal:
            case Codejay.Datas.EBubblePrefabType.Blue_Fairy:
                main.startColor = new ParticleSystem.MinMaxGradient(Color.blue);
                spRenderer.color = Color.blue;
                ColorType = Codejay.Datas.EBubbleColorType.Blue;
                break;
            case Codejay.Datas.EBubblePrefabType.Blackhole:
                main.startColor = new ParticleSystem.MinMaxGradient(Color.gray);
                spRenderer.color = Color.gray;
                ColorType = Codejay.Datas.EBubbleColorType.Blackhole;
                break;
            default:
                spRenderer.color = new Color(1, 1, 1, 0);
                break;
        }
    }


    public void Enabled(bool b)
    {
        if (b)
        {
            particle.Play(true);
        }
        else
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}

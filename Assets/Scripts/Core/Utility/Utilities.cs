namespace Codejay.Utility
{
    public static class Utility
    {
        public static Datas.EBubblePrefabType ColorTypeToNormalPrefabType(Datas.EBubbleColorType colorType)
        {
            switch (colorType)
            {
                case Datas.EBubbleColorType.Red:
                    return Datas.EBubblePrefabType.Red_Normal;
                case Datas.EBubbleColorType.Green:
                    return Datas.EBubblePrefabType.Green_Normal;
                case Datas.EBubbleColorType.Blue:
                    return Datas.EBubblePrefabType.Blue_Normal;
                default:
                    return Datas.EBubblePrefabType.Max;
            }
        }
    }
}
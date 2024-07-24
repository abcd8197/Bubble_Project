namespace Codejay.Datas
{
    public enum EBubbleColorType { Red = 0, Green, Blue, Transparent,Blackhole, Max }
    public enum EBubbleType { Normal = 0, Fairy, Blackhole }
    public enum EBubblePrefabType
    {
        Empty = 0,

        Red_Normal,
        Red_Fairy,

        Green_Normal,
        Green_Fairy,

        Blue_Normal,
        Blue_Fairy,

        Blackhole,

        Max
    }

    [System.Serializable]
    public class BubbleData
    {
        public EBubbleColorType BubbleColorType;
        public EBubbleType BubbleType;
        public int Row;
        public int Column;
        public bool IsActivated;
        public bool IsHighest;

        public BubbleData() { }

        public BubbleData(BubbleData data)
        {
            BubbleColorType = data.BubbleColorType;
            BubbleType = data.BubbleType;
            Row = data.Row;
            Column = data.Column;
            IsActivated = data.IsActivated;
        }

        public void SetData(EBubblePrefabType type, int row, int column, bool isHighest)
        {
            Row = row;
            Column = column;
            IsHighest = isHighest;

            switch (type)
            {
                case EBubblePrefabType.Red_Normal:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Red;
                    BubbleType = EBubbleType.Normal;
                    break;
                case EBubblePrefabType.Red_Fairy:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Red;
                    BubbleType = EBubbleType.Fairy;
                    break;
                case EBubblePrefabType.Green_Normal:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Green;
                    BubbleType = EBubbleType.Normal;
                    break;
                case EBubblePrefabType.Green_Fairy:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Green;
                    BubbleType = EBubbleType.Fairy;
                    break;
                case EBubblePrefabType.Blue_Normal:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Blue;
                    BubbleType = EBubbleType.Normal;
                    break;
                case EBubblePrefabType.Blue_Fairy:
                    IsActivated = true;
                    BubbleColorType = EBubbleColorType.Blue;
                    BubbleType = EBubbleType.Fairy;
                    break;
                case EBubblePrefabType.Blackhole:
                    IsActivated = true;
                    BubbleType = EBubbleType.Blackhole;
                    break;
                default:
                    IsActivated = false;
                    BubbleColorType = EBubbleColorType.Transparent;
                    BubbleType = EBubbleType.Normal;
                    break;
            }
        }
    }
}

using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdSpecialReward : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        主角继续前进的格子数
        */
        [ProtoMember(2)]
        public int gridNum { get; set; }

        /*
        奖励的类型（0:获取所有经过奖励，1：获得彩票，2：获得金币，3：获取jp保底值，4：获取随机颜色jp球）
        */
        [ProtoMember(3)]
        public int rewardType { get; set; }

        /*
        奖励的数值(注：为0不需要配)
        */
        [ProtoMember(4)]
        public int rewardNum { get; set; }

        /*
        获得不同颜色jp球或jp保底值的概率(注：前三项不需要配)
        */
        [ProtoMember(5)]
        public float[] rewardPro { get; set; }

        public override string ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdSpecialReward> StdSpecialReward { get; private set; }

    }
#endif
}

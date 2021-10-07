
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdRoadReward : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型（0:无奖励，1：彩票，2：游戏币）
        */
        [ProtoMember(2)]
        public int type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int rewardCount { get; set; }

        /*
        权重
        */
        [ProtoMember(4)]
        public int weight { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdRoadReward> StdRoadReward { get; private set; }

    }
#endif
}


using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdKardReward : Row<int>
    {

        /*
        键(哪一张卡牌)
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型（0：彩票，1：游戏币，2：jp球A，3：jp球B，4：jp球C，5：jp进度A，6：jp进度B,7:jp进度C，8：随机道具，9:JP关卡）
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

        /*
        等级(1低级，2高级)
        */
        [ProtoMember(5)]
        public int level { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdKardReward> StdKardReward { get; private set; }

    }
#endif
}

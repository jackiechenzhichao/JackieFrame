
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMenghuanqiuReward : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        梦幻球额外奖励抽选（0：彩票，1：游戏币，2：jp球A，3：jp球B，4：jp球C）
        */
        [ProtoMember(2)]
        public int type { get; set; }

        /*
        奖励值(如果不是彩票或游戏币)
        */
        [ProtoMember(3)]
        public int value { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdMenghuanqiuReward> StdMenghuanqiuReward { get; private set; }

    }
#endif
}

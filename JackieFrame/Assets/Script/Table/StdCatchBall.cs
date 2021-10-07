
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdCatchBall : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        目标球数
        */
        [ProtoMember(2)]
        public int ballNum { get; set; }

        /*
        奖励类型（0：彩票，1：金币）
        */
        [ProtoMember(3)]
        public int rewardType { get; set; }

        /*
        奖励数量
        */
        [ProtoMember(4)]
        public int rewardNum { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdCatchBall> StdCatchBall { get; private set; }

    }
#endif
}

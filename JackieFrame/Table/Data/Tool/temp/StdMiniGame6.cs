
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMiniGame6 : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int level { get; set; }

        /*
        奖励的数量
        */
        [ProtoMember(2)]
        public int[] rewardNum { get; set; }

        /*
        策划备注
        */
        [ProtoMember(3)]
        public string des { get; set; }

        public override int ID{ get { return level; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdMiniGame6> StdMiniGame6 { get; private set; }

    }
#endif
}

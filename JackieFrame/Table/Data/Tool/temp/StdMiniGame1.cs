
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMiniGame1 : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        奖励（0-彩票、1-游戏币）
        */
        [ProtoMember(2)]
        public int[] rewardType { get; set; }

        /*
        数量
        */
        [ProtoMember(3)]
        public int[] rewardNum { get; set; }

        /*
        策划备注
        */
        [ProtoMember(4)]
        public string des { get; set; }

        public override string ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdMiniGame1> StdMiniGame1 { get; private set; }

    }
#endif
}

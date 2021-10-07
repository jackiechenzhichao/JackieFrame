
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class MiniGame1 : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        奖励（0-无奖励、1-彩票、2-JP进度、3-游戏币、4-通关资格）
        */
        [ProtoMember(2)]
        public int[] rewards { get; set; }

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

        public static Table<string, MiniGame1> MiniGame1 { get; private set; }

    }
#endif
}

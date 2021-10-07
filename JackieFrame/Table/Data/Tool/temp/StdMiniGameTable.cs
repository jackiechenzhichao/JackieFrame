
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMiniGameTable : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        奖励类型（0：彩票，1：游戏币，2：jp球A，3：jp球B，4：jp球C，5：jp进度A，6：jp进度B,7:jp进度C，8：随机道具，9:JP关卡）
        */
        [ProtoMember(2)]
        public int[] type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int[] rewardCount { get; set; }

        /*
        策划描述
        */
        [ProtoMember(4)]
        public string des { get; set; }

        public override string ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdMiniGameTable> StdMiniGameTable { get; private set; }

    }
#endif
}

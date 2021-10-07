
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdJPChance : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        JP机会转盘奖励（奇数：（0：彩票，1：游戏币，2：jp球A，3：jp球B，4：jp球C，5：jp进度A，6：jp进度B,7:jp进度C，8：随机道具，9:JP关卡）偶数：奖励值）
        */
        [ProtoMember(2)]
        public int[] rewards { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdJPChance> StdJPChance { get; private set; }

    }
#endif
}

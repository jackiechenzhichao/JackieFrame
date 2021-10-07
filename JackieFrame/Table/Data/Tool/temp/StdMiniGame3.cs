
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMiniGame3 : Row<int>
    {

        /*
        键（难度）
        */
        [ProtoMember(1)]
        public int level { get; set; }

        /*
        道具的飞行速度
        */
        [ProtoMember(2)]
        public float flySpeed { get; set; }

        /*
        道具的数量
        */
        [ProtoMember(3)]
        public int flyerNum { get; set; }

        /*
        不同道具的概率（1：无道具，2：彩票，3：高额彩票，4：冲刺道具）
        */
        [ProtoMember(4)]
        public float[] weight { get; set; }

        public override int ID{ get { return level; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdMiniGame3> StdMiniGame3 { get; private set; }

    }
#endif
}

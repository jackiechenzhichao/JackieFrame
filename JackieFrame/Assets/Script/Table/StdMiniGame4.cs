
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdMiniGame4 : Row<string>
    {

        /*
        键（难度）
        */
        [ProtoMember(1)]
        public string level { get; set; }

        /*
        道具的数量
        */
        [ProtoMember(2)]
        public int propCount { get; set; }

        /*
        不同道具的概率（1：无道具，2：无敌药水，3：生命药水，4：特殊炸弹，5：子弹强化，6：高额彩票）
        */
        [ProtoMember(3)]
        public float[] weight { get; set; }

        /*
        策划备注
        */
        [ProtoMember(4)]
        public string des { get; set; }

        public override string ID{ get { return level; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdMiniGame4> StdMiniGame4 { get; private set; }

    }
#endif
}

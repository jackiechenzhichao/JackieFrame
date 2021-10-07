
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffAction : Row<int>
    {

        /*
        Buff行为ID
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        说明
        */
        [ProtoMember(2)]
        public string Explain { get; set; }

        /*
        组别(相同的action会互斥)
        */
        [ProtoMember(3)]
        public byte Group { get; set; }

        /*
        触发时机类型
        */
        [ProtoMember(4)]
        public byte[] TriggerType { get; set; }

        /*
        效果类型
        */
        [ProtoMember(5)]
        public byte ActionType { get; set; }

        /*
        伤害
        */
        [ProtoMember(6)]
        public int ParamID { get; set; }

        /*
        增加buff
        */
        [ProtoMember(7)]
        public int[] AddBuffIDs { get; set; }

        /*
        参数
        */
        [ProtoMember(8)]
        public int[] ParamsInt { get; set; }

        /*
        参数
        */
        [ProtoMember(9)]
        public float[] ParamsFloat { get; set; }

        /*
        参数
        */
        [ProtoMember(10)]
        public string[] ParamsString { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffAction> CBuffAction { get; private set; }

    }
#endif
}

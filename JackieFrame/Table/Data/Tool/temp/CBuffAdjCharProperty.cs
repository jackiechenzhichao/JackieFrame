
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffAdjCharProperty : Row<int>
    {

        /*
        调整角色属性链接ID
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        1为叠加；0为替换
        */
        [ProtoMember(2)]
        public int IsAddtive { get; set; }

        /*
        属性ID
        */
        [ProtoMember(3)]
        public int PropertyID { get; set; }

        /*
        万分比
        */
        [ProtoMember(4)]
        public int Param1 { get; set; }

        /*
        固定值
        */
        [ProtoMember(5)]
        public int Param2 { get; set; }

        /*
        音效
        */
        [ProtoMember(6)]
        public string Sound { get; set; }

        /*
        描述
        */
        [ProtoMember(7)]
        public string Explain { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffAdjCharProperty> CBuffAdjCharProperty { get; private set; }

    }
#endif
}

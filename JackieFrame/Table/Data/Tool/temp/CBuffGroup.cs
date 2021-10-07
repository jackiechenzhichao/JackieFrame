
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffGroup : Row<int>
    {

        /*
        组类别
        */
        [ProtoMember(1)]
        public int GroupID { get; set; }

        /*
        说明
        */
        [ProtoMember(2)]
        public string desc { get; set; }

        /*
        最大数量
        */
        [ProtoMember(3)]
        public int MaxNum { get; set; }

        public override int ID{ get { return GroupID; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffGroup> CBuffGroup { get; private set; }

    }
#endif
}


using ProtoBuf;

namespace fx
{
    [ProtoContract]
    public class GlobalValue : Row<int>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        值
        */
        [ProtoMember(2)]
        public string value { get; set; }

        /*
        策划备注
        */
        [ProtoMember(3)]
        public string desc { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, GlobalValue> GlobalValue { get; private set; }

    }
#endif
}

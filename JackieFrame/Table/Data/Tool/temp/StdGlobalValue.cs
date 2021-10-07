
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdGlobalValue : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        值
        */
        [ProtoMember(2)]
        public int[] value { get; set; }

        /*
        策划备注
        */
        [ProtoMember(3)]
        public string desc { get; set; }

        public override string ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdGlobalValue> StdGlobalValue { get; private set; }

    }
#endif
}

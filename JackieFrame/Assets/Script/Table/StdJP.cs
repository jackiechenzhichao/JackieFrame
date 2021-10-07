
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdJP : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        奖励列表
        */
        [ProtoMember(2)]
        public int[] rewards { get; set; }

        /*
        策划备注
        */
        [ProtoMember(3)]
        public string des { get; set; }

        public override string ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<string, StdJP> StdJP { get; private set; }

    }
#endif
}

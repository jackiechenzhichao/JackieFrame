
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class StdGridReward : Row<string>
    {

        /*
        键
        */
        [ProtoMember(1)]
        public string id { get; set; }

        /*
        奖励列表(id:奖励唯一id,count:数量,weight:权重)
        */
        [ProtoMember(2)]
        public gridReward[] gridRewards { get; set; }

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

        public static Table<string, StdGridReward> StdGridReward { get; private set; }

    }
#endif
}

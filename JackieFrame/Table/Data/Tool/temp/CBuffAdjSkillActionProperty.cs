
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffAdjSkillActionProperty : Row<int>
    {

        /*
        
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        技能ID
        */
        [ProtoMember(2)]
        public int SkillID { get; set; }

        /*
        Action索引
        */
        [ProtoMember(3)]
        public int ActionIDX { get; set; }

        /*
        叠加的Action表ID
        */
        [ProtoMember(4)]
        public int AddtiveActionID { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffAdjSkillActionProperty> CBuffAdjSkillActionProperty { get; private set; }

    }
#endif
}


using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffAdjSkillSelectProperty : Row<int>
    {

        /*
        调整技能select属性链接ID
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        技能ID
        */
        [ProtoMember(2)]
        public int SkillID { get; set; }

        /*
        select索引
        */
        [ProtoMember(3)]
        public int SelectIdx { get; set; }

        /*
        属性ID
        */
        [ProtoMember(4)]
        public int PropertyID { get; set; }

        /*
        优先级
        */
        [ProtoMember(5)]
        public int Priority { get; set; }

        /*
        int值
        */
        [ProtoMember(6)]
        public int Value0 { get; set; }

        /*
        float值
        */
        [ProtoMember(7)]
        public float Value1 { get; set; }

        /*
        int[]值
        */
        [ProtoMember(8)]
        public int[] Value2 { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffAdjSkillSelectProperty> CBuffAdjSkillSelectProperty { get; private set; }

    }
#endif
}

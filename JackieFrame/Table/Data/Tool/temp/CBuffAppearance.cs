
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuffAppearance : Row<int>
    {

        /*
        
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        名称
        */
        [ProtoMember(2)]
        public string Name { get; set; }

        /*
        表现是否互斥
        */
        [ProtoMember(3)]
        public byte Mutex { get; set; }

        /*
        优先级
        */
        [ProtoMember(4)]
        public int Priority { get; set; }

        /*
        添加buff声效
        */
        [ProtoMember(5)]
        public string AddSound { get; set; }

        /*
        移除buff声效
        */
        [ProtoMember(6)]
        public string RemoveSound { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuffAppearance> CBuffAppearance { get; private set; }

    }
#endif
}

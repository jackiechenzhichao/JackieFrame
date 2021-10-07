
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class Sheet1 : Row<int>
    {

        /*
        注释0
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        注释1
        */
        [ProtoMember(2)]
        public int TestInt { get; set; }

        /*
        注释2
        */
        [ProtoMember(3)]
        public float TestFloat { get; set; }

        /*
        注释3
        */
        [ProtoMember(4)]
        public bool TestBool { get; set; }

        /*
        注释4
        */
        [ProtoMember(5)]
        public string TestString { get; set; }

        /*
        注释5
        */
        [ProtoMember(6)]
        public vector3 TestVector3 { get; set; }

        /*
        
        */
        [ProtoMember(7)]
        public vector3[] VecArr { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, Sheet1> Sheet1 { get; private set; }

    }
#endif
}

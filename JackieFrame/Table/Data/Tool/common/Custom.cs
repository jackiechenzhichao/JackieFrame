/*
 * 
 *  自定义的结构在这里申请
 * 
 */
using ProtoBuf;

namespace fs
{
    [ProtoContract]
    public class gridReward
    {
        [ProtoMember(1)]
        public int id;

        [ProtoMember(2)]
        public int count;

        [ProtoMember(3)]
        public int weight;
    }
}

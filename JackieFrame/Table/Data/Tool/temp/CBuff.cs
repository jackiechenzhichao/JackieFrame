
using ProtoBuf;

namespace dc
{
    [ProtoContract]
    public class CBuff : Row<int>
    {

        /*
        实体id
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        实体名称
        */
        [ProtoMember(2)]
        public string Name { get; set; }

        /*
        状态
        */
        [ProtoMember(3)]
        public int State { get; set; }

        /*
        改变状态
        */
        [ProtoMember(4)]
        public int AttrState { get; set; }

        /*
        是否能被移除
        */
        [ProtoMember(5)]
        public byte CanRemove { get; set; }

        /*
        优先级
(值越小优先级越高)
        */
        [ProtoMember(6)]
        public int RemovePriority { get; set; }

        /*
        是否隐藏
        */
        [ProtoMember(7)]
        public byte Hide { get; set; }

        /*
        添加Buff时的特效ID
        */
        [ProtoMember(8)]
        public int AddEffectID { get; set; }

        /*
        移除Buff时的特效ID
        */
        [ProtoMember(9)]
        public int RemoveEffectID { get; set; }

        /*
        飘字
        */
        [ProtoMember(10)]
        public string FloatTips { get; set; }

        /*
        图标
        */
        [ProtoMember(11)]
        public string Icon { get; set; }

        /*
        类型
0：增益
1：减益
        */
        [ProtoMember(12)]
        public byte Type { get; set; }

        /*
        是否可驱散
0：不可驱散
1：可以驱散
        */
        [ProtoMember(13)]
        public byte Dispel { get; set; }

        /*
        生效几率（万分比）
        */
        [ProtoMember(14)]
        public int Ratio { get; set; }

        /*
        是否计算效果命中
0：不计算
1：计算
        */
        [ProtoMember(15)]
        public byte CalEffectHit { get; set; }

        /*
        持续回合
        */
        [ProtoMember(16)]
        public int Round { get; set; }

        /*
        生效次数
(生效次数达到该数值后，buff效果清除，如：战续次数)
        */
        [ProtoMember(17)]
        public byte ApplyTimes { get; set; }

        /*
        持续时间（单位：s，一般用于活动，在时间内对属性生效）
        */
        [ProtoMember(18)]
        public float Duration { get; set; }

        /*
        组类别
        */
        [ProtoMember(19)]
        public int GroupType { get; set; }

        /*
        相同ID的最大实例数
        */
        [ProtoMember(20)]
        public int SameIdMaxInst { get; set; }

        /*
        相同实例是否堆叠（层数显示）
        */
        [ProtoMember(21)]
        public byte Stacking { get; set; }

        /*
        buff行为
        */
        [ProtoMember(22)]
        public int[] Actions { get; set; }

        /*
        调整角色属性
        */
        [ProtoMember(23)]
        public int[] AdjCharProperty { get; set; }

        /*
        调整技能select属性
        */
        [ProtoMember(24)]
        public int[] AdjSkillSelectProperty { get; set; }

        /*
        调整技能action属性
        */
        [ProtoMember(25)]
        public int[] AdjSkillActionProperty { get; set; }

        /*
        响应时间效果
        */
        [ProtoMember(26)]
        public int[] EventActions { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, CBuff> CBuff { get; private set; }

    }
#endif
}

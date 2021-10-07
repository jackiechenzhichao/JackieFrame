using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

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

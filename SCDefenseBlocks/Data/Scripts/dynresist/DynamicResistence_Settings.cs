using ProtoBuf;

namespace StarCore.DynamicResistence
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class DynaResistBlockSettings
    {
        [ProtoMember(1)]
        public float FieldPower;

        [ProtoMember(2)]
        public float Modifier;

        [ProtoMember(3)]
        public bool SiegeModeActivated;
    }
}

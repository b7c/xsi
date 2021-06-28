#load "../xsi-loader.csx"

using Xabbo.Messages;

static T Read<T>(this IPacket packet) {
    Type t = typeof(T);
    switch (Type.GetTypeCode(t)) {
        case TypeCode.Boolean: return (T)Convert.ChangeType(packet.ReadBool(), t);
        case TypeCode.Byte: return (T)Convert.ChangeType(packet.ReadByte(), t);
        case TypeCode.Int16: return (T)Convert.ChangeType(packet.ReadShort(), t);
        case TypeCode.Int32: return (T)Convert.ChangeType(packet.ReadInt(), t);
        case TypeCode.Int64: return (T)Convert.ChangeType(packet.ReadLong(), t);
        case TypeCode.String: return (T)Convert.ChangeType(packet.ReadString(), t);
        case TypeCode.Single: return (T)Convert.ChangeType(packet.ReadFloat(), t);
        default: {
            if (t == typeof(LegacyLong)) return (T)Convert.ChangeType(packet.ReadLegacyLong(), t);
            else if (t == typeof(LegacyShort)) return (T)Convert.ChangeType(packet.ReadLegacyShort(), t);
            else if (t == typeof(LegacyFloat)) return (T)Convert.ChangeType(packet.ReadLegacyFloat(), t);
            throw new Exception($"Invalid type specified: {typeof(T)}.");
        } 
    };
}

static IPacket Write<T>(this IPacket packet, T value) {
    packet.WriteValues(value);
    return packet;
}
#load "../xsi-loader.csx"
#load "../util/packet-generics.csx"

using Xabbo.Messages;

// to be updated in Xabbo libraries

// ReplaceValues -> Replace
static void Replace(this IPacket packet, params object[] values)
    => packet.ReplaceValues(values);

static void ReplaceAt(this IPacket packet, int position, params object[] values) {
    packet.Position = position;
    packet.Replace(values);
}

// ReplaceString -> Replace
static void Replace<T>(this IPacket packet, Func<T, T> modifier) {
    if (modifier is Func<string, string> transform) {
        Replace(packet, transform);
    } else {
        int position = packet.Position;
        T value = packet.Read<T>();
        packet.Position = position;
        packet.Write(modifier(value));
    }
}

static void ReplaceAt<T>(this IPacket packet, int position, Func<T, T> modifier) {
    packet.Position = position;
    packet.Replace(modifier);
}

// Default to string replacement
static void Replace(this IPacket packet, Func<string, string> transform) {
    packet.ReplaceString(transform);
}

static void ReplaceAt(this IPacket packet, int position, Func<string, string> transform) {
    packet.ReplaceString(transform, position);
}
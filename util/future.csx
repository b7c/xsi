#load "../xsi-loader.csx"
#load "../util/packet-generics.csx"

using Xabbo.Messages;

// to be updated in Xabbo libraries

// ReplaceValues -> Replace
static void Replace(this IPacket packet, params object[] values)
    => packet.ReplaceValues(values);

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

// Default to string replacement
static void Replace(this IPacket packet, Func<string, string> transform) {
    packet.ReplaceString(transform);
}
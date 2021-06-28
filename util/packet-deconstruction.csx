#load "../xsi-loader.csx"
#load "packet-generics.csx"

using Xabbo.Messages;

// Provides packet deconstruction syntax
// Read 2 ints x and y
//   var (x, y) = packet.Read<int, int>();
// Read an int, a string, skip an int, read an int
//   var (index, message, _, bubble) = packet.Read<int, string, int, int>();

static (T1, T2)
Read<T1, T2>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p)
);

static (T1, T2, T3)
Read<T1, T2, T3>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p)
);

static (T1, T2, T3, T4)
Read<T1, T2, T3, T4>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p)
);

static (T1, T2, T3, T4, T5)
Read<T1, T2, T3, T4, T5>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p)
);

static (T1, T2, T3, T4, T5, T6)
Read<T1, T2, T3, T4, T5, T6>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p)
);

static (T1, T2, T3, T4, T5, T6, T7)
Read<T1, T2, T3, T4, T5, T6, T7>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8)
Read<T1, T2, T3, T4, T5, T6, T7, T8>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB, TC)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p), Read<TC>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB, TC, TD)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p), Read<TC>(p),
    Read<TD>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB, TC, TD, TE)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p), Read<TC>(p),
    Read<TD>(p), Read<TE>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB, TC, TD, TE, TF)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p), Read<TC>(p),
    Read<TD>(p), Read<TE>(p), Read<TF>(p)
);

static (T1, T2, T3, T4, T5, T6, T7, T8,
        T9, TA, TB, TC, TD, TE, TF, T10)
Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, T10>(this IPacket p) => (
    Read<T1>(p), Read<T2>(p), Read<T3>(p), Read<T4>(p),
    Read<T5>(p), Read<T6>(p), Read<T7>(p), Read<T8>(p),
    Read<T9>(p), Read<TA>(p), Read<TB>(p), Read<TC>(p),
    Read<TD>(p), Read<TE>(p), Read<TF>(p), Read<T10>(p)
);
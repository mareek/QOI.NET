using System;

namespace QOI.Core;

public struct QoiColor : IEquatable<QoiColor>
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    private QoiColor(byte a, byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static QoiColor FromArgb(byte a, byte r, byte g, byte b) => new(a, r, g, b);
    public static QoiColor FromRgb(byte r, byte g, byte b) => new(255, r, g, b);

    public override bool Equals(object? obj) => obj is QoiColor color && Equals(color);
    public bool Equals(QoiColor other) => R == other.R && G == other.G && B == other.B && A == other.A;
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);
    public static bool operator ==(QoiColor left, QoiColor right) => left.Equals(right);
    public static bool operator !=(QoiColor left, QoiColor right) => !(left == right);
}

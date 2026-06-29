using System.Numerics;

namespace DieselExileTools.Common.Structs;

public struct DXTPadding
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
    public readonly int Horizontal => Left + Right;
    public readonly int Vertical => Top + Bottom;
    public DXTPadding(int left, int top, int right, int bottom) {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public DXTPadding(int all) : this(all, all, all, all) { }
    public DXTPadding(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical) { }

    public readonly bool Equals(DXTPadding other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    public override bool Equals(object? obj) => obj is DXTPadding other && Equals(other);
    public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
    public static bool operator ==(DXTPadding a, DXTPadding b) => a.Equals(b);
    public static bool operator !=(DXTPadding a, DXTPadding b) => !a.Equals(b);
    public readonly void Deconstruct(out int left, out int top, out int right, out int bottom) {
        left = Left; top = Top; right = Right; bottom = Bottom;
    }

    public readonly Vector4 ToVector4() => new(Left, Top, Right, Bottom);

    public readonly override string ToString() => $"Padding(L:{Left}, T:{Top}, R:{Right}, B:{Bottom})";

}
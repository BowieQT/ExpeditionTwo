using System.Drawing;
using System.Numerics;

namespace DieselExileTools.Common.Structs;

public struct Rect
{

    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public float Width {
        get => Right - Left;
        set => Right = Left + value;
    }
    public float Height {
        get => Bottom - Top;
        set => Bottom = Top + value;
    }

    public Vector2 TopLeft {
        get => new(Left, Top);
        set { Left = value.X; Top = value.Y; }
    }
    public Vector2 TopRight {
        get => new(Right, Top);
        set { Right = value.X; Top = value.Y; }
    }
    public Vector2 BottomLeft {
        get => new(Left, Bottom);
        set { Left = value.X; Bottom = value.Y; }
    }
    public Vector2 BottomRight {
        get => new(Right, Bottom);
        set { Right = value.X; Bottom = value.Y; }
    }


    public Vector2 Size => new(Width, Height);
    public Vector2 Center => new((Left + Right) * 0.5f, (Top + Bottom) * 0.5f);
    public Vector2 CenterRounded => new(MathF.Round(Center.X), MathF.Round(Center.Y));


    public Rect(float left, float top, float right, float bottom) {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public Rect(int left, int top, int right, int bottom) {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public Rect(Vector2 topLeft, Vector2 bottomRight) {
        Left = topLeft.X;
        Top = topLeft.Y;
        Right = bottomRight.X;
        Bottom = bottomRight.Y;
    }
    public Rect(Vector2 topLeft, float width, float height) {
        Left = topLeft.X;
        Top = topLeft.Y;
        Right = topLeft.X + width;
        Bottom = topLeft.Y + height;
    }
    public Rect(Vector2 topLeft, int width, int height) {
        Left = topLeft.X;
        Top = topLeft.Y;
        Right = topLeft.X + width;
        Bottom = topLeft.Y + height;
    }
    public static Rect FromTopRight(Vector2 topRight, float width, float height) {
        return new Rect(topRight.X - width, topRight.Y, topRight.X, topRight.Y + height);
    }
    public static Rect FromBottomLeft(Vector2 bottomLeft, float width, float height) {
        return new Rect(bottomLeft.X, bottomLeft.Y - height, bottomLeft.X + width, bottomLeft.Y);
    }
    public static Rect FromBottomRight(Vector2 bottomRight, float width, float height) {
        return new Rect(bottomRight.X - width, bottomRight.Y - height, bottomRight.X, bottomRight.Y);
    }

    public override string ToString() => $"Rect(L:{Left}, T:{Top}, R:{Right}, B:{Bottom}, W:{Width}, H:{Height})";

    public Rect Shrink(float amount) {
        return new Rect(Left + amount, Top + amount, Right - amount, Bottom - amount);
    }
    public Rect Expand(float amount) {
        return new Rect(Left - amount, Top - amount, Right + amount, Bottom + amount);
    }
    public Rect Pad(float left, float top, float right, float bottom) {
        return new Rect(Left + left, Top + top, Right - right, Bottom - bottom);
    }
    public Rect Move(Vector2 delta) {
        return new Rect(Left + delta.X, Top + delta.Y, Right + delta.X, Bottom + delta.Y);
    }
    public bool Contains(Vector2 point) {
        return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
    }
    public bool Contains(Point point){
        return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
    }

    public bool Intersects(Rect other) {
        return Left < other.Right && Right > other.Left &&
               Top < other.Bottom && Bottom > other.Top;
    }

}
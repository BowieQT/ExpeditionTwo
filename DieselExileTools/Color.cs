using SColor = System.Drawing.Color;
using SVector4 = System.Numerics.Vector4;

namespace DieselExileTools.Common;

public static partial class Extensions {
    /// <summary>
    /// Converts a <see cref="System.Drawing.Color"/> to a <see cref="SVector4"/> with normalized RGBA components (0–1 range).
    /// Useful for ImGui and mathematical operations.
    /// </summary>
    public static SVector4 ToVector4(this SColor c) =>
        new SVector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

    /// <summary>
    /// Converts a <see cref="System.Drawing.Color"/> to a packed 32-bit unsigned integer in ImGui's expected format (AARRGGBB).
    /// </summary>
    public static uint ToImGui(this SColor c) =>
        (uint)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);

    /// <summary> Converts a System.Drawing.Color to a HSLA tuple. </summary>
    /// <returns>
    /// h = Hue in degrees [0–360]
    /// <para>s = Saturation [0–1]</para>
    /// <para>l = Lightness [0–1]</para>
    /// <para>a = Alpha [0–1]</para>
    /// </returns>
    public static (float h, float s, float l, float a) ToHSLA(this SColor c) {
        float r = c.R / 255f;
        float g = c.G / 255f;
        float b = c.B / 255f;
        float aVal = c.A / 255f;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float lVal = (max + min) / 2f;

        float hVal, sVal;

        if (max == min) {
            hVal = 0f;
            sVal = 0f;
        }
        else {
            float d = max - min;
            sVal = lVal > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r) hVal = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g) hVal = (b - r) / d + 2f;
            else hVal = (r - g) / d + 4f;

            hVal *= 60f; // degrees
        }

        return (hVal, sVal, lVal, aVal);
    }

    public static string ToColorCode(this SColor c) {
        return $"|c{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
    }

    public static string ToHEX(this SColor color) {
        // If alpha is fully opaque, output #RRGGBB
        if (color.A == 255)
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        // Otherwise, output #RRGGBBAA
        else
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }
}

public static partial class DXTC {
    public static class Color
    {
        /// <summary>
        /// Creates a <see cref="System.Drawing.Color"/> from HSLA (Hue [0–360], Saturation [0–1], Lightness [0–1], Alpha [0–1]) values.
        /// </summary>
        /// <param name="h">Hue in degrees [0–360]</param>
        /// <param name="s">Saturation [0–1]</param>
        /// <param name="l">Lightness [0–1]</param>
        /// <param name="a">Alpha [0–1], optional (default is 1)</param>
        /// <returns>A <see cref="System.Drawing.Color"/> representing the specified HSLA values.</returns>
        public static SColor FromHSLA(float h, float s, float l, float a = 1f) {
            h = (h % 360 + 360) % 360;
            s = Math.Clamp(s, 0f, 1f);
            l = Math.Clamp(l, 0f, 1f);
            a = Math.Clamp(a, 0f, 1f);

            float c = (1f - Math.Abs(2f * l - 1f)) * s;
            float x = c * (1f - Math.Abs((h / 60f) % 2f - 1f));
            float m = l - c / 2f;

            float r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; }
            else if (h < 120) { r = x; g = c; }
            else if (h < 180) { g = c; b = x; }
            else if (h < 240) { g = x; b = c; }
            else if (h < 300) { r = x; b = c; }
            else { r = c; b = x; }

            return SColor.FromArgb(
                (int)(a * 255),
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255)
            );
        }

        /// <summary>
        /// Parses a hexadecimal color string (#RRGGBB or #RRGGBBAA) and returns a <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="hex">Hex string representing the color.</param>
        /// <returns>A <see cref="System.Drawing.Color"/> parsed from the hex string.</returns>
        public static SColor FromHEX(string hex) {
            hex = hex.Replace("#", "");
            if (hex.Length == 6) hex += "FF"; // default alpha 255

            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            byte a = Convert.ToByte(hex.Substring(6, 2), 16);

            return SColor.FromArgb(a, r, g, b);
        }

        public static SColor FromRGBA(int r, int g, int b, int a = 255) => SColor.FromArgb(a, r, g, b);

    }
}

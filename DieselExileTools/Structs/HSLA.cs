using SColor = System.Drawing.Color;

namespace DieselExileTools.Common.Structs;
public struct HSLA {

    private float _h, _s, _l, _a;

    public float H {
        get => _h;
        set => _h = Math.Clamp(value, 0f, 360f);
    }
    public float S {
        get => _s;
        set => _s = Math.Clamp(value, 0f, 1f);
    }
    public float L {
        get => _l;
        set => _l = Math.Clamp(value, 0f, 1f);
    }
    public float A {
        get => _a;
        set => _a = Math.Clamp(value, 0f, 1f);
    }

    public float SPercent {
        get => _s * 100f;
        set => S = value / 100f;
    }
    public float LPercent {
        get => _l * 100f;
        set => L = value / 100f;
    }
    public float APercent {
        get => _a * 100f;
        set => A = value / 100f;
    }

    public HSLA(float h, float s, float l, float a = 1f) {
        _h = ((h % 360f) + 360f) % 360f;
        _s = Math.Clamp(s, 0f, 1f);
        _l = Math.Clamp(l, 0f, 1f);
        _a = Math.Clamp(a, 0f, 1f);
    }

    // equality 
    private const float Tolerance = 1f / 255f; // 8-bit color precision
    public static bool operator ==(HSLA left, HSLA right) {
        return Math.Abs(left.H - right.H) < Tolerance &&
                Math.Abs(left.S - right.S) < Tolerance &&
                Math.Abs(left.L - right.L) < Tolerance &&
                Math.Abs(left.A - right.A) < Tolerance;
    }
    public static bool operator !=(HSLA left, HSLA right) {
        return !(left == right);
    }
    public readonly override bool Equals(object obj) {
        return obj is HSLA other && this == other;
    }
    public override int GetHashCode() {
        // use HashCode.Combine for simplicity
        return HashCode.Combine(H, S, L, A);
    }


    /// <summary>
    /// Creates an <see cref="HSLA"/> color from percent[0-100] values for saturation, lightness, and alpha.
    /// </summary>
    /// <param name="h">Hue in degrees (0-360).</param>
    /// <param name="sPercent">Saturation as a percent (0-100).</param>
    /// <param name="lPercent">Lightness as a percent (0-100).</param>
    /// <param name="aPercent">Alpha as a percent (0-100, default is 100).</param>
    /// <returns>HSLA color</returns>
    public static HSLA FromPercent(float h, float sPercent, float lPercent, float aPercent = 100f) {
        return new HSLA(h, sPercent / 100f, lPercent / 100f, aPercent / 100f);
    }

    public SColor ToRGBA() {
        float c = (1f - Math.Abs(2f * L - 1f)) * S;
        float hPrime = H / 60f;
        float x = c * (1f - Math.Abs(hPrime % 2f - 1f));
        float m = L - c / 2f;
        float r1, g1, b1;
        if (hPrime >= 0f && hPrime < 1f) {
            r1 = c; g1 = x; b1 = 0f;
        }
        else if (hPrime >= 1f && hPrime < 2f) {
            r1 = x; g1 = c; b1 = 0f;
        }
        else if (hPrime >= 2f && hPrime < 3f) {
            r1 = 0f; g1 = c; b1 = x;
        }
        else if (hPrime >= 3f && hPrime < 4f) {
            r1 = 0f; g1 = x; b1 = c;
        }
        else if (hPrime >= 4f && hPrime < 5f) {
            r1 = x; g1 = 0f; b1 = c;
        }
        else if (hPrime >= 5f && hPrime < 6f) {
            r1 = c; g1 = 0f; b1 = x;
        }
        else {
            r1 = 0f; g1 = 0f; b1 = 0f;
        }
        byte r = (byte)Math.Round((r1 + m) * 255);
        byte g = (byte)Math.Round((g1 + m) * 255);
        byte b = (byte)Math.Round((b1 + m) * 255);
        byte a = (byte)Math.Round(A * 255);
        return SColor.FromArgb(a, r, g, b);
    }

    public readonly uint ToImGui() {
        var rgba = ToRGBA();
        return ((uint)rgba.R) | ((uint)rgba.G << 8) | ((uint)rgba.B << 16) | ((uint)rgba.A << 24);
    }



}





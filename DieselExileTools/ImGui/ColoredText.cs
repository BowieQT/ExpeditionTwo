using DieselExileTools.Common;
using ExileCore2;
using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;
using Graphics = ExileCore2.Graphics;
using DieselExileTools.Common.Structs;

namespace DieselExileTools;
public static partial class DXT {
    public readonly struct ColorSegment : IEquatable<ColorSegment>
    {
        public string Text { get; }
        public SColor Color { get; }
        public ColorSegment(string text, SColor color) { Text = text; Color = color; }
        /// <summary>Creates a new segment with the provided text, defaulting to the Draw call's default color.</summary>
        public ColorSegment(string text) : this(text, SColor.Empty) { }
        public bool Equals(ColorSegment other) => Text == other.Text && Color.Equals(other.Color);
        public override bool Equals(object? obj) => obj is ColorSegment other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Text, Color);
    }
    public record ColoredTextOptions {
        public SColor DefaultColor { get; set; } = SColor.White;
        public SColor BgColor { get; set; } = SColor.Transparent;
        public SColor BorderColor { get; set; } = SColor.Transparent; 
        public DXTPadding Padding { get; set; } = new DXTPadding(1);
        public int Rounding { get; set; } = 0;
        public int BorderThickness { get; set; } = 1; 

        public static readonly ColoredTextOptions Default = new();
    }

    public class ColoredText
    {
        private readonly List<ColorSegment> _segments = new();
        public IReadOnlyList<ColorSegment> Segments => _segments;
        private readonly Graphics _graphics;

        /// <summary>Gets the total width of the colored text in pixels (cached after first calculation).</summary>
        public float Width => _segments.Sum(seg => _graphics.MeasureText(seg.Text).X);
        /// <summary>Gets the height of the colored text in pixels (cached after first calculation).</summary>
        public float Height => _graphics.MeasureText("A").Y;

        /// <summary>Gets the total size (width and height) of the colored text in pixels (cached after first calculation).</summary>
        public SVector2 Size => new(Width, Height);

        /// <summary>Gets the number of color segments in this text.</summary>
        public int Count => Segments.Count;

        public void Add(string text, SColor color = default) {
            _segments.Add(new ColorSegment(text, color));
        }

        public ColoredText(Graphics graphics) {
            _graphics = graphics;
        }

        // Pass the graphics instance to all constructors
        public ColoredText(Graphics graphics, string input) : this(graphics) {
            if (input != null) _segments.AddRange(ParseSegments(input));
        }

        public ColoredText(Graphics graphics, IEnumerable<ColorSegment> segments) : this(graphics) {
            _segments.AddRange(segments);
        }

        public ColoredText(Graphics graphics, string input, SColor color) : this(graphics) {
            _segments.Add(new ColorSegment(input ?? string.Empty, color));
        }

        /// <summary>Returns the concatenated plain text (without color information).</summary>
        public string ToUncoloredString() {
            var sb = new System.Text.StringBuilder();
            foreach (var seg in Segments)
                sb.Append(seg.Text);
            return sb.ToString();
        }
        /// <summary>Returns the concatenated plain text (without color information).</summary>
        public override string ToString() => ToUncoloredString();

        /// <summary>
        /// Draws the colored text with optional background and border styling.
        /// </summary>
        /// <param name="g">The graphics interface.</param>
        /// <param name="pos">The starting screen position.</param>
        /// <param name="options">Styling configuration; uses defaults if null.</param>
        /// <returns>The total size of the drawn element.</returns>
        public SVector2 Draw(SVector2 pos, ColoredTextOptions options = null) {
            options ??= ColoredTextOptions.Default;

            float width = 0;
            foreach (var seg in Segments) width += _graphics.MeasureText(seg.Text).X;
            float height = _graphics.MeasureText("A").Y;

            SVector2 size = new(width + options.Padding.Left + options.Padding.Right,
                                height + options.Padding.Top + options.Padding.Bottom);

            float rawX = pos.X - options.Padding.Left;
            float rawY = pos.Y - options.Padding.Top;
            SVector2 topLeft = new(MathF.Floor(rawX) + 0.5f, MathF.Floor(rawY) + 0.5f);
            SVector2 sizeSnapped = new(MathF.Round(size.X), MathF.Round(size.Y));
            SVector2 bottomRight = topLeft + sizeSnapped;

            if(options.BgColor.A > 0) {
                _graphics.DrawBox(topLeft, bottomRight, options.BgColor, options.Rounding);
            }
            if (options.BorderColor.A > 0 && options.BorderThickness > 0) {            
                _graphics.DrawFrame(topLeft, bottomRight, options.BorderColor, options.Rounding, options.BorderThickness, 0);
            }

            float textX = topLeft.X + options.Padding.Left;
            float textY = pos.Y;

            foreach (var seg in Segments) {
                var color = seg.Color.IsEmpty ? options.DefaultColor : seg.Color;
                _graphics.DrawText(seg.Text, new SVector2(textX, textY), color);
                textX += _graphics.MeasureText(seg.Text).X;
            }

            return size;
        }

        /// <summary>Returns a new instance truncated to fit within the specified width, appending an ellipsis if needed.</summary>
        public ColoredText Ellipsize(float maxWidth, string ellipsis = "...") {
            var result = new List<ColorSegment>();
            float usedWidth = 0;
            float ellipsisWidth = ImGui.CalcTextSize(ellipsis).X;
            foreach (var seg in Segments) {
                float segWidth = ImGui.CalcTextSize(seg.Text).X;
                if (usedWidth + segWidth > maxWidth) {
                    // Need to truncate this segment
                    int charCount = 0;
                    float partialWidth = 0;
                    foreach (char c in seg.Text) {
                        float charWidth = ImGui.CalcTextSize(c.ToString()).X;
                        if (usedWidth + partialWidth + charWidth + ellipsisWidth > maxWidth)
                            break;
                        partialWidth += charWidth;
                        charCount++;
                    }
                    if (charCount > 0)
                        result.Add(new ColorSegment(seg.Text.Substring(0, charCount) + ellipsis, seg.Color));
                    else if (result.Count > 0)
                        result[result.Count - 1] = new ColorSegment(result.Last().Text + ellipsis, result.Last().Color);
                    else
                        result.Add(new ColorSegment(ellipsis, seg.Color));
                    break;
                }
                else {
                    result.Add(seg);
                    usedWidth += segWidth;
                }
            }
            // If no truncation was needed, return this
            if (result.Count == Segments.Count && result.SequenceEqual(Segments))
                return this;
            return new ColoredText(_graphics, result);
        }

        // Private parser
        private static List<ColorSegment> ParseSegments(string input) {
            var segments = new List<ColorSegment>();
            var colorStack = new Stack<SColor>();
            colorStack.Push(SColor.Empty); // Use Empty to indicate "no color"

            int i = 0;
            int start = 0;
            while (i < input.Length) {
                if (input[i] == '|' && i + 9 < input.Length && input[i + 1] == 'c') {
                    if (i > start)
                        segments.Add(new ColorSegment(input[start..i], colorStack.Peek()));
                    string hex = input.Substring(i + 2, 8);
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte a = Convert.ToByte(hex.Substring(6, 2), 16);
                    colorStack.Push(SColor.FromArgb(a, r, g, b));
                    i += 10;
                    start = i;
                }
                else if (input[i] == '|' && i + 1 < input.Length && input[i + 1] == 'r') {
                    if (i > start)
                        segments.Add(new ColorSegment(input[start..i], colorStack.Peek()));
                    if (colorStack.Count > 1)
                        colorStack.Pop();
                    i += 2;
                    start = i;
                }
                else {
                    i++;
                }
            }
            if (i > start)
                segments.Add(new ColorSegment(input[start..i], colorStack.Peek()));

            return segments;
        }




    }
}

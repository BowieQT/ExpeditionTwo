using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;

namespace DieselExileTools.ExileCore2;

public static partial class DXT {
    public static class Tooltip {

        private static readonly SColor DefaultTitleColor = DXTC.Color.FromRGBA(255, 255, 255, 255); // White
        private static readonly SColor DefaultDescriptionColor = DXTC.Color.FromHSLA(220, .15f, .90f, 255); // Default to light gray
        private static readonly SColor DefaultSeparatorColor = DXTC.Color.FromHSLA(220, .15f, .90f, 255); // Default to light gray
        private static readonly SColor DefaultLeftTextColor = DXTC.Color.FromRGBA(0, 170, 255, 255); // Default to light blue 
        private static readonly SColor DefaultRightTextColor = DXTC.Color.FromRGBA(214, 214, 0, 255); // Default to yellow


        public class Options {
            public List<Line> Lines { get; set; } = new List<Line>();
            public SColor BackgroundColor { get; set; } = DXTC.Color.FromRGBA(0, 0, 0, 200);
            public SColor BorderColor { get; set; } = SColor.Black;
            public SVector2 Offset { get; set; } = new(10, 10); // Position relative to mouse cursor
            public SVector2 Size { get; set; } = new(260, 120);
            public DXTPadding Padding { get; set; } = new(5, 5, 5, 5);
            public bool FitContent { get; set; } = true; // If true, size will be adjusted to fit content
        }

        public abstract class Line { }
        public class Title : Line
        {
            public string Text { get; set; } = string.Empty;
            public SColor Color { get; set; } = DefaultTitleColor;
        }
        public class DoubleLine : Line
        {
            public string LeftText { get; set; } = string.Empty;
            public string RightText { get; set; } = string.Empty;
            public SColor LeftColor { get; set; } = DefaultLeftTextColor;
            public SColor RightColor { get; set; } = DefaultRightTextColor;
        }
        public class Description : Line
        {
            public string Text { get; set; } = string.Empty;
            public SColor Color { get; set; } = DefaultDescriptionColor;
        }
        public class Separator : Line
        {
            /// <summary> Default:(-2, 4, -2, 6) </summary>
            public DXTPadding Padding { get; set; } = new(-2, 4, -2, 6);
            public float Thickness { get; set; } = 1.0f;
            public SColor Color { get; set; } = DefaultSeparatorColor;

        }


        public static Options BasicOptions(string text)
        {
            return new Options
            {
                Lines = new List<Line> {
                new Description { Text = text }
            },
            };
        }
        public static Options BasicOptions(string text, SColor color)
        {
            return new Options
            {
                Lines = new List<Line> {
                new Description { Text = text, Color = color }
            }
            };
        }
        public static Options BasicOptions(string title, string description, bool separator = true)
        {
            var lines = new List<Line> { new Title { Text = title } };
            if (separator) lines.Add(new Separator());
            lines.Add(new Description { Text = description });
            return new Options { Lines = lines };
        }

        public static void Draw(string text)
        {
            Draw(BasicOptions(text));
        }
        public static void Draw(string text, SColor color)
        {
            Draw(BasicOptions(text, color));
        }
        public static void Draw(string title, string description, bool separator = true)
        {
            Draw(BasicOptions(title, description, separator));
        }

        public static void Draw(Options options)
        {
            var mousePos = ImGui.GetMousePos();
            var drawList = ImGui.GetForegroundDrawList();
            var pos = mousePos + options.Offset;
            var size = options.Size;

            // If FitToContent is enabled, measure content
            if (options.FitContent)
            {
                float maxWidth = 0;
                float totalHeight = 0;
                foreach (var line in options.Lines)
                {
                    switch (line)
                    {
                        case Title title:
                            var titleSize = ImGui.CalcTextSize(title.Text);
                            maxWidth = Math.Max(maxWidth, titleSize.X);
                            totalHeight += titleSize.Y;
                            break;
                        case DoubleLine dbl:
                            var leftSize = ImGui.CalcTextSize(dbl.LeftText);
                            var rightSize = ImGui.CalcTextSize(dbl.RightText);
                            float lineWidth = leftSize.X + 10 + rightSize.X;
                            maxWidth = Math.Max(maxWidth, lineWidth);
                            totalHeight += Math.Max(leftSize.Y, rightSize.Y);
                            break;
                        case Description desc:
                            var lines = desc.Text.Split('\n');
                            float descHeight = 0;
                            float descMaxWidth = 0;
                            foreach (var lineText in lines)
                            {
                                var lineSize = ImGui.CalcTextSize(lineText);
                                descHeight += lineSize.Y;
                                descMaxWidth = Math.Max(descMaxWidth, lineSize.X);
                            }
                            maxWidth = Math.Max(maxWidth, descMaxWidth);
                            totalHeight += descHeight;
                            break;
                        case Separator sep:
                            // Add top padding, thickness, and bottom padding
                            totalHeight += sep.Padding.Top + sep.Thickness + sep.Padding.Bottom;
                            break;
                    }
                }
                // Add padding to both sides (left + right, top + bottom)
                size = new SVector2(
                    maxWidth + options.Padding.Left + options.Padding.Right,
                    totalHeight + options.Padding.Top + options.Padding.Bottom
                );
            }

            // Draw background and border
            drawList.AddRectFilled(pos, pos + size, options.BackgroundColor.ToImGui());
            drawList.AddRect(pos, pos + size, options.BorderColor.ToImGui(), 0f, ImDrawFlags.None, 1f);

            // Draw each line
            var textPos = pos + new SVector2(options.Padding.Left, options.Padding.Top);
            foreach (var line in options.Lines)
            {
                switch (line)
                {
                    case Title title:
                        drawList.AddText(textPos, title.Color.ToImGui(), title.Text);
                        var titleSize = ImGui.CalcTextSize(title.Text);
                        textPos.Y += titleSize.Y;
                        break;
                    case DoubleLine dbl:
                        drawList.AddText(textPos, dbl.LeftColor.ToImGui(), dbl.LeftText);
                        var leftSize = ImGui.CalcTextSize(dbl.LeftText);
                        var rightSize = ImGui.CalcTextSize(dbl.RightText);
                        var rightPos = new SVector2(
                            pos.X + size.X - options.Padding.Right - rightSize.X,
                            textPos.Y
                        );
                        drawList.AddText(rightPos, dbl.RightColor.ToImGui(), dbl.RightText);
                        textPos.Y += Math.Max(leftSize.Y, rightSize.Y);
                        break;
                    case Description desc:
                        var lines = desc.Text.Split('\n');
                        foreach (var lineText in lines)
                        {
                            drawList.AddText(textPos, desc.Color.ToImGui(), lineText);
                            var lineSize = ImGui.CalcTextSize(lineText);
                            textPos.Y += lineSize.Y;
                        }
                        break;
                    case Separator sep:
                        // Top padding
                        textPos.Y += sep.Padding.Top; // Top
                                                      // Draw the horizontal separator line
                        var lineY = MathF.Round(textPos.Y);
                        var lineStart = new SVector2(
                            pos.X + options.Padding.Left + sep.Padding.Left, // Left
                            lineY
                        );
                        var lineEnd = new SVector2(
                            pos.X + size.X - options.Padding.Right - sep.Padding.Right, // Right
                            lineY
                        );
                        drawList.AddLine(lineStart, lineEnd, sep.Color.ToImGui(), sep.Thickness);
                        // Move textPos.Y down by thickness and bottom padding
                        textPos.Y += sep.Thickness + sep.Padding.Bottom; // Bottom
                        break;
                }
            }
        }
    }
}
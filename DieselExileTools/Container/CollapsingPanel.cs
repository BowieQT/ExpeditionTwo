
using ImGuiNET;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using SVector2 = System.Numerics.Vector2;
using SColor = System.Drawing.Color;

namespace DieselExileTools.ExileCore2;

public static partial class DXT {
    public static class CollapsingPanel {
        public class Options {
            public required string Label { get; set; }
            public int PadLeft { get; set; } = 3;
            public int PadTop { get; set; } = 3;
            public int HeaderHeight { get; set; } = 20;
            /// <summary> &gt;0 = fixed pixel width, any other value will result in the panel handling it </summary>
            public int? HeaderWidth { get; set; }
            public int HeaderSpacing { get; set; } = 1;

            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Width { get; set; } = 0;
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Height { get; set; }

            public SColor Color { get; set; } = DXTC.Colors.Panel;
            public SColor HeaderColor { get; set; } = DXTC.Colors.PanelHeader;
            public SColor HeaderTextColor { get; set; } = DXTC.Colors.ButtonText;
            public SColor InnerGlowColor { get; set; } = DXTC.Colors.PanelInnerGlow;

            public bool Debug { get; set; } = false;
            public SVector2 CalculatedSize { get; set; }

        }

        public static bool Begin(string uniqueID, ref bool collapsed, Options options) {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));

            var panelOtions = new Panel.Options {
                PadLeft = options.PadLeft,
                PadTop = options.PadTop,
                Width = options.Width,
                Height = options.Height,
                Debug = options.Debug,
                Color = options.Color,
                InnerGlowColor = options.InnerGlowColor,
            };

            // layout 
            var drawList = ImGui.GetWindowDrawList();
            var startingPos = ImGui.GetCursorScreenPos();
            var availWidth = ImGui.GetContentRegionAvail().X;
            // button layout
            var buttonPos = startingPos;
            var buttonWidth = ImGui.CalcTextSize(options.Label).X + 8;
            if (options.HeaderWidth != null && options.HeaderWidth > 0) {
                buttonWidth = options.HeaderWidth.Value;
            }
            else {
                if (options.Width != null) {
                    if (options.Width <= 0) buttonWidth = availWidth + options.Width.Value; // fill space - value
                    else buttonWidth = options.Width.Value; // fixed pixel width
                }
            }

            var buttonHeight = options.HeaderHeight;
            // draw header
            drawList.AddRectFilled(buttonPos, buttonPos + new SVector2(buttonWidth, buttonHeight), options.HeaderColor.ToImGui());
            drawList.AddRect(buttonPos, buttonPos + new SVector2(buttonWidth, buttonHeight), options.InnerGlowColor.ToImGui());
            if (!string.IsNullOrEmpty(options.Label)) {
                var textSize = ImGui.CalcTextSize(options.Label);
                var textPos = buttonPos + new SVector2(4, (float)Math.Ceiling((buttonHeight - textSize.Y) / 2) - 2);
                drawList.AddText(textPos, options.HeaderTextColor.ToImGui(), options.Label);
            }
            // button
            ImGui.SetCursorScreenPos(buttonPos);
            bool toggled = ImGui.InvisibleButton($"{uniqueID}_headerbutton", new SVector2(buttonWidth, buttonHeight));
            if (toggled) collapsed = !collapsed;

            if (collapsed) return false;

            // position panel below header
            var panelPos = ImGui.GetCursorScreenPos();
            panelPos.Y += options.HeaderSpacing;

            ImGui.SetCursorScreenPos(panelPos);
            Panel.Begin(uniqueID, panelOtions);
            // Draw header at the top, inside the margin space
            ImGui.TableSetColumnIndex(1); // Content column, first row
            ImGui.SetCursorPosY(ImGui.GetCursorPosY()); // Already at correct Y after margin dummy

            // Optionally, draw a separator or custom background here
            return true;
        }


        public static void End(string uniqueID) {
            Panel.End(uniqueID);
        }
    }

}

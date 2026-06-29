using DieselExileTools.Common;
using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class ColorSelect
    {

        public class Options
        {
            public SVector2? ColorPickerWindowOffset;
            public int? Width;
            public int? Height;
            public SColor BorderColor = SColor.Black;
        }

        private static void InternalDraw(string uniqueID, string label, ref SColor color, Options options = null)
        {
            if (options == null) options = new Options();

            var size = new SVector2(options.Width ?? ImGui.GetFrameHeight(), options.Height ?? ImGui.GetFrameHeight());
            var pos = ImGui.GetCursorScreenPos();
            var buttonClicked = ImGui.InvisibleButton($"##{uniqueID}InvisibleButton", size);
            var drawList = ImGui.GetWindowDrawList();
            DXT.Draw.Checkerboard(pos, size.X, size.Y);
            drawList.AddRectFilled(pos, pos + size, color.ToImGui());
            drawList.AddRect(pos, pos + size, options.BorderColor.ToImGui());
            // tooltip 
            if (ImGui.IsItemHovered())
            {
                Tooltip.Draw(new Tooltip.Options
                {
                    Lines = new List<Tooltip.Line> {
                new Tooltip.Title { Text = label },
                new Tooltip.Separator(),
                new Tooltip.DoubleLine { LeftText = "RGBA:", RightText = $"{color.R},{color.G},{color.B},{color.A}" },
                new Tooltip.DoubleLine { LeftText = "HEX:", RightText = $"#{color.ToHEX()}" },
            }
                });
            }
            if (buttonClicked) ColorPicker.Open(uniqueID, color, options.ColorPickerWindowOffset);
            ColorPicker.Draw(uniqueID, ref color, new ColorPicker.Options { Title = label });
        }

        /// <summary>
        /// Draws a color swatch widget for the specified color.
        /// When the swatch is clicked, a color picker popup appears. 
        /// </summary>
        public static void Draw(string uniqueID, string label, ref SColor color, Options options = null)
        {
            InternalDraw(uniqueID, label, ref color, options);
        }

    }
}

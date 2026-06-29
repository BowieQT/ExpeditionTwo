using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class Button
    {
        private enum ClickType
        {
            None,
            Left,
            Other
        }

        public class Options
        {
            public string Label { get; set; } = "";
            public int? Width { get; set; }
            public int? Height { get; set; } = 0;
            public SColor? BorderColor { get; set; } = SColor.Black;
            public SColor? InnerGlowColor { get; set; } = DXTC.Colors.ButtonInnerGlow;
            public SColor Color { get; set; } = DXTC.Colors.Button;
            public SColor TextColor { get; set; } = DXTC.Colors.ButtonText;
            public SColor CheckedTextColor { get; set; } = DXTC.Colors.ButtonTextChecked;
            public SColor CheckedColor { get; set; } = DXTC.Colors.ButtonChecked;
            public SColor HoveredColor { get; set; } = DXTC.Colors.ButtonHovered;
            public Tooltip.Options? Tooltip { get; set; }
        }

        // Normal button
        public static bool Draw(string uniqueId, Options? options = null)
        {
            var clickType = InternalDraw(uniqueId, options, null);
            return clickType != ClickType.None;
        }
        // Toggleable button
        public static bool Draw(string uniqueId, ref bool checkedState, Options? options = null)
        {
            var clickType = InternalDraw(uniqueId, options, checkedState);
            if (clickType == ClickType.None) return false;
            if (clickType == ClickType.Left)
            {
                checkedState = !checkedState;
            }
            return true;
        }

        private static ClickType InternalDraw(string uniqueId, Options? options, bool? checkedState)
        {
            if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

            int width = options.Width switch
            {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(options.Label).X + 10),
                <= 0 => (int)ImGui.GetContentRegionAvail().X + options.Width.Value,
                var w => w.Value
            };
            int height = options.Height switch
            {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(options.Label).Y + 4), // adjust padding as needed
                <= 0 => (int)ImGui.GetFrameHeight() + options.Height.Value,
                var h => h.Value
            };

            Rect controlRect = new(ImGui.GetCursorScreenPos(), width, height);
            Rect contentRect = new(controlRect.TopLeft + new SVector2(1, 1), controlRect.BottomRight - new SVector2(1, 1));

            var textSize = new SVector2(0, 0);
            var textPos = new SVector2(0, 0);
            if (!string.IsNullOrEmpty(options.Label))
            {
                textSize = ImGui.CalcTextSize(options.Label);
                textPos = controlRect.TopLeft + new SVector2(
                    (float)Math.Round((controlRect.Width - textSize.X) / 2),
                    (float)Math.Ceiling((controlRect.Height - textSize.Y) / 2) - 2
                );
            }
            uint fillColor = (checkedState.HasValue && checkedState.Value) ? options.CheckedColor.ToImGui() : options.Color.ToImGui();
            uint textColor = (checkedState.HasValue && checkedState.Value) ? options.CheckedTextColor.ToImGui() : options.TextColor.ToImGui();

            // draw
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(controlRect.TopLeft, controlRect.BottomRight, fillColor);
            if (options.BorderColor != null) drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.BorderColor.Value.ToImGui());

            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueId}", new SVector2(controlRect.Width, controlRect.Height));
            if (ImGui.IsItemHovered())
            {
                drawList.AddRectFilled(contentRect.TopLeft, contentRect.BottomRight, options.HoveredColor.ToImGui());
                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }
            if (textSize.X > 0) drawList.AddText(textPos, textColor, options.Label);

            if (options.InnerGlowColor != null) drawList.AddRect(contentRect.TopLeft, contentRect.BottomRight, options.InnerGlowColor.Value.ToImGui());

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) return ClickType.Other;
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) return ClickType.Left;

            return ClickType.None;
        }




    }

}

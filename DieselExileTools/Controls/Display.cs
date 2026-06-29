using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using ExileCore2.Shared.Helpers;
using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;
public static partial class DXT
{
    public static class Display
    {
        public class Options
        {
            public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
            public int? Width { get; set; }
            public int? Height { get; set; }
            public int TextPaddingLeft { get; set; } = 3;
            public int TextPaddingRight { get; set; } = 3;
            public SColor? BackgroundColor { get; set; } = DXTC.Colors.Input;
            public SColor? BorderColor { get; set; } = SColor.Black;
            public SColor TextColor { get; set; } = DXTC.Colors.InputText;
            public Tooltip.Options? Tooltip { get; set; } = null;
            public bool DrawBackground { get; set; } = true;
        }

        private const int UNIFIX16 = 2;


        public static void Draw(string uniqueID, string value, Options? options = null)
        {
            if (options == null) options = new Options(); // safety?

            Rect controlRect = new(ImGui.GetCursorScreenPos(), options.Width ?? ImGui.GetContentRegionAvail().X, options.Height ?? ImGui.GetFrameHeight());
            var displyaTextMaxWidth = controlRect.Width - (options.TextPaddingLeft + options.TextPaddingRight);
            var displayTextPos = new SVector2(
                controlRect.TopLeft.X + options.TextPaddingLeft,
                controlRect.TopLeft.Y + (float)Math.Max(Math.Round((controlRect.Height - ImGui.GetFrameHeight() - UNIFIX16) * 0.5f), 0)
            );

            // custom theme 
            var drawList = ImGui.GetWindowDrawList();
            if (options.DrawBackground)
            {
                if (options.BackgroundColor != null) drawList.AddRectFilled(controlRect.TopLeft, controlRect.BottomRight, options.BackgroundColor.Value.ToImGui());
                if (options.BorderColor != null) drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.BorderColor.Value.ToImGui());
            }
            // Truncate text if needed
            string displayText = value;
            var displayTextSize = ImGui.CalcTextSize(displayText);

            if (displayTextSize.X > displyaTextMaxWidth)
            { // Truncate and add ellipsis if needed
                int len = displayText.Length;
                while (len > 0 && ImGui.CalcTextSize(displayText.Substring(0, len) + "…").X > displyaTextMaxWidth) len--;
                displayText = (len > 0) ? displayText.Substring(0, len) + "…" : "";
            }
            ImGui.SetCursorScreenPos(displayTextPos);
            ImGui.TextColored(options.TextColor.ToImguiVec4(), displayText);

            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueID}display", new SVector2(controlRect.Width, controlRect.Height)); // Covers the full area

            if (ImGui.IsItemHovered() && options.Tooltip != null) Tooltip.Draw(options.Tooltip);

        }

    }
}


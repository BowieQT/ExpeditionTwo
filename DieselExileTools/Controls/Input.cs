using ImGuiNET;
using SVector2 = System.Numerics.Vector2;
using SColor = System.Drawing.Color;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;


namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class Input
    {
        private static readonly uint InputInnerGlow = DXTC.Colors.InputInnerGlow.ToImGui();

        public class Options
        {
            public int? Width { get; set; }
            public int? Height { get; set; }
            public SColor BackgroundColor { get; set; } = DXTC.Colors.Input;
            public SColor? TextColor { get; set; } = null;
            public SColor BorderColor { get; set; } = SColor.Black;
            public SColor HoveredColor { get; set; } = DXTC.Colors.InputHovered;
            public Tooltip.Options? Tooltip { get; set; } = null;
            public ImGuiInputTextFlags InputTextFlags { get; set; } = ImGuiInputTextFlags.None;

        }

        public static bool Draw(string uniqueID, ref string value, Options options) {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
            options ??= new Options();

            // get position and size
            Rect controlRect = new(ImGui.GetCursorScreenPos(), options.Width ?? ImGui.GetContentRegionAvail().X, options.Height ?? ImGui.GetFrameHeight());
            Rect inputRect = new(controlRect.Left + 3, controlRect.Top, controlRect.Right, controlRect.Bottom);
            Rect glowRect = controlRect.Shrink(1);
            // draw
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(controlRect.TopLeft, controlRect.BottomRight, options.BackgroundColor.ToImGui());
            drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.BorderColor.ToImGui());
            drawList.AddRect(glowRect.TopLeft, glowRect.BottomRight, InputInnerGlow);

            bool changed = false;
            // Draw input box
            ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.Border, 0x00000000);
            if (options.TextColor != null) ImGui.PushStyleColor(ImGuiCol.Text, options.TextColor.Value.ToImGui());
            ImGui.SetCursorScreenPos(inputRect.TopLeft);
            ImGui.PushItemWidth(inputRect.Width);
            if (ImGui.InputText($"##{uniqueID}input", ref value, 64, options.InputTextFlags)) changed = true;
            if (ImGui.IsItemHovered()) {
                drawList.AddRectFilled(glowRect.TopLeft, glowRect.BottomRight, options.HoveredColor.ToImGui());

                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }

            ImGui.PopItemWidth();
            ImGui.PopStyleColor(2);
            if (options.TextColor != null) ImGui.PopStyleColor(1);

            return changed;

        }
    }
}


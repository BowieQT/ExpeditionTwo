using ImGuiNET;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;
public static partial class DXT {
    public static bool CheckboxWithTooltip(string label, string tooltip, ref bool value) {
        var b = ImGui.Checkbox(label, ref value);
        if (ImGui.IsItemHovered()) {
            ImGui.BeginTooltip();
            ImGui.Text(tooltip);
            ImGui.EndTooltip();
        }
        return b;
    }

    public static class Checkbox {

        public class Options {
            public string? Label { get; set; }
            public int LabelSpacing { get; set; } = 5;
            public int? Height { get; set; }
            public SColor? BorderColor { get; set; } = SColor.Black;
            public SColor? InnerGlowColor { get; set; } = DXTC.Colors.InputInnerGlow;
            public SColor BackgroundColor { get; set; } = DXTC.Colors.Input;
            public SColor TextColor { get; set; } = DXTC.Colors.Text;
            public SColor CheckColor { get; set; } = DXTC.Colors.ButtonChecked;
            public SColor HoveredColor { get; set; } = DXTC.Colors.InputHovered;

            public Tooltip.Options? Tooltip { get; set; }
        }


        public static bool Draw(string str_id, ref bool value) {
            return Draw(str_id, string.Empty, ref value);
        }

        public static bool Draw(string str_id, string tooltip, ref bool value) {
            var label = str_id.Contains("##") ? str_id.Split(new[] { "##" }, StringSplitOptions.None)[0] : str_id;
            var cleanId = str_id.Contains("##") ? str_id.Split(new[] { "##" }, StringSplitOptions.None)[1] : str_id;

            var options = new Options { Label = label };
            if (!string.IsNullOrWhiteSpace(tooltip)) options.Tooltip = Tooltip.BasicOptions(tooltip);

            return Draw(cleanId, ref value, options);
        }

        public static bool Draw(string uniqueId, ref bool value, Options options) {
            if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
            options ??= new Options();

            Rect checkboxRect = new(ImGui.GetCursorScreenPos(), options.Height ?? ImGui.GetFrameHeight(), options.Height ?? ImGui.GetFrameHeight());
            Rect checkboxGlowRect = checkboxRect.Shrink(1);
            Rect checkRect = checkboxRect.Shrink(4);
            Rect controlRect = checkboxRect;

            var textSize = SVector2.Zero;
            var textPosition = SVector2.Zero;
            if (!string.IsNullOrEmpty(options.Label)) {
                textSize = ImGui.CalcTextSize(options.Label);
                textPosition = checkboxRect.TopRight + new SVector2(
                    options.LabelSpacing,
                    (float)Math.Ceiling((checkboxRect.Height - textSize.Y) / 2) - 2
                    );
                controlRect = new Rect(checkboxRect.TopLeft, checkboxRect.Width + options.LabelSpacing + (int)textSize.X, checkboxRect.Height);
            }

            // draw
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(checkboxRect.TopLeft, checkboxRect.BottomRight, options.BackgroundColor.ToImGui());
            if (options.BorderColor != null) drawList.AddRect(checkboxRect.TopLeft, checkboxRect.BottomRight, options.BorderColor.Value.ToImGui());
            if (options.InnerGlowColor != null) drawList.AddRect(checkboxGlowRect.TopLeft, checkboxGlowRect.BottomRight, options.InnerGlowColor.Value.ToImGui());
            if (value) {
                drawList.AddRectFilled(checkRect.TopLeft, checkRect.BottomRight, options.CheckColor.ToImGui());
                if (options.InnerGlowColor != null) drawList.AddRect(checkRect.TopLeft, checkRect.BottomRight, options.InnerGlowColor.Value.ToImGui());
            }
            if (textSize.X > 0) drawList.AddText(textPosition, options.TextColor.ToImGui(), options.Label!);

            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueId}", new SVector2(controlRect.Width, controlRect.Height));
            if (ImGui.IsItemHovered()) {
                drawList.AddRectFilled(checkboxGlowRect.TopLeft, checkboxGlowRect.BottomRight, options.HoveredColor.ToImGui());
                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) { value = !value; }

            return value;
        }
    }

}

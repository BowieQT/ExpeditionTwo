using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;


namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class Slider
    {

        private static readonly uint GripinnerGlow = DXTC.Color.FromRGBA(255, 255, 255, 10).ToImGui();
        private static readonly uint InputInnerGlow = DXTC.Colors.InputInnerGlow.ToImGui();

        public class Options
        {
            public int GripWidth { get; set; } = 5;
            public int? Width { get; set; }
            public int? Height { get; set; }
            public float Min { get; set; } = 0f;
            public float Max { get; set; } = 100f;
            public float Step { get; set; } = 1f;
            public float? ShiftStep { get; set; }
            public SColor BackgroundColor { get; set; } = DXTC.Colors.Input;
            public SColor BorderColor { get; set; } = SColor.Black;
            public SColor GripColor { get; set; } = DXTC.Colors.Button;
            public SColor TextColor { get; set; } = DXTC.Colors.InputText;
            public Tooltip.Options? Tooltip { get; set; } = null;
            public ImGuiInputTextFlags InputTextFlags { get; set; } = ImGuiInputTextFlags.None;

        }

        private static string FormatDisplayValue(float value) {
            string formatted = value.ToString("F1");
            if (formatted.EndsWith(".0")) formatted = formatted.Substring(0, formatted.Length - 2);

            return formatted;
        }

        private static bool InternalDraw(string uniqueID, ref float value, bool isInt, Options options) {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
            if (options == null) options = new Options(); // safety?
            var drawList = ImGui.GetWindowDrawList();

            // position and size
            Rect controlRect = new(ImGui.GetCursorScreenPos(), options.Width ?? ImGui.GetContentRegionAvail().X, options.Height ?? ImGui.GetFrameHeight());
            Rect trackRect = new(controlRect.TopLeft + new SVector2(1, 1), controlRect.BottomRight - new SVector2(1, 1));
            int UNIFIX16 = 2;
            var framePadding = (float)Math.Max(Math.Floor((controlRect.Height - ImGui.GetFrameHeight() - UNIFIX16) * 0.5f),0);

            // Draw Border 
            drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.BorderColor.ToImGui());
            // Draw Background
            drawList.AddRectFilled(trackRect.TopLeft, trackRect.BottomRight, options.BackgroundColor.ToImGui() );
            drawList.AddRect(trackRect.TopLeft, trackRect.BottomRight, InputInnerGlow);
            // calc Grip 
            float gripNormPosX = (value - options.Min) / (options.Max - options.Min);
            int gripWidth = Math.Max(1, options.GripWidth);
            float gripCenterX = trackRect.TopLeft.X + gripNormPosX * (trackRect.Width - 1);       
            int gripMinX = Math.Clamp( (int)Math.Round(gripCenterX - (gripWidth / 2f)), (int)trackRect.TopLeft.X, (int)trackRect.BottomRight.X - gripWidth);
            Rect gripRect = new(
                new SVector2(gripMinX, trackRect.TopLeft.Y),
                new SVector2(gripMinX + gripWidth, trackRect.BottomRight.Y)
            );
            // Draw grip
            drawList.AddRect(gripRect.TopLeft - new SVector2(1,1), gripRect.BottomRight + new SVector2(1,1), options.BorderColor.ToImGui());
            drawList.AddRectFilled(gripRect.TopLeft, gripRect.BottomRight, options.GripColor.ToImGui());
            drawList.AddRect(gripRect.TopLeft, gripRect.BottomRight, GripinnerGlow);

            bool changed = false;
            // Draw input box
            ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.Border, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.SliderGrab, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, 0x00000000);
            ImGui.PushStyleColor(ImGuiCol.Text, options.TextColor.ToImGui());
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new SVector2(0, framePadding));
            ImGui.PushItemWidth(controlRect.Width);
            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            if (isInt) {
                int int_value = (int)Math.Round(value);
                if (ImGui.SliderInt($"##{uniqueID}slider", ref int_value, (int)options.Min, (int)options.Max)) {
                    value = int_value;
                    changed = true;
                }
            }
            else {
                if (ImGui.SliderFloat($"##{uniqueID}slider", ref value, options.Min, options.Max)) {
                    changed = true;
                }
            }
            ImGui.PopStyleColor(8);
            ImGui.PopStyleVar();
            ImGui.PopItemWidth();
            // button 
            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueID}invbutton", new SVector2(controlRect.Width, controlRect.Height));

            // Handle mouse wheel input for changing value
            if (ImGui.IsItemHovered()) {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0) {
                    if (isInt) {
                        int int_value = (int)Math.Round(value);
                        int_value += (int)(Math.Sign(wheel) * (ImGui.GetIO().KeyShift && options.ShiftStep.HasValue ? options.ShiftStep.Value : options.Step));
                        int_value = (int)Math.Clamp(int_value, options.Min, options.Max);
                        value = int_value;
                        changed = true;
                    }
                    else {
                        float newValue = value;
                        newValue += Math.Sign(wheel) * (ImGui.GetIO().KeyShift && options.ShiftStep.HasValue ? options.ShiftStep.Value : options.Step);
                        newValue = Math.Clamp(newValue, options.Min, options.Max);
                        value = newValue;
                        changed = true;
                    }
                }
                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }

            return changed;
        }

        public static bool Draw(string uniqueID, ref float value, Options? option=null) {
            var options = option ?? new Options();
            return InternalDraw(uniqueID, ref value, false, options);
        }
        public static bool Draw(string uniqueID, ref int value, Options? option = null) {
            var options = option ?? new Options();
            float float_value = value; // Convert int to float for processing
            var changed = InternalDraw(uniqueID, ref float_value, true, options);
            if (changed) {
                value = (int)float_value; // Direct cast, no rounding needed
            }
            return changed;
        }


    }
}


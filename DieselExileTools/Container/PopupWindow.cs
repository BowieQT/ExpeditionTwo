using ImGuiNET;
using System;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;

namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class PopupWindow
    {

        public class Options
        {
            public string? Title;
            public SVector2 Offset { get; set; } = new SVector2(10, 10);
            public SVector2 Size { get; set; } = new SVector2(200, 100); // Default size if not specified
            public int TitleBarHeight { get; set; } = 18; // Height of the title bar
            /// <summary>
            /// 
            /// top, right, bottom, left padding around the panel
            /// </summary>
            public DXTPadding PanelPadding { get; set; } = new(3, 0, 3, 3); // Padding around the panel new SVector4(0, 3, 3, 3); // Padding around the panel
            public SColor BackgroundColor { get; set; } = SColor.Black; // Default background
            public SColor PanelColor { get; set; } = DXTC.Colors.Panel; // Default panel color
            public SColor PanelBorderColor { get; set; } = DXTC.Colors.PanelInnerGlow; // Default panel border color
            public SColor TitleTextColor { get; set; } = DXTC.Colors.ControlText; // Default text color
        }

        private static void PushStrippedStyles()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new SVector2(0, 0));
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new SVector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Border, new SVector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Text, DXTC.Colors.Text.ToImGui());

        }
        private static void PopStrippedStyles()
        {
            ImGui.PopStyleColor(3);
            ImGui.PopStyleVar(2);
        }

        public static void Open(string unique_id, SVector2 offset)
        {
            ImGui.SetNextWindowPos(ImGui.GetMousePos() + offset, ImGuiCond.Always);
            ImGui.OpenPopup($"##{unique_id}POPUP");
        }


        public static bool Begin(string unique_id, Options options)
        {
            if (string.IsNullOrEmpty(unique_id)) throw new ArgumentException("unique_id cannot be null or empty", nameof(unique_id));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

            if (!ImGui.IsPopupOpen($"##{unique_id}POPUP")) return false;

            PushStrippedStyles();

            var popupSize = new SVector2(options.Size.X + 5, options.Size.Y + 5);
            ImGui.SetNextWindowSize(popupSize, ImGuiCond.Always);
            if (!ImGui.BeginPopup($"##{unique_id}POPUP"))
            {
                PopStrippedStyles();
                return false;
            }
            // draw custom background and panel
            var drawList = ImGui.GetWindowDrawList();
            SVector2 winPos = ImGui.GetWindowPos() + new SVector2(1, 1);
            SVector2 panelPos = winPos + new SVector2(options.PanelPadding.Left, options.TitleBarHeight + options.PanelPadding.Top);
            SVector2 panelSize = new SVector2(
                options.Size.X - options.PanelPadding.Right - options.PanelPadding.Left,
                options.Size.Y - options.TitleBarHeight - options.PanelPadding.Top - options.PanelPadding.Bottom
            );
            drawList.AddRectFilled(winPos, winPos + options.Size, options.BackgroundColor.ToImGui(), 0);
            drawList.AddRectFilled(panelPos, panelPos + panelSize, options.PanelColor.ToImGui(), 0.0f);
            drawList.AddRect(panelPos, panelPos + panelSize, options.PanelBorderColor.ToImGui(), 0.0f, ImDrawFlags.None, 1.0f);
            if (!string.IsNullOrEmpty(options.Title))
            {
                drawList.AddText(winPos + new SVector2(2, 0), options.TitleTextColor.ToImGui(), options.Title);
            }
            ImGui.SetCursorScreenPos(panelPos);

            return true;
        }
        public static void End()
        {
            ImGui.EndPopup();
            PopStrippedStyles();
        }



    }
}



using ImGuiNET;
using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using SVector2 = System.Numerics.Vector2;
using SColor = System.Drawing.Color;

namespace DieselExileTools.ExileCore2;


public static partial class DXT
{
    public static class IconPicker
    {
        private static int TitlebarHeight = 20;
        private static readonly DXTPadding PanelPadding = new(3, 0, 3, 3);
        private static readonly SVector2 DefaultWindowOffset = new(10, 10);
        // working variables
        private static SVector2 WindowSize = new SVector2(0, 0);

        public class Options
        {
            public string Title = "Icon Picker";
            public SVector2 WindowOffset = DefaultWindowOffset;

            public SColor HoveredIconColor = DXTC.Colors.ButtonHovered;
            public SColor SelectedIconColor = DXTC.Colors.ButtonChecked;
            public SColor IconColor = SColor.White;

        }

        public static void Open(string uniqueID, IconAtlas iconAtlas, SVector2? windowOffset)
        {
            WindowSize = new(
                PanelPadding.Left + iconAtlas.AtlasSize.X + PanelPadding.Right,
                TitlebarHeight + PanelPadding.Top + iconAtlas.AtlasSize.Y + PanelPadding.Bottom
            );
            PopupWindow.Open(uniqueID, windowOffset ?? DefaultWindowOffset);
        }

        public static void Draw(string uniqueID, IconAtlas iconAtlas, ref int selectedIconIndex, Options options = null)
        {

            options ??= new Options();

            if (PopupWindow.Begin(uniqueID, new PopupWindow.Options { Size = WindowSize, Title = options.Title, PanelPadding = PanelPadding, TitleBarHeight = TitlebarHeight }))
            {
                var contentPos = ImGui.GetCursorScreenPos();
                var drawList = ImGui.GetWindowDrawList();

                for (int y = 0; y < iconAtlas.IconsPerColumn; y++)
                {
                    for (int x = 0; x < iconAtlas.IconsPerRow; x++)
                    {
                        int iconIndex = y * iconAtlas.IconsPerRow + x;
                        (SVector2 uv0, SVector2 uv1) = iconAtlas.GetIconUVs(iconIndex);
                        var buttonPos = contentPos + new SVector2(x * iconAtlas.IconSize.X, y * iconAtlas.IconSize.Y);

                        if (iconIndex == selectedIconIndex) drawList.AddRectFilled(buttonPos, buttonPos + iconAtlas.IconSize, options.SelectedIconColor.ToImGui());

                        ImGui.SetCursorScreenPos(buttonPos);
                        if (ImGui.InvisibleButton($"{uniqueID}textureID_{y}_{x}", iconAtlas.IconSize))
                        {
                            selectedIconIndex = iconIndex;
                            ImGui.CloseCurrentPopup();
                        }
                        if (ImGui.IsItemHovered()) drawList.AddRectFilled(buttonPos, buttonPos + iconAtlas.IconSize, options.HoveredIconColor.ToImGui());

                        drawList.AddImage(iconAtlas.TextureId, buttonPos, buttonPos + iconAtlas.IconSize, uv0, uv1, options.IconColor.ToImGui());
                    }
                }

                PopupWindow.End();
            }
        }



    }
}



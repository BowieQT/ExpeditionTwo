using ImGuiNET;
using DieselExileTools.Common;
using SVector2 = System.Numerics.Vector2;
using SColor = System.Drawing.Color;

namespace DieselExileTools.ExileCore2;

public static partial class DXT {
    public static class Draw { 

        public static void Checkerboard(SVector2 pos, float width, float height, int cellSize = 4, SColor? col1 = null, SColor? col2 = null) {

            col1 ??= DXTC.Color.FromHEX("CCCCCC"); // 0xFFCCCCCC
            col2 ??= DXTC.Color.FromHEX("888888"); // 0xFF888888

            var drawList = ImGui.GetWindowDrawList();
            int cols = (int)Math.Ceiling(width / cellSize);
            int rows = (int)Math.Ceiling(height / cellSize);
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < cols; x++) {
                    uint col = (x + y) % 2 == 0 ? col1.Value.ToImGui() : col2.Value.ToImGui();
                    SVector2 p0 = pos + new SVector2(x * cellSize, y * cellSize);
                    SVector2 p1 = pos + new SVector2(
                        Math.Min((x + 1) * cellSize, width),
                        Math.Min((y + 1) * cellSize, height)
                    );
                    drawList.AddRectFilled(p0, p1, col);
                }
            }
        }

    }
}
using ImGuiNET;

namespace DieselExileTools.ExileCore2;

public static partial class DXT {
    public static bool CollapsingHeader(string label, ref bool open) {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (open) flags |= ImGuiTreeNodeFlags.DefaultOpen;

        open = ImGui.CollapsingHeader(label, flags);

        return open;
    }
}

using ImGuiNET;
using DieselExileTools.Common;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;
public static partial class DXT {
    public static class Panel
    {
        private static readonly Dictionary<string, Options> panelOptions = new();

        public class Options
        {
            public int PadLeft { get; set; } = 3;
            public int PadTop { get; set; } = 3;
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Width { get; set; } = 0;
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Height { get; set; }
            public SColor Color { get; set; } = DXTC.Colors.Panel;
            public SColor InnerGlowColor { get; set; } = DXTC.Colors.PanelInnerGlow;

            public bool Debug { get; set; } = false;
        }

        public static bool Begin(string uniqueID, Options options)
        {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

            panelOptions[uniqueID] = options;

            var avail = ImGui.GetContentRegionAvail();
            var flags = ImGuiChildFlags.None;

            // get panel size
            SVector2 panelSize = new(0, 0);
            if (options.Width.HasValue)
            {
                if (options.Width.Value <= 0)
                { // fill space - value
                    panelSize.X = avail.X + options.Width.Value;
                }
                else panelSize.X = options.Width.Value;
            }
            else flags |= ImGuiChildFlags.AutoResizeX; // auto-size

            if (options.Height.HasValue)
            {
                if (options.Height.Value <= 0)
                { // fill space - value 
                    panelSize.Y = avail.Y + options.Height.Value;
                }
                else panelSize.Y = options.Height.Value;
            }
            else flags |= ImGuiChildFlags.AutoResizeY; // auto-size

            // draw child
            ImGui.PushStyleColor(ImGuiCol.ChildBg, 0);
            ImGui.BeginChild(uniqueID, panelSize, flags, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            // Draw custom background
            var childPos = ImGui.GetWindowPos();
            var childSize = ImGui.GetWindowSize();
            var bgMin = childPos;
            var bgMax = new SVector2(childPos.X + childSize.X, childPos.Y + childSize.Y);

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(bgMin, bgMax, options.Color.ToImGui());
            drawList.AddRect(bgMin, bgMax, options.InnerGlowColor.ToImGui());

            // Padding top
            if (options.PadTop > 0) ImGui.Dummy(new SVector2(0, options.PadTop));
            // Indent for left padding
            if (options.PadLeft > 0) ImGui.Indent(options.PadLeft);

            return true;
        }
        public static void End(string uniqueID)
        {
            var options = (panelOptions.ContainsKey(uniqueID) ? panelOptions[uniqueID] : null) ?? throw new ArgumentException($"No panel with uniqueID:'{uniqueID}' open, did you forget to call Panel.Begin()?", nameof(uniqueID));

            // Unindent left padding
            if (options.PadLeft > 0) ImGui.Unindent(options.PadLeft);
            ImGui.EndChild();
            ImGui.PopStyleColor();
        }

    }
}

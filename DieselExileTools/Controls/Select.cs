using DieselExileTools.Common;
using DieselExileTools.Common.Structs;

namespace DieselExileTools.ExileCore2;

using ImGuiNET;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

public static partial class DXT {

    public static class Select {

        public class Options {
            public Tooltip.Options? Tooltip { get; set; }
            public List<string>? Items { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; } = 0;


            public SColor? SelectBorderColor { get; set; } = SColor.Black;
            public SColor? SelectInnerGlowColor { get; set; } = DXTC.Colors.ButtonInnerGlow;
            public SColor SelectColor { get; set; } = DXTC.Colors.Button;
            public SColor SelectTextColor { get; set; } = DXTC.Colors.ButtonText;
            public SColor HoveredColor { get; set; } = DXTC.Colors.ButtonHovered;


            public DXTPadding DropdownPadding { get; set; } = new(2, 2, 2, 2);
            public int DropdownItemSpacing { get; set; } = 3;
            public SColor DropdownBackgroundColor { get; set; } = DXTC.Colors.Panel;
            public SColor DropdownBorderColor { get; set; } = SColor.Black;
            public SColor DropdownTextColor { get; set; } = DXTC.Colors.ButtonText;

            public SColor DropdownSelectedItemColor { get; set; } = DXTC.Colors.ButtonChecked;
            public SColor DropdownSelectedTextColor { get; set; } = DXTC.Colors.ButtonTextChecked;

        }


        public static bool Drajw(string uniqueId, ref int selected, Options options) {
            if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");
            if (options.Items == null || options.Items.Count == 0) return false;
            bool newSelected = false;
            var selectedText = options.Items[selected] ?? "";
            DXTPadding buttonPadding = new(4, 8, 5, 7);
            // position and size
            int width = options.Width switch {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(selectedText).X + 10),
                <= 0 => (int)ImGui.GetContentRegionAvail().X + options.Width.Value,
                var w => w.Value
            };
            int height = options.Height switch {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(selectedText).Y + 4), // adjust padding as needed
                <= 0 => (int)ImGui.GetFrameHeight() + options.Height.Value,
                var h => h.Value
            };

            Rect controlRect = new(ImGui.GetCursorScreenPos(), width, height);
            Rect contentRect = new(controlRect.TopLeft + new SVector2(1, 1), controlRect.BottomRight - new SVector2(1, 1));
            var textSize = new SVector2(0, 0);
            var textPos = new SVector2(0, 0);
            if (!string.IsNullOrEmpty(selectedText)) {
                textSize = ImGui.CalcTextSize(selectedText);
                var textOffset = ImGui.GetFont().GetDebugName() == "unifont.otf, 16px" ? 2 : 0; // unifont, 16 seems to render 2px lower 
                textPos = controlRect.TopLeft + new SVector2(
                    buttonPadding.Left,
                    (float)Math.Ceiling((controlRect.Height - textSize.Y) / 2) - textOffset
                );
            }

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(controlRect.TopLeft, controlRect.BottomRight, options.SelectColor.ToImGui());
            if (options.SelectBorderColor != null) drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.SelectBorderColor.Value.ToImGui());

            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueId}", new SVector2(controlRect.Width, controlRect.Height));
            if (ImGui.IsItemHovered()) {
                drawList.AddRectFilled(contentRect.TopLeft, contentRect.BottomRight, options.HoveredColor.ToImGui());
                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }
            // Dropdown arrow (drawn triangle)
            float arrowHeight = (controlRect.Height - buttonPadding.Top - buttonPadding.Bottom);
            float arrowWidth = arrowHeight; // Keep it square for a triangle
            float arrowX = controlRect.BottomRight.X - buttonPadding.Right - arrowWidth;
            float arrowY = controlRect.Top + buttonPadding.Top;
            // Arrow points (downward triangle)
            SVector2 p1 = new SVector2(arrowX, arrowY);
            SVector2 p2 = new SVector2(arrowX + arrowWidth, arrowY);
            SVector2 p3 = new SVector2(arrowX + (float)Math.Round(arrowWidth / 2f), arrowY + arrowHeight);
            drawList.AddTriangleFilled(p1, p2, p3, options.SelectTextColor.ToImGui());

            if (textSize.X > 0) drawList.AddText(textPos, options.SelectTextColor.ToImGui(), selectedText);

            // Open popup on click
            // Set cursor to just below the input for the popup trigger
            //ImGui.SetCursorScreenPos(controlRect.BottomLeft);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
                ImGui.OpenPopup($"##{uniqueId}_dropdown_popup");
            }

            // Calculate popup size
            float itemHeight = ImGui.GetFontSize();
            int itemSpacing = options.DropdownItemSpacing;
            int itemCount = options.Items.Count;
            var padding = options.DropdownPadding;
            float contentHeight = itemCount * itemHeight + ((itemCount > 1) ? (itemCount - 1) * itemSpacing : 0);
            float totalHeight = padding.Top + contentHeight + padding.Bottom;

            ImGui.SetNextWindowPos(controlRect.BottomLeft);
            ImGui.SetNextWindowSize(new SVector2(controlRect.Width, totalHeight));
            if (ImGui.BeginPopup($"##{uniqueId}_dropdown_popup")) {
                // Clamp to available screen space below the control
                float screenBottom = ImGui.GetIO().DisplaySize.Y;
                float spaceBelow = screenBottom - controlRect.BottomLeft.Y;
                float dropdownHeight = Math.Min(totalHeight, spaceBelow);

                var popupDrawList = ImGui.GetWindowDrawList();
                var bgStart = ImGui.GetCursorScreenPos();
                var bgEnd = bgStart + new SVector2(width, dropdownHeight);

                // Draw full dropdown background and border
                popupDrawList.AddRectFilled(bgStart, bgEnd, options.DropdownBackgroundColor.ToImGui());
                popupDrawList.AddRect(bgStart, bgEnd, options.DropdownBorderColor.ToImGui());

                // Begin scrollable child region
                ImGui.BeginChild($"##{uniqueId}dropdown_scroll", new SVector2(width, dropdownHeight - options.DropdownPadding.Top - options.DropdownPadding.Bottom), ImGuiChildFlags.None);
                var childDrawList = ImGui.GetWindowDrawList();
                var itemTopLeft = ImGui.GetCursorScreenPos();
                for (int i = 0; i < itemCount; i++) {
                    var itemRect = new Rect(itemTopLeft + new SVector2(padding.Left, padding.Top + i * itemHeight + i * itemSpacing), width - padding.Left - padding.Right, itemHeight);
                    var itemText = options.Items[i];
                    var itemTextSize = ImGui.CalcTextSize(itemText);
                    var itemTextPos = itemRect.TopLeft + new SVector2(6, (float)Math.Ceiling((itemRect.Height - itemTextSize.Y) / 2));

                    // Draw background ONLY for selected item
                    if (i == selected) {
                        childDrawList.AddRectFilled(itemRect.TopLeft, itemRect.BottomRight, options.DropdownSelectedItemColor.ToImGui());
                        childDrawList.AddText(itemTextPos, options.DropdownSelectedTextColor.ToImGui(), itemText);
                    }
                    else
                        childDrawList.AddText(itemTextPos, options.DropdownTextColor.ToImGui(), itemText);

                    if (ImGui.InvisibleButton($"##{uniqueId}_item_{i}", itemRect.Size)) {
                        selected = i;
                        newSelected = true;
                        ImGui.CloseCurrentPopup();
                    }
                    if (ImGui.IsItemHovered()) {
                        // Highlight the hovered item
                        childDrawList.AddRectFilled(itemRect.TopLeft, itemRect.BottomRight, options.HoveredColor.ToImGui());
                    }
                }

                ImGui.EndChild();
                ImGui.EndPopup();
            }

            return newSelected;
        }

        public static bool Draw(string uniqueId, ref int selected, Options options) {
            if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");
            if (options.Items == null || options.Items.Count == 0) return false;
            bool newSelected = false;
            var selectedText = options.Items[selected] ?? "";
            DXTPadding buttonPadding = new(4, 8, 5, 7);

            // position and size
            int width = options.Width switch {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(selectedText).X + 10),
                <= 0 => (int)ImGui.GetContentRegionAvail().X + options.Width.Value,
                var w => w.Value
            };
            int height = options.Height switch {
                null => (int)Math.Ceiling(ImGui.CalcTextSize(selectedText).Y + 4),
                <= 0 => (int)ImGui.GetFrameHeight() + options.Height.Value,
                var h => h.Value
            };

            Rect controlRect = new(ImGui.GetCursorScreenPos(), width, height);
            Rect contentRect = new(controlRect.TopLeft + new SVector2(1, 1), controlRect.BottomRight - new SVector2(1, 1));
            var textSize = new SVector2(0, 0);
            var textPos = new SVector2(0, 0);
            if (!string.IsNullOrEmpty(selectedText)) {
                textSize = ImGui.CalcTextSize(selectedText);
                var textOffset = ImGui.GetFont().GetDebugName() == "unifont.otf, 16px" ? 2 : 0;
                textPos = controlRect.TopLeft + new SVector2(
                    buttonPadding.Left,
                    (float)Math.Ceiling((controlRect.Height - textSize.Y) / 2) - textOffset
                );
            }

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(controlRect.TopLeft, controlRect.BottomRight, options.SelectColor.ToImGui());
            if (options.SelectBorderColor != null) drawList.AddRect(controlRect.TopLeft, controlRect.BottomRight, options.SelectBorderColor.Value.ToImGui());

            ImGui.SetCursorScreenPos(controlRect.TopLeft);
            ImGui.InvisibleButton($"##{uniqueId}", new SVector2(controlRect.Width, controlRect.Height));
            if (ImGui.IsItemHovered()) {
                drawList.AddRectFilled(contentRect.TopLeft, contentRect.BottomRight, options.HoveredColor.ToImGui());
                if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
            }

            // Dropdown arrow
            float arrowHeight = (controlRect.Height - buttonPadding.Top - buttonPadding.Bottom);
            float arrowWidth = arrowHeight;
            float arrowX = controlRect.BottomRight.X - buttonPadding.Right - arrowWidth;
            float arrowY = controlRect.Top + buttonPadding.Top;
            SVector2 p1 = new SVector2(arrowX, arrowY);
            SVector2 p2 = new SVector2(arrowX + arrowWidth, arrowY);
            SVector2 p3 = new SVector2(arrowX + (float)Math.Round(arrowWidth / 2f), arrowY + arrowHeight);
            drawList.AddTriangleFilled(p1, p2, p3, options.SelectTextColor.ToImGui());

            if (textSize.X > 0) drawList.AddText(textPos, options.SelectTextColor.ToImGui(), selectedText);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
                ImGui.OpenPopup($"##{uniqueId}_dropdown_popup");
            }

            // Calculate item sizing constraints
            float itemHeight = ImGui.GetFontSize() + 4; // Add a tiny 4px frame padding constant for safety
            int itemSpacing = options.DropdownItemSpacing;
            int itemCount = options.Items.Count;
            var padding = options.DropdownPadding;

            float contentHeight = itemCount * itemHeight + ((itemCount > 1) ? (itemCount - 1) * itemSpacing : 0);
            float totalHeight = padding.Top + contentHeight + padding.Bottom;

            ImGui.SetNextWindowPos(controlRect.BottomLeft);
            ImGui.SetNextWindowSize(new SVector2(controlRect.Width, totalHeight));

            // Push zero style padding for the container popup window itself 
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(0, 0));

            if (ImGui.BeginPopup($"##{uniqueId}_dropdown_popup")) {
                float screenBottom = ImGui.GetIO().DisplaySize.Y;
                float spaceBelow = screenBottom - controlRect.BottomLeft.Y;
                float dropdownHeight = Math.Min(totalHeight, spaceBelow);

                var popupDrawList = ImGui.GetWindowDrawList();
                var bgStart = ImGui.GetCursorScreenPos();
                var bgEnd = bgStart + new SVector2(width, dropdownHeight);

                popupDrawList.AddRectFilled(bgStart, bgEnd, options.DropdownBackgroundColor.ToImGui());
                popupDrawList.AddRect(bgStart, bgEnd, options.DropdownBorderColor.ToImGui());

                // FIX 1: Set explicit window padding variables inside our scrollable container to prevent engine overflow
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(padding.Left, padding.Top));
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(0, itemSpacing));

                // FIX 2: Passing 0 or -1 to the height constraint tells ImGui to automatically scale without triggering scrollbars
                if (ImGui.BeginChild($"##{uniqueId}dropdown_scroll", new SVector2(width, -1), ImGuiChildFlags.None)) {
                    var childDrawList = ImGui.GetWindowDrawList();

                    for (int i = 0; i < itemCount; i++) {
                        // FIX 3: Let ImGui advance the cursor naturally instead of multiplying padding offsets manually!
                        var currentTopLeft = ImGui.GetCursorScreenPos();
                        var itemRect = new Rect(currentTopLeft, width - padding.Left - padding.Right, itemHeight);

                        var itemText = options.Items[i];
                        var itemTextSize = ImGui.CalcTextSize(itemText);
                        var itemTextPos = itemRect.TopLeft + new SVector2(6, (float)Math.Ceiling((itemRect.Height - itemTextSize.Y) / 2));

                        // Handle Hovering background highlight FIRST so it draws underneath the text
                        ImGui.SetCursorScreenPos(itemRect.TopLeft);
                        if (ImGui.InvisibleButton($"##{uniqueId}_item_{i}", itemRect.Size)) {
                            selected = i;
                            newSelected = true;
                            ImGui.CloseCurrentPopup();
                        }

                        if (ImGui.IsItemHovered()) {
                            childDrawList.AddRectFilled(itemRect.TopLeft, itemRect.BottomRight, options.HoveredColor.ToImGui());
                        }
                        else if (i == selected) {
                            childDrawList.AddRectFilled(itemRect.TopLeft, itemRect.BottomRight, options.DropdownSelectedItemColor.ToImGui());
                        }

                        // Render Text Layer
                        var currentTextColor = (i == selected) ? options.DropdownSelectedTextColor : options.DropdownTextColor;
                        childDrawList.AddText(itemTextPos, currentTextColor.ToImGui(), itemText);
                    }
                    ImGui.EndChild();
                }

                ImGui.PopStyleVar(2); // Pop child layout variables
                ImGui.EndPopup();
            }

            ImGui.PopStyleVar(); // Pop main window padding variable
            return newSelected;
        }
    }
}

using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using ImGuiNET;
using System.Text.RegularExpressions;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;


namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public static class ColorPicker
    {
        private const int ControlHeight = 20;
        private const int PopupPaddingBottom = 3;
        private static readonly SVector2 PickerWindowSize = new SVector2(441, 197);
        private static readonly SVector2 ContentPadding = new SVector2(10, 10);
        private static readonly SVector2 ControlSpacing = new SVector2(8, 8);
        private static readonly SVector2 InputSpacing = new SVector2(5, 5);
        private static readonly SVector2 SliderSize = new(362, ControlHeight);
        private static readonly SVector2 SliderInputSize = new(45, ControlHeight);
        private static readonly SVector2 DefaultWindowOffset = new SVector2(10, 10);
        private static readonly uint BlackImGui = 0xFF000000;
        private static Dictionary<string, List<DXTC.PaletteSwatch>> MaterialFiltered =>
        DXTC.Palettes.Material.All.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Where(swatch => !swatch.Name.EndsWith(" 50")).ToList()
        );

        // working variables
        private static SColor default_color;
        private static HSLA working_HSLA;
        private static SVector2 windowSize = PickerWindowSize;
        private static int calculatedPaletteWindowHeight = 100;
        private static int calculatedPickerWindowHeight = 100;
        private static Options activeOptions = new Options();

        public class Options
        {
            public string Title = "Color Picker";
            public SVector2 WindowOffset = DefaultWindowOffset;
        }

        public static void Open(string uniqueID, SColor color, SVector2? windowOffset)
        {
            default_color = color;
            var (h, s, l, a) = color.ToHSLA();
            working_HSLA = new HSLA(h, s, l, a);
            PopupWindow.Open(uniqueID, windowOffset ?? DefaultWindowOffset);
        }

        public static void Draw(string uniqueID, ref SColor color, Options options = null)
        {
            // Initialize default values if not set
            if (options == null) activeOptions = new Options();

            windowSize.Y = calculatedPickerWindowHeight + calculatedPaletteWindowHeight;

            if (PopupWindow.Begin(uniqueID, new PopupWindow.Options { Size = windowSize, Title = activeOptions.Title, PanelPadding = new DXTPadding(3, 0, 3, calculatedPaletteWindowHeight + PopupPaddingBottom) }))
            {
                var popupTop = ImGui.GetWindowPos().Y; // Top of the popup window
                var contentPos = ImGui.GetCursorScreenPos();
                var layoutPos = contentPos + ContentPadding;


                var old_HSLA = working_HSLA;
                DrawHueSlider(layoutPos, ref working_HSLA);
                layoutPos.Y += ControlHeight + ControlSpacing.Y;
                DrawSaturationSlider(layoutPos, ref working_HSLA);
                layoutPos.Y += ControlHeight + ControlSpacing.Y;
                DrawLightnessSlider(layoutPos, ref working_HSLA);
                layoutPos.Y += ControlHeight + ControlSpacing.Y;
                DrawAlphaSlider(layoutPos, ref working_HSLA);
                if (working_HSLA != old_HSLA)
                {
                    color = DXTC.Color.FromHSLA(working_HSLA.H, working_HSLA.S, working_HSLA.L, working_HSLA.A);
                }

                var old_color = color;

                // Draw hexinput
                layoutPos.Y += ControlHeight + ControlSpacing.Y;
                int hexInputWidth = (int)(SliderInputSize.X * 3 + InputSpacing.X * 2);
                DrawHexColorInput(layoutPos, hexInputWidth, ref color);
                //Draw RGBA Input
                layoutPos.X += hexInputWidth + ControlSpacing.X;
                int rgbaInputWidth = (int)(SliderSize.X - hexInputWidth - ControlSpacing.X);
                DrawRGBAInput(layoutPos, rgbaInputWidth, ref color);
                // Draw Color Swatch with Reset Button
                layoutPos.X += ControlSpacing.X + rgbaInputWidth;
                DrawColorWithReset(layoutPos, (int)SliderInputSize.X, (int)SliderInputSize.X, ref color);

                // Draw Red Input
                layoutPos.X = contentPos.X + ContentPadding.X; // Reset X position for inputs
                layoutPos.Y += ControlHeight + InputSpacing.Y;
                ImGui.SetCursorScreenPos(layoutPos);
                int red = color.R;
                if (InputSpinner.Draw("ColorPickerRed", ref red, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = ControlHeight, Max = 255, Tooltip = Tooltip.BasicOptions("Red Component") }))
                {
                    red = Math.Clamp(red, 0, 255);
                    color = SColor.FromArgb(color.A, red, color.G, color.B);
                }
                // Draw Green Input
                layoutPos.X += SliderInputSize.X + InputSpacing.X;
                ImGui.SetCursorScreenPos(layoutPos);
                int green = color.G;
                if (InputSpinner.Draw("ColorPickerGreen", ref green, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = ControlHeight, Max = 255, Tooltip = Tooltip.BasicOptions("Green Component") }))
                {
                    green = Math.Clamp(green, 0, 255);
                    color = SColor.FromArgb(color.A, color.R, green, color.B);
                }
                // Draw Blue Input
                layoutPos.X += SliderInputSize.X + InputSpacing.X;
                ImGui.SetCursorScreenPos(layoutPos);
                int blue = color.B;
                if (InputSpinner.Draw("ColorPickerBlue", ref blue, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = ControlHeight, Max = 255, Tooltip = Tooltip.BasicOptions("Blue Component") }))
                {
                    blue = Math.Clamp(blue, 0, 255);
                    color = SColor.FromArgb(color.A, color.R, color.G, blue);
                }
                // Draw Palette Select
                layoutPos.X += SliderInputSize.X + ControlSpacing.X;
                ImGui.SetCursorScreenPos(layoutPos);

                //Select.Draw("ColorPickerPaletteSelect", ref selcted, new Select.Options {
                //    Width = rgbaInputWidth,
                //    Height = ControlHeight,
                //    Items = new() { "Google Material" },
                //    Tooltip = new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Palette Select" } } }
                //});

                layoutPos.Y += ControlHeight + ContentPadding.Y;
                calculatedPickerWindowHeight = (int)(layoutPos.Y - popupTop - 1 + PopupPaddingBottom); // cant figureout that 1 

                // Draw Palette 
                layoutPos.X = contentPos.X; // Reset X position for palette
                layoutPos.Y += PopupPaddingBottom;
                ImGui.SetCursorScreenPos(layoutPos);
                calculatedPaletteWindowHeight = DrawPalette(MaterialFiltered, 435, 16, new SVector2(-1, -1), ref color) + PopupPaddingBottom;

                if (old_color != color)
                {
                    var (h, s, l, a) = color.ToHSLA();
                    working_HSLA = new HSLA(h, s, l, a);
                }

                PopupWindow.End();
            }
        }


        private static void DrawHueSlider(SVector2 pos, ref HSLA working_HSLA)
        {
            var drawList = ImGui.GetWindowDrawList();
            // Draw border
            drawList.AddRect(pos, pos + SliderSize, BlackImGui); // Black Border
                                                                 // Draw Slider
            SVector2 sliderPos = pos + new SVector2(1, 1);
            var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
            int segments = 180;
            float segmentWidth = sliderBarSize.X / segments;
            // Draw hue gradient using current s, l, a
            for (int i = 0; i < segments; i++)
            {
                float hueVal = i * 360f / segments;
                var new_hsla = working_HSLA;
                new_hsla.H = hueVal;
                new_hsla.A = 1.0f;
                uint col = new_hsla.ToImGui();
                SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
                SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
                drawList.AddRectFilled(p0, p1, col);
            }
            // Draw slider handle
            float handleX = (float)Math.Round(sliderPos.X + (working_HSLA.H / 360f) * sliderBarSize.X);
            SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
            SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
            drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
            drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), BlackImGui);

            // Mouse interaction
            ImGui.SetCursorScreenPos(pos);
            ImGui.InvisibleButton("##hueslidergrip", SliderSize);
            if (ImGui.IsItemActive() && ImGui.IsMouseDown(0))
            {
                float mouseX = ImGui.GetIO().MousePos.X;
                float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
                working_HSLA.H = relX / sliderBarSize.X * 360f;
            }
            // Mouse wheel
            if (ImGui.IsItemHovered())
            {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    working_HSLA.H = working_HSLA.H + wheel;
                }
                Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Hue Slider" }, } });
            }

            // Draw Input box
            SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
            ImGui.SetCursorScreenPos(inputPos);
            int value = (int)Math.Round(working_HSLA.H);
            if (InputSpinner.Draw("ColorPickerHueSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y, Max = 360 }))
                working_HSLA.H = value;
        }
        private static void DrawSaturationSlider(SVector2 pos, ref HSLA working_HSLA)
        {
            var drawList = ImGui.GetWindowDrawList();
            // Draw border
            drawList.AddRect(pos, pos + SliderSize, BlackImGui); // Black Border
                                                                 // Draw Slider
            SVector2 sliderPos = pos + new SVector2(1, 1);
            var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
            int segments = 100;
            float segmentWidth = sliderBarSize.X / segments;
            // Draw saturation gradient using current hue and lightness
            for (int i = 0; i < segments; i++)
            {
                var new_hsla = working_HSLA;
                new_hsla.A = 1.0f;
                new_hsla.S = i / (float)(segments - 1); // normalized [0,1]
                var col = new_hsla.ToImGui();

                SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
                SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
                drawList.AddRectFilled(p0, p1, col);
            }
            // Draw slider handle
            float handleX = (float)Math.Round(sliderPos.X + working_HSLA.S * sliderBarSize.X);
            SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
            SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
            drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
            drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), BlackImGui);

            // Mouse interaction
            ImGui.SetCursorScreenPos(pos);
            ImGui.InvisibleButton("##satslidergrip", SliderSize);
            if (ImGui.IsItemActive() && ImGui.IsMouseDown(0))
            {
                float mouseX = ImGui.GetIO().MousePos.X;
                float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
                working_HSLA.S = relX / sliderBarSize.X;
            }
            // Mouse wheel
            if (ImGui.IsItemHovered())
            {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    if (ImGui.GetIO().KeyShift)
                    {
                        // Snap to nearest 0.1 and increment by 0.1 per wheel tick
                        float snapped = MathF.Round(working_HSLA.S * 1000);
                        working_HSLA.S = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                    }
                    else
                    {
                        // Snap to nearest 0.01 and increment by 0.01 per wheel tick
                        float snapped = MathF.Round(working_HSLA.S * 100);
                        working_HSLA.S = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                    }
                }
                Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Saturation Slider" }, } });
            }

            // Draw Input box
            SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
            ImGui.SetCursorScreenPos(inputPos);
            float value = working_HSLA.S * 100;
            if (InputSpinner.Draw("ColorPickerSatSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
                working_HSLA.S = value / 100;
        }
        private static void DrawLightnessSlider(SVector2 pos, ref HSLA working_HSLA)
        {
            var drawList = ImGui.GetWindowDrawList();
            // Draw border
            drawList.AddRect(pos, pos + SliderSize, BlackImGui); // Black Border

            // Draw Slider
            SVector2 sliderPos = pos + new SVector2(1, 1);
            var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
            int segments = 100;
            float segmentWidth = sliderBarSize.X / segments;
            // Draw lightness gradient using current hue and saturation
            for (int i = 0; i < segments; i++)
            {
                var new_hsla = working_HSLA;
                new_hsla.A = 1.0f;
                new_hsla.L = i / (float)(segments - 1); // normalized [0,1]
                var col = new_hsla.ToImGui();

                SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
                SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
                drawList.AddRectFilled(p0, p1, col);
            }
            // Draw slider handle
            float handleX = (float)Math.Round(sliderPos.X + working_HSLA.L * sliderBarSize.X);
            SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
            SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
            drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
            drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), BlackImGui);

            // Mouse interaction
            ImGui.SetCursorScreenPos(pos);
            ImGui.InvisibleButton("##lightslidergrip", SliderSize);
            if (ImGui.IsItemActive() && ImGui.IsMouseDown(0))
            {
                float mouseX = ImGui.GetIO().MousePos.X;
                float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
                working_HSLA.L = relX / sliderBarSize.X;
            }
            // Mouse wheel
            if (ImGui.IsItemHovered())
            {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    if (ImGui.GetIO().KeyShift)
                    {
                        float snapped = MathF.Round(working_HSLA.L * 1000);
                        working_HSLA.L = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                    }
                    else
                    {
                        float snapped = MathF.Round(working_HSLA.L * 100);
                        working_HSLA.L = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                    }
                }
                Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Lightness Slider" }, } });
            }

            // Draw Input box
            SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
            ImGui.SetCursorScreenPos(inputPos);
            float value = working_HSLA.L * 100;
            if (InputSpinner.Draw("ColorPickerLightnessSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
                working_HSLA.L = value / 100;
        }
        private static void DrawAlphaSlider(SVector2 pos, ref HSLA working_HSLA)
        {
            var drawList = ImGui.GetWindowDrawList();
            // Draw border
            drawList.AddRect(pos, pos + SliderSize, BlackImGui); // Black Border

            // Draw Slider
            SVector2 sliderPos = pos + new SVector2(1, 1);
            var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
            DXT.Draw.Checkerboard(sliderPos, sliderBarSize.X, sliderBarSize.Y);
            int segments = 100;
            float segmentWidth = sliderBarSize.X / segments;
            // Draw alpha gradient using current hue, saturation, lightness
            for (int i = 0; i < segments; i++)
            {
                var new_hsla = working_HSLA;
                new_hsla.A = i / (float)(segments - 1); // normalized [0,1]
                var col = new_hsla.ToImGui();

                SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
                SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
                drawList.AddRectFilled(p0, p1, col);
            }
            // Draw slider handle
            float handleX = (float)Math.Round(sliderPos.X + working_HSLA.A * sliderBarSize.X);
            SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
            SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
            drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
            drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), BlackImGui);

            // Mouse interaction
            ImGui.SetCursorScreenPos(pos);
            ImGui.InvisibleButton("##alphaslidergrip", SliderSize);
            if (ImGui.IsItemActive() && ImGui.IsMouseDown(0))
            {
                float mouseX = ImGui.GetIO().MousePos.X;
                float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
                working_HSLA.A = relX / sliderBarSize.X;
            }
            // Mouse wheel
            if (ImGui.IsItemHovered())
            {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    if (ImGui.GetIO().KeyShift)
                    {
                        float snapped = MathF.Round(working_HSLA.A * 1000);
                        working_HSLA.A = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                    }
                    else
                    {
                        float snapped = MathF.Round(working_HSLA.A * 100);
                        working_HSLA.A = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                    }
                }
                Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Alpha Slider" }, } });
            }

            // Draw Input box
            SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
            ImGui.SetCursorScreenPos(inputPos);
            float value = working_HSLA.A * 100;
            if (InputSpinner.Draw("ColorPickerAlphaSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
                working_HSLA.A = value / 100;
        }

        private static void DrawRGBAInput(SVector2 pos, int inputWidth, ref SColor color)
        {
            string rgbaBuf = $"{color.R},{color.G},{color.B},{color.A}";

            ImGui.SetCursorScreenPos(pos);
            Display.Draw("##ColorPickerRGBAInput", rgbaBuf, new Display.Options { Width = inputWidth, Height = ControlHeight });

            if (ImGui.IsItemHovered())
            {
                if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    ImGui.SetClipboardText(rgbaBuf);
                }
                if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    string clipboard = ImGui.GetClipboardText() ?? "";
                    var matches = Regex.Matches(clipboard, @"\b([0-9]{1,3})\b");
                    var parts = matches.Cast<Match>()
                        .Select(m => m.Value)
                        .Where(s => int.TryParse(s, out int val) && val >= 0 && val <= 255)
                        .ToArray();
                    if (parts.Length == 4)
                    {
                        var r = int.Parse(parts[0]);
                        var g = int.Parse(parts[1]);
                        var b = int.Parse(parts[2]);
                        var a = int.Parse(parts[3]);
                        color = DXTC.Color.FromRGBA(r, g, b, a);
                    }
                    else if (parts.Length == 3)
                    {
                        var r = int.Parse(parts[0]);
                        var g = int.Parse(parts[1]);
                        var b = int.Parse(parts[2]);
                        color = DXTC.Color.FromRGBA(r, g, b, 255);
                    }
                }

                Tooltip.Draw(new Tooltip.Options
                {
                    Lines = {
                    new Tooltip.Title { Text = $"Red: {color.R} Green: {color.G} Blue: {color.B} Alpha: {color.A}" },
                    new Tooltip.Separator { },
                    new Tooltip.DoubleLine { LeftText = "Ctrl + LeftClick:", RightText = "Copy to clipboard" },
                    new Tooltip.DoubleLine { LeftText = "Ctrl + RightClick:", RightText = "Paste from clipboard" },
                }
                });
            }
        }
        private static void DrawHexColorInput(SVector2 pos, int inputWidth, ref SColor color)
        {
            // convert color to hex
            string hexBuf = color.ToHEX();

            ImGui.SetCursorScreenPos(pos);
            Display.Draw("##ColorPickerHexInput", hexBuf, new Display.Options { Width = inputWidth, Height = ControlHeight });

            if (ImGui.IsItemHovered())
            {
                if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    ImGui.SetClipboardText(hexBuf);
                }
                if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    string clipboard = ImGui.GetClipboardText() ?? "";
                    string hex = clipboard.Replace("#", "");
                    bool valid = (hex.Length == 6 || hex.Length == 8) && hex.All(c =>
                        c >= '0' && c <= '9' ||
                        c >= 'A' && c <= 'F' ||
                        c >= 'a' && c <= 'f'
                    );
                    if (valid)
                    {
                        try
                        {
                            color = SColor.FromArgb(
                                hex.Length == 8 ? Convert.ToInt32(hex.Substring(6, 2), 16) : 255,
                                Convert.ToInt32(hex.Substring(0, 2), 16),
                                Convert.ToInt32(hex.Substring(2, 2), 16),
                                Convert.ToInt32(hex.Substring(4, 2), 16)
                            );
                        }
                        catch
                        {
                            // Do nothing if conversion fails
                        }
                    }
                }

                Tooltip.Draw(new Tooltip.Options
                {
                    Lines = {
                    new Tooltip.Title { Text = "Color Slider" },
                    new Tooltip.Separator { },
                    new Tooltip.DoubleLine { LeftText = "Ctrl + LeftClick:", RightText = "Copy to clipboard" },
                    new Tooltip.DoubleLine { LeftText = "Ctrl + RightClick:", RightText = "Paste from clipboard" },
                }
                });
            }
        }

        private static void DrawColorWithReset(SVector2 pos, int inputWidth, int inputHeight, ref SColor color)
        {
            int smallBoxSize = 19;
            var drawList = ImGui.GetWindowDrawList();
            // Large color box
            DXT.Draw.Checkerboard(pos, inputWidth, inputHeight);
            drawList.AddRectFilled(pos, pos + new SVector2(inputWidth, inputHeight), color.ToImGui());
            drawList.AddRect(pos + new SVector2(1, 1), pos + new SVector2(inputWidth - 1, inputHeight - 1), DXTC.Colors.SwatchInnerGlow.ToImGui());

            drawList.AddRect(pos, pos + new SVector2(inputWidth, inputHeight), BlackImGui);

            // Small reset box (top-left corner, flush with big box)
            SVector2 smallPos = pos;
            DXT.Draw.Checkerboard(pos, smallBoxSize, smallBoxSize);
            drawList.AddRectFilled(smallPos, smallPos + new SVector2(smallBoxSize, smallBoxSize), default_color.ToImGui());
            drawList.AddRect(smallPos + new SVector2(1, 1), smallPos + new SVector2(smallBoxSize - 1, smallBoxSize - 1), DXTC.Colors.SwatchInnerGlow.ToImGui());
            drawList.AddRect(smallPos, smallPos + new SVector2(smallBoxSize, smallBoxSize), BlackImGui);

            // Invisible button for small box
            ImGui.SetCursorScreenPos(smallPos);
            if (ImGui.InvisibleButton("##ResetColorBox", new SVector2(smallBoxSize, smallBoxSize)))
            {
                color = default_color;
            }

            // Tooltip for small box
            if (ImGui.IsItemHovered())
            {
                Tooltip.Draw(new Tooltip.Options
                {
                    Lines = {
                    new Tooltip.Title { Text = "Original Color" },
                    new Tooltip.Separator { },
                    new Tooltip.DoubleLine { LeftText = "Red:", RightText = default_color.R.ToString() },
                    new Tooltip.DoubleLine { LeftText = "Green:", RightText = default_color.G.ToString() },
                    new Tooltip.DoubleLine { LeftText = "Blue:", RightText = default_color.B.ToString() },
                    new Tooltip.DoubleLine { LeftText = "Alpha:", RightText = default_color.A.ToString() },
                }
                });
            }
        }

        public static int DrawPalette(Dictionary<string, List<DXTC.PaletteSwatch>> palette, int paletteWidth, int colorHeight, SVector2 colorSpacing, ref SColor selectedColor)
        {
            var drawList = ImGui.GetWindowDrawList();
            SVector2 cursor = ImGui.GetCursorScreenPos();

            int columnCount = palette.Count;
            int spacingX = (int)Math.Round(colorSpacing.X);

            // Calculate swatch width (all the same, pixel perfect)
            int totalSpacing = (columnCount - 1) * spacingX;
            int swatchWidth = (paletteWidth - totalSpacing) / columnCount;

            // Calculate left/right margin to center the palette
            int usedWidth = columnCount * swatchWidth + totalSpacing;
            int leftMargin = (paletteWidth - usedWidth) / 2;

            // Use leftMargin for top/bottom padding
            int topBottomPad = leftMargin;

            // Calculate palette height (max swatch count in any column)
            int maxSwatchCount = palette.Max(g => g.Value.Count);
            int spacingY = (int)Math.Round(colorSpacing.Y);
            int paletteHeight = maxSwatchCount * colorHeight + ((maxSwatchCount > 1) ? (maxSwatchCount - 1) * spacingY : 0);

            // Draw background box
            var bgStart = cursor;
            var bgEnd = cursor + new SVector2(paletteWidth, paletteHeight + 2 * topBottomPad);
            //drawList.AddRectFilled(bgStart, bgEnd, ColorTools.RGBA2Uint(100,100,100,100));
            drawList.AddRectFilled(bgStart, bgEnd, DXTC.Colors.Panel.ToImGui());
            drawList.AddRect(bgStart, bgEnd, DXTC.Colors.PanelInnerGlow.ToImGui());

            // Draw palette swatches centered with padding
            int colIndex = 0;
            foreach (var group in palette)
            {
                int colX = (int)Math.Round(cursor.X + leftMargin + colIndex * (swatchWidth + spacingX));
                int colY = (int)Math.Round(cursor.Y + topBottomPad);

                int swatchCount = group.Value.Count;

                for (int i = 0; i < swatchCount; i++)
                {
                    var swatch = group.Value[i];
                    int swatchY = colY + i * (colorHeight + spacingY);

                    var swatchPos = new SVector2(colX, swatchY);

                    // Draw swatch background
                    drawList.AddRectFilled(swatchPos, swatchPos + new SVector2(swatchWidth, colorHeight), swatch.Uint);

                    // Draw inner glow
                    drawList.AddRect(swatchPos + new SVector2(1, 1), swatchPos + new SVector2(swatchWidth - 1, colorHeight - 1), DXTC.Colors.SwatchInnerGlow.ToImGui());

                    // Draw black border
                    drawList.AddRect(swatchPos, swatchPos + new SVector2(swatchWidth, colorHeight), 0xFF000000);

                    // Draw invisible button for interaction
                    ImGui.SetCursorScreenPos(swatchPos);
                    if (ImGui.InvisibleButton($"##swatch_{group.Key}_{i}", new SVector2(swatchWidth, colorHeight)))
                    {
                        selectedColor = swatch.Color;
                    }

                    // Optional: Tooltip
                    if (ImGui.IsItemHovered())
                    {
                        Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = $"{swatch.Name}" } } });
                    }
                }
                colIndex++;
            }

            // Return the full height of the panel drawn
            return paletteHeight + 2 * topBottomPad;
        }



    }
}




using DieselExileTools.Common;
using ImGuiNET;
using System.Runtime.CompilerServices;
using System.Text;
using static DieselExileTools.Common.DXTC;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace DieselExileTools.ExileCore2;

public class DBugSettings
{
    public bool ShowLog = false;
    public bool ShowToolbar = false;
    public bool ShowMonitor = false;
    public bool ShowPanels = false;

    public int MaxLogEntries = 500; // Maximum number of log entries to keep

    public Dictionary<string, bool> CategoryPanelsCollapsed = new Dictionary<string, bool>();

    public SColor[] buttonColors = new SColor[]{
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,
        DXTC.Colors.Button,

    };
    public SColor[] buttonTextColors = new SColor[]{
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
    };
    public SColor[] TextColors = new SColor[] {
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
        DXTC.Colors.ControlText,
    };

    public SColor TitleTextColor = DXTC.Colors.ControlText;
    public SColor PanelColor = DXTC.Colors.Panel;
    public SColor sliderColor = DXTC.Colors.Input;
    public SColor sliderTextColor = DXTC.Colors.InputText;
    public int PanelSliderTest = 50;

    public SColor displayColor = DXTC.Colors.Input;
    public SColor displayTextColor = DXTC.Colors.InputText;

}

public static class DBug {

    private static class _Logger {
        private const uint BUFFER_SIZE = (uint)65536; // 64 KB buffer size
        private static readonly LinkedList<(DateTime Date, string Description, int Count)> _logEntries = new();
        private static string _logText = string.Empty;
        private static string _lastEntry = string.Empty;
        private static int _repeatCount = 1;
        public static void AddLogEntry(string entry) {
            if (_logEntries.Count > 0 && entry == _logEntries.Last.Value.Description) {
                var last = _logEntries.Last.Value;
                _logEntries.RemoveLast();
                _logEntries.AddLast((DateTime.Now, entry, last.Count + 1));
            }
            else {
                _repeatCount = 1;
                _lastEntry = entry;
                _logEntries.AddLast((DateTime.Now, entry, 1));
            }
            while (_logEntries.Count > Settings.MaxLogEntries)
                _logEntries.RemoveFirst();
        }
        public static void Clear() {
            _logEntries.Clear();
            _lastEntry = string.Empty;
            _repeatCount = 1;
        }
        public static void Render(bool newestFirst = false) {
            if (DXT.Window.Begin($"{DXT.PluginName}Log", ref Settings.ShowLog, new DXT.Window.Options { Title = $"{DXT.PluginName} DBug Log", Resizable = true })) {
                var contentPos = ImGui.GetCursorScreenPos();
                ImGui.Indent(2);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(1, 0));
                if (DXT.Button.Draw($"{DXT.PluginName}LogClear", new DXT.Button.Options { Label = "Clear", Width = 80, Height = 22, Tooltip = DXT.Tooltip.BasicOptions("Clear Log History") })) {
                    Clear();
                }
                ImGui.SameLine();
                LogHeader?.Invoke(80, 22);
                ImGui.PopStyleVar();

                ImGui.Dummy(new SVector2(0, 2));
                ImGui.Indent(1);

                var panelName = $"{DXT.PluginName}LogPanel";
                if (DXT.Panel.Begin(panelName, new DXT.Panel.Options { Width = -3, Height = -3 })) {

                    var logString = new StringBuilder((int)BUFFER_SIZE);
                    var entries = newestFirst ? _logEntries.Reverse() : _logEntries;
                    foreach (var (dateTime, message, count) in entries) {
                        var display = count > 1 ? $"[{count}] {message}" : message;
                        logString.AppendLine($"{dateTime:HH:mm:ss.fff}: {display}");
                    }
                    string logText = logString.ToString();
                    var avail = ImGui.GetContentRegionAvail();
                    ImGui.InputTextMultiline($"{DXT.PluginName}LogText", ref logText, BUFFER_SIZE, new SVector2(avail.X - 4, avail.Y - 4), ImGuiInputTextFlags.ReadOnly);

                    DXT.Panel.End(panelName);
                }

                ImGui.Unindent(3);
                DXT.Window.End();
            }
        }
    }

    private static class _Monitor {
        private static SortedDictionary<string, SortedDictionary<string, MonitoredVariable>> monitoredVariables = new SortedDictionary<string, SortedDictionary<string, MonitoredVariable>>();
        private class MonitoredVariable
        {
            public DateTime DateChanged;
            public DateTime DateUpdated;
            public string Category;
            public string Name;
            public object Value;
            public string FilePath;
            public string? FileName;
            public string? ShortFilePath;
            public int Line;
            public string Member;
            public List<(DateTime Time, object Value)> ChangeHistory = new(); // Store up to 10 changes
        }

        public static void AddMonitoredVariable(string category, string name, object variable, string filePath, int line, string member) {
            // ensure setting exists for categories collapsing panel
            if (!Settings.CategoryPanelsCollapsed.ContainsKey(category)) Settings.CategoryPanelsCollapsed[category] = false;

            // Get or create category dictionary
            if (!monitoredVariables.TryGetValue(category, out var categoryDict)) {
                categoryDict = new SortedDictionary<string, MonitoredVariable>();
                monitoredVariables[category] = categoryDict;
            }

            // Get or create monitored variable
            if (!categoryDict.TryGetValue(name, out var monitored)) {
                string shortPath = null, fileName = null;
                if (!string.IsNullOrEmpty(filePath)) {
                    string directory = Path.GetDirectoryName(filePath);
                    string lastDir = directory?.Split(Path.DirectorySeparatorChar).Last();
                    fileName = Path.GetFileName(filePath);
                    shortPath = lastDir != null ? $"{lastDir}{Path.DirectorySeparatorChar}{fileName}" : fileName;
                }

                monitored = new MonitoredVariable {
                    Category = category,
                    Name = name,
                    Value = variable,
                    DateChanged = DateTime.Now,
                    FilePath = filePath,
                    Line = line,
                    Member = member,
                    ShortFilePath = shortPath,
                    FileName = fileName,
                    ChangeHistory = new List<(DateTime, object)>()
                };
                categoryDict[name] = monitored;
                return;
            }
            monitored.DateUpdated = DateTime.Now;

            // Track changes
            bool changed = monitored.Value == null ? variable != null : !Equals(monitored.Value, variable);
            if (changed) {
                // Add previous value to history
                if (monitored.Value != null) {
                    monitored.ChangeHistory.Add((monitored.DateChanged, monitored.Value));
                    if (monitored.ChangeHistory.Count > 10)
                        monitored.ChangeHistory.RemoveAt(0); // Keep only last 10
                }
                monitored.DateChanged = DateTime.Now;
            }

            monitored.Value = variable;
        }

        private static SColor defaultColor = Colors.InputText;
        private static SColor red = Palettes.Material.Red.RedA700.Color;
        private static SColor yellow = Palettes.Material.Yellow.YellowA700.Color;
        private static SColor orange = Palettes.Material.Orange.OrangeA700.Color;
        private static SColor green = Palettes.Material.Green.GreenA700.Color;
        private static SColor lightGreen = Palettes.Material.LightGreen.LightGreenA700.Color;
        private static SColor lightBlue = Palettes.Material.LightBlue.LightBlueA700.Color;
        private static SColor purple = Palettes.Material.Purple.PurpleA700.Color;
        private static (SColor Color, string Value) FormatVariable(object variable) {
            if (variable == null) return (red, "null"); // Use a distinct color for nulls if you want

            switch (variable) {
                case int i:
                    return (yellow, i.ToString());
                case float f:
                    return (yellow, f.ToString());
                case double d:
                    return (yellow, d.ToString());
                case bool b:
                    return (purple, b.ToString());
                case Enum e:
                    return (purple, e.ToString());
                case string s:
                    if (string.IsNullOrWhiteSpace(s)) return (red, "empty string");
                    return (lightGreen, s);
                case IEnumerable<object> list:
                    return (defaultColor, $"List[{list.Count()}]");
                case DateTime dt:
                    return (lightBlue, dt.ToString("yyyy-MM-dd HH:mm:ss"));
                default:
                    return (defaultColor, variable.ToString());
            }
        }

        private static int windowHeight = 400;
        private static int _int = 0;
        public static void RenderTest() {
            // Basic types
            String s = "t";
            Monitor("Basic", "Bool", true);
            Monitor("Basic", "Int", _int++);
            Monitor("Basic", "Float", 0.016f);
            Monitor("Basic", "String", s);

            Monitor("Edge", "NullValue", null);
            Monitor("Edge", "EmptyString", "");
            Monitor("Edge", "NegativeInt", -42);
            Monitor("Edge", "LargeFloat", 1e10f);

            // Complex types
            Monitor("Complex", "Position", new SVector2(100, 200));
            Monitor("Complex", "Inventory", new List<string> { "Sword", "Shield" });
            Monitor("Complex", "Settings", Settings);
        }
        public static void Render(bool newestFirst = false) {
            //RenderTest();
            if (DXT.Window.Begin($"{DXT.PluginName}DBuggerMonitor", ref Settings.ShowMonitor, new DXT.Window.Options { Title = $"{DXT.PluginName} DBug Monitor", Resizable = true, MinWidth = 200, LockHeight = windowHeight })) {
                windowHeight = 20; // titel bar
                var startinPos = ImGui.GetCursorScreenPos();
                ImGui.Indent(3);

                foreach (var categoryPair in monitoredVariables) {
                    bool isCollapsed = Settings.CategoryPanelsCollapsed[categoryPair.Key];
                    var panelName = $"{DXT.PluginName}DBuggerMonitor{categoryPair.Key}Panel";
                    if (DXT.CollapsingPanel.Begin(panelName, ref isCollapsed, new DXT.CollapsingPanel.Options { Label = $"{categoryPair.Key}", Width = -3 })) {
                        var first = true;
                        foreach (var variablePair in categoryPair.Value) {
                            if (!first) ImGui.Dummy(new SVector2(0, 3));

                            var monitoredVariable = variablePair.Value;
                            var avail = ImGui.GetContentRegionAvail();
                            var inputWidth = (int)(avail.X * 0.6f); // float is percent of available width 0.7f = 70%
                            var textWidth = avail.X - inputWidth;

                            DXT.Display.Draw($"{panelName}{variablePair.Key}_name", monitoredVariable.Name, new DXT.Display.Options { DrawBackground = false, Width = (int)textWidth, Height = 20 });
                            ImGui.SameLine();
                            var (color, value) = FormatVariable(monitoredVariable.Value);
                            DXT.Display.Draw($"{panelName}{variablePair.Key}_var", value, new DXT.Display.Options { Width = inputWidth - 3, Height = 20, TextColor = color });
                            if (ImGui.IsItemHovered()) {
                                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) ImGui.SetClipboardText(value.ToString());

                                var tooltipLines = new List<DXT.Tooltip.Line> {
                                    new DXT.Tooltip.DoubleLine { LeftText = monitoredVariable.Name , LeftColor = Colors.ControlText, RightText = monitoredVariable.Value != null ? monitoredVariable.Value.GetType().ToString() : "null" },
                                    new DXT.Tooltip.Separator { },
                                    new DXT.Tooltip.DoubleLine { LeftText = "File:", RightText = $"{monitoredVariable.ShortFilePath ?? monitoredVariable.FileName ?? monitoredVariable.FilePath}" },
                                    new DXT.Tooltip.DoubleLine { LeftText = "Line:", RightText =  $"{monitoredVariable.Line}" },
                                    new DXT.Tooltip.DoubleLine { LeftText = "Member:", RightText =  $"{monitoredVariable.Member}" },
                                    new DXT.Tooltip.Separator { },
                                    new DXT.Tooltip.DoubleLine { LeftText = "Updated:", RightText = $"{monitoredVariable.DateUpdated:HH:mm:ss.fff}" },
                                    new DXT.Tooltip.DoubleLine { LeftText = "Changed:", RightText = $"{monitoredVariable.DateChanged:HH:mm:ss.fff}" },
                                    new DXT.Tooltip.Separator { }
                                };
                                // Add change history if available
                                if (monitoredVariable.ChangeHistory.Count > 0) {
                                    foreach (var (time, val) in monitoredVariable.ChangeHistory) {
                                        var (c, s) = FormatVariable(val);
                                        tooltipLines.Add(new DXT.Tooltip.DoubleLine {
                                            LeftText = time.ToString("HH:mm:ss.fff"),
                                            RightText = s
                                        });
                                    }
                                    tooltipLines.Add(new DXT.Tooltip.Separator { });
                                }
                                tooltipLines.Add(new DXT.Tooltip.DoubleLine { LeftText = "LeftClick:", RightText = "Copy to clipboard" });

                                DXT.Tooltip.Draw(new DXT.Tooltip.Options { Lines = tooltipLines });

                            }
                            first = false;
                        }

                        ImGui.Dummy(new SVector2(0, 3)); // Panel PadBottom
                        DXT.CollapsingPanel.End(panelName);
                    }
                    ImGui.Dummy(new SVector2(0, 3)); // panel spacing

                    Settings.CategoryPanelsCollapsed[categoryPair.Key] = isCollapsed;
                }

                ImGui.Unindent(3);
                windowHeight += (int)(ImGui.GetCursorScreenPos().Y - startinPos.Y);
                DXT.Window.End();
            }
        }
    }

    private static class _UIColors
    {
        private static float windowHeight = 20;
        private static SVector2 controlSize = new(80, 22);

        public static void Render(bool newestFirst = false) {

            var windowOptions = new DXT.Window.Options {
                Title = $"{DXT.PluginName} UI Coloring",
                Resizable = true,
                MinHeight = 300,
                MinWidth = 400,
                TitleTextColor = Settings.TitleTextColor,
            };
            var panelOptions = new DXT.Panel.Options {
                Width = -3,
                Height = -3,
                Color = Settings.PanelColor,
            };
            var panelName = $"{DXT.PluginName}LogPanel";
            var colorSelectOptions = new DXT.ColorSelect.Options { Width = (int)controlSize.Y, Height = (int)controlSize.Y, };
            var buttonOptions = new DXT.Button.Options { Width = (int)controlSize.X, Height = (int)controlSize.Y, };


            if (DXT.Window.Begin($"{DXT.PluginName}UIColoring", ref Settings.ShowPanels, windowOptions)) {
                var contentPos = ImGui.GetCursorScreenPos();
                windowHeight = windowOptions.TitleBarHeight; 
                ImGui.Indent(3);


                if (DXT.Panel.Begin(panelName, panelOptions)) {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(3,3));

                    DXT.ColorSelect.Draw($"{DXT.PluginName}TitleTextColor", "TitleTextColor", ref Settings.TitleTextColor, colorSelectOptions);

                    DXT.ColorSelect.Draw($"{DXT.PluginName}PanelColor", "PanelColor", ref Settings.PanelColor, colorSelectOptions);

                    // Buttons
                    buttonOptions.Label = $"Default";
                    buttonOptions.Tooltip = DXT.Tooltip.BasicOptions($"Default Button");
                    DXT.Button.Draw($"{panelName}Defaultbutton", buttonOptions);
                    ImGui.SameLine();
                    ImGui.Dummy(new SVector2(controlSize.Y, controlSize.Y));
                    ImGui.SameLine();
                    ImGui.Dummy(new SVector2(controlSize.Y, controlSize.Y));

                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            int index = i * 3 + j;
                            buttonOptions.Color = Settings.buttonColors[index];
                            buttonOptions.TextColor = Settings.buttonTextColors[index];
                            buttonOptions.Label = $"Button {index}";
                            buttonOptions.Tooltip = DXT.Tooltip.BasicOptions($"Button {index} Test");
                            DXT.Button.Draw($"{panelName}button{index}", buttonOptions);
                            ImGui.SameLine();
                            DXT.ColorSelect.Draw($"{panelName}button{index}_Swatch", $"Button {index} Color", ref Settings.buttonColors[index], colorSelectOptions);
                            ImGui.SameLine();
                            DXT.ColorSelect.Draw($"{panelName}button{index}_TextSwatch", $"Button {index} Text Color", ref Settings.buttonTextColors[index], colorSelectOptions);
                            if (j < 2) ImGui.SameLine();
                        }
                    }

                    // Sliders
                    var sliderOptions = new DXT.Slider.Options { Width = (int)controlSize.X, Height = (int)controlSize.Y, Min = 0, Max = 100 };
                    DXT.Slider.Draw($"{DXT.PluginName}DefaultSlider", ref Settings.PanelSliderTest, sliderOptions);

                    sliderOptions.BackgroundColor = Settings.sliderColor;
                    sliderOptions.TextColor = Settings.sliderTextColor;
                    DXT.Slider.Draw($"{DXT.PluginName}Slider", ref Settings.PanelSliderTest, sliderOptions);
                    ImGui.SameLine();
                    DXT.ColorSelect.Draw($"{panelName}Slider_Swatch", $"Slider Color", ref Settings.sliderColor, colorSelectOptions);
                    ImGui.SameLine();
                    DXT.ColorSelect.Draw($"{panelName}Slider_TextSwatch", $"Slider Text Color", ref Settings.sliderTextColor, colorSelectOptions);

                    // Display 
                    var displayOptions = new DXT.Display.Options { Width = (int)controlSize.X, Height = (int)controlSize.Y };

                    DXT.Display.Draw($"{panelName}DisplayDefault", "Default", displayOptions);

                    displayOptions.BackgroundColor = Settings.displayColor;
                    displayOptions.TextColor = Settings.displayTextColor;
                    DXT.Display.Draw($"{panelName}Display", "Style", displayOptions);
                    ImGui.SameLine();
                    DXT.ColorSelect.Draw($"{panelName}Display_Swatch", $"Display Color", ref Settings.displayColor, colorSelectOptions);
                    ImGui.SameLine();
                    DXT.ColorSelect.Draw($"{panelName}Display_TextSwatch", $"Display Text Color", ref Settings.displayTextColor, colorSelectOptions);

                    // color text 
                    var numTextColors = 5;
                    for (int i = 0; i < numTextColors; i++) {
                        DXT.ColorSelect.Draw($"{panelName}textColorSwatch{i}", $"Text Color {i}", ref Settings.TextColors[i], colorSelectOptions);
                        if (i < numTextColors - 1) ImGui.SameLine();
                    }
                    for (int i = 0; i < numTextColors; i++) {
                        ImGui.TextColored(Settings.TextColors[i].ToVector4(), $"Text Color {i} ");
                        ImGui.SameLine();
                    }

                    ImGui.Dummy(new SVector2(0, 2));
                    ImGui.PopStyleVar();
                    DXT.Panel.End(panelName);
                }

                ImGui.Unindent(3);
                DXT.Window.End();
            }

        }

    }

    public static DBugSettings Settings { get; private set; } = new DBugSettings();
    public static DXT.FloatingToolbar.Options? ToolbarOptions { get; set; }
    public static List<DXT.FloatingToolbar.Tool> AdditionalTools { get; set; } = new();
    public static Action<int, int>? LogHeader { get; set; }


    public static void Initialize() {
        Settings = DXT.Settings.DBug;

        if (ToolbarOptions == null) { // dont override if already set
            ToolbarOptions = new DXT.FloatingToolbar.Options {
                Tools = new List<DXT.FloatingToolbar.Tool>{
                    new DXT.FloatingToolbar.Label { Text = $"{DXT.PluginName} DBug" },
                    new DXT.FloatingToolbar.Button {
                        Label = "Log",
                        SetChecked = (bool state) => { Settings.ShowLog = state; },
                        GetChecked = () => Settings.ShowLog,
                    },
                    new DXT.FloatingToolbar.Button {
                        Label = "Monitor",
                        SetChecked = (bool state) => { Settings.ShowMonitor = state; },
                        GetChecked = () => Settings.ShowMonitor,
                    },
                },
            };

            if (DXT.PluginName == "Playground") {
                ToolbarOptions.Tools.Add(new DXT.FloatingToolbar.Button {
                    Label = "Panels",
                    SetChecked = (bool state) => { Settings.ShowPanels = state; },
                    GetChecked = () => Settings.ShowPanels,
                });
            }
        }
        foreach (var tool in AdditionalTools) ToolbarOptions.Tools.Add(tool);
        // add close button
        ToolbarOptions.Tools.Add(new DXT.FloatingToolbar.Button {
            Width = 20,
            Height = 20,
            Color = DXTC.Colors.ButtonClose,
            OnClick = () => Settings.ShowToolbar = false,
            Tooltip = DXT.Tooltip.BasicOptions("Close Toolbar"),
        });
    }

    public static void Log(string message, bool whenVisibleOnly = true) {
        if (!whenVisibleOnly || Settings.ShowLog) {
            _Logger.AddLogEntry(message);
        }
    }
    public static void ClearLog() {
        _Logger.Clear();
    }
    public static void Monitor(string category, string name, object variable, bool whenVisibleOnly = true, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "") {
        if (whenVisibleOnly && !Settings.ShowMonitor) return;
        _Monitor.AddMonitoredVariable(category, name, variable, file, line, member);
    }

    public static void Render() {
        if (ToolbarOptions != null && Settings.ShowToolbar) DXT.FloatingToolbar.Draw($"{DXT.PluginName}DBuggerToolbar", ToolbarOptions);

        _Monitor.Render();
        _Logger.Render();
        _UIColors.Render();
    }
}













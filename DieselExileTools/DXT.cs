using ExileCore2;
using ImGuiNET;
using SVector2 = System.Numerics.Vector2;
using ExileGraphics = ExileCore2.Graphics;

namespace DieselExileTools.ExileCore2;

public static partial class DXT {
    public static String PluginName { get; private set; } = "place_holder";
    public static String PluginDirectory { get; private set; } = "place_holder";

    private static ExileGraphics? _graphics;
    public static ExileGraphics Graphics {
        get => _graphics ?? throw new InvalidOperationException("Plugin must Initialise DXT before use, DXT.Initialise");
        private set => _graphics = value;
    }

    private static GameController? _gameController;
    public static GameController GameController {
        get => _gameController ?? throw new InvalidOperationException("Plugin must Initialise DXT before use, DXT.Initialise");
        private set => _gameController = value;
    }

    private static DXTSettings? _settings;
    public static DXTSettings Settings {
        get => _settings ?? throw new InvalidOperationException("Plugin must Initialise DXT before use, DXT.Initialise");
        set => _settings = value;
    }

    /// <summary> checks if the game window is currently focused or any ImGui item is being interacted with (hovered or active). </summary>
    public static bool IsGameFocused => GameController.Window.IsForeground() || ImGui.IsAnyItemActive() || ImGui.IsAnyItemHovered() || ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);
    public static bool IsInventoryPanelVisible => GameController?.IngameState?.IngameUi?.InventoryPanel?.IsVisible ?? false;

    public static SVector2 GameWindowPos => GameController.Window.GetWindowRectangleTimeCache.TopLeft;


    public class Config
    {
        public required string PluginName { get; set; }
        public required string PluginDirectory { get; set; }
        public required GameController GameController { get; set; }
        public required ExileGraphics Graphics { get; set; }
        public required DXTSettings Settings { get; set; }
    }

    public static void Initialise(Config config) {
        PluginName = config.PluginName;
        PluginDirectory = config.PluginDirectory;
        GameController = config.GameController;
        Graphics = config.Graphics;
        Settings = config.Settings;

        DBug.Initialize();

        DBug.Monitor("DXT", "Initialised", true, false);
        DBug.Log($"DieselExileTools for {PluginName} initialized", false);
    }

}

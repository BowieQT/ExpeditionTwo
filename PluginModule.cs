namespace ExpeditionTwo;

public abstract class PluginModule {
    protected Plugin Plugin { get; }
    protected Settings Settings => Plugin.Settings;
    protected ExileCore2.GameController GameController => Plugin.GameController;
    protected ExileCore2.Graphics Graphics => Plugin.Graphics;

    protected PluginModule(Plugin plugin) {
        Plugin = plugin;
    }

}

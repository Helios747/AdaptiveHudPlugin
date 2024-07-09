using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using AdaptiveHud.Windows;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace AdaptiveHud;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IGameConfig gameConfig { get; private set; } = null!;
    [PluginService] internal static IFramework framework { get; private set; } = null!;
    [PluginService] internal static IPluginLog logger { get; private set; } = null!;

    private const string CommandName = "/pah";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("AdaptiveHud");

    // number doesn't mean anything (nice). Just makes sure the first check loop works right
    private int currentLayout = 69;

    private ConfigWindow ConfigWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);

        WindowSystem.AddWindow(ConfigWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens configuration window for Adaptive Hud"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // register the config monitor function to run every framework update tick
        framework.Update += Check;

    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our config ui
        ToggleConfigUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();

    private int GetDisplaySetting()
    {
        // retrieve the current screen mode, fullscreen, borderless, or windowed
        gameConfig.TryGet(Dalamud.Game.Config.SystemConfigOption.ScreenMode, out uint retVal);
        return (int)retVal;
    }

    private void Check(object? _)
    {
        if (Configuration.LayoutForWindowedMode != Configuration.LayoutForFullscreenMode)
        {
            //windowed
            if (GetDisplaySetting() == 0 && currentLayout != Configuration.LayoutForWindowedMode)
            {
                //make sure this is never null before trying to access it.
                unsafe
                {
                    if (AddonConfig.Instance()->ChangeHudLayout != null)
                    {
                        AddonConfig.Instance()->ChangeHudLayout((uint)Configuration.LayoutForWindowedMode);
                    }
                }
                currentLayout = Configuration.LayoutForWindowedMode;
            }
            //fullscreen or borderless
            else if (GetDisplaySetting() > 0 && currentLayout != Configuration.LayoutForFullscreenMode)
            {
                unsafe
                {
                    if (AddonConfig.Instance()->ChangeHudLayout != null)
                    {
                        AddonConfig.Instance()->ChangeHudLayout((uint)Configuration.LayoutForFullscreenMode);
                    }
                }
                currentLayout = Configuration.LayoutForFullscreenMode;
            }
        }
    }
}

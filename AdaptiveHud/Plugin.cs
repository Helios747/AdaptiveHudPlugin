using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace AdaptiveHud
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Adaptive Hud";

        private const string commandName = "/pah";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        private IGameConfig GameConfig { get; }

        private int currentLayout = 69;

        private Chat ch = new Chat();

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IFramework framework,
            [RequiredVersion("1.0")] IGameConfig GameConfig)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            this.PluginUi = new PluginUI(this.Configuration);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens configuration window for Adaptive Hud"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            framework.Update += Check;
        }

        public void Dispose()
        {
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(commandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            this.PluginUi.SettingsVisible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            this.PluginUi.SettingsVisible = true;
        }

        // Please don't use this as a reference for your plugin. I don't understand half of the shit I had to do
        // to get XIVCommon's functions working and ripped it out instead.
        private void Check(object? _)
        {
            if (Configuration.LayoutForWindowedMode != Configuration.LayoutForFullscreenMode)
            {
                GameConfig.System.TryGetUInt("ScreenMode", out var currentScreenMode);
                // windowed mode
                if (currentScreenMode == 0 && currentLayout != Configuration.LayoutForWindowedMode)
                {
                    try
                    {
                        int adjustedLayoutValue = Configuration.LayoutForWindowedMode + 1;
                        string rawCmd = $"/hudlayout {adjustedLayoutValue}";
                        ch.SendMessage(rawCmd);
                        currentLayout = Configuration.LayoutForWindowedMode;
                    }
                    catch (Exception e)
                    {
                        PluginLog.LogError("Error sending hudlayout command.", e);
                    }
                }
                else if (currentScreenMode > 0 && currentLayout != Configuration.LayoutForFullscreenMode)
                {
                    try
                    {
                        int adjustedLayoutValue = Configuration.LayoutForFullscreenMode + 1;
                        string rawCmd = $"/hudlayout {adjustedLayoutValue}";
                        ch.SendMessage(rawCmd);
                        currentLayout = Configuration.LayoutForFullscreenMode;
                    }
                    catch (Exception e)
                    {
                        PluginLog.LogError("Error sending hudlayout command.", e);
                    }
                }

            }
        }

    }
}

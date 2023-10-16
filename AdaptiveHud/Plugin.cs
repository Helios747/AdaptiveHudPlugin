using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using XivCommon;

namespace AdaptiveHud
{
    public sealed class Plugin : IDalamudPlugin
    {
        public static string Name => "Adaptive Hud";

        private const string commandName = "/pah";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        private IGameConfig GameConfig { get; init; }

        private int currentLayout = 69;

        private static XivCommonBase? chatHandler { get; set; }


        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IFramework framework,
            [RequiredVersion("1.0")] IGameConfig gameConfig)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;


            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            this.PluginUi = new PluginUI(this.Configuration);
            this.GameConfig = GameConfig;

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens configuration window for Adaptive Hud"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            framework.Update += Check;

            chatHandler = new XivCommonBase(pluginInterface);
            GameConfig = gameConfig;
        }

        public void Dispose()
        {
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(commandName);
            chatHandler.Dispose();
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

        private int GetDisplaySetting()
        {
            GameConfig.System.TryGetUInt("ScreenMode", out var retVal);
            return (int)retVal;
        }

        private void Check(object? _)
        {
            if (Configuration.LayoutForWindowedMode != Configuration.LayoutForFullscreenMode)
            {
                // windowed mode
                if (GetDisplaySetting() == 0 && currentLayout != Configuration.LayoutForWindowedMode)
                {
                    try
                    {
                        int adjustedLayoutValue = Configuration.LayoutForWindowedMode + 1;
                        string rawCmd = $"/hudlayout {adjustedLayoutValue}";
                        string cleanCmd = chatHandler.Functions.Chat.SanitiseText(rawCmd);
                        chatHandler.Functions.Chat.SendMessage(cleanCmd);
                        currentLayout = Configuration.LayoutForWindowedMode;
                    }
                    catch (Exception e)
                    {
                        PluginLog.LogError("Error sending hudlayout command.", e);
                    }
                }
                else if (GetDisplaySetting() > 0 && currentLayout != Configuration.LayoutForFullscreenMode)
                {
                    try
                    {
                        int adjustedLayoutValue = Configuration.LayoutForFullscreenMode + 1;
                        string rawCmd = $"/hudlayout {adjustedLayoutValue}";
                        string cleanCmd = chatHandler.Functions.Chat.SanitiseText(rawCmd);
                        chatHandler.Functions.Chat.SendMessage(cleanCmd);
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

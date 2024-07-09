using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AdaptiveHud.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("AdaptiveHud Configuration")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(250, 100);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
        string[] layoutOptions = ["1", "2", "3", "4"];

        ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
        int windowedValue = Configuration.LayoutForWindowedMode;
        if (ImGui.Combo("Windowed mode layout", ref windowedValue, layoutOptions, 4))
        {
            Configuration.LayoutForWindowedMode = windowedValue;
            Configuration.Save();
        }
        ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
        int fullscreenValue = Configuration.LayoutForFullscreenMode;
        if (ImGui.Combo("Fullscreen mode layout", ref fullscreenValue, layoutOptions, 4))
        {
            Configuration.LayoutForFullscreenMode = fullscreenValue;
            Configuration.Save();
        }
    }
}

using Dalamud.Configuration;
using System;

namespace AdaptiveHud;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public int LayoutForWindowedMode { get; set; } = 0;
    public int LayoutForFullscreenMode { get; set; } = 1;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}

﻿using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Interface.Utility;

namespace AdaptiveHud
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private readonly Configuration configuration;
        
        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(250, 100), ImGuiCond.Once);
            if (ImGui.Begin("Adaptive Hud Configuration", ref this.settingsVisible,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                string[] layoutOptions = new string[] {"1", "2", "3", "4"};

                ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
                int windowedValue = this.configuration.LayoutForWindowedMode;
                if (ImGui.Combo("Windowed mode layout",ref windowedValue, layoutOptions, 4))
                {
                    this.configuration.LayoutForWindowedMode = windowedValue;
                    this.configuration.Save();
                }
                ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
                int fullscreenValue = this.configuration.LayoutForFullscreenMode;
                if (ImGui.Combo("Fullscreen mode layout",ref fullscreenValue, layoutOptions, 4))
                {
                    this.configuration.LayoutForFullscreenMode = fullscreenValue;
                    this.configuration.Save();
                }
            }
            ImGui.End();
        }
    }
}

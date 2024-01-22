using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GyrfalconToolKit.Editor.Widget
{
    enum PreferencesType
    {
        Themes,
        Others
    }
    internal static class Settings
    {
        static PreferencesType PrefType = PreferencesType.Themes;
        internal static void ShowSettings()
        {
            if (EditorState.Settings)
            {
                if (ImGui.Begin("Preferences", ref EditorState.Settings, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking))
                {
                    ImGui.SetWindowSize(new Vector2(600, ImGui.GetIO().DisplaySize.Y * 0.8f));
                    ImGui.Columns(2);
                    ImGui.SetColumnWidth(0, 100);
                    if(ImGui.Selectable("Themes"))
                    {
                        PrefType = PreferencesType.Themes;
                    }
                    if (ImGui.Selectable("Others"))
                    {
                        PrefType = PreferencesType.Others;
                    }
                    ImGui.NextColumn();
                    if(PrefType == PreferencesType.Themes)
                    {
                        ImGui.Text("Themes");
                    }
                    ImGui.End();
                }
            }
        }
    }
}

using Gyrfalcon.Engine.Debugs;
using GyrfalconToolKit.Editor.Utils;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace GyrfalconToolKit.Editor.Widget
{
    enum PreferencesType
    {
        Themes,
        Others
    }
    [Serializable]
    internal class ThemeSave
    {
        public string SelectedTheme;
    }
    internal static class Settings
    {
        static PreferencesType PrefType = PreferencesType.Themes;
        static List<Theme> Themes = new();
        static ThemeSave Themesave;
        internal static void LoadThemes()
        {
            Themes.Clear();
            string[] themes = Directory.GetFiles("Themes", "*.json");
            foreach (string theme in themes)
            {
                Themes.Add(JsonConvert.DeserializeObject<Theme>(File.ReadAllText(theme)));
            }

        }
        internal static void LoadThemesSave()
        {
            Themesave = JsonConvert.DeserializeObject<ThemeSave>(File.ReadAllText("Themes/Theme.thc"));
            foreach (Theme theme in Themes)
            {
                if (theme.Name == Themesave.SelectedTheme)
                {
                    theme.LoadTheme(ImGui.GetStyle());
                }
            }
        }
        internal static void SaveThemesSave()
        {
            string Json = JsonConvert.SerializeObject(Themesave);
            File.WriteAllText("Themes/Theme.thc", Json);
        }
        internal static void ShowSettings()
        {
            if (EditorState.Settings)
            {
                if (ImGui.Begin("Preferences", ref EditorState.Settings, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking))
                {
                    ImGui.SetWindowSize(new Vector2(600, ImGui.GetIO().DisplaySize.Y * 0.8f));
                    ImGui.Columns(2);
                    ImGui.SetColumnWidth(0, 100);
                    if (ImGui.Selectable("Themes"))
                    {
                        PrefType = PreferencesType.Themes;
                    }
                    if (ImGui.Selectable("Others"))
                    {
                        PrefType = PreferencesType.Others;
                    }
                    ImGui.NextColumn();
                    if (PrefType == PreferencesType.Themes)
                    {
                        ImGui.Text("Themes");
                        if (ImGui.Button($"{FontAwesome6.Repeat}"))
                        {
                            LoadThemes();
                        }
                        foreach (Theme theme in Themes)
                        {
                            if (ImGui.Selectable(theme.Name))
                            {
                                theme.LoadTheme(ImGui.GetStyle());
                                Themesave.SelectedTheme = theme.Name;
                                SaveThemesSave();
                            }
                        }
                    }
                    ImGui.End();
                }
            }
        }
    }
}

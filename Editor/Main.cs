using Gyrfalcon.Maths;
using Gyrfalcon.Subsystems;
using GyrfalconToolKit.Editor.Utils;
using GyrfalconToolKit.Editor.Widget;
using ImGuiNET;
using OpenTK.Mathematics;
namespace GyrfalconToolKit.Editor
{
    internal static class Main
    {
        internal static void ShowUi()
        {
            MainMenuBar();
            OverLay();
            InfoWindow();
            Settings.ShowSettings();
        }
        static void MainMenuBar()
        {
            var MainSubsystem = (MainSubsystem)SubsystemManager.GetSubsystem("MainSubsystem");
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 10));
            if (ImGui.BeginMainMenuBar())
            {
                ImGui.PopStyleVar();

                ImGui.Image(MainSubsystem.Icon.Handle, new Vector2(35, 35));
                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Skeleton Editor"))
                    {
                        EditorState.SkeletonEditorActive = !EditorState.SkeletonEditorActive;
                        EditorState.AnimationEditorActive = false;
                    }
                    if (ImGui.MenuItem("Animation Editor"))
                    {
                        EditorState.SkeletonEditorActive = false;
                        EditorState.AnimationEditorActive = !EditorState.AnimationEditorActive;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Camera"))
                {
                    Vector3 CamPos = CameraManager.Cam.Target;
                    if (ImGui.InputFloat3("Camera Target", ref CamPos))
                    {
                        CameraManager.Cam.Target = CamPos;
                    }
                    if (ImGui.MenuItem("Reset Cam Pos", "Shift+R"))
                    {
                        CameraManager.Cam.Target = OpenTK.Mathematics.Vector3.Zero;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Settings"))
                {
                    if (ImGui.MenuItem("Preferences"))
                    {
                        EditorState.Settings = !EditorState.Settings;
                    }
                    if (ImGui.MenuItem("Overlay"))
                    {
                        EditorState.Overlay = !EditorState.Overlay;
                    }
                    if (ImGui.MenuItem("About"))
                    {
                        EditorState.CopyRight = !EditorState.CopyRight;
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }
        static void InfoWindow()
        {
            if (EditorState.CopyRight)
            {
                if (ImGui.Begin("About", ref EditorState.CopyRight, ImGuiWindowFlags.NoResize |ImGuiWindowFlags.NoDocking))
                {
                    ImGui.SetWindowSize(new Vector2(250, 200));
                    ImGui.TextWrapped(String.Format(@"Gyrfalcon ToolKit v:{0}

Gyrfalcon ToolKit est une suite d'outils pour le moteur Gyrfalcon

©OutlawTeam ©Florian Pfeiffer

Gyrfalcon v:{1}

Dev:
-Florian Pfeiffer
"
                    ,Version.ToolKitVersion ,Gyrfalcon.Engine.Version.EngineVersion));
                    ImGui.End();
                }
            }
        }
        static void OverLay()
        {
            if (EditorState.Overlay)
            {
                ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
                const float PAD = 10.0f;
                var viewport = ImGui.GetMainViewport();
                Vector2 work_pos = viewport.WorkPos; // Use work area to avoid menu-bar/task-bar, if any!
                Vector2 work_size = viewport.WorkSize;
                Vector2 window_pos, window_pos_pivot;
                window_pos.X = work_pos.X + work_size.X - PAD;
                window_pos.Y = work_pos.Y + work_size.Y - PAD;
                window_pos_pivot.X = 1.0f;
                window_pos_pivot.Y = 1.0f;
                ImGui.SetNextWindowPos(window_pos, ImGuiCond.Always, window_pos_pivot);
                window_flags |= ImGuiWindowFlags.NoMove;
                ImGui.SetNextWindowBgAlpha(0.35f);
                if (ImGui.Begin("Editor Overlay", window_flags))
                {
                    ImGui.Text($" {1000.0f / ImGui.GetIO().Framerate:0.##} ms/frame ({ImGui.GetIO().Framerate:0.#} FPS)");

                }
                ImGui.End();
            }
        }
    }
}

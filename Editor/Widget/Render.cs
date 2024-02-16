using Gyrfalcon.Engine.Module.Luxon;
using GyrfalconToolKit.Editor.Utils;
using ImGuiNET;
using OpenTK.Mathematics;

namespace GyrfalconToolKit.Editor.Widget
{
    internal static class Render
    {
        internal static void ShowRender()
        {
            ImGui.Begin("Render");
            ImGui.Image(Luxon.GetViewport(), new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().X * (9f / 16f)), new Vector2(0, 1), new Vector2(1, 0));
            if (ImGui.IsWindowHovered())
            {
                CameraManager.Update();
            }
            ImGui.End();
        }
    }
}

using Gyrfalcon.Engine;
using Gyrfalcon.Engine.Core;
using Gyrfalcon.Engine.Module.Luxon;
using Gyrfalcon.Engine.Module.Luxon.Data;
using GyrfalconToolKit.Editor;
using GyrfalconToolKit.Editor.Utils;
using GyrfalconToolKit.Editor.Widget;
using ImGuiNET;
using OpenTK.Mathematics;
using StbImageSharp;
namespace GyrfalconToolKit
{
    internal class GyrfalconToolKitApp : ClientApplication
    {
        internal Texture Icon;

        public override void Load()
        {
            base.Load();
            ImageResult image = ImageResult.FromMemory(Properties.Resources.logo);
            Engine.SetWindowTitle("GyrfalconToolKit");
            Engine.SetWindowIcon(image.Width, image.Height, image.Data);
            ImageResult image2 = ImageResult.FromMemory(Properties.Resources.logo, ColorComponents.RedGreenBlueAlpha);
            Icon = Texture.LoadFromBinaryData(image.Data, image.Width, image.Height);
            ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename.ToString());
            SetupImGuiStyle();
            GridRenderer.GenerateGridVertices();
            GraphicsSettings.Bloom = 0;
            GraphicsSettings.Skybox = false;
            GraphicsSettings.Gamma = 1;
        }
        public override void ForwardRenderPass()
        {
            base.ForwardRenderPass();
            GridRenderer.Render(CameraManager.Cam.GetViewMatrix(), Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), 16f / 9f, 0.1f, 100));
        }
        public static void SetupImGuiStyle()
        {
            Settings.LoadThemes();
            Settings.LoadThemesSave();

        }
        public override void Update(float DT)
        {
            base.Update(DT);
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
            ShortCut.Update();
            Main.ShowUi();
            Editor.Editor.ShowEditor();
            LuxonCamera Cam = new()
            {
                View = CameraManager.Cam.GetViewMatrix(),
                Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), 16f / 9f, 0.1f, 500),
                Near = 0.1f,
                Far = 500,
                Position = CameraManager.Cam.Position,
                Up = CameraManager.Cam.Up,
                Front = CameraManager.Cam.Front,
            };
            Luxon.SetMainCamera(Cam);
        }
    }
}

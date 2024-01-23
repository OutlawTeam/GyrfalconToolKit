using Gyrfalcon.Debugs;
using Gyrfalcon.Render;
using Gyrfalcon.Render.Components;
using Gyrfalcon.Subsystems;
using GyrfalconToolKit.Editor;
using GyrfalconToolKit.Editor.Utils;
using ImGuiNET;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GyrfalconToolKit.Editor.Widget;

namespace GyrfalconToolKit
{
    internal class MainSubsystem : ISubsystem
    {
        public string Name => "MainSubsystem";
        internal Texture Icon;
        public void Dispose() { }
        public void Initialize()
        {
            ImageResult image = ImageResult.FromMemory(Properties.Resources.logo, ColorComponents.RedGreenBlueAlpha);
            Icon = Texture.LoadFromBinaryData(image.Data, image.Width, image.Height);
            Renderer.Camera = false;
            ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename.ToString());
            
           
            SetupImGuiStyle();
            GridRenderer.GenerateGridVertices();
            GraphicsSettings.Bloom = 0;
            GraphicsSettings.Skybox = false;
            GraphicsSettings.Gamma = 1;
        }

        public void Update()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

            ShortCut.Update();
            Main.ShowUi();
            Editor.Editor.ShowEditor();
            Renderer.View = CameraManager.Cam.GetViewMatrix();

        }
        public static void SetupImGuiStyle()
        {
            Settings.LoadThemes();
            Settings.LoadThemesSave();

        }
    }
}

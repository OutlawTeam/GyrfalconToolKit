using Gyrfalcon.Render;
using Gyrfalcon.Render.Components;
using Gyrfalcon.Render.Sky;
using Gyrfalcon.Subsystems;
using GyrfalconToolKit.Editor;
using GyrfalconToolKit.Editor.Tools;
using GyrfalconToolKit.Editor.Utils;
using OpenTK.Mathematics;

namespace GyrfalconToolKit
{
    internal class MainRenderTask : IRendererTask
    {
        
        public string Name => "MainRenderTask";

        public void ForwardPass()
        {
            GridRenderer.Render(CameraManager.Cam.GetViewMatrix(), Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), Renderer.Aspect, 0.1f, 100)); ;

        }

        public void PreRender()
        {
        }

        public void ShadowPass()
        {
        }

        public void Update()
        {

        }
    }
}

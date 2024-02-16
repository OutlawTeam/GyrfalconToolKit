using Gyrfalcon.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GyrfalconToolKit.Editor.Utils
{
    internal static class CameraManager
    {
        internal static ArcBallCamera Cam = new();

        internal static void Update()
        {
            if (InputSystem.Mouse.IsButtonDown(MouseButton.Middle))
            {
                Cam.Rotate(new Vector2(InputSystem.Mouse.Delta.X, -InputSystem.Mouse.Delta.Y) * (float)InputSystem.FrameEvent.Time);
            }
            Cam.Zoom(InputSystem.Mouse.ScrollDelta.Y);
            if (InputSystem.KeyPressed(Keys.T))
            {
                Cam.Target += new OpenTK.Mathematics.Vector3(0, 0.25f, 0);
            }
            else if (InputSystem.KeyPressed(Keys.G))
            {
                Cam.Target -= new OpenTK.Mathematics.Vector3(0, 0.25f, 0);

            }
            if (InputSystem.KeyDown(Keys.W))
            {
                Cam.Target += Cam.Front * (float)InputSystem.FrameEvent.Time * 2;
            }
            if (InputSystem.KeyDown(Keys.S))
            {
                Cam.Target -= Cam.Front * (float)InputSystem.FrameEvent.Time * 2;
            }
            if (InputSystem.KeyDown(Keys.D))
            {
                Cam.Target += Cam.Right * (float)InputSystem.FrameEvent.Time * 2;
            }
            if (InputSystem.KeyDown(Keys.A))
            {
                Cam.Target -= Cam.Right * (float)InputSystem.FrameEvent.Time * 2;
            }
        }
    }
}

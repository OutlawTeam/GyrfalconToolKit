using Gyrfalcon.Engine;
using GyrfalconToolKit.Editor.Tools;
using GyrfalconToolKit.Editor.Utils;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GyrfalconToolKit.Editor
{
    internal static class ShortCut
    {
        internal static void Update()
        {

            if (InputSystem.KeyDown(Keys.LeftShift) && InputSystem.KeyDown(Keys.R))
            {
                CameraManager.Cam.Target = OpenTK.Mathematics.Vector3.Zero;

            }
            if (EditorState.AnimationEditorActive)
            {
                if (InputSystem.KeyDown(Keys.LeftControl) && InputSystem.KeyDown(Keys.S))
                {
                    AnimationEditor.Save();

                }
                if (InputSystem.KeyDown(Keys.LeftControl) && InputSystem.KeyDown(Keys.O))
                {
                    AnimationEditor.OpenSkeleton();
                }
                if (InputSystem.KeyDown(Keys.LeftControl) && InputSystem.KeyDown(Keys.M))
                {
                    AnimationEditor.OpenDebugModel();
                }
                if (InputSystem.KeyDown(Keys.LeftControl) && InputSystem.KeyDown(Keys.A))
                {
                    AnimationEditor.OpenModelAnimation();
                }
            }

        }
    }
}

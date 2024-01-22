using Gyrfalcon.Maths;
using Gyrfalcon.Render;
using GyrfalconToolKit.Editor.Tools;
using GyrfalconToolKit.Editor.Utils;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Mathematics;

namespace GyrfalconToolKit.Editor.Widget
{
    internal static class Render
    {
        internal static void ShowRender()
        {
            ImGui.Begin("Render");
            ImGuizmo.SetOrthographic(false);
            ImGuizmo.SetDrawlist();
            ImGuizmo.SetRect(ImGui.GetCursorScreenPos().X, ImGui.GetCursorScreenPos().Y, ImGui.GetWindowSize().X, ImGui.GetWindowSize().X * (9f / 16f));
            ImGui.Image(Renderer.GetViewport(), new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().X * (9f / 16f)), new Vector2(0, 1), new Vector2(1, 0));
            float[] ViewMatrice = CameraManager.Cam.GetViewMatrix().ToArray();
            if (EditorState.AnimationEditorActive)
            {
                if (AnimationEditor.Anim.JointsAnimationData.ContainsKey(AnimationEditor.SelectedJoint))
                {
                    Quaternion Rotation = AnimationEditor.Anim.JointsAnimationData[AnimationEditor.SelectedJoint].Data[AnimationEditor.CurrentTick].Rotation;
                    Vector3 Position = AnimationEditor.Ske.GetJointPosition(AnimationEditor.SelectedJoint);
                    float[] Matrice = (Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position)).ToArray();
                    
                    ImGuizmo.Manipulate(ref CameraManager.Cam.GetViewMatrix().ToArray()[0], ref Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), 16f / 9f, 0.1f, 100).ToArray()[0], OPERATION.ROTATE, MODE.LOCAL, ref Matrice[0]);
                    AnimationEditor.Anim.JointsAnimationData[AnimationEditor.SelectedJoint].Data[AnimationEditor.CurrentTick].Rotation = Matrice.ToMatrix().ExtractRotation();

                }


            }

            if (ImGui.IsWindowHovered())
            {
                CameraManager.Update();

            }
            ImGui.End();

        }
    }
}

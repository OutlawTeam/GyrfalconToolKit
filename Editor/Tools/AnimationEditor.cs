using Assimp;
using Gyrfalcon.Engine;
using Gyrfalcon.Engine.Module.Animation;
using Gyrfalcon.Engine.Module.Luxon;
using Gyrfalcon.Engine.Module.Luxon.Data;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Animation = Gyrfalcon.Engine.Module.Animation.Animation;
using Material = Gyrfalcon.Engine.Module.Luxon.Data.Material;
namespace GyrfalconToolKit.Editor.Tools
{
    internal static class AnimationEditor
    {
        static public Skeleton Ske = null;
        static public Animation Anim = new();
        static public AnimationController Animator;
        static AnimatedModel DebugModel = null;
        static bool ShowDebugModel = false;
        static bool PlayAnimation = false;
        static public string SelectedJoint = "";
        internal static void Save()
        {
            var Result = NativeFileDialogSharp.Dialog.FileSave("gan");
            if (Result.IsOk)
            {
                string Text = JsonConvert.SerializeObject(Anim, Formatting.Indented);
                File.WriteAllText(Result.Path + ".gan", Text);
            }

        }
        internal static void OpenSkeleton()
        {
            var Result = NativeFileDialogSharp.Dialog.FileOpen("gsk");
            if (Result.IsOk)
            {
                Ske = JsonConvert.DeserializeObject<Skeleton>(File.ReadAllText(Result.Path));
                Ske.ResetPose();
                Animator = new(Ske);
            }
        }
        internal static void OpenDebugModel()
        {
            var Result = NativeFileDialogSharp.Dialog.FileOpen("gltf,fbx");
            if (Result.IsOk)
            {
                DebugModel = new(Result.Path);
            }

        }
        internal static void OpenModelAnimation()
        {
            var Result = NativeFileDialogSharp.Dialog.FileOpen("gltf,fbx,glb");
            if (Result.IsOk)
            {
                LoadAnimation(Result.Path);
            }
        }
        internal static void DrawJointTree(Joint joint)
        {
            if (ImGui.TreeNode(joint.Name))
            {

                ImGui.SameLine();
                if (ImGui.Selectable(joint.Name + "##0", true, ImGuiSelectableFlags.AllowOverlap))
                {
                    SelectedJoint = joint.Name;
                }

                foreach (string ChildName in joint.Child)
                {
                    Joint child = Ske.GetJoint(ChildName);

                    DrawJointTree(child);
                }
                ImGui.TreePop();
            }
        }
        internal static void LoadAnimation(string path)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(path);
            var animation = scene.Animations[0];
            Anim.Duration = (float)(animation.DurationInTicks / animation.TicksPerSecond);
            Anim.TickPerSecond = (int)animation.TicksPerSecond;
            ReadAnimationBones(animation);
        }
        static void ReadAnimationBones(Assimp.Animation animation)
        {
            int size = animation.NodeAnimationChannelCount;

            var boneInfoMap = Ske.JointsMap;//getting m_BoneInfoMap from Model class
            //reading channels(bones engaged in an animation and their keyframes)
            Anim.Joints = new(size);
            for (int i = 0; i < size; i++)
            {
                var channel = animation.NodeAnimationChannels[i];
                string boneName = channel.NodeName;
                if (boneInfoMap.ContainsKey(boneName))
                {
                    Anim.Joints.Add(new AnimationJoint(boneName, channel));
                }
            }
        }
        internal static void Update()
        {

            var viewport = ImGui.GetMainViewport();
            float height = ImGui.GetFrameHeight();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.MenuBar;
            if (ImGui.BeginViewportSideBar("##SecondaryMenuBar", viewport, ImGuiDir.Up, height, window_flags))
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Save", "Ctrl+S"))
                        {
                            Save();
                        }
                        if (ImGui.MenuItem("Open Skeleton File", "Ctrl+O"))
                        {
                            OpenSkeleton();
                        }
                        if (ImGui.MenuItem("Open 3d model for test", "Ctrl+M"))
                        {
                            OpenDebugModel();
                        }
                        if (ImGui.MenuItem("Open 3d model for animation", "Ctrl+A"))
                        {
                            OpenModelAnimation();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                ImGui.End();
            }
            Widget.Render.ShowRender();
            /*
            ImGui.Begin("Animation Timeline");
            AnimationTimeLine.Update(ref CurrentTick, ref Anim.TickPerSecond, ref Anim.Duration, ref CurrentTimelineZoom, Anim);
            ImGui.End();*/
            ImGui.Begin("Animation");
            if (Ske != null)
            {
                if (ImGui.Button("Reset Pose"))
                {
                    Ske.ResetPose();
                }
                ImGui.Text("Selected Joint:" + SelectedJoint);
                DrawJointTree(Ske.InitialJoint);

                ImGui.Checkbox("Show Debug Model", ref ShowDebugModel);
                if (ImGui.Button("Play/Pause"))
                {
                    PlayAnimation = !PlayAnimation;
                }
                if (ImGui.Button("ResetAnimation"))
                {
                    Animator.PlayAnimation(Anim, true);
                }
            }
            ImGui.End();

            if (Ske != null)
            {
                if (PlayAnimation)
                {
                    Animator.Update((float)InputSystem.FrameEvent.Time);
                }
                Ske.DebugDraw();

            }
            if (DebugModel != null && ShowDebugModel)
            {
                DebugModel.Render(Matrix4.Identity, Ske,null, true);
            }
            if (ImGui.BeginViewportSideBar("##InfoMenuBar", viewport, ImGuiDir.Down, height, window_flags))
            {
                if (ImGui.BeginMenuBar())
                {

                    if (Ske != null && Anim != null)
                    {
                        ImGui.Text("Frame number: " + Anim.Duration * Anim.TickPerSecond);
                    }
                    else
                    {
                        ImGui.Text("Frame number: None");
                    }
                    ImGui.EndMenuBar();
                }
                ImGui.End();
            }
        }

    }
}

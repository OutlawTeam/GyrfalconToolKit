
using Gyrfalcon.Animation;
using Gyrfalcon.Render.PBR;
using Gyrfalcon.Render;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Assimp;
using Animation = Gyrfalcon.Animation.Animation;
using GyrfalconToolKit.Editor.Widget;
using Gyrfalcon.Maths;
using Gyrfalcon.Engine;
namespace GyrfalconToolKit.Editor.Tools
{
    internal static class AnimationEditor
    {
        static public  Skeleton Ske = null;
        static public Animation Anim = new();
        static PBRAnimatedModel DebugModel = null;
        static Timeline AnimationTimeLine = new();
        static bool ShowDebugModel = false;
        static public int CurrentTick = 0;
        static float CurrentTimelineZoom = 1;
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
            var Result = NativeFileDialogSharp.Dialog.FileOpen("gltf,fbx");
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
                if(ImGui.Selectable(joint.Name+"##0",true, ImGuiSelectableFlags.AllowOverlap))
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
            int boneCount = Ske.Joints.Count;
            //reading channels(bones engaged in an animation and their keyframes)
            for (int i = 0; i < size; i++)
            {
                var channel = animation.NodeAnimationChannels[i];
                string boneName = channel.NodeName;
                if (Anim.JointsAnimationData.ContainsKey(boneName))
                {
                    var Jt = Anim.JointsAnimationData[boneName];
                    int m_NumRotations = channel.RotationKeyCount;
                    for (int rotationIndex = 0; rotationIndex < m_NumRotations; ++rotationIndex)
                    {
                        double timeStamp = channel.RotationKeys[rotationIndex].Time;
                        RotationKey data = new();
                        data.Rotation = new Quaternion(channel.RotationKeys[rotationIndex].Value.X, channel.RotationKeys[rotationIndex].Value.Y, channel.RotationKeys[rotationIndex].Value.Z, channel.RotationKeys[rotationIndex].Value.W);
                        data.Timestamp = (float)timeStamp;
                        Jt.Data.Add(data);
                        //Console.WriteLine("Rotation:" + data.orientation);
                    }


                }
            }


        }
        internal static void Update(MainSubsystem sub)
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
            ImGui.Begin("Animation Timeline");
            AnimationTimeLine.Update(ref CurrentTick, ref Anim.TickPerSecond, ref Anim.Duration, ref CurrentTimelineZoom, Anim);
            ImGui.End();
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
            }
            ImGui.PushItemWidth(100);

            ImGui.InputFloat("Animation duration", ref Anim.Duration);

            ImGui.InputInt("Animation tick per second", ref Anim.TickPerSecond);
            ImGui.PopItemWidth();

            if (ImGui.Button("Create Animation From Skeleton"))
            {
                foreach (Joint jt in Ske.Joints.Values)
                {
                    var JtData = new JointAnimationData()
                    {
                        JointName = jt.Name,
                        DataCount = 1,
                    };
                    for (int i = 0; i < Anim.TickPerSecond * Anim.Duration; i++)
                    {
                        JtData.Data.Add(new RotationKey() { Rotation = jt.Rotation, Timestamp = i });

                    }
                    Anim.JointsAnimationData.Add(JtData.JointName, JtData
                    );
                }
            }
            ImGui.End();

            if (Ske != null)
            {
                if (PlayAnimation)
                {
                    CurrentTick += (int)(Anim.TickPerSecond / (float)InputSystem.FrameEvent.Time * 0.1f);
                    if (CurrentTick > Anim.Duration * Anim.TickPerSecond)
                    {
                        CurrentTick = 0;
                    }
                }
                foreach (var Jt in Anim.JointsAnimationData.Values)
                {
                    Ske.GetJoint(Jt.JointName).Rotation = Jt.Data[CurrentTick].Rotation;
                }
                Ske.DebugDraw();

            }
            if(DebugModel != null && ShowDebugModel)
            {
                DebugModel.Render(Matrix4.Identity, Ske, true);
            }
        }
        internal static void Render()
        {
            if (ShowDebugModel && DebugModel != null)
            {
                DebugModel.Draw(Shaders.PBRShader, Matrix4.Identity, Ske.GetAllJointsTransforms());
            }
        }

    }
}

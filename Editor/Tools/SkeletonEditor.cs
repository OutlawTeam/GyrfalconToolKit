using Assimp;
using Assimp.Configs;
using Gyrfalcon.Animation;
using Gyrfalcon.Maths;
using Gyrfalcon.Render;
using Gyrfalcon.Render.PBR;
using GyrfalconToolKit.Editor.Widget;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
namespace GyrfalconToolKit.Editor.Tools
{
    internal static class SkeletonEditor
    {
        static Skeleton Ske = null;
        static bool SkeletonImportopen = false;
        static string ImportPath = "";
        static string RootJoitName = "";
        static List<string> TempJoints = new();
        static List<Joint> TempJointList = new();
        static Dictionary<string, JointInfo> TempJointsMap = new();
        static int m_BoneCounter = 0;
        static Vector3 InitialRootPos = new Vector3(0);
        static PBRAnimatedModel DebugModel = null;
        static bool ShowDebugModel = false;
        internal static void DrawJointTree(Joint joint)
        {
            if (ImGui.TreeNode(joint.Name))
            {
                Vector3 Rotation = joint._IntialRotation.ToEulerAngles();
                Rotation.X = MathHelper.RadiansToDegrees(Rotation.X);
                Rotation.Y = MathHelper.RadiansToDegrees(Rotation.Y);
                Rotation.Z = MathHelper.RadiansToDegrees(Rotation.Z);
                if (ImGui.DragFloat3("Rotation in euler angle", ref Rotation))
                {
                    joint._IntialRotation = Quaternion.FromEulerAngles(
                        MathHelper.DegreesToRadians(Rotation.X),
                        MathHelper.DegreesToRadians(Rotation.Y),
                        MathHelper.DegreesToRadians(Rotation.Z)
                        );
                }
                ImGui.InputFloat("Distance", ref joint.Distance);
                foreach (string ChildName in joint.Child)
                {
                    Joint child = Ske.GetJoint(ChildName);
                    DrawJointTree(child);
                }
                ImGui.TreePop();
            }
        }
        static void ShowSkeletonEditor()
        {
            ImGui.Begin("Skeleton Editor", ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open"))
                    {
                        var Result = NativeFileDialogSharp.Dialog.FileOpen("gsk");
                        if (Result.IsOk)
                        {
                            Ske = JsonConvert.DeserializeObject<Skeleton>(File.ReadAllText(Result.Path));
                            Ske.ResetPose();
                            InitialRootPos = Ske.InitialJoint._IntialPosition;
                        }
                    }
                    if (ImGui.MenuItem("Save","Ctrl+S"))
                    {
                        var Result = NativeFileDialogSharp.Dialog.FileSave("gsk");
                        if (Result.IsOk)
                        {
                            File.WriteAllText(Result.Path + ".gsk", JsonConvert.SerializeObject(Ske, Formatting.Indented));
                        }
                    }
                    if (ImGui.MenuItem("Import from 3d model"))
                    {
                        SkeletonImportopen = true;

                    }
                    if (ImGui.MenuItem("Open 3d model for test"))
                    {
                        var Result = NativeFileDialogSharp.Dialog.FileOpen("gltf,fbx");
                        if (Result.IsOk)
                        {
                            DebugModel = new(Result.Path);
                        }
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
                if (Ske != null)
                {
                    if (ImGui.Button("Reset Pose"))
                    {
                        Ske.ResetPose();
                    }
                    if (ImGui.DragFloat3("Root Position", ref InitialRootPos, 0.01f))
                    {
                        Ske.InitialJoint._IntialPosition = InitialRootPos;
                    }
                    DrawJointTree(Ske.InitialJoint);

                    ImGui.Checkbox("Show Debug Model", ref ShowDebugModel);
                }
            }
            ImGui.End();
        }
        internal static void Update(MainSubsystem sub)
        {
            var viewport = ImGui.GetMainViewport();
            ShowSkeletonEditor();
            if (Ske != null)
            {
                InitialRootPos = Ske.InitialJoint._IntialPosition;
            }
            ImGui.Begin("Infos");

            if (Ske != null)
            {
                ImGui.Text("Joints number: " + Ske.Joints.Count);
            }
            ImGui.End();
            if (SkeletonImportopen)
            {
                ImGui.OpenPopup("Import Skeleton");
            }

            if (ImGui.BeginPopupModal("Import Skeleton", ref SkeletonImportopen, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration))
            {

                ImGui.Text("Import skeleton from 3d model.");
                ImGui.Text("Supported formats are GLTF and FBX.");
                if (ImGui.Button("Pick a file"))
                {
                    var Result = NativeFileDialogSharp.Dialog.FileOpen("gltf,fbx");
                    if (Result.IsOk)
                    {
                        ImportPath = Result.Path;
                    }
                }
                ImGui.InputText("Path", ref ImportPath, 100);
                ImGui.InputText("Root Name", ref RootJoitName, 100);
                if (ImGui.Button("Ok"))
                {
                    ImGui.CloseCurrentPopup();
                    ExtractSkelette();
                    SkeletonImportopen = false;
                    ImportPath = "";
                    RootJoitName = "";
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImportPath = "";

                    ImGui.CloseCurrentPopup();
                    SkeletonImportopen = false;
                    RootJoitName = "";

                }
                ImGui.EndPopup();
            }
            Widget.Render.ShowRender();
            Renderer.Aspect = 16f / 9f;
            if (Ske != null)
            {
                Ske.DebugDraw();
            }
        }
        internal static void Render()
        {
            if (ShowDebugModel && DebugModel != null)
            {
                DebugModel.Draw(Shaders.PBRShader, Matrix4.Identity, Ske.GetAllJointsTransforms());
            }
        }
        static bool GetRootNode(Node nd, out Node pel)
        {
            if (nd.Name == RootJoitName)
            {
                pel = nd;
                return true;
            }
            foreach (Node nd1 in nd.Children)
            {
                if (GetRootNode(nd1, out Node pel1))
                {
                    pel = pel1;
                    return true;
                }
            }
            pel = null;
            return false;

        }
        public static Matrix4 TKMatrix(Assimp.Matrix4x4 InputSystem)
        {
            return new Matrix4(InputSystem.A1, InputSystem.B1, InputSystem.C1, InputSystem.D1, InputSystem.A2, InputSystem.B2, InputSystem.C2, InputSystem.D2, InputSystem.A3, InputSystem.B3, InputSystem.C3, InputSystem.D3, InputSystem.A4, InputSystem.B4, InputSystem.C4, InputSystem.D4);
        }
        static void processNode(Matrix4 Transform, Joint Current, Node node)
        {
            Current.Child = new();
            Matrix4 Final = TKMatrix(node.Transform) * Transform;
            if (TempJoints.Contains(node.Name) && node.Name != "" && !node.Name.EndsWith("_end"))
            {
                if (node.Name == RootJoitName)
                {
                    Current._IntialPosition = Final.ExtractTranslation();
                    Current.InitialJoint = true;
                    Final = Transform;
                }
                else
                {
                    Current._IntialRotation = TKMatrix(node.Transform).ExtractRotation();
                }
                Current.Name = node.Name;
                Current.Distance = Vector3.Distance(Final.ExtractTranslation(), Transform.ExtractTranslation());

                TempJointList.Add(Current);
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                if (node.Children[i].Name != null && !node.Children[i].Name.EndsWith("_end"))
                {
                    Joint newJoint = new Joint();
                    Current.Child.Add(node.Children[i].Name);
                    processNode(Final, newJoint, node.Children[i]);
                }
            }
        }
        static void ExtractSkelette()
        {
            AssimpContext import = new AssimpContext();
            import.SetConfig(new RemoveEmptyBonesConfig(false));
            // Configure the post-process steps
            Scene scene = import.ImportFile(ImportPath);
            TempJoints.Clear();
            TempJointList.Clear();
            TempJointsMap.Clear();
            m_BoneCounter = 0;
            foreach (Mesh mesh in scene.Meshes)
            {
                if (mesh.HasBones)
                {
                    foreach (Bone bone in mesh.Bones)
                    {
                        if (!TempJoints.Contains(bone.Name))
                        {
                            TempJoints.Add(bone.Name);
                        }
                        int boneID = -1;
                        if (!TempJointsMap.ContainsKey(bone.Name))
                        {
                            JointInfo newJointInfo = new();
                            newJointInfo.id = m_BoneCounter;
                            newJointInfo.offset = TKMatrix(bone.OffsetMatrix);
                            TempJointsMap.Add(bone.Name, newJointInfo);
                            boneID = m_BoneCounter;
                            m_BoneCounter++;
                        }
                        else
                        {
                            boneID = TempJointsMap[bone.Name].id;
                        }
                        if (!(boneID != -1))
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            if (GetRootNode(scene.RootNode, out Node Root))
            {
                processNode(Matrix4.Identity, new Joint(), Root);
            }
            Ske = new(TempJointList, TempJointsMap);
            Ske.ResetPose();
            DebugModel = new(ImportPath);
        }
    }
}

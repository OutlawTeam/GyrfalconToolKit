using GyrfalconToolKit.Editor.Tools;

namespace GyrfalconToolKit.Editor
{
    internal static class Editor
    {
        internal static void ShowEditor()
        {
            if (EditorState.SkeletonEditorActive)
            {
                SkeletonEditor.Update();
            }
            else if (EditorState.AnimationEditorActive)
            {
                AnimationEditor.Update();
            }
        }
    }
}

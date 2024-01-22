using Gyrfalcon.Subsystems;
using GyrfalconToolKit.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyrfalconToolKit.Editor
{
    internal static class Editor
    {
        internal static void ShowEditor()
        {
            var MainSubsystem = (MainSubsystem)SubsystemManager.GetSubsystem("MainSubsystem");

            if (EditorState.SkeletonEditorActive)
            {
                SkeletonEditor.Update(MainSubsystem);
            }
            else if (EditorState.AnimationEditorActive)
            {
                AnimationEditor.Update(MainSubsystem);
            }
        }
    }
}

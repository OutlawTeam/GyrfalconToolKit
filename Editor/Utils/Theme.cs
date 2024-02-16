using Gyrfalcon.Engine.Core.Maths;
using ImGuiNET;

namespace GyrfalconToolKit.Editor.Utils
{
    [Serializable]
    public class Theme
    {
        public void LoadTheme(ImGuiStylePtr Style)
        {
            Style.Alpha = this.Alpha;
            Style.DisabledAlpha = this.DisabledAlpha;
            Style.WindowPadding = this.WindowPadding.ToOpenTKVector();
            Style.WindowRounding = this.WindowRounding;
            Style.WindowBorderSize = this.WindowBorderSize;
            Style.WindowMinSize = this.WindowMinSize.ToOpenTKVector();
            Style.WindowTitleAlign = this.WindowTitleAlign.ToOpenTKVector();
            Style.WindowTitleAlign = this.WindowTitleAlign.ToOpenTKVector();
            Style.ChildRounding = this.ChildRounding;
            Style.ChildBorderSize = this.ChildBorderSize;
            Style.PopupBorderSize = this.PopupBorderSize;
            Style.PopupRounding = this.PopupRounding;
            Style.FramePadding = this.FramePadding.ToOpenTKVector();
            Style.FrameRounding = this.FrameRounding;
            Style.FrameBorderSize = this.FrameBorderSize;
            Style.ItemSpacing = this.ItemSpacing.ToOpenTKVector();
            Style.ItemInnerSpacing = this.ItemInnerSpacing.ToOpenTKVector();
            Style.CellPadding = this.CellPadding.ToOpenTKVector();
            Style.TouchExtraPadding = this.TouchExtraPadding.ToOpenTKVector();
            Style.IndentSpacing = this.IndentSpacing;
            Style.ColumnsMinSpacing = this.ColumnsMinSpacing;
            Style.ScrollbarSize = this.ScrollbarSize;
            Style.ScrollbarRounding = this.ScrollbarRounding;
            Style.GrabMinSize = this.GrabMinSize;
            Style.GrabRounding = this.GrabRounding;
            Style.LogSliderDeadzone = this.LogSliderDeadzone;
            Style.TabRounding = this.TabRounding;
            Style.TabBorderSize = this.TabBorderSize;
            Style.TabMinWidthForCloseButton = this.TabMinWidthForCloseButton;
            Style.ButtonTextAlign = this.ButtonTextAlign.ToOpenTKVector();
            Style.SelectableTextAlign = this.SelectableTextAlign.ToOpenTKVector();
            Style.SeparatorTextBorderSize = this.SeparatorTextBorderSize;
            Style.SelectableTextAlign = this.SelectableTextAlign.ToOpenTKVector();
            Style.SeparatorTextPadding = this.SeparatorTextPadding.ToOpenTKVector();
            Style.DisplayWindowPadding = this.DisplayWindowPadding.ToOpenTKVector();
            Style.DisplaySafeAreaPadding = this.DisplaySafeAreaPadding.ToOpenTKVector();
            Style.DockingSeparatorSize = this.DockingSeparatorSize;
            Style.MouseCursorScale = this.MouseCursorScale;
            Style.AntiAliasedFill = this.AntiAliasedFill;
            Style.AntiAliasedLines = this.AntiAliasedLines;
            Style.AntiAliasedLinesUseTex = this.AntiAliasedLinesUseTex;
            Style.CurveTessellationTol = this.CurveTessellationTol;
            Style.CircleTessellationMaxError = this.CircleTessellationMaxError;
            Style.HoverDelayNormal = this.HoverDelayNormal;
            Style.HoverDelayShort = this.HoverDelayShort;
            Style.HoverStationaryDelay = this.HoverStationaryDelay;
            foreach (ImGuiCol color in Enum.GetValues(typeof(ImGuiCol)))
            {
                try
                {
                    Style.Colors[(int)color] = this.Colors[color].ToOpenTKVector();
                }
                catch
                {

                }
            }
        }
        public static Theme CreateFromStyle(ImGuiStylePtr Style, string Name)
        {
            Theme theme = new();
            theme.Name = Name;
            theme.Alpha = Style.Alpha;
            theme.DisabledAlpha = Style.DisabledAlpha;
            theme.WindowPadding = Style.WindowPadding.ToNumericVector();
            theme.WindowRounding = Style.WindowRounding;
            theme.WindowBorderSize = Style.WindowBorderSize;
            theme.WindowMinSize = Style.WindowMinSize.ToNumericVector();
            theme.WindowTitleAlign = Style.WindowTitleAlign.ToNumericVector();
            theme.WindowTitleAlign = Style.WindowTitleAlign.ToNumericVector();
            theme.ChildRounding = Style.ChildRounding;
            theme.ChildBorderSize = Style.ChildBorderSize;
            theme.PopupBorderSize = Style.PopupBorderSize;
            theme.PopupRounding = Style.PopupRounding;
            theme.FramePadding = Style.FramePadding.ToNumericVector();
            theme.FrameRounding = Style.FrameRounding;
            theme.FrameBorderSize = Style.FrameBorderSize;
            theme.ItemSpacing = Style.ItemSpacing.ToNumericVector();
            theme.ItemInnerSpacing = Style.ItemInnerSpacing.ToNumericVector();
            theme.CellPadding = Style.CellPadding.ToNumericVector();
            theme.TouchExtraPadding = Style.TouchExtraPadding.ToNumericVector();
            theme.IndentSpacing = Style.IndentSpacing;
            theme.ColumnsMinSpacing = Style.ColumnsMinSpacing;
            theme.ScrollbarSize = Style.ScrollbarSize;
            theme.ScrollbarRounding = Style.ScrollbarRounding;
            theme.GrabMinSize = Style.GrabMinSize;
            theme.GrabRounding = Style.GrabRounding;
            theme.LogSliderDeadzone = Style.LogSliderDeadzone;
            theme.TabRounding = Style.TabRounding;
            theme.TabBorderSize = Style.TabBorderSize;
            theme.TabMinWidthForCloseButton = Style.TabMinWidthForCloseButton;
            theme.ButtonTextAlign = Style.ButtonTextAlign.ToNumericVector();
            theme.SelectableTextAlign = Style.SelectableTextAlign.ToNumericVector();
            theme.SeparatorTextBorderSize = Style.SeparatorTextBorderSize;
            theme.SelectableTextAlign = Style.SelectableTextAlign.ToNumericVector();
            theme.SeparatorTextPadding = Style.SeparatorTextPadding.ToNumericVector();
            theme.DisplayWindowPadding = Style.DisplayWindowPadding.ToNumericVector();
            theme.DisplaySafeAreaPadding = Style.DisplaySafeAreaPadding.ToNumericVector();
            theme.DockingSeparatorSize = Style.DockingSeparatorSize;
            theme.MouseCursorScale = Style.MouseCursorScale;
            theme.AntiAliasedFill = Style.AntiAliasedFill;
            theme.AntiAliasedLines = Style.AntiAliasedLines;
            theme.AntiAliasedLinesUseTex = Style.AntiAliasedLinesUseTex;
            theme.CurveTessellationTol = Style.CurveTessellationTol;
            theme.CircleTessellationMaxError = Style.CircleTessellationMaxError;
            theme.HoverDelayNormal = Style.HoverDelayNormal;
            theme.HoverDelayShort = Style.HoverDelayShort;
            theme.HoverStationaryDelay = Style.HoverStationaryDelay;
            foreach (ImGuiCol color in Enum.GetValues(typeof(ImGuiCol)))
            {
                try
                {
                    theme.Colors.Add(color, Style.Colors[(int)color].ToNumericVector());
                }
                catch
                {

                }
            }
            return theme;
        }
        public string Name;
        public float Alpha;
        public float DisabledAlpha;
        public System.Numerics.Vector2 WindowPadding;
        public float WindowRounding;
        public float WindowBorderSize;
        public System.Numerics.Vector2 WindowMinSize;
        public System.Numerics.Vector2 WindowTitleAlign;
        public float ChildRounding;
        public float ChildBorderSize;
        public float PopupRounding;
        public float PopupBorderSize;
        public System.Numerics.Vector2 FramePadding;
        public float FrameRounding;
        public float FrameBorderSize;
        public System.Numerics.Vector2 ItemSpacing;
        public System.Numerics.Vector2 ItemInnerSpacing;
        public System.Numerics.Vector2 CellPadding;
        public System.Numerics.Vector2 TouchExtraPadding;
        public float IndentSpacing;
        public float ColumnsMinSpacing;
        public float ScrollbarSize;
        public float ScrollbarRounding;
        public float GrabMinSize;
        public float GrabRounding;
        public float LogSliderDeadzone;
        public float TabRounding;
        public float TabBorderSize;
        public float TabMinWidthForCloseButton;
        public System.Numerics.Vector2 ButtonTextAlign;
        public System.Numerics.Vector2 SelectableTextAlign;
        public float SeparatorTextBorderSize;
        public System.Numerics.Vector2 SeparatorTextAlign;
        public System.Numerics.Vector2 SeparatorTextPadding;
        public System.Numerics.Vector2 DisplayWindowPadding;
        public System.Numerics.Vector2 DisplaySafeAreaPadding;
        public float DockingSeparatorSize;
        public float MouseCursorScale;
        public bool AntiAliasedLines;
        public bool AntiAliasedLinesUseTex;
        public bool AntiAliasedFill;
        public float CurveTessellationTol;
        public float CircleTessellationMaxError;
        public float HoverStationaryDelay;
        public float HoverDelayShort;
        public float HoverDelayNormal;
        public Dictionary<ImGuiCol, System.Numerics.Vector4> Colors = new();
    }
}

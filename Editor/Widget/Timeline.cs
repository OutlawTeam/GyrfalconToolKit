using Gyrfalcon.Animation;
using ImGuiNET;
using OpenTK.Mathematics;
using System.Speech.Recognition;

namespace GyrfalconToolKit.Editor.Widget
{
    public class TimelineTrack
    {
        public string Name { get; set; }
        public List<float> Keyframes { get; set; } = new List<float>();
    }
    internal class Timeline
    {
        static float LegendWith = 0.2f;

        public void PlayHead(ref int CurrentTick, int TickPerSecond, float TotalDuration, float Zoom, Vector2 Pos, Vector2 Canvas)
        {
            float playheadX = Pos.X + CurrentTick / (TotalDuration * TickPerSecond) * Canvas.X * Zoom;

            Vector2 playheadStart = new Vector2(playheadX, Pos.Y);
            Vector2 playheadEnd = new Vector2(playheadX, playheadStart.Y + Canvas.Y);
            Vector4 HeadColor = new(1, 0, 0, 1);
            if (ImGui.IsMouseHoveringRect(playheadStart + new Vector2(-11.5f, 0), playheadStart + new Vector2(11.5f, 20)))
            {
                HeadColor = new(0.5f, 0, 0, 1);

                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    CurrentTick += (int)ImGui.GetIO().MouseDelta.X;
                    HeadColor = new(0f, 0, 1f, 1);
                }
            }
            CurrentTick = (int)Math.Min(Math.Max(0, CurrentTick), TickPerSecond * TotalDuration);
            ImGui.GetWindowDrawList().AddTriangleFilled(playheadStart + new Vector2(-11.5f, 0), playheadStart + new Vector2(11.5f, 0), playheadStart + new Vector2(0f, 20), ImGui.GetColorU32(new Vector4(1, 1, 1, 1)));
            ImGui.GetWindowDrawList().AddLine(playheadStart + new Vector2(0, 20), playheadEnd, ImGui.GetColorU32(HeadColor));

        }
        public void Update(ref int CurrentTick, ref int TickPerSecond, ref float TotalDuration, ref float CurrentZoom, Animation anim)
        {
            Vector2 CanvasSize = ImGui.GetContentRegionAvail();
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X * 0.2f);
            ImGui.InputFloat("Zoom", ref CurrentZoom);
            ImGui.SameLine();

            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X * 0.2f);
            ImGui.InputInt("Tick Per Second", ref TickPerSecond);
            ImGui.SameLine();
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X * 0.3f);

            ImGui.InputFloat("Duration", ref TotalDuration);
            ImGui.SameLine();
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X * 0.4f);

            ImGui.InputInt("Current Tick", ref CurrentTick);

            ImGui.BeginChild("Time", new Vector2(CanvasSize.X, anim.JointsAnimationData.Count * 20 + 50),false, ImGuiWindowFlags.AlwaysHorizontalScrollbar);

            Vector2 Pos = ImGui.GetCursorScreenPos();
            CanvasSize = ImGui.GetContentRegionAvail();


            float Width = HandleLegend(Pos, CanvasSize, anim);
            Pos.X += Width;
            CanvasSize.X -= Width;
            ImGui.GetWindowDrawList().AddRectFilled(Pos, Pos + new Vector2(CanvasSize.X, 20), ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1)));

            ImGui.GetWindowDrawList().AddRectFilled(Pos + new Vector2(0, 20), Pos + CanvasSize, ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1)));
            RenderTickLine(TickPerSecond, TotalDuration, CurrentZoom, Pos, CanvasSize);
            PlayHead(ref CurrentTick, TickPerSecond, TotalDuration, CurrentZoom, Pos, CanvasSize);
            ImGui.EndChild();
        }

        public float HandleLegend(Vector2 Pos, Vector2 Canvas, Animation anim)
        {


            Vector2 Size = new(LegendWith * Canvas.X, Canvas.Y);

            ImGui.GetWindowDrawList().AddRectFilled(Pos, Pos + Size, ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]));
            ImGui.GetWindowDrawList().AddRectFilled(Pos + new Vector2(Size.X, 0), Pos + new Vector2(Size.X, 0) + new Vector2(Canvas.X - Size.X, 20), ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1)));

            bool Odd = true;
            float CurrentY = Pos.Y + 20;
            foreach (var Joint in anim.JointsAnimationData.Values)
            {
                ImGui.GetWindowDrawList().AddRectFilled(new Vector2(Pos.X, CurrentY), new Vector2(Pos.X + Size.X, CurrentY + 20), ImGui.GetColorU32(Odd ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(0.7f, 0.7f, 0.7f, 1)));
                ImGui.GetWindowDrawList().AddText(new Vector2(Pos.X + 5, CurrentY + 5), ImGui.GetColorU32(new Vector4(0, 0, 0, 1)), Joint.JointName);
                CurrentY += 20;
                Odd = !Odd;
            }
            if (ImGui.IsMouseHoveringRect(Pos + Size + new Vector2(-6, -Canvas.Y), Pos + Size + new Vector2(4f, 0)))
            {
                Vector4 SliderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgHovered];

                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    LegendWith += ImGui.GetIO().MouseDelta.X / Canvas.X;
                    LegendWith = Math.Min(Math.Max(0.1f, LegendWith), 0.35f);
                    SliderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgActive];
                }
                ImGui.GetWindowDrawList().AddRectFilled(Pos + Size + new Vector2(-6, -Canvas.Y), Pos + Size,
                    ImGui.GetColorU32(SliderColor));
            }
            return Size.X;
        }
        public void RenderTickLine(int TickPerSecond, float TotalDuration, float Zoom, Vector2 Pos, Vector2 Canvas)
        {
            for (int i = 0; i < TickPerSecond * TotalDuration; i++)
            {
                float playheadX = Pos.X + i / (TotalDuration * TickPerSecond) * Canvas.X * Zoom;
                ImGui.GetWindowDrawList().AddLine(new Vector2(playheadX, Pos.Y + 20), new(playheadX, Pos.Y + Canvas.Y), ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 1)));
            }
        }
    }
}

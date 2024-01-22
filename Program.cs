using Gyrfalcon.Render;
using GyrfalconToolKit;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using OpenTK.Windowing.Common.Input;
using Gyrfalcon.Subsystems;
using Gyrfalcon.Programs;
using Gyrfalcon.Engine;
SubsystemManager.AddSubsystem(new MainSubsystem());
Renderer.AddRenderTask(new MainRenderTask(),true);
ImageResult image = ImageResult.FromMemory(GyrfalconToolKit.Properties.Resources.logo);

NativeWindowSettings nativeWindowSettings = new()
{
    Title = "GyrfalconToolKit",
    Flags = ContextFlags.ForwardCompatible,
    Icon = new WindowIcon(new Image[]{ new Image(image.Width, image.Height, image.Data) } ),
    Size = new OpenTK.Mathematics.Vector2i(900,600)
};
EngineOption options = new EngineOption()
{
    AdvancedPhysics = false,
    Intro = false,
    Viewport = true,
};
using (Window window = new(options, GameWindowSettings.Default, nativeWindowSettings))
{
    window.VSync = VSyncMode.On;
    window.Run();
}

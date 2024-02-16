using Gyrfalcon.Engine;
EngineStartupOption options = new EngineStartupOption()
{
    AdvancedPhysics = false,
    Intro = false,
    Viewport = true,
};
Engine.Startup(options, new GyrfalconToolKit.GyrfalconToolKitApp(), EngineWindowing.Window);

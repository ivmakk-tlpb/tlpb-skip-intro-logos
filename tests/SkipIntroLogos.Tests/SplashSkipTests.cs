using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using SkipIntroLogos;
using SkipIntroLogos.Game;
using UnityEngine.Rendering;
using Xunit;

// The start of the splash skip in Plugin.Load, with the real delegates against the fake game types.
public class SplashSkipTests : GameTestBase
{
    [Fact]
    public void With_skip_splash_on_the_load_registers_the_signal_adds_it_and_starts_one_thread()
    {
        Assert.Equal(new[] { typeof(FirstFrameSignal) }, ClassInjector.Registered);
        Assert.Equal(new[] { typeof(FirstFrameSignal) }, BasePlugin.Added);
        Assert.Single(FakeState.Started);
    }

    [Fact]
    public void With_skip_splash_off_no_thread_starts()
    {
        FakeState.Reset();
        BepInEx.Configuration.ConfigFile.Preset["SkipSplash"] = false;

        new Plugin().Load();

        Assert.Empty(FakeState.Started);
        Assert.Empty(BasePlugin.Added);
    }

    [Fact]
    public void The_thread_attaches_to_the_domain_stops_the_splash_immediately_and_detaches()
    {
        SplashScreen.OnStop = () => SplashStopper.FirstFrame = true;

        FakeState.Started[0]();

        Assert.Equal(new[] { SplashScreen.StopBehavior.StopImmediate }, SplashScreen.Stops);
        Assert.Equal(new[] { "attach " + IL2CPP.Domain.ToInt64(), "detach " + IL2CPP.Thread.ToInt64() }, IL2CPP.Calls);
    }

    [Fact]
    public void A_failed_start_logs_one_warning_and_leaves_the_splash()
    {
        FakeState.Reset();
        ClassInjector.Throw = true;

        new Plugin().Load();

        Assert.Empty(FakeState.Started);
        Assert.Single(Plugin.Log.Warnings);
        Assert.StartsWith("Skip Intro Logos shows the intro logos as in the game", Plugin.Log.Warnings[0]);
    }

    [Fact]
    public void The_start_writes_a_verbose_line()
    {
        FakeState.Reset();
        BepInEx.Configuration.ConfigFile.Preset["Verbose"] = true;

        new Plugin().Load();

        Assert.Contains("Splash stop thread started", Plugin.Log.Debugs);
    }

    [Fact]
    public void The_first_frame_signal_sets_the_flag_and_removes_itself()
    {
        var signal = new FirstFrameSignal(IntPtr.Zero);

        PatchCall.RunInstance(signal, "Update");

        Assert.True(SplashStopper.FirstFrame);
        Assert.Equal(new UnityEngine.Object[] { signal }, UnityEngine.Object.Destroyed);
    }
}

using HarmonyLib;

// Puts the static state of the fake game types back to a known start.
internal static class FakeState
{
    public static void Reset()
    {
        Harmony.Patched.Clear();
        Harmony.Unpatched.Clear();
        Harmony.ThrowFor = null;
        BepInEx.Configuration.ConfigFile.Preset.Clear();
        BepInEx.Unity.IL2CPP.BasePlugin.Added.Clear();
        Il2CppInterop.Runtime.Injection.ClassInjector.Registered.Clear();
        Il2CppInterop.Runtime.Injection.ClassInjector.Throw = false;
        Il2CppInterop.Runtime.IL2CPP.Calls.Clear();
        UnityEngine.Object.Destroyed.Clear();
        UnityEngine.Rendering.SplashScreen.Stops.Clear();
        UnityEngine.Rendering.SplashScreen.OnStop = null;
        SkipIntroLogos.Game.SplashStopper.FirstFrame = false;
        SkipIntroLogos.Patches.NearLoadPatches.Reset();
        SkipIntroLogos.Game.SplashStopper.StartThread = start => Started.Add(start);
        Started.Clear();
    }

    // The thread bodies that the plugin started. A test runs one by hand.
    public static readonly List<ThreadStart> Started = new List<ThreadStart>();
}

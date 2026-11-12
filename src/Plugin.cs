using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using SkipIntroLogos.Game;
using UnityEngine.Rendering;

namespace SkipIntroLogos
{
    [BepInPlugin(PluginGuid, "Skip Intro Logos", "1.0.0")]
    [BepInProcess("TheLastPlague.exe")]
    public sealed class Plugin : BasePlugin
    {
        public const string PluginGuid = "com.ivmakk.thelastplagueblight.skipintrologos";

        internal static new ManualLogSource Log;
        internal static ConfigEntry<bool> SkipSplash;
        internal static ConfigEntry<float> FadeSeconds;
        internal static ConfigEntry<bool> FastWorldLoad;
        internal static ConfigEntry<bool> Verbose;

        // A patch group is applied alone with its own Harmony id: a missing target removes only that group.
        internal sealed class PatchGroup
        {
            public readonly string Name;
            public readonly string OffText;
            public readonly List<Type> Types;

            public PatchGroup(string name, string offText, params Type[] types)
            {
                Name = name;
                OffText = offText;
                Types = new List<Type>(types);
            }
        }

        internal static readonly List<PatchGroup> Groups = new List<PatchGroup>
        {
            new PatchGroup("fade", "Skip Intro Logos leaves the black fades as in the game", typeof(Patches.FadePatches)),
            new PatchGroup("near load", "Skip Intro Logos loads the world as in the game", typeof(Patches.NearLoadPatches)),
        };

        private static readonly HashSet<string> warned = new HashSet<string>();

        // The names of the patch groups that are in place.
        internal static readonly HashSet<string> Applied = new HashSet<string>();

        internal static float Fade => Settings.FadeSeconds(FadeSeconds.Value);

        public override void Load()
        {
            Log = base.Log;
            warned.Clear();
            Applied.Clear();
            SkipSplash = Config.Bind(
                "General", "SkipSplash", true,
                "Skips the studio logos and the \"Made with Unity\" logo at the game start. With false, the logos show as in the game.");
            FadeSeconds = Config.Bind(
                "General", "FadeSeconds", Settings.DefaultFadeSeconds,
                new ConfigDescription(
                    "The longest black fade at the title screen, around each save load, at a new game, a host or a join, and at a respawn, in seconds. The game fades for 1 to 2 seconds. 0 changes the screen at once. The fades in the world, such as the wake-up after sleep, stay as in the game.",
                    new AcceptableValueRange<float>(0f, 1f)));
            FastWorldLoad = Config.Bind(
                "General", "FastWorldLoad", true,
                "Ends the load screen when the land near you is ready. The far land loads in the first second after the load. Your View Distance setting does not change. With false, the world loads as in the game.");
            Verbose = Config.Bind(
                "General", "Verbose", false,
                "Writes extra diagnostic lines to the BepInEx log for troubleshooting. Keep off in normal play.");
            StartSplashSkip();
            foreach (var group in Groups) Apply(group);
            Log.LogInfo("Skip Intro Logos loaded.");
        }

        // The splash plays between this load and the first game frame. A thread asks the engine to stop it.
        private void StartSplashSkip()
        {
            if (!SkipSplash.Value) return;
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<FirstFrameSignal>();
                AddComponent<FirstFrameSignal>();
                var clock = Stopwatch.StartNew();
                IntPtr thread = IntPtr.Zero;
                var stopper = new SplashStopper(
                    attach: () => thread = IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get()),
                    stop: () => SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate),
                    detach: () => IL2CPP.il2cpp_thread_detach(thread),
                    sleep: Thread.Sleep,
                    clock: () => clock.Elapsed.TotalSeconds);
                SplashStopper.StartThread(() => stopper.Run());
                Debug("Splash stop thread started");
            }
            catch (Exception e)
            {
                Log.LogWarning("Skip Intro Logos shows the intro logos as in the game: " + e.Message);
            }
        }

        // A game update can rename a target. Then the patches of that group go, and the other groups stay.
        private static void Apply(PatchGroup group)
        {
            var harmony = new Harmony(PluginGuid + "." + group.Name);
            try
            {
                foreach (var patches in group.Types) harmony.PatchAll(patches);
                Applied.Add(group.Name);
                Debug("Patch group " + group.Name + " applied");
            }
            catch (Exception e)
            {
                harmony.UnpatchSelf();
                Log.LogWarning(group.OffText + ": " + e.Message);
            }
        }

        // One warning for each distinct error, so a repeated error does not flood the log.
        internal static void WarnOnce(string where, Exception e)
        {
            string text = where + ": " + e.GetType().Name + ": " + e.Message;
            if (warned.Add(text)) Log.LogWarning(text + "\n" + e);
        }

        internal static void Debug(string line)
        {
            if (Verbose.Value) Log.LogDebug(line);
        }
    }
}

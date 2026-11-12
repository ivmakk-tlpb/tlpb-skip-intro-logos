using System;
using System.Globalization;
using HarmonyLib;

namespace SkipIntroLogos.Patches
{
    // The screen fades of the menu and load paths: the title screen, a new game, a load, a host, a join, and a
    // respawn. The HUD fades of the world reach the fading image directly and keep the game length.
    internal static class FadePatches
    {
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.FadeScreenOverlayIn)), HarmonyPrefix]
        internal static void FadeIn(ref float duration) => Cap("in", ref duration);

        [HarmonyPatch(typeof(UIManager), nameof(UIManager.FadeScreenOverlayOut)), HarmonyPrefix]
        internal static void FadeOut(ref float duration) => Cap("out", ref duration);

        private static void Cap(string direction, ref float duration)
        {
            try
            {
                float game = duration;
                duration = FadeCap.Cap(game, Plugin.Fade);
                Plugin.Debug("Screen fade " + direction + ": " + Seconds(game) + " s -> " + Seconds(duration) + " s");
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Skip Intro Logos fade", e);
            }
        }

        private static string Seconds(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}

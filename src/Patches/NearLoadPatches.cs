using System;
using System.Globalization;
using HarmonyLib;

namespace SkipIntroLogos.Patches
{
    // A save load and a new game set the view distance of the world once, before the chunks load. The world loads
    // with the near view distance, and the game's fields come back when the world is active.
    internal static class NearLoadPatches
    {
        private static NearLoad near = new NearLoad();

        internal static void Reset() => near = new NearLoad();

        [HarmonyPatch(typeof(WorldManager), "setViewDistance", new[] { typeof(World) }), HarmonyPostfix]
        internal static void AfterSetViewDistance(World world)
        {
            try
            {
                float min = world.minViewDistance;
                float max = world.maxViewDistance;
                if (!near.Keep(min, max, Plugin.FastWorldLoad.Value))
                {
                    Plugin.Debug("Near load: off for this load (setting off or view distance max " + Number(max) + " at most " + Number(NearLoad.NearMax) + ")");
                    return;
                }
                world.SetViewDistance(NearLoad.NearValue);
                Plugin.Debug("Near load: game view distance min " + Number(min) + " max " + Number(max) +
                    ", load with min " + Number(world.minViewDistance) + " max " + Number(world.maxViewDistance));
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Skip Intro Logos near load", e);
            }
        }

        [HarmonyPatch(typeof(World), nameof(World.SetWorldState)), HarmonyPrefix]
        internal static void BeforeSetWorldState(World __instance, WorldState state)
        {
            if (state != WorldState.ACTIVE) return;
            try
            {
                if (!near.TakeBack(out float min, out float max))
                {
                    Plugin.Debug("Near load: world active, no near load to take back");
                    return;
                }
                __instance.minViewDistance = min;
                __instance.maxViewDistance = max;
                Plugin.Debug("Near load: world active, view distance back to min " + Number(min) + " max " + Number(max));
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Skip Intro Logos near load write-back", e);
            }
        }

        private static string Number(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}

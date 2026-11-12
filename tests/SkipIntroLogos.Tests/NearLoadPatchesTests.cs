using System.Reflection;
using HarmonyLib;
using SkipIntroLogos;
using SkipIntroLogos.Patches;
using Xunit;

public class NearLoadPatchesTests : GameTestBase
{
    // A world as the game sets it at the maximum view distance setting.
    private static World FarWorld() => new World { minViewDistance = 50f, maxViewDistance = 90f };

    private static void AfterSetViewDistance(World world) =>
        PatchCall.Run(typeof(NearLoadPatches), "AfterSetViewDistance", world);

    private static void BeforeSetWorldState(World world, WorldState state) =>
        PatchCall.Run(typeof(NearLoadPatches), "BeforeSetWorldState", world, state);

    [Fact]
    public void The_load_uses_the_near_view_distance()
    {
        var world = FarWorld();

        AfterSetViewDistance(world);

        Assert.Equal(new[] { 0.5f }, world.ViewDistanceCalls);
        Assert.Equal(42f, world.minViewDistance);
        Assert.Equal(66f, world.maxViewDistance);
    }

    [Fact]
    public void Active_writes_the_kept_fields_back()
    {
        var world = FarWorld();
        AfterSetViewDistance(world);

        BeforeSetWorldState(world, WorldState.ACTIVE);

        Assert.Equal(50f, world.minViewDistance);
        Assert.Equal(90f, world.maxViewDistance);
    }

    [Theory]
    [InlineData(WorldState.LOADING)]
    [InlineData(WorldState.LOADED)]
    [InlineData(WorldState.UNLOADED)]
    public void Another_state_leaves_the_near_fields(WorldState state)
    {
        var world = FarWorld();
        AfterSetViewDistance(world);

        BeforeSetWorldState(world, state);

        Assert.Equal(66f, world.maxViewDistance);
    }

    [Fact]
    public void A_later_active_with_nothing_kept_leaves_the_fields()
    {
        var world = FarWorld();
        AfterSetViewDistance(world);
        BeforeSetWorldState(world, WorldState.ACTIVE);
        world.SetViewDistance(0.2f);

        BeforeSetWorldState(world, WorldState.ACTIVE);

        Assert.Equal(51.6f, world.maxViewDistance, 3);
    }

    [Fact]
    public void Fast_world_load_off_leaves_the_view_distance()
    {
        Plugin.FastWorldLoad.Value = false;
        var world = FarWorld();

        AfterSetViewDistance(world);

        Assert.Empty(world.ViewDistanceCalls);
        Assert.Equal(90f, world.maxViewDistance);
    }

    [Fact]
    public void A_near_view_distance_setting_stays()
    {
        var world = new World { minViewDistance = 38.8f, maxViewDistance = 56.4f };

        AfterSetViewDistance(world);

        Assert.Empty(world.ViewDistanceCalls);
    }

    [Fact]
    public void Each_step_writes_one_verbose_line_with_the_values()
    {
        Plugin.Verbose.Value = true;
        var world = FarWorld();

        AfterSetViewDistance(world);
        BeforeSetWorldState(world, WorldState.ACTIVE);

        Assert.Equal(new[]
        {
            "Near load: game view distance min 50 max 90, load with min 42 max 66",
            "Near load: world active, view distance back to min 50 max 90",
        }, Plugin.Log.Debugs);
    }

    [Fact]
    public void A_skipped_near_load_writes_why()
    {
        Plugin.Verbose.Value = true;
        Plugin.FastWorldLoad.Value = false;

        AfterSetViewDistance(FarWorld());

        Assert.Equal(new[] { "Near load: off for this load (setting off or view distance max 90 at most 66)" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void An_active_world_with_nothing_kept_writes_why()
    {
        Plugin.Verbose.Value = true;

        BeforeSetWorldState(FarWorld(), WorldState.ACTIVE);

        Assert.Equal(new[] { "Near load: world active, no near load to take back" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_is_caught_and_logged_once()
    {
        AfterSetViewDistance(null);
        AfterSetViewDistance(null);
        Assert.Single(Plugin.Log.Warnings);

        AfterSetViewDistance(FarWorld());
        BeforeSetWorldState(null, WorldState.ACTIVE);
        Assert.Equal(2, Plugin.Log.Warnings.Count);
    }

    [Fact]
    public void The_postfix_targets_the_one_argument_set_view_distance()
    {
        var info = typeof(NearLoadPatches).GetMethod("AfterSetViewDistance", BindingFlags.Static | BindingFlags.NonPublic);
        var patch = info.GetCustomAttribute<HarmonyPatch>();

        Assert.Equal(typeof(WorldManager), patch.Type);
        Assert.Equal("setViewDistance", patch.Method);
        Assert.Equal(new[] { typeof(World) }, patch.Parameters);
        Assert.NotNull(info.GetCustomAttribute<HarmonyPostfix>());
    }

    [Fact]
    public void The_prefix_targets_set_world_state()
    {
        var info = typeof(NearLoadPatches).GetMethod("BeforeSetWorldState", BindingFlags.Static | BindingFlags.NonPublic);
        var patch = info.GetCustomAttribute<HarmonyPatch>();

        Assert.Equal(typeof(World), patch.Type);
        Assert.Equal(nameof(World.SetWorldState), patch.Method);
        Assert.NotNull(info.GetCustomAttribute<HarmonyPrefix>());
    }

    [Fact]
    public void The_near_load_patches_are_the_near_load_group()
    {
        var group = Plugin.Groups.Single(g => g.Name == "near load");
        Assert.Equal(new[] { typeof(NearLoadPatches) }, group.Types);
        Assert.Contains("near load", Plugin.Applied);
    }
}

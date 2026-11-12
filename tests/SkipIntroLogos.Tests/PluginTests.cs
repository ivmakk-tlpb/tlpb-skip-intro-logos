using BepInEx.Configuration;
using SkipIntroLogos;
using Xunit;

public class PluginTests : GameTestBase
{
    [Fact]
    public void The_guid_names_the_mod()
    {
        Assert.Equal("com.ivmakk.thelastplagueblight.skipintrologos", Plugin.PluginGuid);
    }

    [Fact]
    public void The_switches_are_on_by_default_and_debug_lines_are_off()
    {
        Assert.True(Plugin.SkipSplash.Value);
        Assert.True(Plugin.FastWorldLoad.Value);
        Assert.False(Plugin.Verbose.Value);
    }

    [Fact]
    public void The_fade_seconds_have_their_default_and_range()
    {
        Assert.Equal(0.1f, Plugin.FadeSeconds.Value);
        var range = (AcceptableValueRange<float>)Plugin.FadeSeconds.Description.AcceptableValues;
        Assert.Equal(0f, range.MinValue);
        Assert.Equal(1f, range.MaxValue);
    }

    [Theory]
    [InlineData(5f, 1f)]
    [InlineData(-1f, 0f)]
    [InlineData(0.3f, 0.3f)]
    public void The_fade_seconds_are_limited_when_they_are_read(float value, float read)
    {
        Plugin.FadeSeconds.Value = value;
        Assert.Equal(read, Plugin.Fade);
    }

    [Fact]
    public void The_debug_helper_writes_only_while_verbose_is_on()
    {
        Plugin.Debug("hidden");
        Plugin.Verbose.Value = true;
        Plugin.Debug("shown");

        Assert.Equal(new[] { "shown" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void A_repeated_error_is_logged_once()
    {
        var error = new InvalidOperationException("broken");
        Plugin.WarnOnce("fade", error);
        Plugin.WarnOnce("fade", error);

        Assert.Single(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_plugin_logs_its_load_line()
    {
        Assert.Contains("Skip Intro Logos loaded.", Plugin.Log.Infos);
    }

    private sealed class FirstPatches { }
    private sealed class SecondPatches { }

    // Loads the plugin with two test groups in place of the real ones, and puts the real groups back after.
    private static void LoadWithTestGroups()
    {
        var real = new List<Plugin.PatchGroup>(Plugin.Groups);
        Plugin.Groups.Clear();
        Plugin.Groups.Add(new Plugin.PatchGroup("first", "First off", typeof(FirstPatches)));
        Plugin.Groups.Add(new Plugin.PatchGroup("second", "Second off", typeof(SecondPatches)));
        try
        {
            new Plugin().Load();
        }
        finally
        {
            Plugin.Groups.Clear();
            Plugin.Groups.AddRange(real);
        }
    }

    [Fact]
    public void Each_group_is_applied_with_its_own_harmony_id()
    {
        FakeState.Reset();
        LoadWithTestGroups();

        Assert.Equal(new[] { typeof(FirstPatches), typeof(SecondPatches) }, HarmonyLib.Harmony.Patched);
        Assert.Equal(new[] { "first", "second" }, Plugin.Applied.OrderBy(n => n));
        Assert.Empty(Plugin.Log.Warnings);
    }

    [Fact]
    public void A_missing_target_removes_only_its_group_with_one_warning()
    {
        FakeState.Reset();
        HarmonyLib.Harmony.ThrowFor = typeof(FirstPatches);
        LoadWithTestGroups();

        Assert.Equal(new[] { "second" }, Plugin.Applied);
        Assert.Equal(new[] { Plugin.PluginGuid + ".first" }, HarmonyLib.Harmony.Unpatched);
        Assert.Single(Plugin.Log.Warnings);
        Assert.StartsWith("First off: ", Plugin.Log.Warnings[0]);
    }
}

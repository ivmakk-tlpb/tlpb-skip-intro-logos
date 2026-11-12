using System.Reflection;
using HarmonyLib;
using SkipIntroLogos;
using SkipIntroLogos.Patches;
using Xunit;

public class FadePatchesTests : GameTestBase
{
    private static float Run(string method, float duration)
    {
        var args = new object[] { duration };
        PatchCall.Run(typeof(FadePatches), method, args);
        return (float)args[0];
    }

    [Theory]
    [InlineData("FadeIn")]
    [InlineData("FadeOut")]
    public void A_fade_of_the_menu_and_load_paths_is_capped(string method)
    {
        Assert.Equal(0.1f, Run(method, 1.0f));
        Assert.Equal(0.1f, Run(method, 2.0f));
    }

    [Fact]
    public void A_short_game_fade_stays()
    {
        Assert.Equal(0.05f, Run("FadeIn", 0.05f));
    }

    [Fact]
    public void The_setting_is_read_live()
    {
        Plugin.FadeSeconds.Value = 0.5f;
        Assert.Equal(0.5f, Run("FadeOut", 2.0f));
    }

    [Fact]
    public void Each_fade_writes_one_verbose_line_with_both_durations()
    {
        Plugin.Verbose.Value = true;
        Run("FadeIn", 1.0f);
        Run("FadeOut", 2.0f);

        Assert.Equal(new[] { "Screen fade in: 1 s -> 0.1 s", "Screen fade out: 2 s -> 0.1 s" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_leaves_the_game_duration_and_is_logged_once()
    {
        Plugin.FadeSeconds = null;

        Assert.Equal(1.0f, Run("FadeIn", 1.0f));
        Assert.Equal(1.0f, Run("FadeIn", 1.0f));
        Assert.Single(Plugin.Log.Warnings);
    }

    [Theory]
    [InlineData("FadeIn", "FadeScreenOverlayIn")]
    [InlineData("FadeOut", "FadeScreenOverlayOut")]
    public void Each_prefix_targets_the_ui_manager_fade(string method, string target)
    {
        var info = typeof(FadePatches).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic);
        var patch = info.GetCustomAttribute<HarmonyPatch>();

        Assert.Equal(typeof(UIManager), patch.Type);
        Assert.Equal(target, patch.Method);
        Assert.NotNull(info.GetCustomAttribute<HarmonyPrefix>());
    }

    [Fact]
    public void The_fade_patches_are_the_fade_group()
    {
        var group = Plugin.Groups.Single(g => g.Name == "fade");
        Assert.Equal(new[] { typeof(FadePatches) }, group.Types);
        Assert.Contains("fade", Plugin.Applied);
    }
}

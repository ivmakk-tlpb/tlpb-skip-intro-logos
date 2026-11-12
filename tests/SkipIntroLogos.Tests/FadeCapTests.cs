using SkipIntroLogos;
using Xunit;

public class FadeCapTests
{
    [Theory]
    [InlineData(1.0f, 0.1f, 0.1f)]
    [InlineData(2.0f, 0.1f, 0.1f)]
    [InlineData(0.05f, 0.1f, 0.05f)]
    [InlineData(1.0f, 1.0f, 1.0f)]
    [InlineData(2.0f, 0.5f, 0.5f)]
    public void A_fade_lasts_the_setting_at_most(float game, float setting, float capped)
    {
        Assert.Equal(capped, FadeCap.Cap(game, setting));
    }

    [Fact]
    public void A_setting_of_0_gives_the_floor_so_the_fade_ends_in_one_frame()
    {
        Assert.Equal(0.001f, FadeCap.Floor);
        Assert.Equal(FadeCap.Floor, FadeCap.Cap(1.0f, 0f));
    }

    [Fact]
    public void A_game_fade_shorter_than_the_floor_stays()
    {
        Assert.Equal(0.0005f, FadeCap.Cap(0.0005f, 0f));
    }
}

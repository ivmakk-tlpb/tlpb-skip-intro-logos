using SkipIntroLogos;
using Xunit;

public class SettingsTests
{
    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0f, 0f)]
    [InlineData(0.1f, 0.1f)]
    [InlineData(0.75f, 0.75f)]
    [InlineData(1f, 1f)]
    [InlineData(5f, 1f)]
    public void The_fade_seconds_are_limited_to_0_to_1(float value, float read)
    {
        Assert.Equal(read, Settings.FadeSeconds(value));
    }

    [Fact]
    public void A_fade_seconds_value_that_is_not_a_number_is_the_default()
    {
        Assert.Equal(Settings.DefaultFadeSeconds, Settings.FadeSeconds(float.NaN));
    }
}

using SkipIntroLogos;
using Xunit;

public class NearLoadTests
{
    [Fact]
    public void A_far_view_distance_asks_for_the_near_value()
    {
        var near = new NearLoad();

        Assert.True(near.Keep(50f, 90f, enabled: true));
        Assert.Equal(0.5f, NearLoad.NearValue);
    }

    [Theory]
    [InlineData(66f)]
    [InlineData(42f)]
    public void A_view_distance_at_the_near_value_or_lower_asks_for_nothing(float max)
    {
        Assert.False(new NearLoad().Keep(34f, max, enabled: true));
    }

    [Fact]
    public void Fast_world_load_off_asks_for_nothing()
    {
        Assert.False(new NearLoad().Keep(50f, 90f, enabled: false));
    }

    [Fact]
    public void The_kept_values_come_back_once()
    {
        var near = new NearLoad();
        near.Keep(50f, 90f, enabled: true);

        Assert.True(near.TakeBack(out float min, out float max));
        Assert.Equal(50f, min);
        Assert.Equal(90f, max);
        Assert.False(near.TakeBack(out _, out _));
    }

    [Fact]
    public void A_second_load_before_active_replaces_the_kept_values()
    {
        var near = new NearLoad();
        near.Keep(50f, 90f, enabled: true);
        near.Keep(45f, 80f, enabled: true);

        near.TakeBack(out float min, out float max);
        Assert.Equal(45f, min);
        Assert.Equal(80f, max);
    }

    [Fact]
    public void A_second_load_with_a_near_view_distance_clears_the_kept_values()
    {
        var near = new NearLoad();
        near.Keep(50f, 90f, enabled: true);
        near.Keep(34f, 42f, enabled: true);

        Assert.False(near.TakeBack(out _, out _));
    }

    [Fact]
    public void Nothing_kept_gives_nothing_back()
    {
        Assert.False(new NearLoad().TakeBack(out _, out _));
    }
}

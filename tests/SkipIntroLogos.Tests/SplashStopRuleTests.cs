using SkipIntroLogos;
using Xunit;

public class SplashStopRuleTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(4.5)]
    [InlineData(9.99)]
    public void Before_the_first_frame_and_the_time_limit_the_thread_calls_again(double elapsed)
    {
        Assert.Equal(SplashStopEnd.None, SplashStopRule.Next(elapsed, firstFrameSeen: false));
    }

    [Fact]
    public void The_first_frame_ends_the_thread()
    {
        Assert.Equal(SplashStopEnd.FirstFrame, SplashStopRule.Next(0.5, firstFrameSeen: true));
    }

    [Theory]
    [InlineData(10.0)]
    [InlineData(25.0)]
    public void The_time_limit_ends_the_thread(double elapsed)
    {
        Assert.Equal(SplashStopEnd.TimeLimit, SplashStopRule.Next(elapsed, firstFrameSeen: false));
    }

    [Fact]
    public void The_first_frame_is_the_reason_also_after_the_time_limit()
    {
        Assert.Equal(SplashStopEnd.FirstFrame, SplashStopRule.Next(12.0, firstFrameSeen: true));
    }

    [Fact]
    public void The_thread_waits_50_ms_between_calls_and_stops_after_10_s()
    {
        Assert.Equal(50, SplashStopRule.WaitMs);
        Assert.Equal(10.0, SplashStopRule.LimitSeconds);
    }

    [Theory]
    [InlineData((int)SplashStopEnd.FirstFrame, "first frame")]
    [InlineData((int)SplashStopEnd.TimeLimit, "time limit")]
    [InlineData((int)SplashStopEnd.Error, "error")]
    [InlineData((int)SplashStopEnd.None, "none")]
    public void Each_end_has_a_reason_for_the_log(int end, string reason)
    {
        Assert.Equal(reason, SplashStopRule.Reason((SplashStopEnd)end));
    }
}

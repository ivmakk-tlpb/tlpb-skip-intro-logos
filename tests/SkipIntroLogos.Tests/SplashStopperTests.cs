using SkipIntroLogos;
using SkipIntroLogos.Game;
using Xunit;

public class SplashStopperTests : GameTestBase
{
    private readonly List<string> calls = new List<string>();
    private int nowMs;

    // A stopper whose sleep moves the clock by the wait, and runs the given action after each sleep.
    private SplashStopper Stopper(Action afterSleep = null, Action stop = null) =>
        new SplashStopper(
            attach: () => calls.Add("attach"),
            stop: stop ?? (() => calls.Add("stop")),
            detach: () => calls.Add("detach"),
            sleep: ms => { nowMs += ms; afterSleep?.Invoke(); },
            clock: () => nowMs / 1000.0);

    [Fact]
    public void The_thread_attaches_once_and_calls_stop_until_the_first_frame()
    {
        int sleeps = 0;
        var result = Stopper(afterSleep: () => { if (++sleeps == 3) SplashStopper.FirstFrame = true; }).Run();

        Assert.Equal(new[] { "attach", "stop", "stop", "stop", "detach" }, calls);
        Assert.Equal(SplashStopEnd.FirstFrame, result.End);
        Assert.Equal(3, result.Calls);
    }

    [Fact]
    public void The_thread_ends_at_the_time_limit()
    {
        var result = Stopper().Run();

        Assert.Equal(SplashStopEnd.TimeLimit, result.End);
        Assert.Equal(200, result.Calls);
        Assert.Equal("detach", calls.Last());
    }

    [Fact]
    public void A_stop_call_that_throws_ends_the_thread_with_one_warning()
    {
        int tries = 0;
        var result = Stopper(stop: () => { tries++; throw new InvalidOperationException("main thread only"); }).Run();

        Assert.Equal(SplashStopEnd.Error, result.End);
        Assert.Equal(0, result.Calls);
        Assert.Equal(1, tries);
        Assert.Single(Plugin.Log.Warnings);
        Assert.Contains("main thread only", Plugin.Log.Warnings[0]);
        Assert.Equal(new[] { "attach", "detach" }, calls);
    }

    [Fact]
    public void An_attach_that_throws_ends_the_thread_with_one_warning_and_no_stop_call()
    {
        var stopper = new SplashStopper(
            attach: () => throw new InvalidOperationException("no domain"),
            stop: () => calls.Add("stop"),
            detach: () => calls.Add("detach"),
            sleep: ms => nowMs += ms,
            clock: () => nowMs / 1000.0);

        var result = stopper.Run();

        Assert.Equal(SplashStopEnd.Error, result.End);
        Assert.Empty(calls);
        Assert.Single(Plugin.Log.Warnings);
        Assert.Contains("no domain", Plugin.Log.Warnings[0]);
    }

    [Fact]
    public void A_detach_that_throws_ends_the_thread_with_one_warning()
    {
        SplashStopper.FirstFrame = true;
        var stopper = new SplashStopper(
            attach: () => calls.Add("attach"),
            stop: () => calls.Add("stop"),
            detach: () => throw new InvalidOperationException("detach failed"),
            sleep: ms => nowMs += ms,
            clock: () => nowMs / 1000.0);

        var result = stopper.Run();

        Assert.Equal(SplashStopEnd.Error, result.End);
        Assert.Single(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_end_writes_one_verbose_line_with_the_count_and_the_reason()
    {
        Plugin.Verbose.Value = true;
        SplashStopper.FirstFrame = true;

        Stopper().Run();

        Assert.Equal(new[] { "Splash stop thread ended: 0 stop calls, reason first frame" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void A_first_frame_before_the_start_makes_no_stop_call()
    {
        SplashStopper.FirstFrame = true;

        var result = Stopper().Run();

        Assert.Equal(0, result.Calls);
        Assert.Equal(new[] { "attach", "detach" }, calls);
    }
}

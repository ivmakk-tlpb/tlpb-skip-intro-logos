using System;
using System.Threading;

namespace SkipIntroLogos.Game
{
    // The body of the splash stop thread. No plugin code runs on the main thread while the splash plays, so a
    // background thread asks the engine to stop it. The calls come in as delegates, so a test runs the loop
    // with no thread and no game.
    internal sealed class SplashStopper
    {
        // Set on the main thread by the first Update of FirstFrameSignal.
        internal static volatile bool FirstFrame;

        // Starts the thread body. A test replaces it, so no real thread starts.
        internal static Action<ThreadStart> StartThread = start => new Thread(start) { IsBackground = true, Name = "SkipIntroLogos splash stop" }.Start();

        internal readonly struct Result
        {
            public readonly SplashStopEnd End;
            public readonly int Calls;

            public Result(SplashStopEnd end, int calls)
            {
                End = end;
                Calls = calls;
            }
        }

        private readonly Action attach;
        private readonly Action stop;
        private readonly Action detach;
        private readonly Action<int> sleep;
        private readonly Func<double> clock;

        internal SplashStopper(Action attach, Action stop, Action detach, Action<int> sleep, Func<double> clock)
        {
            this.attach = attach;
            this.stop = stop;
            this.detach = detach;
            this.sleep = sleep;
            this.clock = clock;
        }

        // An exception that leaves a thread ends the game process, so the attach and the detach are caught too.
        internal Result Run()
        {
            Result result;
            try
            {
                attach();
                try
                {
                    result = Loop();
                }
                finally
                {
                    detach();
                }
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Skip Intro Logos splash stop thread", e);
                result = new Result(SplashStopEnd.Error, 0);
            }
            Plugin.Debug("Splash stop thread ended: " + result.Calls + " stop calls, reason " + SplashStopRule.Reason(result.End));
            return result;
        }

        private Result Loop()
        {
            double start = clock();
            int calls = 0;
            while (true)
            {
                var end = SplashStopRule.Next(clock() - start, FirstFrame);
                if (end != SplashStopEnd.None) return new Result(end, calls);
                try
                {
                    stop();
                    calls++;
                }
                catch (Exception e)
                {
                    Plugin.WarnOnce("Skip Intro Logos cannot stop the intro logos", e);
                    return new Result(SplashStopEnd.Error, calls);
                }
                sleep(SplashStopRule.WaitMs);
            }
        }
    }
}

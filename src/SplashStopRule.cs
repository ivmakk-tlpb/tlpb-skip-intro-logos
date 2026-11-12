namespace SkipIntroLogos
{
    internal enum SplashStopEnd
    {
        None,
        FirstFrame,
        TimeLimit,
        Error,
    }

    // When the splash stop thread calls again and when it ends. The engine plays the splash before the first
    // game frame, so the thread has work only until that frame.
    internal static class SplashStopRule
    {
        internal const int WaitMs = 50;
        internal const double LimitSeconds = 10.0;

        internal static SplashStopEnd Next(double elapsedSeconds, bool firstFrameSeen)
        {
            if (firstFrameSeen) return SplashStopEnd.FirstFrame;
            return elapsedSeconds >= LimitSeconds ? SplashStopEnd.TimeLimit : SplashStopEnd.None;
        }

        internal static string Reason(SplashStopEnd end)
        {
            switch (end)
            {
                case SplashStopEnd.FirstFrame: return "first frame";
                case SplashStopEnd.TimeLimit: return "time limit";
                case SplashStopEnd.Error: return "error";
                default: return "none";
            }
        }
    }
}

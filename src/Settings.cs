using System;

namespace SkipIntroLogos
{
    // The limits of the config values. A value is limited when it is read, also after an edit of the config file by hand.
    internal static class Settings
    {
        internal const float DefaultFadeSeconds = 0.1f;

        internal static float FadeSeconds(float value) =>
            float.IsNaN(value) ? DefaultFadeSeconds : Math.Clamp(value, 0f, 1f);
    }
}

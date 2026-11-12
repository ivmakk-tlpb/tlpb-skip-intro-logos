using System;

namespace SkipIntroLogos
{
    // The length of a screen fade of the menu and load paths.
    internal static class FadeCap
    {
        // The fade adds (frame time / duration) to its alpha each frame, so a duration of 0 gives NaN on a frame
        // with no time and the fade never ends. The floor still ends the fade in one frame.
        internal const float Floor = 0.001f;

        // The game fade, at most the setting. A game fade that is shorter stays.
        internal static float Cap(float game, float setting) => Math.Min(game, Math.Max(setting, Floor));
    }
}

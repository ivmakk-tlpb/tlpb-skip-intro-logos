namespace SkipIntroLogos
{
    // The near view distance of a world load. The chunk radius of a load follows the far view distance of the
    // world, so a load at the near value has fewer chunks to make. The game's own fields are kept and come back
    // once when the world is active, and the game then loads the far chunks while the player plays.
    internal sealed class NearLoad
    {
        // The view distance setting for the load, and the far view distance that it gives (lerp of 42 to 90).
        internal const float NearValue = 0.5f;
        internal const float NearMax = 66f;

        private bool kept;
        private float keptMin;
        private float keptMax;

        // True when the load uses the near value. A load with a view distance at the near value or lower stays,
        // and it also clears the values of an earlier load that never became active.
        internal bool Keep(float min, float max, bool enabled)
        {
            kept = enabled && max > NearMax;
            keptMin = min;
            keptMax = max;
            return kept;
        }

        internal bool TakeBack(out float min, out float max)
        {
            min = keptMin;
            max = keptMax;
            bool had = kept;
            kept = false;
            return had;
        }
    }
}

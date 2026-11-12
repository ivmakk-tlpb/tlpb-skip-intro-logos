using System;
using UnityEngine;

namespace SkipIntroLogos.Game
{
    // Tells the splash stop thread that the first game frame runs, then removes itself.
    public sealed class FirstFrameSignal : MonoBehaviour
    {
        public FirstFrameSignal(IntPtr pointer) : base(pointer) { }

        private void Update()
        {
            SplashStopper.FirstFrame = true;
            Destroy(this);
        }
    }
}

#region Using Statements
using System;
#endregion

namespace FreeRadicals.Simulation
{
    public enum ScreenRes
    {
        a1920x1200 = 0,
        b1680x1050 = 1,
        c1440x900 = 2,
        d1280x800 = 3
    }

    /// <summary>
    /// Adjustable game settings.
    /// </summary>
    public static class WorldRules
    {
        public static int ScoreLimit = 25;
        public static ScreenRes ScreenRes = ScreenRes.d1280x800;
        public static bool MotionBlur = false;
        public static bool NeonEffect = false;
        public static bool FullScreen = false;
    }
}
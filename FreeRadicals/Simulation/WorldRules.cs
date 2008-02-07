#region Using Statements
using System;
#endregion

namespace FreeRadicals.Simulation
{
    public enum AtomDensity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    /// <summary>
    /// Adjustable game settings.
    /// </summary>
    public static class WorldRules
    {
        public static int ScoreLimit = 25;
        public static AtomDensity AtomDensity = AtomDensity.None;
        public static bool MotionBlur = false;
        public static bool NeonEffect = false;
        public static bool FullScreen = true;
    }
}

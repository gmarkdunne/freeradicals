#region Using Statements
using System;
#endregion

namespace FreeRadicals
{
    public enum AtomDensity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum WallStyle
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3
    }

    /// <summary>
    /// Adjustable game settings.
    /// </summary>
    public static class WorldRules
    {
        public static int ScoreLimit = 25;
        public static WallStyle WallStyle = WallStyle.None;
        public static AtomDensity AtomDensity = AtomDensity.None;
        public static bool MotionBlur = true;
        public static bool NeonEffect = true;
        public static bool FullScreen = true;
    }
}

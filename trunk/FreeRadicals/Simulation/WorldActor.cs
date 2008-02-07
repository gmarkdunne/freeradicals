#region Using Statements
using System;
#endregion

namespace FreeRadicals.Simulation
{
    /// <summary>
    /// An actor that stands in for the world, in wall collisions, etc.
    /// </summary>
    class WorldActor : Actor
    {
        #region Initialization
        /// <summary>
        /// Construct a new world actor.
        /// </summary>
        /// <param name="world">The world that this world-actor belongs to.</param>
        public WorldActor(World world)
            : base(world)
        {
            radius = 0f;
            mass = 100f;
        }
        #endregion
    }
}

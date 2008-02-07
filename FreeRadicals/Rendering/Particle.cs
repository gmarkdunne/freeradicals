#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals.Rendering
{
    /// <summary>
    /// The data for a single particle in this game's particle systems.
    /// </summary>
    struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
    }
}

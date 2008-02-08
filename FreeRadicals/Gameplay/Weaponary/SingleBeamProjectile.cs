#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
#endregion

namespace FreeRadicals.Gameplay.Weaponary
{
    /// <summary>
    /// A laser bolt projectile.
    /// </summary>
    class SingleBeamProjectile : Projectile
    {
        #region Constants
        /// <summary>
        /// The length of the laser-bolt line, expressed as a percentage of velocity.
        /// </summary>
        const float lineLengthVelocityPercent = 0.01f;

        /// <summary>
        /// Particle system colors effect.
        /// </summary>
        static readonly Color[] explosionColors = 
        { 
            Color.White, Color.Gray, Color.Gray, Color.Silver, Color.Yellow
        };
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new laser projectile.
        /// </summary>
        /// <param name="world">The world that this projectile belongs to.</param>
        /// <param name="owner">The ship that fired this projectile, if any.</param>
        /// <param name="direction">The initial direction for this projectile.</param>
        public SingleBeamProjectile(World world, NanoBot owner, Vector2 direction)
            : base(world, owner, direction)
        {
            this.radius = 0.5f;
            this.speed = 1000f;
            this.duration = 5f;
            this.damageAmount = 20f;
            this.damageOwner = false;
            this.mass = 2f;
            this.explodes = false;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Render the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        /// <param name="lineBatch">The LineBatch to render to.</param>
        public override void Draw(float elapsedTime, LineBatch lineBatch)
        {
            if (lineBatch == null)
            {
                throw new ArgumentNullException("lineBatch");
            }
            //// Vector Polygon
            //VectorPolygon circle = VectorPolygon.CreateCircle(Vector2.Zero, 4f, 100);
            //lineBatch.DrawPolygon(circle, Color.Yellow);
            // draw a simple line
            lineBatch.DrawLine(position,
                position - velocity * lineLengthVelocityPercent, Color.Yellow);
        }
        #endregion 

        #region Interaction
        /// <summary>
        /// Defines the interaction between this projectile and a target actor
        /// when they touch.
        /// </summary>
        /// <param name="target">The actor that is touching this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public override bool Touch(Actor target)
        {
            // add a particle effect if we touched anything
            if (base.Touch(target))
            {
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 16, 32f, 64f, 1f, 0.1f, explosionColors));
                return true;
            }
            return false;
        }
        #endregion
    }
}

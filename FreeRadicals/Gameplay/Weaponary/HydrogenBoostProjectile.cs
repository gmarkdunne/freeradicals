#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.FreeRadicals;
using FreeRadicals.Gameplay.GreenhouseGases;
using FreeRadicals.Gameplay.JointMolecules;
#endregion

namespace FreeRadicals.Gameplay.Weaponary
{
    /// <summary>
    /// A laser bolt projectile.
    /// </summary>
    class HydrogenBoostProjectile : Projectile
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
            Color.White, Color.Gold, Color.Silver, Color.Yellow
        };
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new laser projectile.
        /// </summary>
        /// <param name="world">The world that this projectile belongs to.</param>
        /// <param name="owner">The ship that fired this projectile, if any.</param>
        /// <param name="direction">The initial direction for this projectile.</param>
        public HydrogenBoostProjectile(World world, NanoBot owner, Vector2 direction)
            : base(world, owner, direction)
        {
            this.life = 1f * world.ResVar;
            this.duration = 1f * world.ResVar;
            this.radius = 1f * world.ResVar;
            this.speed = 500f;
            this.damageAmount = 20f;
            this.damageOwner = false;
            this.mass = 4f;
            this.explodes = true;
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
        /// 

        public override bool Touch(Actor target)
        {
            if ((target is Nitrogen) == true ||
                (target is Oxygen) == true ||
                (target is Hydrogen) == true ||
                (target is Bromine) == true ||
                (target is Chlorine) == true ||
                (target is Carbon) == true ||
                (target is Fluorine) == true ||
                (target is OxygenTwo) == true ||
                (target is NitrogenTwo) == true ||
                (target is Methylene) == true ||
                (target is Deuterium) == true ||
                (target is Water) == true ||
                (target is Ozone) == true ||
                (target is NitrousOxide) == true ||
                (target is CarbonDioxide) == true ||
                (target is NitricOxide) == true ||
                (target is Hydroxyl) == true ||
                (target is CFC1) == true ||
                (target is CFC2) == true ||
                (target is NanoBot) == true)
            {
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 16, 32f * world.ResVar, 64f * world.ResVar, 
                    1f * world.ResVar, 0.1f * world.ResVar, explosionColors));

                return base.Touch(target);
            }
            return false;
        }

        /// <summary>
        /// Place this rocket in the world.
        /// </summary>
        /// <param name="findSpawnPoint">
        /// If true, the rocket's position is changed to a valid, non-colliding point.
        /// </param>
        public override void Spawn(bool findSpawnPoint)
        {
            base.Spawn(findSpawnPoint);

            // twitch the motors of the launching ship
            owner.FireGamepadMotors(0.5f, 0.25f);
        }

        /// <summary>
        /// Damages all actors in a radius around the rocket.
        /// </summary>
        /// <param name="touchedActor">The actor that was originally hit.</param>
        public override void Explode(Actor touchedActor)
        {
            // add a double-particle system effect
            world.ParticleSystems.Add(new ParticleSystem(this.position,
                Vector2.Zero, 16, 16f * world.ResVar, 32f * world.ResVar, 
                0.15f * world.ResVar, 0.015f * world.ResVar, explosionColors));
            world.ParticleSystems.Add(new ParticleSystem(this.position,
                Vector2.Zero, 4, 32f * world.ResVar, 64f * world.ResVar, 
                0.2f * world.ResVar, 0.025f * world.ResVar, explosionColors));

            base.Explode(touchedActor);
        }
        #endregion
    }
}

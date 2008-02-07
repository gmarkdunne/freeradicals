#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
#endregion

namespace FreeRadicals.Gameplay
{
    /// <summary>
    /// Atoms that fill the game simulation
    /// </summary>
    class Nitrogen : Actor
    {
        #region Constants
        /// <summary>
        /// The ratio between the mass and the radius of an oxygen.
        /// </summary>
        const float massRadiusRatio = 4f;

        /// <summary>
        /// The amount of drag applied to velocity per second, 
        /// as a percentage of velocity.
        /// </summary>
        const float dragPerSecond = 0.20f;

        /// <summary>
        /// Scalar for calculated damage values that asteroids apply to players.
        /// </summary>
        const float damageScalar = 0.001f;

        /// <summary>
        /// Scalar to convert the velocity / mass ratio into a "nice" rotational value.
        /// </summary>
        const float velocityMassRatioToRotationScalar = 0.005f;

        /// <summary>
        /// Particle system colors for the ship-explosion effect.
        /// </summary>
        static readonly Color[] N2OColors = 
        { 
            Color.Blue, Color.Red
        };
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new oxygen.
        /// </summary>
        /// <param name="world">The world that this oxygen belongs to.</param>
        /// <param name="radius">The size of the oxygen.</param>
        public Nitrogen(World world)
            : base(world)
        {
            // Nitrogen Radius
            this.radius = 14.0f; //(14.00674);
            // all atoms are coloured according to which type they are
            this.color = Color.Blue;
            // create the polygon
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, radius, 100);
            // the atom polygon might not be as big as the original radius, 
            // so find out how big it really is
            for (int i = 0; i < this.polygon.Points.Length; i++)
            {
                float length = this.polygon.Points[i].Length();
                if (length > this.radius)
                {
                    this.radius = length;
                }
            }
            // calculate the mass
            this.mass = radius * massRadiusRatio;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public override void Update(float elapsedTime)
        {
            // spin the oxygen based on the size and velocity
            this.rotation += (this.velocity.LengthSquared() / this.mass) * elapsedTime *
                velocityMassRatioToRotationScalar;

            // apply some drag so the asteroids settle down
            velocity -= velocity * (elapsedTime * dragPerSecond);

            // check if there is an Oxygen
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Oxygen) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.005f;
                        return;
                    }
                }
            }
            // check if there is an Nitrogen
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Nitrogen) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.005f;
                        return;
                    }
                }
            }
            // check if there is an NitrogenTwo
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is NitrogenTwo) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.005f;
                        return;
                    }
                }
            }

            base.Update(elapsedTime);
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Defines the interaction between the oxygen and a target actor
        /// when they touch.
        /// </summary>
        /// <param name="target">The actor that is touching this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public override bool Touch(Actor target)
        {
            // if the oxygen has touched a player, then damage it
            NanoBot player = target as NanoBot;
            if (player != null)
            {
                // calculate damage as a function of how much the two actor's
                // velocities were going towards one another
                Vector2 playerAsteroidVector =
                    Vector2.Normalize(this.position - player.Position);
                float rammingSpeed =
                    Vector2.Dot(playerAsteroidVector, player.Velocity) -
                    Vector2.Dot(playerAsteroidVector, this.velocity);
                //player.Damage(this, this.mass * rammingSpeed * damageScalar);

            }
            // if the oxygen didn't hit a projectile, play the oxygen-touch cue
            if ((target is Projectile) == false)
            {
                this.world.AudioManager.PlayCue("asteroidTouch");
            }
            // if the nitrogen hit an nitrogen, Bond them N2
            if ((target is Nitrogen) == true)
            {
                int N = 1;
                world.BondNitrogenTwo(this, target, N);
            }
            // if the Nitrogen hit an Nitric Oxide, Bond them to N2O
            if ((target is NitricOxide) == true)
            {
                this.Die(this);
                target.Die(target);
                Vector2 newPosition = (target.Position + this.position) / 2;
                Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                Vector2 newDirection = (target.Direction + this.direction) / 2;
                world.BondNitrousOxide(newPosition, newVelocity, newDirection);
                world.ParticleSystems.Add(new ParticleSystem(newPosition,
                    newDirection, 36, 64f, 128f, 2f, 0.05f, N2OColors));
                this.world.AudioManager.PlayCue("asteroidTouch");
            }
            return base.Touch(target);
        }


        /// <summary>
        /// Damage this oxygen by the amount provided.
        /// </summary>
        /// <remarks>
        /// This function is provided in lieu of a Life mutation property to allow 
        /// classes of objects to restrict which kinds of objects may damage them,
        /// and under what circumstances they may be damaged.
        /// </remarks>
        /// <param name="source">The actor responsible for the damage.</param>
        /// <param name="damageAmount">The amount of damage.</param>
        /// <returns>If true, this object was damaged.</returns>
        public override bool Damage(Actor source, float damageAmount)
        {
            // nothing hurst asteroids, nothing!
            return false;
        }
        #endregion
    }
}
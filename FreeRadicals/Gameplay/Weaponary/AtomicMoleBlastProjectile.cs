#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using Microsoft.Xna.Framework.Audio;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.GreenhouseGases;
using FreeRadicals.Gameplay.FreeRadicals;
using FreeRadicals.Gameplay.JointMolecules;
#endregion

namespace FreeRadicals.Gameplay.Weaponary
{
    /// <summary>
    /// A rocket projectile.
    /// </summary>
    class AtomicMoleBlastProjectile : Projectile
    {
        #region Fields
        /// <summary>
        /// The sound effect of the rocket as it flies.
        /// </summary>
        protected Cue rocketCue = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new rocket projectile.
        /// </summary>
        /// <param name="world">The world that this projectile belongs to.</param>
        /// <param name="owner">The ship that fired this projectile, if any.</param>
        /// <param name="direction">The initial direction for this projectile.</param>
        public AtomicMoleBlastProjectile(World world, NanoBot owner, Vector2 direction)
            : base(world, owner, direction)
        {
            this.radius = 4f * world.ResVar;
            this.life = 0.65f * world.ResVar;
            this.duration = 0.65f * world.ResVar;
            this.mass = 4f;
            this.speed = 500f;
            this.damageAmount = 50f;
            this.damageOwner = false;
            this.damageRadius = 16f;
            this.explodes = true;
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, 4f * world.ResVar, 20);
            this.color = Color.Gray;
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
            if ((target is CFC1) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionLarge");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 128, 32f * world.ResVar, 64f * world.ResVar, 
                    3f * world.ResVar, 0.1f * world.ResVar, world.CFC1Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 64, 64f * world.ResVar, 128f * world.ResVar, 
                    4f * world.ResVar, 0.15f * world.ResVar, world.CFC1Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 128f * world.ResVar, 256f * world.ResVar, 
                    5f * world.ResVar, 0.2f * world.ResVar, world.CFC1Color));
                owner.FireGamepadMotors(0.75f, 0.25f);

                target.Die(target);

                return base.Touch(target);
            }
            if ((target is CFC2) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionLarge");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 128, 32f * world.ResVar, 64f * world.ResVar, 
                    3f * world.ResVar, 0.1f * world.ResVar, world.CFC1Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 64, 64f * world.ResVar, 128f * world.ResVar, 
                    4f * world.ResVar, 0.15f * world.ResVar, world.CFC1Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 128f * world.ResVar, 256f * world.ResVar, 
                    5f * world.ResVar, 0.2f * world.ResVar, world.CFC1Color));
                owner.FireGamepadMotors(0.75f, 0.25f);

                target.Die(target);

                return base.Touch(target);
            }
            if ((target is Hydroxyl) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 64, 32f * world.ResVar, 64f * world.ResVar, 
                    3f * world.ResVar, 0.05f * world.ResVar, world.OHColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 16, 128f * world.ResVar, 256f * world.ResVar, 
                    4f * world.ResVar, 0.1f * world.ResVar, world.OHColor));
                owner.FireGamepadMotors(0.35f, 0.25f);

                target.Die(target);
                world.UnbondHydroxyl(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is NitricOxide) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 64, 32f * world.ResVar, 64f * world.ResVar, 
                    3f * world.ResVar, 0.05f * world.ResVar, world.NOColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 16, 128f * world.ResVar, 256f * world.ResVar, 
                    4f * world.ResVar, 0.1f * world.ResVar, world.NOColor));
                owner.FireGamepadMotors(0.35f, 0.25f);

                target.Die(target);
                world.UnbondNitricOxide(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is CarbonDioxide) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.CO2Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.CO2Color));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondCarbonDioxide(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is Methane) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.CH4Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.CH4Color));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondMethane(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is NitrousOxide) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.N2OColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.N2OColor));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondNitrousOxide(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is Ozone) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.O3Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.O3Color));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondOzone(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is Water) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.H2OColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.H2OColor));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondWater(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is Deuterium) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.HHColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.HHColor));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondDeuterium(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is Methylene) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.CH2Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.CH2Color));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondMethylene(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is NitrogenTwo) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.CH2Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.CH2Color));
                owner.FireGamepadMotors(0.0f, 0.25f);

                target.Die(target);
                world.UnbondNitrogenTwo(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            if ((target is OxygenTwo) == true)
            {
                // stop the rocket-flying cue
                if (rocketCue != null)
                {
                    rocketCue.Stop(AudioStopOptions.Immediate);
                    rocketCue.Dispose();
                    rocketCue = null;
                }

                // play the explosion cue
                world.AudioManager.PlayCue("explosionMedium");

                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.O2Color));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.O2Color));
                owner.FireGamepadMotors(0.25f, 0.25f);

                target.Die(target);
                world.UnbondOxygenTwo(target.Position, target.Velocity, target.Direction);

                return base.Touch(target);
            }
            else if ((target is Nitrogen) == true ||
                (target is Oxygen) == true ||
                (target is Hydrogen) == true ||
                (target is Bromine) == true ||
                (target is Chlorine) == true ||
                (target is Carbon) == true ||
                (target is Fluorine) == true ||
                (target is NanoBot) == true)
            {
                // add a double-particle system effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                    0.3f * world.ResVar, 0.025f * world.ResVar, world.AMBColor));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                    0.4f * world.ResVar, 0.05f * world.ResVar, world.AMBColor));
                owner.FireGamepadMotors(0.0f, 0.25f);

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

            // get and play the rocket-flying cue
            rocketCue = world.AudioManager.GetCue("rocket");
            if (rocketCue != null)
            {
                rocketCue.Play();
            }
            // twitch the motors of the launching ship
            owner.FireGamepadMotors(0f, 0.25f);
        }

        /// <summary>
        /// Damages all actors in a radius around the rocket.
        /// </summary>
        /// <param name="touchedActor">The actor that was originally hit.</param>
        public override void Explode(Actor touchedActor)
        {
            // stop the rocket-flying cue
            if (rocketCue != null)
            {
                rocketCue.Stop(AudioStopOptions.Immediate);
                rocketCue.Dispose();
                rocketCue = null;
            }

            // play the explosion cue
            world.AudioManager.PlayCue("explosionMedium");

            // add a double-particle system effect
            world.ParticleSystems.Add(new ParticleSystem(this.position,
                Vector2.Zero, 32, 16f * world.ResVar, 32f * world.ResVar, 
                0.3f * world.ResVar, 0.025f * world.ResVar, world.AMBColor));
            world.ParticleSystems.Add(new ParticleSystem(this.position,
                Vector2.Zero, 8, 32f * world.ResVar, 64f * world.ResVar, 
                0.4f * world.ResVar, 0.05f * world.ResVar, world.AMBColor));


            // twitch the motors of the launching ship
            owner.FireGamepadMotors(0.15f, 0.25f);

            base.Explode(touchedActor);
        }
        #endregion
    }
}

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay.Weaponary;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.JointMolecules;
using FreeRadicals.Gameplay.GreenhouseGases;
using FreeRadicals.Gameplay.FreeRadicals;
#endregion

namespace FreeRadicals.Gameplay
{
    /// <summary>
    /// The NanoBot, which is the primary playing-piece in the game.
    /// </summary>
    class NanoBot : Actor
    {
        #region Constants
        /// <summary>
        /// The value of the spawn timer set when the ship dies.
        /// </summary>
        const float respawnTimerOnDeath = 5f;

        /// <summary>
        /// How long, in seconds, for the ship to fade in.
        /// </summary>
        const float fadeInTimerMaximum = 0.5f;

        /// <summary>
        /// The maximum value of the "safe" timer.
        /// </summary>
        const float safeTimerMaximum = 4f;

        /// <summary>
        /// The amount of drag applied to velocity per second, 
        /// as a percentage of velocity.
        /// </summary>
        const float dragPerSecond = 1f;

        /// <summary>
        /// Scalar for calculated damage values that 
        /// oxygen atoms apply to player.
        /// </summary>
        const float damageScalar = 0.002f;

        /// <summary>
        /// The amount that the right-stick must be pressed to fire, squared so that
        /// we can use LengthSquared instead of Length, which has a square-root in it.
        /// </summary>
        const float fireThresholdSquared = 0.25f;

        /// <summary>
        /// The number of radians that the ship can turn in a second at full left-stick.
        /// </summary>
        const float rotationRadiansPerSecond = 6f;

        /// <summary>
        /// The maximum length of the velocity vector on a ship.
        /// </summary>
        const float velocityLengthMaximum = 320f;

        /// <summary>
        /// The maximum strength of the shield.
        /// </summary>
        const float shieldMaximum = 100f;

        /// <summary>
        /// How much the shield recharges per second.
        /// </summary>
        const float shieldRechargePerSecond = 50f;
        const float innerRechargePerSecond = 50f;

        /// <summary>
        /// The duration of the shield-recharge timer when the ship is hit.
        /// </summary>
        const float shieldRechargeTimerOnDamage = 2.5f;
        const float innerRechargeTimerOnDamage = 2.5f;

        /// <summary>
        /// The amount at which to vibrate the large motor if the timer is active.
        /// </summary>
        const float largeMotorSpeed = 0.5f;

        /// <summary>
        /// The amount at which to vibrate the small motor if the timer is active.
        /// </summary>
        const float smallMotorSpeed = 0.5f;

        /// <summary>
        /// The amount of time that the A button must be held to join the game.
        /// </summary>
        const float aButtonHeldToPlay = 2f;

        /// <summary>
        /// The amount of time that the B button must be held to leave the game.
        /// </summary>
        const float bButtonHeldToLeave = 5f;
        const float xButtonHeldToLeave = 0.5f;

        /// <summary>
        /// The number of radians that the shield rotates per second.
        /// </summary>
        const float shieldRotationPeriodPerSecond = 2f;
        const float innerRotationPeriodPerSecond = 2f;

        /// <summary>
        /// The relationship between the shield rotation and it's scale.
        /// </summary>
        const float shieldRotationToScaleScalar = 0.025f;
        const float innerRotationToScaleScalar = 0.025f;

        /// <summary>
        /// The relationship between the shield rotation and it's scale period.
        /// </summary>
        const float shieldRotationToScalePeriodScalar = 4f;
        const float innerRotationToScalePeriodScalar = 4f;

        /// <summary>
        /// The colors used for each nanobot, given it's player-index.
        /// </summary>
        static readonly Color[] nanobotColorsByPlayerIndex = 
        {
            Color.Yellow, Color.CornflowerBlue, Color.DeepPink, Color.Indigo
        };
        static readonly Color[] CO2Color = 
        {
            Color.Gray, Color.Red
        };
        static readonly Color[] CH4Color = 
        {
            Color.Gray, Color.Yellow
        };
        static readonly Color[] H2OColor = 
        {
            Color.Red, Color.Yellow
        };
        static readonly Color[] N2OColor = 
        {
            Color.Red, Color.Blue
        };

        /// <summary>
        /// Particle system colors for the ship-explosion effect.
        /// </summary>
        static readonly Color[] explosionColors = 
            { 
                Color.Red, Color.Green, Color.Yellow, 
                Color.LightSalmon, Color.LightGreen, Color.LightYellow, 
                Color.CornflowerBlue, Color.DeepPink, Color.Indigo
            };
        #endregion

        #region Fields
        /// <summary>
        /// A second polygon for rendering the power-up.
        /// </summary>
        private VectorPolygon innerPolygon;

        /// <summary>
        /// If true, this ship is active in-game.
        /// </summary>
        private bool playing = false;

        /// <summary>
        /// The current score for this ship.
        /// </summary>
        private int score = 0;

        /// <summary>
        /// The speed at which the ship moves.
        /// </summary>
        private float speed = 750f;

        /// <summary>
        /// The strength of the shield.
        /// </summary>
        private float shield = 0f;

        /// <summary>
        /// The rotation of the shield effect.
        /// </summary>
        private float shieldRotation = 0f;
        private float innerRotation = 0f;

        /// <summary>
        /// The polygon used to render the shield effect
        /// </summary>
        private VectorPolygon shieldPolygon = null;

        /// <summary>
        /// The NanBot's main weapon.
        /// </summary>
        private AtomicMoleBlast weapon = null;

        /// <summary>
        /// The nanobot's Hydrogen Boost Weapon.
        /// </summary>
        private HydrogenBoost hydrogenBoostWeapon = null;
        
        /// <summary>
        /// The Gamepad player index that is controlling this ship.
        /// </summary>
        private PlayerIndex playerIndex;

        /// <summary>
        /// The current state of the Gamepad that is controlling this ship.
        /// </summary>
        private GamePadState currentGamePadState;

        /// <summary>
        /// The previous state of the Gamepad that is controlling this ship.
        /// </summary>
        private GamePadState lastGamePadState;

        /// <summary>
        /// The current state of the Keyboard that is controlling this ship.
        /// </summary>
        private KeyboardState currentKeyboardState;

        /// <summary>
        /// The previous state of the Keyboard that is controlling this ship.
        /// </summary>
        private KeyboardState lastKeyboardState;

        /// <summary>
        /// Timer for how long the player has been holding the A button (to join).
        /// </summary>
        private float aButtonTimer = 0f;

        /// <summary>
        /// Timer for how long the player has been holding the B button (to leave).
        /// </summary>
        private float bButtonTimer = 0f;

        /// <summary>
        /// Timer for how much longer to vibrate the small motor.
        /// </summary>
        private float smallMotorTimer = 0f;

        /// <summary>
        /// Timer for how much longer to vibrate the large motor.
        /// </summary>
        private float largeMotorTimer = 0f;

        /// <summary>
        /// Timer for how much longer the player has to wait for the ship to respawn.
        /// </summary>
        private float respawnTimer = 0f;

        /// <summary>
        /// Timer for how long until the shield starts to recharge.
        /// </summary>
        private float shieldRechargeTimer = 0f;

        /// <summary>
        /// Timer for how long the player is safe after spawning.
        /// </summary>
        private float safeTimer = 0f;

        /// <summary>
        /// Timer for how long the player has been spawned for, to fade in
        /// </summary>
        private float fadeInTimer = 0f;

        /// <summary>
        /// Negavtive Charge nano bot applied to hual
        /// </summary>
        public bool negativeCharge = false;

        /// <summary>
        /// Positive Charge nano bot applied to hual
        /// </summary>
        public bool positiveCharge = false;

        private int oxygenAmmo = 20;
        private int hydrogenAmmo = 100;
        private int carbonAmmo = 60;

        #endregion

        #region Properties
        public int OxygenAmmo
        {
            get { return oxygenAmmo; }
        }

        public int HydrogenAmmo
        {
            get { return hydrogenAmmo; }
        }

        public int CarbonAmmo
        {
            get { return carbonAmmo; }
        }

        public bool Playing
        {
            get { return playing; }
        }

        public bool Safe
        {
            get { return (safeTimer > 0f); }
            set
            {
                if (value)
                {
                    safeTimer = safeTimerMaximum;
                }
                else
                {
                    safeTimer = 0f;
                }
            }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new ship, for the given player.
        /// </summary>
        /// <param name="world">The world that this ship belongs to.</param>
        /// <param name="playerIndex">
        /// The Gamepad player index that controls this ship.
        /// </param>
        public NanoBot(World world, PlayerIndex playerIndex)
            : base(world)
        {
            this.playerIndex = playerIndex;

            // Collision Radius (Radius * 10)
            this.collisionRadius = this.radius * 30;
            this.radius = 60f;
            this.mass = 50f;
            this.color = nanobotColorsByPlayerIndex[(int)this.playerIndex];
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, 60f, 100);
            this.innerPolygon = VectorPolygon.CreateCircle(Vector2.Zero, 30f, 30);
            this.shieldPolygon = VectorPolygon.CreateCircle(Vector2.Zero, 100f, 100);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the ship.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public override void Update(float elapsedTime)
        {
            // process all input
            ProcessInput(elapsedTime, false);

            // if this player isn't in the game, then quit now
            if (playing == false)
            {
                return;
            }

            if (dead == true)
            {
            // if we've died, then we're counting down to respawning
                if (respawnTimer > 0f)
                {
                    respawnTimer = Math.Max(respawnTimer - elapsedTime, 0f);
                }
                if (respawnTimer <= 0f)
                {
                    Spawn(true);
                }
            }
            else
            {
                // apply drag to the velocity
                velocity -= velocity * (elapsedTime * dragPerSecond);
                if (velocity.LengthSquared() <= 0f)
                {
                    velocity = Vector2.Zero;
                }
                // decrement the heal timer if necessary
                if (shieldRechargeTimer > 0f)
                {
                    shieldRechargeTimer = Math.Max(shieldRechargeTimer - elapsedTime, 
                        0f);
                }
                // recharge the shields if the timer has come up
                if (shieldRechargeTimer <= 0f)
                {
                    if (shield < 100f)
                    {
                        shield = Math.Min(100f,
                            shield + shieldRechargePerSecond * elapsedTime);
                    }
                }
            }

            // update the weapons
            if (weapon != null)
            {
                weapon.Update(elapsedTime);
            }
            if (hydrogenBoostWeapon != null)
            {
                hydrogenBoostWeapon.Update(elapsedTime);
            }

            // decrement the safe timer
            if (safeTimer > 0f)
            {
                safeTimer = Math.Max(safeTimer - elapsedTime, 0f);
            }

            // update the radius based on the shield
            radius = (shield > 0f) ? 90f : 60f;

            // update the spawn-in timer
            if (fadeInTimer < fadeInTimerMaximum)
            {
                fadeInTimer = Math.Min(fadeInTimer + elapsedTime,
                    fadeInTimerMaximum);
            }

            // update and apply the vibration
            smallMotorTimer -= elapsedTime;
            largeMotorTimer -= elapsedTime;
            GamePad.SetVibration(playerIndex,
                (largeMotorTimer > 0f) ? largeMotorSpeed : 0f,
                (smallMotorTimer > 0f) ? smallMotorSpeed : 0f);

            if (negativeCharge)
            {
                this.speed += this.speed + 500f;
                this.mass += this.mass + 100f;

                // check if there is a NanoBot
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NanoBot && world.Actors[i] != this) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }

                //// check if there is a Chlorine
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Chlorine) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.01f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is a Fluorine
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Fluorine) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.01f;
                //            return;
                //        }
                //    }
                //}

                // check if there is a Nitric Oxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NitricOxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is a Hydroxyl
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Hydroxyl) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an CFC1
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CFC1) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an CFC2
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CFC2) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                //// check if there is an Nitrogen
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Nitrogen) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Methylene
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Methylene) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Carbon
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Carbon) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Deuterium
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Deuterium) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Hydrogen
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Hydrogen) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Oxygen
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Oxygen) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an OxygenTwo
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is OxygenTwo) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an NitrogenTwo
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is NitrogenTwo) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity += -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                // check if there is an CarbonDioxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CarbonDioxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Water
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Water) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Ozone
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Ozone) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an NitrousOxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NitrousOxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Methane
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Methane) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity += -distance * 0.02f;
                            return;
                        }
                    }
                }
            }
            else if (positiveCharge)
            {
                this.speed += this.speed + 500f;
                this.mass += this.mass + 100f;

                // check if there is a NanoBot
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NanoBot && world.Actors[i] != this) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }
                
                //// check if there is an Nitrogen
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Nitrogen) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Methylene
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Methylene) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Carbon
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Carbon) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Deuterium
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Deuterium) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an Hydrogen
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is Hydrogen) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                // check if there is an Oxygen
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Oxygen) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                //// check if there is an OxygenTwo
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is OxygenTwo) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                //// check if there is an NitrogenTwo
                //for (int i = 0; i < world.Actors.Count; ++i)
                //{
                //    if ((world.Actors[i] is NitrogenTwo) == true)
                //    {
                //        Vector2 distance = this.position - world.Actors[i].Position;
                //        if (distance.Length() <= this.collisionRadius)
                //        {
                //            world.Actors[i].Velocity -= -distance * 0.02f;
                //            return;
                //        }
                //    }
                //}

                // check if there is an CarbonDioxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CarbonDioxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Water
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Water) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Ozone
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Ozone) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an NitrousOxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NitrousOxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an Methane
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Methane) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }
            }
            else
            {
                this.speed += this.speed + 750f;
                this.mass += this.mass + 50f;

                // check if there is a Nitric Oxide
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is NitricOxide) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is a Hydroxyl
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is Hydroxyl) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an CFC1
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CFC1) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }

                // check if there is an CFC2
                for (int i = 0; i < world.Actors.Count; ++i)
                {
                    if ((world.Actors[i] is CFC2) == true)
                    {
                        Vector2 distance = this.position - world.Actors[i].Position;
                        if (distance.Length() <= this.collisionRadius)
                        {
                            world.Actors[i].Velocity -= -distance * 0.02f;
                            return;
                        }
                    }
                }
            }

            base.Update(elapsedTime);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Render the ship.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        /// <param name="lineBatch">The LineBatch to render to.</param>
        public override void Draw(float elapsedTime, LineBatch lineBatch)
        {
            // if the ship isn't in the game, or it's dead, don't draw
            if ((playing == false) || (dead == true))
            {
                return;
            }

            // Save old color
            Color oldColor = nanobotColorsByPlayerIndex[(int)this.playerIndex];

            if (negativeCharge)
            {
                this.speed = 1000f;
                this.mass = 100f;
                // update the shield rotation
                shieldRotation += elapsedTime * shieldRotationPeriodPerSecond;
                // calculate the current color
                color = Color.Red;
                // transform the shield polygon
                Matrix translationMatrix = Matrix.CreateTranslation(position.X,
                    position.Y, 0f);
                shieldPolygon.Transform(Matrix.CreateScale(1f + shieldRotationToScaleScalar
                    * (float)Math.Cos(shieldRotation * shieldRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(shieldRotation) * translationMatrix);
                // draw the shield
                if (Safe)
                {
                    lineBatch.DrawPolygon(shieldPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(shieldPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);


                // update the inner ploygon rotation
                innerRotation += elapsedTime * -innerRotationPeriodPerSecond;
                // transform the shield polygon
                innerPolygon.Transform(Matrix.CreateScale(1f + innerRotationToScaleScalar
                    * (float)Math.Cos(innerRotation * innerRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(innerRotation) * translationMatrix);
                // draw the inner ploygon
                if (Safe)
                {
                    lineBatch.DrawPolygon(innerPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(innerPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);

            }
            else if (positiveCharge)
            {
                this.speed = 1000f;
                this.mass = 100f;
                // update the shield rotation
                shieldRotation += elapsedTime * -shieldRotationPeriodPerSecond;
                // calculate the current color
                color = Color.Lime;
                // transform the shield polygon
                Matrix translationMatrix = Matrix.CreateTranslation(position.X,
                    position.Y, 0f);
                shieldPolygon.Transform(Matrix.CreateScale(1f + shieldRotationToScaleScalar
                    * (float)Math.Cos(shieldRotation * shieldRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(shieldRotation) * translationMatrix);
                // draw the shield
                if (Safe)
                {
                    lineBatch.DrawPolygon(shieldPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(shieldPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);


                // update the inner ploygon rotation
                innerRotation += elapsedTime * innerRotationPeriodPerSecond;
                innerPolygon.Transform(Matrix.CreateScale(1f + innerRotationToScaleScalar
                    * (float)Math.Cos(innerRotation * innerRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(innerRotation) * translationMatrix);
                // draw the inner ploygon
                if (Safe)
                {
                    lineBatch.DrawPolygon(innerPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(innerPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);

            }
            else
            {
                this.speed = 500f;
                this.mass = 50f;
                // update the shield rotation
                shieldRotation += 0;// elapsedTime* shieldRotationPeriodPerSecond;
                // calculate the current color
                this.color = oldColor;
                //color = new Color(color.R, color.G, color.B, (byte)(255f * fadeInTimer /
                //    fadeInTimerMaximum));
                // transform the shield polygon
                Matrix translationMatrix = Matrix.CreateTranslation(position.X,
                    position.Y, 0f);
                shieldPolygon.Transform(Matrix.CreateScale(1f + shieldRotationToScaleScalar
                    * (float)Math.Cos(shieldRotation * shieldRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(shieldRotation) * translationMatrix);
                //draw the shield
                if (Safe)
                {
                    lineBatch.DrawPolygon(shieldPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(shieldPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);


                // update the inner ploygon rotation
                innerRotation += 0;// elapsedTime * -innerRotationPeriodPerSecond;
                // transform the shield polygon
                innerPolygon.Transform(Matrix.CreateScale(1f + innerRotationToScaleScalar
                    * (float)Math.Cos(innerRotation * innerRotationToScalePeriodScalar)) *
                    Matrix.CreateRotationZ(innerRotation) * translationMatrix);
                // draw the inner ploygon
                if (Safe)
                {
                    lineBatch.DrawPolygon(innerPolygon, color);
                }
                else if (shield > 0f)
                {
                    lineBatch.DrawPolygon(innerPolygon, new Color(color.R, color.G,
                        color.B, (byte)(255f * shield / shieldMaximum)), true);
                }
                base.Draw(elapsedTime, lineBatch);
            }
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Set the ship up to join the game, if it's not in it already.
        /// </summary>
        public void JoinGame()
        {
            if (playing == false)
            {
                playing = true;
                score = 0;
                Spawn(true);
            }
        }


        /// <summary>
        /// Remove the ship from the game, if it's in it.
        /// </summary>
        public void LeaveGame()
        {
            if (playing == true)
            {
                playing = false;
                Die(null);
            }
        }


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
            if ((player != null && player != this) == true)
            {
                // calculate damage as a function of how much the two actor's
                // velocities were going towards one another
                Vector2 playerAsteroidVector =
                    Vector2.Normalize(this.position - player.Position);
                float rammingSpeed =
                    Vector2.Dot(playerAsteroidVector, player.Velocity) -
                    Vector2.Dot(playerAsteroidVector, this.velocity);
                player.Damage(this, this.mass * rammingSpeed * damageScalar);

            }

            if (negativeCharge)
            {
                world.ParticleSystems.Add(new ParticleSystem(target.Position,
                    this.velocity * 0.1f, 24, 32f, 64f, 2f, 0.05f, Color.Red));
            }
            else if (positiveCharge)
            {
                world.ParticleSystems.Add(new ParticleSystem(target.Position,
                    this.velocity * 0.1f, 24, 32f, 64f, 2f, 0.05f, Color.Lime));
            }
            else
            {
                world.ParticleSystems.Add(new ParticleSystem(target.Position,
                    this.velocity * 0.1f, 24, 32f, 64f, 2f, 0.05f, this.color));
            }

            if (positiveCharge)
            {
                // Touches Oxygen
                if ((target is Oxygen) == true)
                {
                    this.oxygenAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.Red));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                }

                // Touches Hydrogen
                if ((target is Hydrogen) == true)
                {
                    this.hydrogenAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.Yellow));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                }

                // Touches Carbon
                if ((target is Carbon) == true)
                {
                    this.carbonAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.Yellow));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                }

                //// Touches OxygenTwo
                //if ((target is OxygenTwo) == true)
                //{
                //    this.oxygenAmmo += this.oxygenAmmo + 2;
                //    target.Die(target);
                //    Vector2 newPosition = (target.Position + this.position) / 2;
                //    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                //    Vector2 newDirection = (target.Direction + this.direction) / 2;
                //    //world.BondWater(newPosition, newVelocity, newDirection);
                //    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                //        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.Red));
                //    this.world.AudioManager.PlayCue("asteroidTouch");
                //}

                // Touches CarbonDioxide
                if ((target is CarbonDioxide) == true)
                {
                    this.oxygenAmmo += 2;
                    this.carbonAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, CO2Color));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                }

                // Touches Water
                if ((target is Water) == true)
                {
                    this.hydrogenAmmo += 2;
                    this.oxygenAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, H2OColor));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                }

                //// Touches Ozone
                //if ((target is Ozone) == true)
                //{
                //    this.oxygenAmmo += this.oxygenAmmo + 3;
                //    target.Die(target);
                //    Vector2 newPosition = (target.Position + this.position) / 2;
                //    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                //    Vector2 newDirection = (target.Direction + this.direction) / 2;
                //    //world.BondWater(newPosition, newVelocity, newDirection);
                //    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                //        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.Red));
                //    this.world.AudioManager.PlayCue("asteroidTouch");
                //}

                // Touches NitrousOxide
                if ((target is NitrousOxide) == true)
                {
                    this.oxygenAmmo += 1;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, N2OColor));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                    
                    NitrogenTwo nitrogenTwo = new NitrogenTwo(world);
                    nitrogenTwo.Spawn(false);
                    nitrogenTwo.Position = target.Position;
                    nitrogenTwo.Velocity = newVelocity;
                    nitrogenTwo.Direction = newDirection;
                    world.ParticleSystems.Add(new ParticleSystem(target.Position,
                        newDirection, 18, 32f, 64f, 1.5f, 0.05f, Color.Blue));
                }

                // Touches Methane
                if ((target is Methane) == true)
                {
                    this.carbonAmmo += 1;
                    this.hydrogenAmmo += 4;
                    target.Die(target);
                    Vector2 newPosition = (target.Position + this.position) / 2;
                    Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                    Vector2 newDirection = (target.Direction + this.direction) / 2;
                    //world.BondWater(newPosition, newVelocity, newDirection);
                    world.ParticleSystems.Add(new ParticleSystem(newPosition,
                        newDirection, 36, 64f, 128f, 2f, 0.05f, CH4Color));
                    this.world.AudioManager.PlayCue("asteroidTouch");
                } 
            }

            return base.Touch(target);
        }


        /// <summary>
        /// Damage this ship by the amount provided.
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
            // if the safe timer hasn't yet gone off, then the ship can't be hurt
            if (safeTimer > 0f)
            {
                return false;
            }

            // tickle the gamepad vibration motors
            FireGamepadMotors(0f, 0.25f);

            // once you're hit, the shield-recharge timer starts over
            shieldRechargeTimer = 2.5f;

            // damage the shield first, then life
            if (shield <= 0f)
            {
                life -= damageAmount;
            }
            else
            {
                shield -= damageAmount;
                if (shield < 0f)
                {
                    // shield has the overflow value as a negative value, just add it
                    life += shield;
                    shield = 0f;
                }
            }

            // if the ship is out of life, it dies
            if (life < 0f)
            {
                Die(source);
            }

            return true;
        }


        /// <summary>
        /// Kills this ship, in response to the given actor.
        /// </summary>
        /// <param name="source">The actor responsible for the kill.</param>
        public override void Die(Actor source)
        {
            if (dead == false)
            {
                // hit the gamepad vibration motors
                FireGamepadMotors(0.75f, 0.25f);
                // play several explosion cues
                world.AudioManager.PlayCue("playerDeath");
                // add several particle systems for effect
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 128, 64f, 256f, 3f, 0.05f, explosionColors));
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    Vector2.Zero, 64, 256f, 1024f, 3f, 0.05f, explosionColors));
                // reset the respawning timer
                respawnTimer = respawnTimerOnDeath;

                // change the score
                NanoBot ship = source as NanoBot;
                if (ship == null)
                {
                    Projectile projectile = source as Projectile;
                    if (projectile != null)
                    {
                        ship = projectile.Owner;
                    }
                }
                if (ship != null)
                {
                    if (ship == this)
                    {
                        // reduce the score, since i blew myself up
                        ship.Score--;
                    }
                    else
                    {
                        // add score to the ship who shot this object
                        ship.Score++;
                    }
                }
                else
                {
                    // if it wasn't a ship, then this object loses score
                    this.Score--;
                }

                // ships should not be added to the garbage list, so just set dead
                dead = true;
            }
        }
        #endregion


        /// <summary>
        /// Place this ship in the world.
        /// </summary>
        /// <param name="findSpawnPoint">
        /// If true, the actor's position is changed to a valid, non-colliding point.
        /// </param>
        public override void Spawn(bool findSpawnPoint)
        {
            // do not call the base Spawn, as the actor never is added or removed when
            // dying or respawning, because we always need to be processing input
            // respawn this actor
            if (dead == true)
            {
                // I LIVE
                dead = false;
                // find a new spawn point if requested
                if (findSpawnPoint)
                {
                    position = world.FindSpawnPoint(this);
                }
                // reset the velocity
                velocity = Vector2.Zero;
                // reset the shield and life values
                life = 25f;
                shield = shieldMaximum;
                // reset the safety timers
                safeTimer = safeTimerMaximum;
                // create the default weapons
                weapon = new AtomicMoleBlast(this);
                hydrogenBoostWeapon = new HydrogenBoost(this);
                // play the ship-spawn cue
                world.AudioManager.PlayCue("playerSpawn");
                // add a particle effect at the ship's new location
                world.ParticleSystems.Add(new ParticleSystem(this.position, 
                    Vector2.Zero, 128, 128f, 64f, 3f, 0.1f, this.color)); // new Color[] { this.color }));
                // remind the player that we're spawning
                FireGamepadMotors(0.25f, 0f);
            }
        }
        #endregion

        #region Input Methods
        /// <summary>
        /// Vibrate the gamepad motors for the given period of time.
        /// </summary>
        /// <param name="largeMotorTime">The time to run the large motor.</param>
        /// <param name="smallMotorTime">The time to run the small motor.</param>
        public void FireGamepadMotors(float largeMotorTime, float smallMotorTime)
        {
            // use the maximum timer value
            this.largeMotorTimer = Math.Max(this.largeMotorTimer, largeMotorTime);
            this.smallMotorTimer = Math.Max(this.smallMotorTimer, smallMotorTime);
        }


        /// <summary>        
        /// Process the input for this ship, from the gamepad assigned to it.        
        /// </summary>        
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>  
        public virtual void ProcessInput(float elapsedTime, bool overlayPresent)        
        {            
            currentGamePadState = GamePad.GetState(playerIndex);
            currentKeyboardState = Keyboard.GetState(playerIndex);            
            if (overlayPresent == false)            
            {              
	            if (playing == false)            
	            {                  
                    // trying to join - update the a-button timer              
                    if ((currentGamePadState.Buttons.A == ButtonState.Pressed) || 
                        ( currentKeyboardState.IsKeyDown(Keys.A) && playerIndex == PlayerIndex.One ) )      
                    {                   
                        aButtonTimer += elapsedTime;     
                    }
                    else        
	                {                      
	                    aButtonTimer = 0f;           
	                }                 
	                // if the timer has exceeded the expected value, join the game       
	                if (aButtonTimer > aButtonHeldToPlay)                   
                    {                       
		                JoinGame();                
		            }              
		         }              
		         else            
			     {                
			        // check if we're trying to leave  
			        if ((currentGamePadState.Buttons.B == ButtonState.Pressed) || 
                        ( currentKeyboardState.IsKeyDown(Keys.B) && playerIndex == PlayerIndex.One ) )     
				    {                     
				        bButtonTimer += elapsedTime;       
				    }              
				    else                
				    {                     
					    bButtonTimer = 0f;         
				    }                 
					// if the timer has exceeded the expected value, leave the game 
					if (bButtonTimer > bButtonHeldToLeave)                   
					{                       
					    LeaveGame();              
					}                    
					else if (dead == false)     
					{
			            // calculate the current forward vector      
						Vector2 forward = new Vector2((float)Math.Sin(Rotation),   
						    -(float)Math.Cos(Rotation));                     
						Vector2 right = new Vector2(-forward.Y, forward.X);     
						// calculate the current left stick value               
						Vector2 leftStick = currentGamePadState.ThumbSticks.Left;   
					        leftStick.Y *= -1f;
                            if (leftStick.LengthSquared() > 0f && 
                                (currentGamePadState.Buttons.B == ButtonState.Released))          
						{                            
							Vector2 wantedForward = Vector2.Normalize(leftStick);  
							float angleDiff = (float)Math.Acos(                          
								Vector2.Dot(wantedForward, forward));                    
							float facing = (Vector2.Dot(wantedForward, right) > 0f) ?    
								1f : -1f;                        
							if (angleDiff > 0f)      
							{                    
								Rotation += Math.Min(angleDiff, facing * elapsedTime *                      
									rotationRadiansPerSecond);                         
							}                          
							// add velocity                
							Velocity += leftStick * (elapsedTime * speed);        
							if (Velocity.Length() > velocityLengthMaximum)        
							{                             
								Velocity = Vector2.Normalize(Velocity) * velocityLengthMaximum;      
							}                       
						}                      
						else if ( currentKeyboardState != null && playerIndex == PlayerIndex.One )   
						{                   
							// Rotate Left                        
							if (currentKeyboardState.IsKeyDown(Keys.Left))       
							{                       
                                Rotation -= elapsedTime * rotationRadiansPerSecond;                      
	                        }                           
	                        // Rotate Right                          
	                        if (currentKeyboardState.IsKeyDown(Keys.Right))  
		                    {                               
			                    Rotation += elapsedTime * rotationRadiansPerSecond;           
			                }                         
			                //create some velocity if the right trigger is down             
			                Vector2 shipVelocityAdd = Vector2.Zero;                        
			                //now scale our direction by how hard/long the trigger/keyboard is down  
			                if (currentKeyboardState.IsKeyDown(Keys.Up))                        
				            {                               
				                //find out what direction we should be thrusting, using rotation             
				                shipVelocityAdd.X = (float)Math.Sin(Rotation);                            
				                shipVelocityAdd.Y = (float)-Math.Cos(Rotation);                            
				                shipVelocityAdd = shipVelocityAdd / elapsedTime * MathHelper.ToRadians(9.0f);      
				            }                            
				            //finally, add this vector to our velocity.           
				            Velocity += shipVelocityAdd;           
				            // Lets drop some Mines                     
				            if (currentKeyboardState.IsKeyDown(Keys.A)) 
					        {
                                if ((this.carbonAmmo >= 1) == true)
                                {
                                    this.direction = Vector2.Normalize(forward);
                                    this.carbonAmmo -= 1;
                                    world.ParticleSystems.Add(new ParticleSystem(this.position,
                                        -this.direction * 5f, 32, 64f, 128f, 0.75f, 0.05f, explosionColors));
                                    weapon.Fire(Vector2.Normalize(this.direction));
                                    world.AudioManager.PlayCue("explosionMedium");
                                }           
					        }                           
					        // Lets drop some Mines                  
					        if (currentKeyboardState.IsKeyDown(Keys.Down))    
					        {                            
					            // fire behind the ship                        
                                if ((this.hydrogenAmmo >= 1) == true)
                                {

                                    this.direction = Vector2.Normalize(forward);// forward; 
                                    this.hydrogenAmmo -= 1;
                                    world.ParticleSystems.Add(new ParticleSystem(this.position,
                                        -this.direction * 5f, 32, 64f, 128f, 0.75f, 0.05f, explosionColors));
                                    hydrogenBoostWeapon.Fire(this.direction);
                                    world.AudioManager.PlayCue("explosionMedium");
                                    if (Velocity.Length() > 500f)
                                    {
                                        Velocity = Vector2.Normalize(Velocity) * 500f;
                                    }
                                    else
                                    {
                                        Velocity += Velocity * 2f;
                                    }
                                }                    
					        }                      
						}      

                        // Fire the Ozone molecule upwards       
                        if ((currentGamePadState.Buttons.X == ButtonState.Pressed) &&
                           (lastGamePadState.Buttons.X == ButtonState.Released))
                        {
                            if (((this.oxygenAmmo >= 3) && (this.hydrogenAmmo >= 1)) == true)
                            {
                                Ozone ozone = new Ozone(world);
                                ozone.Spawn(false);
                                ozone.Position = this.position + new Vector2(0, -130f);
                                ozone.Velocity = this.velocity;
                                ozone.Direction = Vector2.Normalize(leftStick);// forward;
                                this.oxygenAmmo -= 3;
                                world.ParticleSystems.Add(new ParticleSystem(ozone.Position + new Vector2(15f, -25f),
                                    ozone.Velocity * 1.5f, 128, 64f, 128f, 0.85f, 0.1f, ozone.Color));
                                world.AudioManager.PlayCue("playerSpawn");
                                this.hydrogenAmmo -= 1;
                                world.ParticleSystems.Add(new ParticleSystem(this.position,
                                    -Vector2.Normalize(new Vector2(0, -130f)) * 5f, 32, 64f, 128f, 0.75f, 0.05f, explosionColors));
                                hydrogenBoostWeapon.Fire(Vector2.Normalize(new Vector2(0, -130f)));
                            }
                        }

                        // Fire the Atomic Mole Blast 
                        if ((currentGamePadState.Buttons.A == ButtonState.Pressed) &&
                           (lastGamePadState.Buttons.A == ButtonState.Released))
                        {
                            if ((this.carbonAmmo >= 1) == true)
                            {
                                this.direction = Vector2.Normalize(forward); 
                                this.carbonAmmo -= 1;
                                world.ParticleSystems.Add(new ParticleSystem(this.position,
                                    -this.direction * 5f, 32, 64f, 128f, 0.75f, 0.05f, explosionColors));
                                weapon.Fire(this.direction);
                                world.AudioManager.PlayCue("explosionMedium");
                            }
                        }

                        // Fire the Hydrogen Boost 
                        if ((currentGamePadState.Buttons.B == ButtonState.Pressed) &&   
				                (lastGamePadState.Buttons.B == ButtonState.Released) &&
                                leftStick.LengthSquared() > 0f)         
					    {
                            if ((this.hydrogenAmmo >= 1) == true)
                            {

                                this.direction = Vector2.Normalize(leftStick); 
                                this.hydrogenAmmo -= 1;
                                world.ParticleSystems.Add(new ParticleSystem(this.position,
                                    -this.direction * 5f, 32, 64f, 128f, 0.75f, 0.05f, explosionColors));
                                hydrogenBoostWeapon.Fire(this.direction);
                                world.AudioManager.PlayCue("explosionMedium");
                                if (Velocity.Length() > 500f)
                                {
                                    Velocity = Vector2.Normalize(Velocity) * 500f;
                                }
                                else
                                {
                                    Velocity += Velocity * 2f;
                                }
                            }                     
                        }
                        // Apply positive or negative force to 
                        // the exterior of the nano bot
                        if (currentGamePadState.Triggers.Right != 0 || 
                            (currentKeyboardState.IsKeyDown(Keys.Space) && playerIndex == PlayerIndex.One ))
                        {
                            negativeCharge = true;
                        }
                        else if (currentGamePadState.Triggers.Right == 0 || 
                            (currentKeyboardState.IsKeyUp(Keys.Space) && playerIndex == PlayerIndex.One))
                        {
                            negativeCharge = false;
                        }
                        if (currentGamePadState.Triggers.Left != 0 || 
                            (currentKeyboardState.IsKeyDown(Keys.LeftAlt) && playerIndex == PlayerIndex.One ) )
                        {
                            positiveCharge = true;
                        }
                        else if (currentGamePadState.Triggers.Left == 0 || 
                            (currentKeyboardState.IsKeyUp(Keys.LeftAlt) && playerIndex == PlayerIndex.One))
                        {
                            positiveCharge = false;
                        }
					}             
				}        
			}              
			// update the gamepad state  
            lastGamePadState = currentGamePadState;        
	        lastKeyboardState = currentKeyboardState;      
	        return;   
        }

        #endregion
    }
}


//protected override void Draw(GameTime gameTime)
//{
//GraphicsDevice device = graphics.GraphicsDevice;

//device.Clear(Color.CornflowerBlue);

//spriteBatch.Begin();

//// draw the tank, cat and mouse...
//spriteBatch.Draw(tankTexture, tankPosition, null, Color.White,
//tankOrientation, tankTextureCenter, 1.0f, SpriteEffects.None, 0.0f);
//spriteBatch.Draw(catTexture, catPosition, null, Color.White,
//0.0f, catTextureCenter, 1.0f, SpriteEffects.None, 0.0f);
//spriteBatch.Draw(mouseTexture, mousePosition, null, Color.White,
//mouseOrientation, mouseTextureCenter, 1.0f, SpriteEffects.None, 0.0f);

//// and then draw some text showing the tank's and mouse's current state.
//// to make the text stand out more, we'll draw the text twice, once black
//// and once white, to create a drop shadow effect.
//Vector2 shadowOffset = Vector2.One;

//spriteBatch.DrawString(spriteFont, "Tank State: " + tankState.ToString(),
//new Vector2(50, 50) + shadowOffset, Color.Black);
//spriteBatch.DrawString(spriteFont, "Tank State: " + tankState.ToString(),
//new Vector2(50, 50), Color.White);

//spriteBatch.DrawString(spriteFont, "Mouse State: " + mouseState.ToString(),
//new Vector2(50, 75) + shadowOffset, Color.Black);
//spriteBatch.DrawString(spriteFont, "Mouse State: " + mouseState.ToString(),
//new Vector2(50, 75), Color.White);

//spriteBatch.End();

//base.Draw(gameTime);
//}
//thats got the tex tthing in it there
//spriteBatch.DrawString(spriteFont, "Tank State: " + tankState.ToString(),
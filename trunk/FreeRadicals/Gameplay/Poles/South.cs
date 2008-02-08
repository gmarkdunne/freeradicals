#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.JointMolecules;
using FreeRadicals.Gameplay.Weaponary;
using FreeRadicals.Gameplay.FreeRadicals;
using FreeRadicals.Gameplay.GreenhouseGases;
#endregion

namespace FreeRadicals.Gameplay.Poles
{
    /// <summary>
    /// South Pole Repels Ozone Upwards
    /// </summary>
    class South : Actor
    {
        #region Constants
        /// <summary>
        /// The ratio between the mass and the radius 
        /// of an oxygen atom.
        /// </summary>
        const float massRadiusRatio = 100f;

        /// <summary>
        /// The amount of drag applied to velocity per second, 
        /// as a percentage of velocity.
        /// </summary>
        const float dragPerSecond = 100f;
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new Pole.
        /// </summary>
        public South(World world)
            : base(world)
        {
            // Carbon Radius
            this.radius = 100.0f * world.ResVar;
            // Collision Radius (Radius * 1000)
            this.collisionRadius = this.radius * 12;
            // Carbon Color
            this.color = Color.Orange;
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

            // Mass
            this.mass = 10000000f;
            // Velocity
            this.velocity = Vector2.Zero;
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
            if (polygon != null)
            {
                if (lineBatch == null)
                {
                    throw new ArgumentNullException("lineBatch");
                }
                // create the transformation
                Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
                Matrix world = rotationMatrix *
                    Matrix.CreateTranslation(position.X, position.Y, 0f);
                // transform the polygon
                polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(polygon, color);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public override void Update(float elapsedTime)
        {
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Oxygen) == true ||
                        (world.Actors[i] is Nitrogen) == true ||
                        (world.Actors[i] is Hydrogen) == true ||
                        (world.Actors[i] is Fluorine) == true ||
                        (world.Actors[i] is Chlorine) == true ||
                        (world.Actors[i] is Carbon) == true ||
                        (world.Actors[i] is Bromine) == true ||
                        (world.Actors[i] is OxygenTwo) == true ||
                        (world.Actors[i] is Deuterium) == true ||
                        (world.Actors[i] is Methylene) == true ||
                        (world.Actors[i] is NitrogenTwo) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.0005f;
                    }
                }
                if ((world.Actors[i] is Ozone) == true ||
                        (world.Actors[i] is CFC1) == true ||
                        (world.Actors[i] is CFC2) == true ||
                        (world.Actors[i] is Hydroxyl) == true ||
                        (world.Actors[i] is NitricOxide) == true ||
                        (world.Actors[i] is Methane) == true ||
                        (world.Actors[i] is NitrousOxide) == true ||
                        (world.Actors[i] is Water) == true ||
                        (world.Actors[i] is CarbonDioxide) == true ||
                        (world.Actors[i] is Ozone))
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity += -distance * 0.001f;
                    }
                }
                if (world.Actors.Count == i)
                {
                    return;
                }
            }

            // Mass
            this.mass = 10000000f;
            // Velocity
            this.velocity = Vector2.Zero;

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
            if ((target is Oxygen) == true ||
                (target is Nitrogen) == true ||
                (target is Hydrogen) == true ||
                (target is Fluorine) == true ||
                (target is Chlorine) == true ||
                (target is Carbon) == true ||
                (target is Bromine) == true ||
                (target is OxygenTwo) == true ||
                (target is Deuterium) == true ||
                (target is Methylene) == true ||
                (target is NitrogenTwo) == true)
            {
                target.Die(target);
                return base.Touch(target);
            }
            return false;
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
            // nothing hurts atoms, nothing!
            return false;
        }
        #endregion
    }
}


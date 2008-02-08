#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay.Weaponary;
using FreeRadicals.Gameplay.JointMolecules;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.FreeRadicals;
using FreeRadicals.Gameplay.RepelPoints;
using FreeRadicals.Gameplay.Poles;
#endregion

namespace FreeRadicals.Gameplay.GreenhouseGases
{
    /// <summary>
    /// Atoms that fill the game simulation
    /// </summary>
    class Ozone : Actor
    {
        #region Constants

        /// <summary>
        /// The ratio between the mass and the radius 
        /// of an oxygen atom.
        /// </summary>
        const float massRadiusRatio = 4f;

        /// <summary>
        /// The amount of drag applied to velocity per second, 
        /// as a percentage of velocity.
        /// </summary>
        const float dragPerSecond = 0.3f;

        /// <summary>
        /// Scalar for calculated damage values that 
        /// oxygen atoms apply to player.
        /// </summary>
        const float damageScalar = 0.001f;

        /// <summary>
        /// Scalar to convert the velocity / mass 
        /// ratio into a "nice" rotational value.
        /// </summary>
        const float velocityMassRatioToRotationScalar = 0.005f;

        #endregion

        #region Fields

        /// <summary>
        /// The polygon used to render the oxygen atom
        /// </summary>
        private VectorPolygon oxygen1Polygon = null;
        private VectorPolygon oxygen2Polygon = null;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new oxygen.
        /// </summary>
        /// <param name="world">The world that this oxygen belongs to.</param>
        /// <param name="radius">The size of the oxygen.</param>
        public Ozone(World world)
            : base(world)
        {
            // Oxygen Radius
            this.radius = 16f * world.ResVar; //(15.9994);
            // Collision Radius (Radius * 10)
            this.collisionRadius = this.radius * 10;
            // all atoms are coloured according to which type they are
            this.color = Color.Red;
            // create the polygon
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, radius, 100);
            this.oxygen1Polygon = VectorPolygon.CreateCircle(new Vector2(34f * world.ResVar, 0), radius, 100);
            this.oxygen2Polygon = VectorPolygon.CreateCircle(new Vector2(0, 34f * world.ResVar), radius, 100);
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

                // draw the motion blur
                if (useMotionBlur && velocity.LengthSquared() > 1024f)
                {
                    // draw several "blur" polygons behind the real polygon
                    Vector2 backwards = Vector2.Normalize(position - lastPosition);
                    float speed = velocity.Length();
                    for (int i = 1; i < speed / 16; ++i)
                    {
                        // calculate the "blur" polygon's position
                        Vector2 blurPosition = this.position - backwards * (i * 4);
                        // calculate the transformation for the "blur" polygon
                        Matrix blurWorld = rotationMatrix *
                            Matrix.CreateTranslation(blurPosition.X, blurPosition.Y, 0);
                        // transform the polygon to the "blur" location
                        polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(polygon,
                            new Color(color.R, color.G, color.B, alpha));
                    }
                }
            }
            if (oxygen1Polygon != null)
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
                oxygen1Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(oxygen1Polygon, color);

                // draw the motion blur
                if (useMotionBlur && velocity.LengthSquared() > 1024f)
                {
                    // draw several "blur" polygons behind the real polygon
                    Vector2 backwards = Vector2.Normalize(position - lastPosition);
                    float speed = velocity.Length();
                    for (int i = 1; i < speed / 16; ++i)
                    {
                        // calculate the "blur" polygon's position
                        Vector2 blurPosition = this.position - backwards * (i * 4);
                        // calculate the transformation for the "blur" polygon
                        Matrix blurWorld = rotationMatrix *
                            Matrix.CreateTranslation(blurPosition.X, blurPosition.Y, 0);
                        // transform the polygon to the "blur" location
                        oxygen1Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(oxygen1Polygon,
                            new Color(color.R, color.G, color.B, alpha));
                    }
                }
            }
            if (oxygen2Polygon != null)
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
                oxygen2Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(oxygen2Polygon, color);

                // draw the motion blur
                if (useMotionBlur && velocity.LengthSquared() > 1024f)
                {
                    // draw several "blur" polygons behind the real polygon
                    Vector2 backwards = Vector2.Normalize(position - lastPosition);
                    float speed = velocity.Length();
                    for (int i = 1; i < speed / 16; ++i)
                    {
                        // calculate the "blur" polygon's position
                        Vector2 blurPosition = this.position - backwards * (i * 4);
                        // calculate the transformation for the "blur" polygon
                        Matrix blurWorld = rotationMatrix *
                            Matrix.CreateTranslation(blurPosition.X, blurPosition.Y, 0);
                        // transform the polygon to the "blur" location
                        oxygen2Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(oxygen2Polygon,
                            new Color(color.R, color.G, color.B, alpha));
                    }
                }
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
            // spin the oxygen based on the size and velocity
            this.rotation += (this.velocity.LengthSquared() / this.mass) * elapsedTime *
                velocityMassRatioToRotationScalar;

            // apply some drag so the asteroids settle down
            velocity -= velocity * (elapsedTime * dragPerSecond);

            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Ozone) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius / 1.75f)
                    {
                        world.Actors[i].Velocity += -distance * 0.075f;
                    }
                }
                if ((world.Actors[i] is NitrousOxide) == true ||
                    (world.Actors[i] is Water) == true ||
                    (world.Actors[i] is Methane) == true ||
                    (world.Actors[i] is CarbonDioxide) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity += -distance * 0.04f;
                    }
                }
                if ((world.Actors[i] is NitricOxide) == true ||
                    (world.Actors[i] is Hydroxyl) == true ||
                    (world.Actors[i] is CFC1) == true ||
                    (world.Actors[i] is CFC2) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.01f;
                    }
                }
                if ((world.Actors[i] is Actor) == true &&
                    (world.Actors[i] is NanoBot) != true ||
                    (world.Actors[i] is North) != true ||
                    (world.Actors[i] is South) != true ||
                    (world.Actors[i] is West) != true ||
                    (world.Actors[i] is East) != true ||
                    (world.Actors[i] is One) != true ||
                    (world.Actors[i] is Two) != true ||
                    (world.Actors[i] is Three) != true ||
                    (world.Actors[i] is Four) != true ||
                    (world.Actors[i] is Five) != true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity += -distance * 0.01f;
                    }
                }
                if (world.Actors.Count == i)
                {
                    return;
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

                return base.Touch(target);
            }

            if ((target is Hydroxyl) ||
                (target is NitricOxide) ||
                (target is CFC1) ||
                (target is CFC2) ||
                (target is Six) ||
                (target is North))
            {
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
            // nothing hurst asteroids, nothing!
            return false;
        }
        #endregion
    }
}

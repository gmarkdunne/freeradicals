#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay.GreenhouseGases;
using FreeRadicals.Gameplay.Weaponary;
using FreeRadicals.Gameplay.Poles;
using FreeRadicals.Gameplay.RepelPoints;
#endregion

namespace FreeRadicals.Gameplay.FreeRadicals
{
    /// <summary>
    /// Atoms that fill the game simulation
    /// </summary>
    class CFC2 : Actor
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
        const float velocityMassRatioToRotationScalar = -0.005f;

        #endregion

        #region Fields

        /// <summary>
        /// The polygon used to render the oxygen atom
        /// </summary>
        private VectorPolygon chlorine1Polygon = null;
        private VectorPolygon chlorine2Polygon = null;
        private VectorPolygon chlorine3Polygon = null;
        private VectorPolygon fluorine1Polygon = null;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new oxygen.
        /// </summary>
        /// <param name="world">The world that this oxygen belongs to.</param>
        /// <param name="radius">The size of the oxygen.</param>
        public CFC2(World world)
            : base(world)
        {
            // Carbon Radius
            this.radius = 25.0f * world.ResVar; //(12.0107);
            // Collision Radius (Radius * 10)
            this.collisionRadius = this.radius * 8f;
            // Carbon Color
            this.color = Color.Gray;
            // create the polygon
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, 12f * world.ResVar, 100);
            this.chlorine1Polygon = VectorPolygon.CreateCircle(new Vector2(50f * world.ResVar, 0), 35.5f * world.ResVar, 100);
            this.chlorine2Polygon = VectorPolygon.CreateCircle(new Vector2(0, 50f * world.ResVar), 35.5f * world.ResVar, 100);
            this.chlorine3Polygon = VectorPolygon.CreateCircle(new Vector2(-50f * world.ResVar, 0), 35.5f * world.ResVar, 100);
            this.fluorine1Polygon = VectorPolygon.CreateCircle(new Vector2(0, -33.5f * world.ResVar), 19f * world.ResVar, 100);
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
            if (chlorine1Polygon != null)
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
                chlorine1Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(chlorine1Polygon, Color.Green);

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
                        chlorine1Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(chlorine1Polygon,
                            new Color(Color.Green.R, Color.Green.G, Color.Green.B, alpha));
                    }
                }
            }
            if (chlorine2Polygon != null)
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
                chlorine2Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(chlorine2Polygon, Color.Green);

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
                        chlorine2Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(chlorine2Polygon,
                            new Color(Color.Green.R, Color.Green.G, Color.Green.B, alpha));
                    }
                }
            }
            if (chlorine3Polygon != null)
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
                chlorine3Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(chlorine3Polygon, Color.Green);

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
                        chlorine3Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(chlorine3Polygon,
                            new Color(Color.Green.R, Color.Green.G, Color.Green.B, alpha));
                    }
                }
            }
            if (fluorine1Polygon != null)
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
                fluorine1Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(fluorine1Polygon, Color.Purple);

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
                        fluorine1Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(fluorine1Polygon,
                            new Color(Color.Purple.R, Color.Purple.G, Color.Purple.B, alpha));
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

            // apply some drag so the atoms settle down
            velocity -= velocity * (elapsedTime * dragPerSecond);


            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Ozone) == true ||
                    (world.Actors[i] is AtomicMoleBlastProjectile) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.2f;
                    }
                }
                if ((world.Actors[i] is Actor) == true &&
                    (world.Actors[i] is AtomicMoleBlastProjectile) != true ||
                    (world.Actors[i] is Ozone) != true ||
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

                if (player.positiveCharge == true)
                {
                    player.Damage(this, this.mass * rammingSpeed * damageScalar * 2f);
                }
                else if (player.negativeCharge != true &&
                    player.positiveCharge != true)
                {
                    player.Damage(this, this.mass * rammingSpeed * damageScalar);
                }

                return base.Touch(target);
            }
            if ((target is AtomicMoleBlastProjectile) == true)
            {
                target.Die(target);
                this.Die(this);
                world.ParticleSystems.Add(new ParticleSystem(this.position,
                    this.direction, 36, 64f * world.ResVar, 128f * world.ResVar, 
                    2f * world.ResVar, 0.05f * world.ResVar, world.CFC2Color));

                return base.Touch(target);
            }
            if ((target is Ozone) == true)
            {
                target.Die(target);
                Vector2 newPosition = (target.Position + this.position) / 2;
                Vector2 newVelocity = (target.Velocity + this.velocity) / 2;
                Vector2 newDirection = (target.Direction + this.direction) / 2;
                world.UnbondOzone(newPosition, newVelocity, newDirection);
                world.ParticleSystems.Add(new ParticleSystem(target.Position,
                    target.Direction, 36, 64f * world.ResVar, 128f * world.ResVar, 
                    2f * world.ResVar, 0.05f * world.ResVar, world.O3Color));
                this.world.AudioManager.PlayCue("asteroidTouch");

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


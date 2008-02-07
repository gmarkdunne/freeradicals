#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
using FreeRadicals.BaseAI;
using FreeRadicals.Rendering;
using FreeRadicals.CustomAI;
#endregion

namespace FreeRadicals.Gameplay
{
    /// <summary>
    /// Atoms that fill the game simulation
    /// </summary>
    class Methane : Actor, IBaseAgent
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
        const float dragPerSecond = 0.02f;

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

        /// <summary>
        /// Particle system colors for the ship-explosion effect.
        /// </summary>
        static readonly Color[] CO2Colors = 
        { 
            Color.Red, Color.Gray
        };

        #endregion

        #region Fields

        /// <summary>
        /// The polygon used to render the oxygen atom
        /// </summary>
        private VectorPolygon hydrogen1Polygon = null;
        private VectorPolygon hydrogen2Polygon = null;
        private VectorPolygon hydrogen3Polygon = null;
        private VectorPolygon hydrogen4Polygon = null;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new oxygen.
        /// </summary>
        /// <param name="world">The world that this oxygen belongs to.</param>
        /// <param name="radius">The size of the oxygen.</param>
        public Methane(World world)
            : base(world)
        {
            // Carbon Radius
            this.radius = 12.0f; //(12.0107);
            // Collision Radius (Radius * 10)
            this.collisionRadius = this.radius * 10;
            // Carbon Color
            this.color = Color.Gray;
            // create the polygon
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, radius, 100);
            // create the polygon
            this.hydrogen1Polygon = VectorPolygon.CreateCircle(new Vector2(-18f, 0), 4f, 100);
            this.hydrogen2Polygon = VectorPolygon.CreateCircle(new Vector2(18f, 0), 4f, 100);
            this.hydrogen3Polygon = VectorPolygon.CreateCircle(new Vector2(0, -18f), 4f, 100);
            this.hydrogen4Polygon = VectorPolygon.CreateCircle(new Vector2(0, 18f), 4f, 100);
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

            // Set the AI agent up with an initial state.
            agent = new BasicModelAgent(this, new State_Patrol());   
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
            if (hydrogen1Polygon != null)
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
                hydrogen1Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(hydrogen1Polygon, Color.Yellow);

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
                        hydrogen1Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(hydrogen1Polygon,
                            new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, alpha));
                    }
                }
            }
            if (hydrogen2Polygon != null)
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
                hydrogen2Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(hydrogen2Polygon, Color.Yellow);

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
                        hydrogen2Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(hydrogen2Polygon,
                            new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, alpha));
                    }
                }
            }
            if (hydrogen3Polygon != null)
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
                hydrogen3Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(hydrogen3Polygon, Color.Yellow);

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
                        hydrogen3Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(hydrogen3Polygon,
                            new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, alpha));
                    }
                }
            }
            if (hydrogen4Polygon != null)
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
                hydrogen4Polygon.Transform(world);
                // draw the polygon
                lineBatch.DrawPolygon(hydrogen4Polygon, Color.Yellow);

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
                        hydrogen4Polygon.Transform(blurWorld);
                        // calculate the alpha of the "blur" location
                        byte alpha = (byte)(160 / (i + 1));
                        if (alpha < 1)
                            break;
                        // draw the "blur" polygon
                        lineBatch.DrawPolygon(hydrogen4Polygon,
                            new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, alpha));
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

            //// This is basicly the healing rate of out NPC. Each update it gets .125 hits back.
            //if (HP < 100)
            //    HP += .125f;

            // This is the AI bit, Execute the agents current state.
            agent.ExecuteState();

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

            //// Testing
            //if ((target is NanoBot) == true)
            //{
            //    this.Die(this);
            //    Vector2 newPosition = this.position;
            //    Vector2 newVelocity = this.velocity;
            //    Vector2 newDirection = this.direction;
            //    world.UnbondMethane(newPosition, newVelocity, newDirection);
            //    world.ParticleSystems.Add(new ParticleSystem(newPosition,
            //        newDirection, 36, 64f, 128f, 2f, 0.05f, Color.White));
            //    this.world.AudioManager.PlayCue("asteroidTouch");
            //}

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
            // nothing hurts atoms, nothing!
            return false;
        }
        #endregion
 
        #region Finite State Machine Methods
        /// <summary>
        /// Method called to patrol
        /// </summary>
        public void Patrol()
        {
            //// wander works just like the mouse's.
            //Wander(position, ref direction, ref orientation,
            //    turnSpeed);
            //currentSpeed = .25f * maxSpeed;
            Move(position, new Vector2(0.5f, 0.5f), currentSpeed);
        }
        /// <summary>
        /// Yes, you guessed it my fight method.
        /// </summary>
        public void Bond()
        {
            //// This is where you would put all your lovely shoot the Player and dive for cover stuff
            //// and maybe some more path finding to get a better shot on your target.

            //// Simulate that I am taking damage. 
            //HP -= .5f;
            //// Stop and fight.
            //speed = 0;
            //// Set to a threatenign color.
            //AmbientColor = Color.Red.ToVector4();

            //// Look at the Player with an angry face.... or in this case prop.
            //LookAt(Camera.myPosition, .1f);
        }
        /// <summary>
        /// Guess what this method is for??
        /// </summary>
        public void Flee()
        {
            //// Run really fast!
            //speed = .02f;

            //// Yellow belly!
            //AmbientColor = Color.Yellow.ToVector4();

            //// Probably be better to again pathfind your way out, but this is a simple tut, so just run!
            //velocity.Z -= .25f;

            //Move();
        }

        /// <summary>
        /// Simply, am I in any danger, or have I come into contact with the player in this case.
        /// </summary>
        /// <returns>true if safe else false.</returns>
        public bool isSafe()
        {
            //bool retVal = true;

            //// Am I in range of the player or are my hits too low?
            //if (Vector3.Distance(myPosition, Camera.myPosition) <= 10 || HP < 70)
            //    retVal = false;

            //return retVal;
            return false;
        }

        /// <summary>
        /// Do I think I should do a runner??
        /// </summary>
        /// <returns>true, "yes I should leave.". false "naa I am OK"</returns>
        public bool runAway()
        {
            //// If I have lost half or more of my hits, I want to leave...
            //if (HP <= 50)
            //    return true;
            //else
            //    return false;
            return false;
        }

        /// <summary>
        /// Once a certain target is found chase until it is bound.
        /// </summary>
        /// <returns>true if safe else false.</returns>
        public bool Chase()
        {
            //bool retVal = true;

            //// Am I in range of the player or are my hits too low?
            //if (Vector3.Distance(position, Camera.myPosition) <= 10 || HP < 70)
            //    retVal = false;

            //return retVal;
            return false;
        }

        /// <summary>
        /// Seek a certain target??
        /// </summary>
        /// <returns>true, "yes I should leave.". false "naa I am OK"</returns>
        public bool Seek()
        {
            //// If I have lost half or more of my hits, I want to leave...
            //if (HP <= 50)
            //    return true;
            //else
            //    return false;
            return false;
        }
        #endregion
    }
}


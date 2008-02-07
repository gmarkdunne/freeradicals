#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals.Simulation
{
    /// <summary>
    /// A base class for all active objects in the game.
    /// </summary>
    abstract class Actor
    {
        #region Fields
        /// <summary>
        /// The world which owns this actor.
        /// </summary>
        protected World world;

        // Actor Variables
        protected bool dead = true;
        protected float life = 0f;
        protected Vector2 position = Vector2.Zero;
        protected Vector2 lastPosition = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;
        protected float rotation = 0f;
        protected float orientation = 0f;
        protected float currentSpeed = 1.0f;
        protected float maxSpeed = 1.0f;
        protected float turnSpeed = 0.10f;
        protected float chaseDistance = 250.0f;
        protected float bondDistance = 60.0f;
        protected float hysteresis = 15.0f;
        protected float evadeDistance = 200.0f;
        protected Vector2 direction;

        // collision data
        protected bool collidable = true;
        protected float mass = 1f;
        protected float radius = 16f;
        protected bool collidedThisFrame = false;
        protected float collisionRadius = 16f;

        // visual data
        protected Rendering.VectorPolygon polygon = null;
        protected Color color = Color.White;
        protected bool useMotionBlur = WorldRules.MotionBlur;
        protected Random random = new Random();
        #endregion

        #region Properties
        public float Life
        {
            get { return life; }
        }

        public World World
        {
            get { return world; }
        }

        public bool Dead
        {
            get { return dead; }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                lastPosition = position;
                position = value;
            }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public float Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        public float CurrentSpeed
        {
            get { return currentSpeed; }
            set { currentSpeed = value; }
        }
        
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }

        public float TurnSpeed
        {
            get { return turnSpeed; }
            set { turnSpeed = value; }
        }

        public float ChaseDistance
        {
            get { return chaseDistance; }
            set { chaseDistance = value; }
        }

        public float BondDistance
        {
            get { return bondDistance; }
            set { bondDistance = value; }
        }

        public float Hysteresis
        {
            get { return hysteresis; }
            set { hysteresis = value; }
        }

        public float EvadeDistance
        {
            get { return evadeDistance; }
            set { evadeDistance = value; }
        }

        public bool Collidable
        {
            get { return collidable; }
        }

        public float Mass
        {
            get { return mass; }
        }

        public bool CollidedThisFrame
        {
            get { return collidedThisFrame; }
            set { collidedThisFrame = value; }
        }

        public float CollisionRadius
        {
            get { return collisionRadius; }
        }

        public float Radius
        {
            get { return radius; }
        }

        public Color Color
        {
            get { return color; }
        }

        public Random Random
        {
            get { return random; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new actor.
        /// </summary>
        /// <param name="world">The world that this actor belongs to.</param>
        public Actor(World world)
        {
            if (world == null)
            {
                throw new ArgumentNullException("world");
            }
            this.world = world;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public virtual void Update(float elapsedTime) 
        {
            collidedThisFrame = false;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Render the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        /// <param name="lineBatch">The LineBatch to render to.</param>
        public virtual void Draw(float elapsedTime, Rendering.LineBatch lineBatch)
        {
            if (polygon != null)
            {
                if (lineBatch == null)
                {
                    throw new ArgumentNullException("lineBatch");
                }
                // create the transformation
                Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
                Matrix world =  rotationMatrix *
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
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Defines the interaction between this actor and a target actor
        /// when they touch.
        /// </summary>
        /// <param name="target">The actor that is touching this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public virtual bool Touch(Actor target) 
        { 
            return true; 
        }


        /// <summary>
        /// Damage this actor by the amount provided.
        /// </summary>
        /// <remarks>
        /// This function is provided in lieu of a Life mutation property to allow 
        /// classes of objects to restrict which kinds of objects may damage them,
        /// and under what circumstances they may be damaged.
        /// </remarks>
        /// <param name="source">The actor responsible for the damage.</param>
        /// <param name="damageAmount">The amount of damage.</param>
        /// <returns>If true, this object was damaged.</returns>
        public virtual bool Damage(Actor source, float damageAmount)
        {
            // reduce life by the given amound
            life -= damageAmount;
            // if life had gone below 0, then we're dead
            // -- 0 health actors are destroyed by any damage
            if (life < 0f)
            {
                Die(source);
            }
            return true;
        }

        
        /// <summary>
        /// Kills this actor, in response to the given actor.
        /// </summary>
        /// <param name="source">The actor responsible for the kill.</param>
        public virtual void Die(Actor source) 
        {
            if (dead == false)
            {
                // arrrggghhhh
                dead = true;
                // remove this actor from the world
                world.Actors.Garbage.Add(this);
            }
        }

        
        /// <summary>
        /// Place this actor in the world.
        /// </summary>
        /// <param name="findSpawnPoint">
        /// If true, the actor's position is changed to a valid, non-colliding point.
        /// </param>
        public virtual void Spawn(bool findSpawnPoint)
        {
            // find a new spawn point if requested
            if (findSpawnPoint)
            {
                position = world.FindSpawnPoint(this);
            }
            // reset the velocity
            velocity = Vector2.Zero;

            // respawn this actor
            if (dead == true)
            {
                // I LIVE
                dead = false;
                // add this object to the world's actors list
                world.Actors.Add(this);
            }
        }
        #endregion

        #region General Actor behavior stuff
        /// <summary>
        /// Wander contains functionality that is shared between both the mouse and the
        /// tank, and does just what its name implies: makes them wander around the
        /// screen. The specifics of the function are described in more detail in the
        /// accompanying doc.
        /// </summary>
        /// <param name="position">the position of the character that is wandering
        /// </param>
        /// <param name="wanderDirection">the direction that the character is currently
        /// wandering. this parameter is passed by reference because it is an input and
        /// output parameter: Wander accepts it as input, and will update it as well.
        /// </param>
        /// <param name="orientation">the character's orientation. this parameter is
        /// also passed by reference and is an input/output parameter.</param>
        /// <param name="turnSpeed">the character's maximum turning speed.</param>
        public void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            // The wander effect is accomplished by having the character aim in a random
            // direction. Every frame, this random direction is slightly modified.
            // Finally, to keep the characters on the center of the screen, we have them
            // turn to face the screen center. The further they are from the screen
            // center, the more they will aim back towards it.

            // the first step of the wander behavior is to use the random number
            // generator to offset the current wanderDirection by some random amount.
            // .25 is a bit of a magic number, but it controls how erratic the wander
            // behavior is. Larger numbers will make the characters "wobble" more,
            // smaller numbers will make them more stable. we want just enough
            // wobbliness to be interesting without looking odd.
            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

            // we'll renormalize the wander direction, ...
            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            // ... and then turn to face in the wander direction. We don't turn at the
            // maximum turning speed, but at 15% of it. Again, this is a bit of a magic
            // number: it works well for this sample, but feel free to tweak it.
            orientation = Turn(position, position + wanderDirection, orientation,
                .15f * turnSpeed);


            // next, we'll turn the characters back towards the center of the screen, to
            // prevent them from getting stuck on the edges of the screen.
            Vector2 screenCenter = new Vector2(0.5f, 0.5f);

            // Here we are creating a curve that we can apply to the turnSpeed. This
            // curve will make it so that if we are close to the center of the screen,
            // we won't turn very much. However, the further we are from the screen
            // center, the more we turn. At most, we will turn at 30% of our maximum
            // turn speed. This too is a "magic number" which works well for the sample.
            // Feel free to play around with this one as well: smaller values will make
            // the characters explore further away from the center, but they may get
            // stuck on the walls. Larger numbers will hold the characters to center of
            // the screen. If the number is too large, the characters may end up
            // "orbiting" the center.
            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            // once we've calculated how much we want to turn towards the center, we can
            // use the TurnToFace function to actually do the work.
            orientation = Turn(position, screenCenter, orientation,
                turnToCenterSpeed);
        }


        /// <summary>
        /// Calculates the distance that an object should move, given its position, its
        /// target's position, maximum moving speed.
        /// </summary>
        public Vector2 Move(Vector2 oldPosition, Vector2 newPosition, float moveSpeed)
        {
            if (oldPosition != newPosition)
            {
                float oldDifference = newPosition.X - oldPosition.X;

                float difference = MathHelper.Clamp(oldDifference, -moveSpeed, moveSpeed);

                oldPosition += oldPosition * difference;
            }
            return oldPosition;
        }


        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float Turn(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         B 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // A--------
            //     x
            // 
            // where A is the position of the object, B is the position of the target,
            // and "o" is the angle that the object should be facing in order to 
            // point at the target. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        #endregion
    }
}

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals
{
    /// <summary>
    /// Owns all game state and executes all game-wide logic.
    /// </summary>
    class World
    {
        #region Constants
        /// <summary>
        /// The number of seconds before the first power-up appears in a game.
        /// </summary>
        const float initialPowerUpDelay = 10f;

        /// <summary>
        /// The time between each power-up spawn.
        /// </summary>
        const float powerUpDelay = 20f;

        /// <summary>
        /// The number of stars to generate in the starfield.
        /// </summary>
        const int atomCount = 5000;

        /// <summary>
        /// How far atmosphere should generate outside the dimensions of the game field.
        /// </summary>
        const int atmosphereBuffer = 512;
        #endregion

        #region Fields
        // Oxeygen Variables
        int deadO = 0;
        int deadH = 0;
        int deadN = 0;

        // Create a Random Number
        Random random = new Random();

        /// <summary>
        /// The dimensions of the game board.
        /// </summary>
        Vector2 dimensions;

        /// <summary>
        /// The safe dimensions of the game board.
        /// </summary>
        Rectangle safeDimensions;

        /// <summary>
        /// The timer to see if another power-up can arrive.
        /// </summary>
        float powerUpTimer;

        /// <summary>
        /// The audio manager that all objects in the world will use.
        /// </summary>
        private AudioManager audioManager;

        /// <summary>
        /// All ships that might enter the game.
        /// </summary>
        Gameplay.NanoBot[] ships;

        /// <summary>
        /// The walls in the game.
        /// </summary>
        Vector2[] walls;

        /// <summary>
        /// The Atmosphere effect behind the game-board.
        /// </summary>
        Atmosphere atmosphere;

        /// <summary>
        /// All actors in the game.
        /// </summary>
        CollectCollection<Actor> actors;

        /// <summary>
        /// All particle-systems in the game.
        /// </summary>
        CollectCollection<ParticleSystem> particleSystems;

        /// <summary>
        /// Cached list of collision results, for more optimal collision detection.
        /// </summary>
        List<CollisionResult> collisionResults = new List<CollisionResult>();
        #endregion

        #region Properties
        public int DeadO
        {
            get { return deadO; }
        }

        public int DeadH
        {
            get { return deadH; }
        }

        public int DeadN
        {
            get { return deadN; }
        }

        public AudioManager AudioManager
        {
            get { return audioManager; }
            set { audioManager = value; }
        }

        public Atmosphere Atmosphere
        {
            get { return atmosphere; }
        }

        public Gameplay.NanoBot[] Ships
        {
            get { return ships; }
        }

        public CollectCollection<Actor> Actors
        {
            get { return actors; }
        }

        public CollectCollection<ParticleSystem> ParticleSystems
        {
            get { return particleSystems; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new World object, holding the game simulation.
        /// </summary>
        public World(Vector2 dimensions)
        {
            this.dimensions = dimensions;
            safeDimensions = new Rectangle(
                (int)(dimensions.X * 0.05f), (int)(dimensions.Y * 0.05f), 
                (int)(dimensions.X * 0.90f), (int)(dimensions.Y * 0.90f));
            // create the players
            ships = new Gameplay.NanoBot[4];
            ships[0] = new Gameplay.NanoBot(this, PlayerIndex.One);
            ships[1] = new Gameplay.NanoBot(this, PlayerIndex.Two);
            ships[2] = new Gameplay.NanoBot(this, PlayerIndex.Three);
            ships[3] = new Gameplay.NanoBot(this, PlayerIndex.Four);

            // create the Atmosphere
            atmosphere = new Atmosphere(atomCount, new Rectangle(
                atmosphereBuffer * -1,
                atmosphereBuffer  * -1,
                (int)this.dimensions.X + atmosphereBuffer * 2,
                (int)this.dimensions.Y + atmosphereBuffer * 2));

            // create a new list of actors
            actors = new CollectCollection<Actor>(this);

            // create a new list of particle systems
            particleSystems = new CollectCollection<ParticleSystem>(this);
        }

        #endregion

        #region New Game
        public void StartNewGame()
        {
            //// create the walls
            CreateWalls();

            // clear out the actors list
            actors.Clear();
            // add the world actor
            WorldActor worldActor = new WorldActor(this);
            actors.Add(worldActor);
            // add the players to the actor list - they won't be removed
            for (int i = 0; i < ships.Length; i++)
            {
                actors.Add(ships[i]);
            }

            // spawn atoms density
            switch (WorldRules.AtomDensity)
            {
                case AtomDensity.None:
                    SpawnAtoms(0, 0, 0, 0, 0, 0, 0);
                    SpawnFreeRadicals(1, 1, 1, 1);
                    SpawnGreenHouseGases(0, 0, 0, 0, 0);
                    SpawnJointMolecules(0, 0, 0, 0);
                    break;
                case AtomDensity.Low:
                    SpawnAtoms(30, 22, 6, 50, 10, 10, 1);
                    break;
                case AtomDensity.Medium:
                    SpawnAtoms(60, 44, 12, 100, 20, 20, 1);
                    break;
                case AtomDensity.High:
                    SpawnAtoms(120, 88, 24, 200, 40, 40, 1);
                    break;
            }

            // set up the power-up timer for the initial delay
            powerUpTimer = initialPowerUpDelay;

            // set up the atmosphere
            atmosphere.SetTargetPosition(dimensions * 0.5f);
        }


        /// <summary>
        /// Initialize the walls based on the current world rules.
        /// </summary>
        private void CreateWalls()
        {
            switch (WorldRules.WallStyle)
            {
                case WallStyle.None:
                    walls = new Vector2[8];
                    break;
                case WallStyle.One:
                    walls = new Vector2[10];
                    break;
                case WallStyle.Two:
                    walls = new Vector2[12];
                    break;
                case WallStyle.Three:
                    walls = new Vector2[14];
                    break;
            }

            // The outer boundaries
            walls[0] = new Vector2(safeDimensions.X, safeDimensions.Y);
            walls[1] = new Vector2(safeDimensions.X,
                safeDimensions.Y + safeDimensions.Height);
            walls[2] = new Vector2(safeDimensions.X + safeDimensions.Width,
                safeDimensions.Y);
            walls[3] = new Vector2(safeDimensions.X + safeDimensions.Width,
                safeDimensions.Y + safeDimensions.Height);
            walls[4] = new Vector2(safeDimensions.X, safeDimensions.Y);
            walls[5] = new Vector2(safeDimensions.X + safeDimensions.Width,
                safeDimensions.Y);
            walls[6] = new Vector2(safeDimensions.X,
                safeDimensions.Y + safeDimensions.Height);
            walls[7] = new Vector2(safeDimensions.X + safeDimensions.Width,
                safeDimensions.Y + safeDimensions.Height);

            int quarterX = safeDimensions.Width / 4;
            int quarterY = safeDimensions.Height / 4;
            int halfY = safeDimensions.Height / 2;

            switch (WorldRules.WallStyle)
            {
                case WallStyle.One:
                    // Cross line
                    walls[8] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + halfY);
                    walls[9] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + halfY);
                    break;
                case WallStyle.Two:
                    walls[8] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + quarterY);
                    walls[9] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + 3 * quarterY);
                    walls[10] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + quarterY);
                    walls[11] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + 3 * quarterY);
                    break;
                case WallStyle.Three:
                    // Cross line
                    walls[8] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + halfY);
                    walls[9] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + halfY);
                    walls[10] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + quarterY);
                    walls[11] = new Vector2(safeDimensions.X + quarterX,
                        safeDimensions.Y + 3 * quarterY);
                    walls[12] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + quarterY);
                    walls[13] = new Vector2(safeDimensions.X + 3 * quarterX,
                        safeDimensions.Y + 3 * quarterY);
                    break;
            }
        }


        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        private void SpawnFreeRadicals(int NO, int CFCa, int CFCb, int HO)
        {
            // create Nitric Oxide atoms
            for (int i = 0; i < NO; ++i)
            {
                Gameplay.NitricOxide nitricOxide = new Gameplay.NitricOxide(this);
                nitricOxide.Spawn(true);
            }
            // create CFC1 atoms
            for (int i = 0; i < CFCa; ++i)
            {
                Gameplay.CFC1 cfc1 = new Gameplay.CFC1(this);
                cfc1.Spawn(true);
            }
            // create CFC2 atoms
            for (int i = 0; i < CFCb; ++i)
            {
                Gameplay.CFC2 cfc2 = new Gameplay.CFC2(this);
                cfc2.Spawn(true);
            }
            // create Hydroxyl atoms
            for (int i = 0; i < HO; ++i)
            {
                Gameplay.Hydroxyl hydroxyl = new Gameplay.Hydroxyl(this);
                hydroxyl.Spawn(true);
            }
        }


        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        private void SpawnGreenHouseGases(int O3, int H2O, int N2O, int CO2, int CH4)
        {
            // create Ozone atoms
            for (int i = 0; i < O3; ++i)
            {
                Gameplay.Ozone ozone = new Gameplay.Ozone(this);
                ozone.Spawn(true);
            }
            // create Water atoms
            for (int i = 0; i < H2O; ++i)
            {
                Gameplay.Water water = new Gameplay.Water(this);
                water.Spawn(true);
            }
            // create Nitrous Oxide atoms
            for (int i = 0; i < N2O; ++i)
            {
                Gameplay.NitrousOxide nitrousOxide = new Gameplay.NitrousOxide(this);
                nitrousOxide.Spawn(true);
            }
            // create Carbon Dioxide atoms
            for (int i = 0; i < CO2; ++i)
            {
                Gameplay.CarbonDioxide carbonDioxide = new Gameplay.CarbonDioxide(this);
                carbonDioxide.Spawn(true);
            }
            // create Methane atoms
            for (int i = 0; i < CH4; ++i)
            {
                Gameplay.Methane methane = new Gameplay.Methane(this);
                methane.Spawn(true);
            }
        }


        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        private void SpawnAtoms(int H, int C, int N, int O, int F, int Cl, int Br)
        {
            // create hydrogen atoms
            for (int i = 0; i < H; ++i)
            {
                Gameplay.Hydrogen hydrogen = new Gameplay.Hydrogen(this);
                hydrogen.Spawn(true);
            }
            // create carbon atoms
            for (int i = 0; i < C; ++i)
            {
                Gameplay.Carbon carbon = new Gameplay.Carbon(this);
                carbon.Spawn(true);
            }
            // create nitrogen atoms
            for (int i = 0; i < N; ++i)
            {
                Gameplay.Nitrogen nitrogen = new Gameplay.Nitrogen(this);
                nitrogen.Spawn(true);
            }
            // create oxygen atoms
            for (int i = 0; i < O; ++i)
            {
                Gameplay.Oxygen oxygen = new Gameplay.Oxygen(this);
                oxygen.Spawn(true);
            }
            // create fluorine atoms
            for (int i = 0; i < F; ++i)
            {
                Gameplay.Fluorine fluorine = new Gameplay.Fluorine(this);
                fluorine.Spawn(true);
            }
            // create chlorine atoms
            for (int i = 0; i < Cl; ++i)
            {
                Gameplay.Chlorine chlorine = new Gameplay.Chlorine(this);
                chlorine.Spawn(true);
            }
            // create Bromine atoms
            for (int i = 0; i < Br; ++i)
            {
                Gameplay.Bromine bromine = new Gameplay.Bromine(this);
                bromine.Spawn(true);
            }
        }

        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        private void SpawnJointMolecules(int O2, int N2, int HH, int CH2)
        {
            // create Oxygen Two atoms
            for (int i = 0; i < O2; ++i)
            {
                Gameplay.OxygenTwo oxygenTwo = new Gameplay.OxygenTwo(this);
                oxygenTwo.Spawn(true);
            }
            // create Nitrogen Two atoms
            for (int i = 0; i < N2; ++i)
            {
                Gameplay.NitrogenTwo nitrogenTwo = new Gameplay.NitrogenTwo(this);
                nitrogenTwo.Spawn(true);
            }
            // create Deuterium atoms
            for (int i = 0; i < HH; ++i)
            {
                Gameplay.Deuterium deuterium = new Gameplay.Deuterium(this);
                deuterium.Spawn(true);
            }
            // create Methylene atoms
            for (int i = 0; i < CH2; ++i)
            {
                Gameplay.Methylene methylene = new Gameplay.Methylene(this);
                methylene.Spawn(true);
            }
        }
        
        
        /// <summary>
        /// Create a new power-up in the world, if possible
        /// </summary>
        void SpawnPowerUp()
        {
            // check if there is a powerup in the world
            for (int i = 0; i < actors.Count; ++i)
            {
                //if (actors[i] is PowerUp)
                //{
                //    return;
                //}
            }
            // create the new power-up
            //PowerUp powerup = null;
            switch (random.Next(3))
            {
                case 0:
                    //powerup = new DoubleLaserPowerUp(this);
                    break;
                case 1:
                    //powerup = new TripleLaserPowerUp(this);
                    break;
                case 2:
                    //powerup = new RocketPowerUp(this);
                    break;
            }
            // add the new power-up to the world
            //powerup.Spawn(true);
        }        
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update the world simulation.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public void Update(float elapsedTime)
        {
            // update all actors
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].Update(elapsedTime);
            }

            // update collision
            MoveWorld(elapsedTime);

            // update particle systems
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Update(elapsedTime);
                if (particleSystems[i].IsActive == false)
                {
                    particleSystems.Garbage.Add(particleSystems[i]);
                }
            }

            // update the starfield
            Vector2 starfieldTarget = Vector2.Zero;
            int playingPlayers = 0;
            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i].Playing)
                {
                    starfieldTarget += ships[i].Position;
                    playingPlayers++;
                }
            }
            if (playingPlayers > 0)
            {
                //starfield.SetTargetPosition(starfieldTarget / playingPlayers);
            }
            //starfield.Update(elapsedTime);

            // check if we can create a new power-up yet
            if (powerUpTimer > 0f)
            {
                powerUpTimer = Math.Max(powerUpTimer - elapsedTime, 0f);
            }
            if (powerUpTimer <= 0.0f)
            {
                SpawnPowerUp();
                powerUpTimer = powerUpDelay;
            }

            // clean up the lists
            actors.Collect();
            particleSystems.Collect();
        }


        /// <summary>
        /// Draw the walls.
        /// </summary>
        /// <param name="lineBatch">The LineBatch to render to.</param>
        public void DrawWalls(LineBatch lineBatch)
        {
            if (lineBatch == null)
            {
                throw new ArgumentNullException("lineBatch");
            }
            // draw each wall-line
            for (int wall = 0; wall < walls.Length / 2; wall++)
            {
                lineBatch.DrawLine(walls[wall * 2], walls[wall * 2 + 1], Color.Yellow);
            }
        }
        #endregion

        #region Collision
        /// <summary>
        /// Move all of the actors in the world.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        private void MoveWorld(float elapsedTime)
        {
            Vector2 point = Vector2.Zero;
            // move each actor
            for (int i = 0; i < actors.Count; ++i)
            {
                if (actors[i].Dead)
                {
                    continue;
                }
                // determine how far they are going to move
                Vector2 movement = actors[i].Velocity * elapsedTime;
                // only allow actors that have not collided yet this frame to collide
                // -- otherwise, objects can "double-hit" and trade their momentum fast
                if (actors[i].CollidedThisFrame == false)
                {
                    movement = MoveAndCollide(actors[i], movement);
                }
                // determine the new position
                actors[i].Position += movement;
                // determine if their new position taks them through a wall
                for (int w = 0; w < walls.Length / 2; ++w)
                {
                    if (actors[i] is Projectile)
                    {
                        if (Collision.LineLineIntersect(actors[i].Position, 
                            actors[i].Position - movement, walls[w * 2], 
                            walls[w * 2 + 1], out point))
                        {
                            actors[i].Touch(actors[0]);
                        }
                    }
                    else
                    {
                        Collision.CircleLineCollisionResult result = 
                            new Collision.CircleLineCollisionResult();
                        if (Collision.CircleLineCollide(actors[i].Position, 
                            actors[i].Radius, walls[w * 2], walls[w * 2 + 1], 
                            ref result))
                        {
                            // if a non-projectile hits a wall, bounce slightly
                            float vn = Vector2.Dot(actors[i].Velocity, result.Normal);
                            actors[i].Velocity -= (2.0f * vn) * result.Normal;
                            actors[i].Position += result.Normal * result.Distance;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Move the given actor by the given movement, colliding and adjusting
        /// as necessary.
        /// </summary>
        /// <param name="actor">The actor who is moving.</param>
        /// <param name="movement">The desired movement vector for this update.</param>
        /// <returns>The movement vector after considering all collisions.</returns>
        private Vector2 MoveAndCollide(Actor actor, Vector2 movement)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }
            // make sure we care about where this actor goes
            if (actor.Dead || (actor.Collidable == false))
            {
                return movement;
            }
            // make sure the movement is significant
            if (movement.LengthSquared() <= 0f)
            {
                return movement;
            }

            // generate the list of collisions
            Collide(actor, movement);

            // determine if we had any collisions
            if (collisionResults.Count > 0)
            {
                collisionResults.Sort(CollisionResult.Compare);
                foreach (CollisionResult collision in collisionResults)
                {
                    // let the two actors touch each other, and see what happens
                    if (actor.Touch(collision.Actor) && collision.Actor.Touch(actor))
                    {
                        actor.CollidedThisFrame = 
                            collision.Actor.CollidedThisFrame = true;
                        // they should react to the other, even if they just died
                        AdjustVelocities(actor, collision.Actor);
                        return Vector2.Zero;
                    }
                }
            }

            return movement;
        }


        /// <summary>
        /// Determine all collisions that will happen as the given actor moves.
        /// </summary>
        /// <param name="actor">The actor that is moving.</param>
        /// <param name="movement">The actor's movement vector this update.</param>
        /// <remarks>The results are stored in the cached list.</remarks>
        public void Collide(Actor actor, Vector2 movement)
        {
            collisionResults.Clear();

            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }
            if (actor.Dead || (actor.Collidable == false))
            {
                return;
            }

            // determine the movement direction and scalar
            float movementLength = movement.Length();
            if (movementLength <= 0f)
            {
                return;
            }

            // check each actor
            foreach (Actor checkActor in actors)
            {
                if ((actor == checkActor) || checkActor.Dead || !checkActor.Collidable)
                {
                    continue;
                }

                // calculate the target vector
                Vector2 checkVector = checkActor.Position - actor.Position;
                float distanceBetween = checkVector.Length() - 
                    (checkActor.Radius + actor.Radius);

                // check if they could possibly touch no matter the direction
                if (movementLength < distanceBetween)
                {
                    continue;
                }

                // determine how much of the movement is bringing the two together
                float movementTowards = Vector2.Dot(movement, checkVector);

                // check to see if the movement is away from each other
                if (movementTowards < 0f)
                {
                    continue;
                }

                if (movementTowards < distanceBetween)
                {
                    continue;
                }

                CollisionResult result = new CollisionResult();
                result.Distance = distanceBetween;
                result.Normal = Vector2.Normalize(checkVector);
                result.Actor = checkActor;

                collisionResults.Add(result);
            }
        }


        /// <summary>
        /// Adjust the velocities of the two actors as if they have collided,
        /// distributing their velocities according to their masses.
        /// </summary>
        /// <param name="actor1">The first actor.</param>
        /// <param name="actor2">The second actor.</param>
        private static void AdjustVelocities(Actor actor1, Actor actor2)
        {
            // don't adjust velocities if at least one has negative mass
            if ((actor1.Mass <= 0f) || (actor2.Mass <= 0f))
            {
                return;
            }

            // determine the vectors normal and tangent to the collision
            Vector2 collisionNormal = Vector2.Normalize(
                actor2.Position - actor1.Position);
            Vector2 collisionTangent = new Vector2(
                -collisionNormal.Y, collisionNormal.X);

            // determine the velocity components along the normal and tangent vectors
            float velocityNormal1 = Vector2.Dot(actor1.Velocity, collisionNormal);
            float velocityTangent1 = Vector2.Dot(actor1.Velocity, collisionTangent);
            float velocityNormal2 = Vector2.Dot(actor2.Velocity, collisionNormal);
            float velocityTangent2 = Vector2.Dot(actor2.Velocity, collisionTangent);

            // determine the new velocities along the normal
            float velocityNormal1New = ((velocityNormal1 * (actor1.Mass - actor2.Mass))
                + (2f * actor2.Mass * velocityNormal2)) / (actor1.Mass + actor2.Mass);
            float velocityNormal2New = ((velocityNormal2 * (actor2.Mass - actor1.Mass))
                + (2f * actor1.Mass * velocityNormal1)) / (actor1.Mass + actor2.Mass);

            // determine the new total velocities
            actor1.Velocity = (velocityNormal1New * collisionNormal) + 
                (velocityTangent1 * collisionTangent);
            actor2.Velocity = (velocityNormal2New * collisionNormal) + 
                (velocityTangent2 * collisionTangent);
        }
        
        
        /// <summary>
        /// Find a valid point for the actor to spawn.
        /// </summary>
        /// <param name="actor">The actor to find a location for.</param>
        /// <remarks>This query is not bounded, which would be needed in a more complex
        /// game with a likelihood of no valid spawn locations.</remarks>
        /// <returns>A valid location for the user to spawn.</returns>
        public Vector2 FindSpawnPoint(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }

            Vector2 spawnPoint;
            float radius = actor.Radius;

            // fudge the radius slightly so we're not right on top of another actor
            if (actor is Gameplay.NanoBot)
            {
                radius *= 2f;
            }
            else
            {
                radius *= 1.1f;
            }
            radius = (float)Math.Ceiling(radius);

            Vector2 spawnMinimum = new Vector2(
                safeDimensions.X + radius, 
                safeDimensions.Y + radius);
            Vector2 spawnDimensions = new Vector2(
                (float)Math.Floor(safeDimensions.Width - 2f * radius),
                (float)Math.Floor(safeDimensions.Height - 2f * radius));
            Vector2 spawnMaximum = spawnMinimum + spawnDimensions;

            Collision.CircleLineCollisionResult result = 
                new Collision.CircleLineCollisionResult();
            bool valid = true;
            while (true)
            {
                valid = true;
                // generate a new spawn point
                spawnPoint = new Vector2(
                    spawnMinimum.X + spawnDimensions.X * (float)random.NextDouble(),
                    spawnMinimum.Y + spawnDimensions.Y * (float)random.NextDouble());
                if ((spawnPoint.X < spawnMinimum.X) ||
                    (spawnPoint.Y < spawnMinimum.Y) ||
                    (spawnPoint.X > spawnMaximum.X) ||
                    (spawnPoint.Y > spawnMaximum.Y))
                {
                    continue;
                }
                // if we don't collide, then one is good enough
                if (actor.Collidable == false)
                {
                    break; 
                }
                // check against the walls
                if (valid == true)
                {
                    for (int wall = 0; wall < walls.Length / 2; wall++)
                    {
                        if (Collision.CircleLineCollide(spawnPoint, radius, 
                            walls[wall * 2], walls[wall * 2 + 1], ref result))
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                // check against all other actors
                if (valid == true)
                {
                    foreach (Actor checkActor in actors)
                    {
                        if ((actor == checkActor) || checkActor.Dead)
                        {
                            continue;
                        }
                        if (Collision.CircleCircleIntersect(checkActor.Position,
                            checkActor.Radius, spawnPoint, radius))
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                // if we have gotten this far, then the spawn point is good
                if (valid == true)
                {
                    break;
                }
            }
            return spawnPoint;
        }
        #endregion

        #region Molecular Bonds

        /// <summary>
        /// Defines the interaction between the actors when they touch.
        /// </summary>
        /// <param name="target">The actor that is bonding this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public void BondOxygenTwo(Actor oxygen1, Actor oxygen2, int O)
        {
            deadO = deadO + O;
            if (deadO == 2)
            {
                oxygen1.Die(oxygen1);
                oxygen2.Die(oxygen2);
                Vector2 pos = (oxygen1.Position + oxygen2.Position) / 2;
                Vector2 vel = (oxygen1.Velocity + oxygen2.Velocity) / 2;
                Vector2 dir = (oxygen1.Direction + oxygen2.Direction) / 2;
                Gameplay.OxygenTwo oxygenTwo = new Gameplay.OxygenTwo(this);
                oxygenTwo.Spawn(true);
                oxygenTwo.Position = pos;
                oxygenTwo.Velocity = vel;
                oxygenTwo.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 36, 64f, 128f, 2f, 0.05f, Color.Red));
                AudioManager.PlayCue("asteroidTouch");
                deadO = 0;
            }
        }
        public void BondDeuterium(Actor hydrogen1, Actor hydrogen2, int H)
        {
            deadH = deadH + H;
            if (deadH == 2)
            {
                hydrogen1.Die(hydrogen1);
                hydrogen2.Die(hydrogen2);
                Vector2 pos = (hydrogen1.Position + hydrogen2.Position) / 2;
                Vector2 vel = (hydrogen1.Velocity + hydrogen2.Velocity) / 2;
                Vector2 dir = (hydrogen1.Direction + hydrogen2.Direction) / 2;
                Gameplay.Deuterium deuterium = new Gameplay.Deuterium(this);
                deuterium.Spawn(true);
                deuterium.Position = pos;
                deuterium.Velocity = vel;
                deuterium.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 18, 32f, 64f, 1.5f, 0.05f, Color.Yellow));
                AudioManager.PlayCue("asteroidTouch");
                deadH = 0;
            }
        }
        public void BondNitrogenTwo(Actor nitrogen1, Actor nitrogen2, int N)
        {
            deadN = deadN + N;
            if (deadN == 2)
            {
                nitrogen1.Die(nitrogen1);
                nitrogen2.Die(nitrogen2);
                Vector2 pos = (nitrogen1.Position + nitrogen2.Position) / 2;
                Vector2 vel = (nitrogen1.Velocity + nitrogen2.Velocity) / 2;
                Vector2 dir = (nitrogen1.Direction + nitrogen2.Direction) / 2;
                Gameplay.NitrogenTwo nitrogenTwo = new Gameplay.NitrogenTwo(this);
                nitrogenTwo.Spawn(true);
                nitrogenTwo.Position = pos;
                nitrogenTwo.Velocity = vel;
                nitrogenTwo.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 18, 32f, 64f, 1.5f, 0.05f, Color.Blue));
                AudioManager.PlayCue("asteroidTouch");
                deadN = 0;
            }
        }
        public void BondOzone(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.Ozone ozone = new Gameplay.Ozone(this);
            ozone.Spawn(true);
            ozone.Position = pos;
            ozone.Velocity = vel;
            ozone.Direction = dir;
        }
        public void UnbondOzone(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.OxygenTwo oxygenTwo = new Gameplay.OxygenTwo(this);
            oxygenTwo.Spawn(true);
            oxygenTwo.Position = pos;
            oxygenTwo.Velocity = vel * -2f;
            oxygenTwo.Direction = dir;
            Gameplay.Oxygen oxygen = new Gameplay.Oxygen(this);
            oxygen.Spawn(true);
            Vector2 newPos = new Vector2(0, 31.5f);
            Vector2 newDir = new Vector2(0, 31.5f);
            oxygen.Position = pos;
            oxygen.Velocity = vel * 2f;
            oxygen.Direction = dir;
        }
        public void BondCarbonDioxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.CarbonDioxide CO2 = new Gameplay.CarbonDioxide(this);
            CO2.Spawn(true);
            CO2.Position = pos;
            CO2.Velocity = vel;
            CO2.Direction = dir;
        }
        public void BondHydroxyl(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.Hydroxyl OH = new Gameplay.Hydroxyl(this);
            OH.Spawn(true);
            OH.Position = pos;
            OH.Velocity = vel;
            OH.Direction = dir;
        }
        public void BondNitricOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.NitricOxide NO = new Gameplay.NitricOxide(this);
            NO.Spawn(true);
            NO.Position = pos;
            NO.Velocity = vel;
            NO.Direction = dir;
        }
        public void BondWater(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.Water H2O = new Gameplay.Water(this);
            H2O.Spawn(true);
            H2O.Position = pos;
            H2O.Velocity = vel;
            H2O.Direction = dir;
        }
        public void BondMethylene(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.Methylene CH2 = new Gameplay.Methylene(this);
            CH2.Spawn(true);
            CH2.Position = pos;
            CH2.Velocity = vel;
            CH2.Direction = dir;
        }
        public void BondMethane(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.Methane CH4 = new Gameplay.Methane(this);
            CH4.Spawn(true);
            CH4.Position = pos;
            CH4.Velocity = vel;
            CH4.Direction = dir;
        }
        public void BondNitrousOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Gameplay.NitrousOxide N2O = new Gameplay.NitrousOxide(this);
            N2O.Spawn(true);
            N2O.Position = pos;
            N2O.Velocity = vel;
            N2O.Direction = dir;
        }
        #endregion

    }
}

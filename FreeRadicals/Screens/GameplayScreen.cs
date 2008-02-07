#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FreeRadicals.ScreenManager;
using FreeRadicals.BloomPostprocess;
using FreeRadicals.Rendering;
using FreeRadicals.Simulation;
#endregion

namespace FreeRadicals.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    /// <remarks>
    /// This class is somewhat similar to one of the same name in the 
    /// GameStateManagement sample.
    /// </remarks>
    class GameplayScreen : GameScreen
    {
        #region Fields
        BloomComponent bloomComponent;
        ContentManager content;
        LineBatch lineBatch;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D atomTexture;
        World world;
        AudioManager audio;
        bool gameOver;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }


        /// <summary>
        /// Initialize the game, after the ScreenManager is set, but not every time
        /// the graphics are reloaded.
        /// </summary>
        public void Initialize()
        {
            // create and add the bloom effect
            bloomComponent = new BloomComponent(ScreenManager.Game);
            ScreenManager.Game.Components.Add(bloomComponent);
            // do not automatically draw the bloom component
            bloomComponent.Visible = false;

            // create the world
            world = new World(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height));

            // retrieve the audio manager
            audio = (AudioManager)ScreenManager.Game.Services.GetService(
                typeof(AudioManager));
            world.AudioManager = audio;
            // start up the music
            audio.PlayMusic("gameMusic");
            // start up the game
            world.StartNewGame();
            gameOver = false;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            lineBatch = new LineBatch(ScreenManager.GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("Fonts/Arial");
            atomTexture = content.Load<Texture2D>("Textures/blank");

            // update the projection in the line-batch
            lineBatch.SetProjection(Matrix.CreateOrthographicOffCenter(0.0f,
                ScreenManager.GraphicsDevice.Viewport.Width, 
                ScreenManager.GraphicsDevice.Viewport.Height, 0.0f, 0.0f, 1.0f));
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            if (spriteBatch != null)
            {
                spriteBatch.Dispose();
                spriteBatch = null;
            }
            if (lineBatch != null)
            {
                lineBatch.Dispose();
                lineBatch = null;
            }
            content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // if this screen is leaving, then stop the music
            if (IsExiting)
            {
                audio.StopMusic();
            }
            else if ((otherScreenHasFocus == true) || (coveredByOtherScreen == true))
            {
                // make sure nobody's controller is vibrating
                for (int i = 0; i < 4; i++)
                {
                    GamePad.SetVibration((PlayerIndex)i, 0f, 0f);
                }
                if (gameOver == false)
                {
                    for (int i = 0; i < world.NanoBots.Length; i++)
                    {
                        world.NanoBots[i].ProcessInput(gameTime.TotalGameTime.Seconds, 
                            true);
                    }
                }
            }
            else
            {
                // check for a winner
                if (gameOver == false)
                {
                    for (int i = 0; i < world.NanoBots.Length; i++)
                    {
                        if (world.NanoBots[i].Score >= WorldRules.ScoreLimit)
                        {
                            ScreenManager.AddScreen(new GameOverScreen("Player " +
                                (i + 1).ToString() + " wins the game!"));
                            gameOver = true;
                            break;
                        }
                    }
                }
                // update the world
                if (gameOver == false)
                {
                    world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            lineBatch.Begin();

            // draw all actors
            foreach (Actor actor in world.Actors)
            {
                if (actor.Dead == false)
                {
                    actor.Draw(elapsedTime, lineBatch);
                }
            }

            // draw all particle systems
            foreach (ParticleSystem particleSystem in world.ParticleSystems)
            {
                if (particleSystem.IsActive)
                {
                    particleSystem.Draw(lineBatch);
                }
            }

            // draw the walls
            //world.DrawWalls(lineBatch);

            lineBatch.End();

            // draw the stars
            //spriteBatch.Begin();
            //world.Atmosphere.Draw(spriteBatch, atomTexture);
            //spriteBatch.End();

            if (WorldRules.NeonEffect)
            {
                bloomComponent.Draw(gameTime);
            }

            DrawHud(elapsedTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        /// <summary>
        /// Draw the user interface elements of the game (scores, etc.).
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        private void DrawHud(float elapsedTime)
        {
            spriteBatch.Begin();

            Vector2 position = new Vector2(128, 64);
            int offset = (1280) / 5;

            for (int i = 0; i < world.NanoBots.Length; ++i)
            {
                string message;

                if (world.NanoBots[i].Playing)
                {
                    message = "Dobson " + (i + 1).ToString() + ".0 Reserves:\n" +
                        "   Oxygen(O): " + world.NanoBots[i].OxygenAmmo.ToString() + "\n" +
                        "   Carbon(C): " + world.NanoBots[i].CarbonAmmo.ToString() + "\n" +
                        "   Hydrogen(H): " + world.NanoBots[i].HydrogenAmmo.ToString() + "\n" +
                        "   Shield: " + world.NanoBots[i].Life.ToString(); 
                }
                else
                {
                    message = ""; // Hold A to Join";
                }

                float scale = 1f;

                Vector2 size = spriteFont.MeasureString(message) * scale;
                position.X = (i + 1) * offset - size.Y / 2;
                spriteBatch.DrawString(spriteFont, message, position,
                    world.NanoBots[i].Color, 0.0f, Vector2.Zero, scale, 
                    SpriteEffects.None, 1.0f);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            Vector2 shadowOffset = Vector2.One;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
            {
                if (world.OzoneCount > 0)
                {
                    spriteBatch.DrawString(spriteFont, "Ozone Level: " + world.OzoneCount.ToString(),
                    new Vector2(50, 225) + shadowOffset, Color.Red);
                    spriteBatch.DrawString(spriteFont, "Ozone Level: " + world.OzoneCount.ToString(),
                    new Vector2(50, 225), Color.White);
                }

                if (world.FreeRadicalCount > 0)
                {
                    spriteBatch.DrawString(spriteFont, "Free Radical Level: " + world.FreeRadicalCount.ToString(),
                        new Vector2(50, 250) + shadowOffset, Color.Yellow);
                    spriteBatch.DrawString(spriteFont, "Free Radical Level: " + world.FreeRadicalCount.ToString(),
                    new Vector2(50, 250), Color.White);
                }

                if (world.GreenhouseGasesCount > 0)
                {
                    spriteBatch.DrawString(spriteFont, "Greenhouse Gases Level: " + world.GreenhouseGasesCount.ToString(),
                        new Vector2(50, 275) + shadowOffset, Color.Green);
                    spriteBatch.DrawString(spriteFont, "Greenhouse Gases Level: " + world.GreenhouseGasesCount.ToString(),
                    new Vector2(50, 275), Color.White);
                } 
            }

            spriteBatch.End();
        }
        #endregion
    }
}

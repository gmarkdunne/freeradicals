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

        // When Resolution is 1920 x 1200
        Texture2D aatomTexture;
        Texture2D auvc1LightTexture;
        Texture2D auvc2LightTexture;
        Texture2D auvc3LightTexture;
        Texture2D auvc4LightTexture;
        Texture2D auvc5LightTexture;

        // When Resolution is 1680 x 1050
        Texture2D batomTexture;
        Texture2D buvc1LightTexture;
        Texture2D buvc2LightTexture;
        Texture2D buvc3LightTexture;
        Texture2D buvc4LightTexture;
        Texture2D buvc5LightTexture;

        // When Resolution is 1440 x 900
        Texture2D catomTexture;
        Texture2D cuvc1LightTexture;
        Texture2D cuvc2LightTexture;
        Texture2D cuvc3LightTexture;
        Texture2D cuvc4LightTexture;
        Texture2D cuvc5LightTexture;

        // When Resolution is 1280 x 800
        Texture2D datomTexture;
        Texture2D duvc1LightTexture;
        Texture2D duvc2LightTexture;
        Texture2D duvc3LightTexture;
        Texture2D duvc4LightTexture;
        Texture2D duvc5LightTexture;

        Texture2D waterfallTexture;
        Texture2D titleTexture;
        Effect refractionEffect;
        World world;
        AudioManager audio;
        bool gameOver;
        int resolution;
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
            titleTexture = content.Load<Texture2D>("Textures/title");
            refractionEffect = content.Load<Effect>("Effects/refraction");
            waterfallTexture = content.Load<Texture2D>("Textures/waterfall");

            // When Resolution is 1920 x 1200
            aatomTexture = content.Load<Texture2D>("Textures/ablank");
            auvc1LightTexture = content.Load<Texture2D>("Textures/aUVC1");
            auvc2LightTexture = content.Load<Texture2D>("Textures/aUVC2");
            auvc3LightTexture = content.Load<Texture2D>("Textures/aUVC3");
            auvc4LightTexture = content.Load<Texture2D>("Textures/aUVC4");
            auvc5LightTexture = content.Load<Texture2D>("Textures/aUVC5");

            // When Resolution is 1680 x 1050
            batomTexture = content.Load<Texture2D>("Textures/bblank");
            buvc1LightTexture = content.Load<Texture2D>("Textures/bUVC1");
            buvc2LightTexture = content.Load<Texture2D>("Textures/bUVC2");
            buvc3LightTexture = content.Load<Texture2D>("Textures/bUVC3");
            buvc4LightTexture = content.Load<Texture2D>("Textures/bUVC4");
            buvc5LightTexture = content.Load<Texture2D>("Textures/bUVC5");

            // When Resolution is 1440 x 900
            catomTexture = content.Load<Texture2D>("Textures/cblank");
            cuvc1LightTexture = content.Load<Texture2D>("Textures/cUVC1");
            cuvc2LightTexture = content.Load<Texture2D>("Textures/cUVC2");
            cuvc3LightTexture = content.Load<Texture2D>("Textures/cUVC3");
            cuvc4LightTexture = content.Load<Texture2D>("Textures/cUVC4");
            cuvc5LightTexture = content.Load<Texture2D>("Textures/cUVC5");

            // When Resolution is 1280 x 800
            datomTexture = content.Load<Texture2D>("Textures/dblank");
            duvc1LightTexture = content.Load<Texture2D>("Textures/dUVC1");
            duvc2LightTexture = content.Load<Texture2D>("Textures/dUVC2");
            duvc3LightTexture = content.Load<Texture2D>("Textures/dUVC3");
            duvc4LightTexture = content.Load<Texture2D>("Textures/dUVC4");
            duvc5LightTexture = content.Load<Texture2D>("Textures/dUVC5");

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


            if (Simulation.WorldRules.ScreenRes == ScreenRes.a1920x1200)
            {
                resolution = 1920;
            }
            else if (Simulation.WorldRules.ScreenRes == ScreenRes.b1680x1050)
            {
                resolution = 1680;
            }
            else if (Simulation.WorldRules.ScreenRes == ScreenRes.c1440x900)
            {
                resolution = 1440;
            }
            else if (Simulation.WorldRules.ScreenRes == ScreenRes.d1280x800)
            {
                resolution = 1280;
            }

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
                        if (world.OzoneCount >= 35 || world.OzoneCount < 5)
                        {
                            if (world.OzoneCount >= 35)
                            {
                                ScreenManager.AddScreen(new GameOverScreen("             Dobson 1.0 wins!" + "\n\n" +
                                                        "35 Ozone Molecules in the Atmosphere."));
                                gameOver = true;
                            }
                            else
                            {
                                ScreenManager.AddScreen(new GameOverScreen("                  Dobson 1.0 Loses!" + "\n\n" +
                                                        "Less than 5 Ozone Molecules in the Atmosphere."));
                                gameOver = true;
                            }
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
            spriteBatch.Begin();
            if (Simulation.WorldRules.ScreenRes == ScreenRes.a1920x1200)
            {
                world.Atmosphere.Draw(spriteBatch, aatomTexture);
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.b1680x1050)
            {
                world.Atmosphere.Draw(spriteBatch, batomTexture);
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.c1440x900)
            {
                world.Atmosphere.Draw(spriteBatch, catomTexture);
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.d1280x800)
            {
                world.Atmosphere.Draw(spriteBatch, datomTexture);
            }
            spriteBatch.End();

            if (WorldRules.NeonEffect)
            {
                bloomComponent.Draw(gameTime);
            }

            DrawHud(elapsedTime);
            DrawUVCLight(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        /// <summary>
        /// Draw the user interface elements of the game (scores, etc.).
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public void DrawHud(float elapsedTime)
        {
            spriteBatch.Begin();

            Vector2 position = new Vector2(128, 64);

            int offset = (resolution) / 5;

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
                    message = "";//Hold A to Join";
                }

                float scale = 1.2f * world.ResVar;

                Vector2 size = spriteFont.MeasureString(message) * scale;
                position.X = (i + 1) * offset - size.Y / 2;
                spriteBatch.DrawString(spriteFont, message, position,
                    world.NanoBots[i].Color, 0.0f, Vector2.Zero, scale, 
                    SpriteEffects.None, 1.0f);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            Vector2 shadowOffset = Vector2.One;

            if (world.NanoBots[0].Playing == false)
            {
                spriteBatch.DrawString(spriteFont, "Hold A to start game...", 
                    new Vector2(50, 375) + shadowOffset, Color.Indigo);
                spriteBatch.DrawString(spriteFont, "Hold A to start game...",
                    new Vector2(50, 375), Color.White);
            }

            if (world.OzoneCount > 0)
            {
                spriteBatch.DrawString(spriteFont, "Ozone Level: " + world.OzoneCount.ToString(),
                new Vector2(50, 400) + shadowOffset, Color.Red);
                spriteBatch.DrawString(spriteFont, "Ozone Level: " + world.OzoneCount.ToString(),
                new Vector2(50, 400), Color.White);
            }

            if (world.FreeRadicalCount > 0)
            {
                spriteBatch.DrawString(spriteFont, "Free Radical Level: " + world.FreeRadicalCount.ToString(),
                    new Vector2(50, 425) + shadowOffset, Color.Yellow);
                spriteBatch.DrawString(spriteFont, "Free Radical Level: " + world.FreeRadicalCount.ToString(),
                new Vector2(50, 425), Color.White);
            }

            if (world.GreenhouseGasesCount > 0)
            {
                spriteBatch.DrawString(spriteFont, "Greenhouse Gases Level: " + world.GreenhouseGasesCount.ToString(),
                    new Vector2(50, 450) + shadowOffset, Color.Lime);
                spriteBatch.DrawString(spriteFont, "Greenhouse Gases Level: " + world.GreenhouseGasesCount.ToString(),
                new Vector2(50, 450), Color.White);
            } 

            spriteBatch.End();
        }


        /// <summary>
        // Effect uses a scrolling displacement texture to offset the position of
        // the main texture.
        /// </summary>
        public void DrawUVCLight(GameTime gameTime)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            // title
            Vector2 titlePosition = new Vector2( 0, 0);
            Color titleColor = Color.White;

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.Additive,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            // Set the displacement texture.
            ScreenManager.GraphicsDevice.Textures[1] = waterfallTexture;

            // Set an effect parameter to make the
            // displacement texture scroll in a giant circle.
            refractionEffect.Parameters["DisplacementScroll"].SetValue(
                                                        MoveInCircle(gameTime, 0.1f));

            // Begin the custom effect.
            refractionEffect.Begin();
            refractionEffect.CurrentTechnique.Passes[0].Begin();
            
            if (Simulation.WorldRules.ScreenRes == ScreenRes.a1920x1200)
            {
                if (world.OzoneCount >= 26)
                {
                    ScreenManager.SpriteBatch.Draw(auvc1LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 19 && world.OzoneCount <= 25)
                {
                    ScreenManager.SpriteBatch.Draw(auvc2LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 12 && world.OzoneCount <= 18)
                {
                    ScreenManager.SpriteBatch.Draw(auvc3LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 6 && world.OzoneCount <= 11)
                {
                    ScreenManager.SpriteBatch.Draw(auvc4LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 0 && world.OzoneCount <= 5)
                {
                    ScreenManager.SpriteBatch.Draw(auvc5LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                } 
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.b1680x1050)
            {
                if (world.OzoneCount >= 26)
                {
                    ScreenManager.SpriteBatch.Draw(buvc1LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 19 && world.OzoneCount <= 25)
                {
                    ScreenManager.SpriteBatch.Draw(buvc2LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 12 && world.OzoneCount <= 18)
                {
                    ScreenManager.SpriteBatch.Draw(buvc3LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 6 && world.OzoneCount <= 11)
                {
                    ScreenManager.SpriteBatch.Draw(buvc4LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 0 && world.OzoneCount <= 5)
                {
                    ScreenManager.SpriteBatch.Draw(buvc5LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                } 
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.c1440x900)
            {
                if (world.OzoneCount >= 26)
                {
                    ScreenManager.SpriteBatch.Draw(cuvc1LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 19 && world.OzoneCount <= 25)
                {
                    ScreenManager.SpriteBatch.Draw(cuvc2LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 12 && world.OzoneCount <= 18)
                {
                    ScreenManager.SpriteBatch.Draw(cuvc3LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 6 && world.OzoneCount <= 11)
                {
                    ScreenManager.SpriteBatch.Draw(cuvc4LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 0 && world.OzoneCount <= 5)
                {
                    ScreenManager.SpriteBatch.Draw(cuvc5LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                } 
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.d1280x800)
            {
                if (world.OzoneCount >= 26)
                {
                    ScreenManager.SpriteBatch.Draw(duvc1LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 19 && world.OzoneCount <= 25)
                {
                    ScreenManager.SpriteBatch.Draw(duvc2LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 12 && world.OzoneCount <= 18)
                {
                    ScreenManager.SpriteBatch.Draw(duvc3LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 6 && world.OzoneCount <= 11)
                {
                    ScreenManager.SpriteBatch.Draw(duvc4LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                }
                else if (world.OzoneCount >= 0 && world.OzoneCount <= 5)
                {
                    ScreenManager.SpriteBatch.Draw(duvc5LightTexture, titlePosition,
                            new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
                } 
            }

            // End the sprite batch, then end our custom effect.
            ScreenManager.SpriteBatch.End();

            refractionEffect.CurrentTechnique.Passes[0].End();
            refractionEffect.End();
        }


        /// <summary>
        /// Helper for moving a value around in a circle.
        /// </summary>
        static Vector2 MoveInCircle(GameTime gameTime, float speed)
        {
            double time = gameTime.TotalGameTime.TotalSeconds * speed;

            float x = (float)Math.Cos(time);
            float y = (float)Math.Sin(time);

            return new Vector2(x, y);
        }
        #endregion
    }
}

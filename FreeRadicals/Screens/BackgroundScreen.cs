#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    /// <remarks>
    /// This class is somewhat similar to one of the same name in the 
    /// GameStateManagement sample.
    /// </remarks>
    class BackgroundScreen : GameScreen
    {
        #region Fields
        Random random;
        CollectCollection<ParticleSystem> particleSystems;
        float addTimer;
        Color[] explosionColors = new Color[] 
            {
                Color.Red, Color.Red, Color.Silver, Color.Gray, Color.Orange, 
                Color.Yellow 
            };
        LineBatch lineBatch;
        Texture2D titleTexture;

        // Effect
        Effect refractionEffect;

        // Textures used by this sample.
        Texture2D waterfallTexture;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            random = new Random();
            particleSystems = new CollectCollection<ParticleSystem>(null);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the ScreenManager, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            lineBatch = new LineBatch(ScreenManager.GraphicsDevice);
            titleTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/title");
            refractionEffect = ScreenManager.Game.Content.Load<Effect>("Effects/refraction");
            waterfallTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/waterfall");

            int viewportWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewportHeight = ScreenManager.GraphicsDevice.Viewport.Height;

            // update the projection in the line-batch
            lineBatch.SetProjection(Matrix.CreateOrthographicOffCenter(0.0f,
                viewportWidth, viewportHeight, 0.0f, 0.0f, 1.0f));

            // recreate the particle systems
            particleSystems.Clear();
            for (int i = 0; i < 8; i++)
            {
                //particleSystems.Add(new ParticleSystem(
                //   new Vector2(random.Next(viewportWidth), random.Next(viewportHeight)),
                //   Vector2.Zero, 100, 256, 128,
                //   1f + 2f * (float)random.NextDouble(), 0.075f, explosionColors));
                particleSystems.Add(new ParticleSystem(
                   new Vector2(random.Next(viewportWidth), random.Next(viewportHeight)),
                    Vector2.Zero, 64, 256f, 1024f, 3f, 0.05f, explosionColors));
                particleSystems.Add(new ParticleSystem(
                   new Vector2(random.Next(viewportWidth), random.Next(viewportHeight)),
                    Vector2.Zero, 128, 64f, 256f, 3f, 0.05f, explosionColors));
            }
            base.LoadContent();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            addTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (addTimer > 0.25f)
            {
                addTimer -= 0.15f;
                //particleSystems.Add(new ParticleSystem(
                //   new Vector2(random.Next(ScreenManager.GraphicsDevice.Viewport.Width),
                //       random.Next(ScreenManager.GraphicsDevice.Viewport.Height)),
                //       Vector2.Zero, 100, 256, 128,
                //       1f + 2f * (float)random.NextDouble(), 0.075f, explosionColors));
                particleSystems.Add(new ParticleSystem(
                   new Vector2(random.Next(ScreenManager.GraphicsDevice.Viewport.Width),
                       random.Next(ScreenManager.GraphicsDevice.Viewport.Height)),
                    Vector2.Zero, 64, 256f, 1024f, 3f, 0.05f, explosionColors));
                particleSystems.Add(new ParticleSystem(
                   new Vector2(random.Next(ScreenManager.GraphicsDevice.Viewport.Width),
                       random.Next(ScreenManager.GraphicsDevice.Viewport.Height)),
                    Vector2.Zero, 128, 64f, 256f, 3f, 0.05f, explosionColors));
            }

            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                if (particleSystems[i].IsActive == false)
                {
                    particleSystems.Garbage.Add(particleSystems[i]);
                }
            }
            particleSystems.Collect();

            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            lineBatch.Begin();
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Draw(lineBatch);
            }
            lineBatch.End();

            // title
            Vector2 titlePosition = new Vector2(
                (viewportSize.X - titleTexture.Width) / 2f,
                viewportSize.Y * 0.18f);
            titlePosition.Y -= (float)Math.Pow(TransitionPosition, 2) * titlePosition.Y;
            Color titleColor = Color.White;

            //ScreenManager.SpriteBatch.Begin(SpriteBlendMode.Additive,
            //                  SpriteSortMode.Immediate,
            //                  SaveStateMode.None);
            //ScreenManager.SpriteBatch.Draw(titleTexture, titlePosition,
            //    new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
            //ScreenManager.SpriteBatch.End();
            // Begin the sprite batch.
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.Additive,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            // Set the displacement texture.
            ScreenManager.GraphicsDevice.Textures[1] = waterfallTexture;

            // Set an effect parameter to make the
            // displacement texture scroll in a giant circle.
            refractionEffect.Parameters["DisplacementScroll"].SetValue(
                                                        MoveInCircle(gameTime, 0.3f));

            // Begin the custom effect.
            refractionEffect.Begin();
            refractionEffect.CurrentTechnique.Passes[0].Begin();

            ScreenManager.SpriteBatch.Draw(titleTexture, titlePosition,
                new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));

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

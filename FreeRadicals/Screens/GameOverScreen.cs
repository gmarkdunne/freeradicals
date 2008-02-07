#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace FreeRadicals
{
    /// <summary>
    /// The GameOverScreen screen object is displayed when the game is over.
    /// </summary>
    class GameOverScreen : MenuScreen
    {
        #region Fields
        Texture2D titleTexture;
        string message;

        // Effect
        Effect refractionEffect;

        // Textures used by this sample.
        Texture2D waterfallTexture;
        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public GameOverScreen(string message)
        {
            this.message = message;

            MenuEntry playAgainMenuEntry = new MenuEntry("Play Again");
            MenuEntry returnToMenuMenuEntry = new MenuEntry("Return to Menu");

            playAgainMenuEntry.Selected += PlayAgainMenuEntrySelected;
            returnToMenuMenuEntry.Selected += ReturnToMenuMenuEntrySelected;

            MenuEntries.Add(playAgainMenuEntry);
            MenuEntries.Add(returnToMenuMenuEntry);

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
            IsPopup = true;
        }


        /// <summary>
        /// Loads graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            titleTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/title");
            refractionEffect = ScreenManager.Game.Content.Load<Effect>("Effects/refraction");
            waterfallTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/waterfall");
            base.LoadContent();
        }


        #endregion


        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Again menu entry is selected.
        /// </summary>
        void PlayAgainMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, LoadGameplayScreen, true);
        }


        /// <summary>
        /// Loading screen callback for activating the gameplay screen.
        /// </summary>
        void LoadGameplayScreen(object sender, EventArgs e)
        {
            GameplayScreen gameplayScreen = new GameplayScreen();
            gameplayScreen.ScreenManager = this.ScreenManager;
            gameplayScreen.Initialize();
            ScreenManager.AddScreen(gameplayScreen);
        }


        /// <summary>
        /// Event handler for when the Return to Menu menu entry is selected.
        /// </summary>
        void ReturnToMenuMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, LoadMainMenuScreen, false);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void LoadMainMenuScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }


        protected override void OnCancel() 
        {
            LoadingScreen.Load(ScreenManager, LoadMainMenuScreen, false);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the game-over screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            // calculate the texture position
            Vector2 titlePosition = new Vector2(
                (viewportSize.X - titleTexture.Width) / 2f, 
                viewportSize.Y * 0.18f);
            titlePosition.Y -= (float)Math.Pow(TransitionPosition, 2) * titlePosition.Y;
            Color titleColor = Color.White;

            // calculate the message position and size
            float textScale = 1.5f + 0.05f * 
                (float)Math.Cos(gameTime.TotalRealTime.TotalSeconds * 8.0f);
            Vector2 textSize = ScreenManager.Font.MeasureString(message);
            Vector2 textPosition = new Vector2(
                viewportSize.X / 2f - textSize.X / 2f * textScale,
                viewportSize.Y * 0.417f);
            titlePosition.Y -= (float)Math.Pow(TransitionPosition, 2) * titlePosition.Y;
            textSize.X = 0f;
            textSize.Y /= 2f;

            // draw the texture and text
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

            ScreenManager.SpriteBatch.Begin();
            //ScreenManager.SpriteBatch.Draw(titleTexture, titlePosition, 
            //    new Color(titleColor.R, titleColor.G, titleColor.B, TransitionAlpha));
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message,
                textPosition, new Color(0, 255, 0, TransitionAlpha), 0.0f, textSize,
                textScale, SpriteEffects.None, 1.0f);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
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

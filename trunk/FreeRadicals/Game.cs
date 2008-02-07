#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FreeRadicals.Screens;
using FreeRadicals.Simulation;
#endregion

namespace FreeRadicals
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FreeRadicalsGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        ScreenManager.ScreenManager screenManager;
        AudioManager audioManager;

        bool fullScreen = WorldRules.FullScreen;
        bool fullScreenEnabled = false;

        public FreeRadicalsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (Simulation.WorldRules.ScreenRes == ScreenRes.a1920x1200)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1200;
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.b1680x1050)
            {
                graphics.PreferredBackBufferWidth = 1680;
                graphics.PreferredBackBufferHeight = 1050;
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.c1440x900)
            {
                graphics.PreferredBackBufferWidth = 1440;
                graphics.PreferredBackBufferHeight = 900;
            }
            if (Simulation.WorldRules.ScreenRes == ScreenRes.d1280x800)
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 800;
            }
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            graphics.SynchronizeWithVerticalRetrace = true;
            
            if (fullScreen)
            {
                if (!fullScreenEnabled)
                {
                    graphics.ToggleFullScreen();
                    fullScreenEnabled = true; 
                }
            }
            if (!fullScreen)
            {
                if (fullScreenEnabled)
                {
                    graphics.ToggleFullScreen();
                    fullScreenEnabled = false; 
                }
            }    

            // create the screen manager
            screenManager = new ScreenManager.ScreenManager(this);
            Components.Add(screenManager);

            // create the audio manager
            audioManager = new AudioManager(this, 
                "Content\\Audio\\FreeRadicals.xgs",
                "Content\\Audio\\FreeRadicals.xwb",
                "Content\\Audio\\FreeRadicals.xsb");
            Services.AddService(typeof(AudioManager), audioManager);

            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
        }


        /// <summary>
        /// Overridden from Game.Initialize().  Sets up the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // add the background screen to the screen manager
            screenManager.AddScreen(new BackgroundScreen());

            // add the main menu screen to the screen manager
            screenManager.AddScreen(new MainMenuScreen());
        }

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update the audio manager
            audioManager.Update(gameTime);

            if (fullScreen)
            {
                if (!fullScreenEnabled)
                {
                    graphics.ToggleFullScreen();
                    fullScreenEnabled = true;
                }
            }
            if (!fullScreen)
            {
                if (fullScreenEnabled)
                {
                    graphics.ToggleFullScreen();
                    fullScreenEnabled = false;
                }
            } 

            base.Update(gameTime);
        }

        #endregion


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // the screen manager owns the real drawing

            base.Draw(gameTime);
        }

        #region Entry Point
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            using (FreeRadicalsGame game = new FreeRadicalsGame())
            {
                game.Run();
            }
        }

        #endregion
    }
}

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
using System.Windows.Forms;
#endregion

namespace FreeRadicals
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FreeRadicalsGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        bool on = false;
        bool off = true;

        ScreenManager.ScreenManager screenManager;
        AudioManager audioManager;

        public FreeRadicalsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (SystemInformation.VirtualScreen.Width == 1920)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1200;
                Simulation.WorldRules.ScreenRes = ScreenRes.a1920x1200;
            }
            if (SystemInformation.VirtualScreen.Width == 1680 ||
                SystemInformation.VirtualScreen.Width == 1600)
            {
                graphics.PreferredBackBufferWidth = 1680;
                graphics.PreferredBackBufferHeight = 1050;
                Simulation.WorldRules.ScreenRes = ScreenRes.b1680x1050;
            }
            if (SystemInformation.VirtualScreen.Width == 1440)
            {
                graphics.PreferredBackBufferWidth = 1440;
                graphics.PreferredBackBufferHeight = 900;
                Simulation.WorldRules.ScreenRes = ScreenRes.c1440x900;
            }
            if (SystemInformation.VirtualScreen.Width == 1280)
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 800;
                Simulation.WorldRules.ScreenRes = ScreenRes.d1280x800;
            }

            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            graphics.SynchronizeWithVerticalRetrace = true;

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

            // Toggle Full Screen On/Off
            if (WorldRules.FullScreen == true && 
                off == true)
            {
                graphics.ToggleFullScreen();
                off = false;
                on = true;
            }
            else if (WorldRules.FullScreen == false && 
                on == true)
            {
                graphics.ToggleFullScreen();
                off = true;
                on = false;
            }

            // Change Screen Resolution
            //if (Simulation.WorldRules.ScreenRes == ScreenRes.a1920x1200)
            //{
            //    graphics.PreferredBackBufferWidth = 1920;
            //    graphics.PreferredBackBufferHeight = 1200;
            //}
            //if (Simulation.WorldRules.ScreenRes == ScreenRes.b1680x1050)
            //{
            //    graphics.PreferredBackBufferWidth = 1680;
            //    graphics.PreferredBackBufferHeight = 1050;
            //}
            //if (Simulation.WorldRules.ScreenRes == ScreenRes.c1440x900)
            //{
            //    graphics.PreferredBackBufferWidth = 1440;
            //    graphics.PreferredBackBufferHeight = 900;
            //}
            //if (Simulation.WorldRules.ScreenRes == ScreenRes.d1280x800)
            //{
            //    graphics.PreferredBackBufferWidth = 1280;
            //    graphics.PreferredBackBufferHeight = 800;
            //}

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
            //MessageBox.Show("VirtualScreen: " +
            //    SystemInformation.VirtualScreen);

            //MessageBox.Show("Monitor Size:" +
            //    SystemInformation.PrimaryMonitorSize);

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

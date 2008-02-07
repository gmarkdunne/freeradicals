#region Using Statements
using System;
using Microsoft.Xna.Framework;
using FreeRadicals.Simulation;
#endregion

namespace FreeRadicals.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    /// <remarks>
    /// This class is similar to one of the same name in the GameStateManagement sample.
    /// </remarks>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        static string[] screenRes = { "a1920x1200", "b1680x1050", "c1440x900", "d1280x800" };
        static int currentScreenRes = 0;

        static int scoreLimit = 10;
        static bool motionBlur = true;
        static bool neonEffect = true;
        static bool fullScreen = false;

        MenuEntry screenResMenuEntry = new MenuEntry(String.Empty);
        MenuEntry motionBlurMenuEntry = new MenuEntry(String.Empty);
        MenuEntry neonEffectMenuEntry = new MenuEntry(String.Empty);
        MenuEntry fullScreenMenuEntry = new MenuEntry(String.Empty);


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor populates the menu with empty strings: the real values
        /// are filled in by the Update method to reflect the changing settings.
        /// </summary>
        public OptionsMenuScreen()
        {
            currentScreenRes = (int)WorldRules.ScreenRes;
            motionBlur = WorldRules.MotionBlur;
            neonEffect = WorldRules.NeonEffect;
            fullScreen = WorldRules.FullScreen;

            screenResMenuEntry.Selected += ScreenResMenuEntrySelected;
            motionBlurMenuEntry.Selected += MotionBlurMenuEntrySelected;
            neonEffectMenuEntry.Selected += NeonEffectMenuEntrySelected;
            fullScreenMenuEntry.Selected += FullScreenMenuEntrySelected;

            MenuEntries.Add(screenResMenuEntry);
            MenuEntries.Add(motionBlurMenuEntry);
            MenuEntries.Add(neonEffectMenuEntry);
            MenuEntries.Add(fullScreenMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Updates the options screen, filling in the latest values for the menu text.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            screenResMenuEntry.Text = "Screen Resolution : " +
                screenRes[currentScreenRes];
            motionBlurMenuEntry.Text = "Motion Blur : " + motionBlur.ToString();
            neonEffectMenuEntry.Text = "Neon Effect : " + neonEffect.ToString();
            fullScreenMenuEntry.Text = "Full Screen : " + fullScreen.ToString();
        }


        /// <summary>
        /// Event handler for when the Score Limit menu entry is selected.
        /// </summary>
        void ScoreLimitMenuEntrySelected(object sender, EventArgs e)
        {
            scoreLimit += 5;
            if (scoreLimit > 25)
                scoreLimit = 5;
        }


        /// <summary>
        /// Event handler for when the Screen Resolution menu entry is selected.
        /// </summary>
        void ScreenResMenuEntrySelected(object sender, EventArgs e)
        {
            currentScreenRes = (currentScreenRes + 1) %
                screenRes.Length;
        }


        /// <summary>
        /// Event handler for when the Motion Blur menu entry is selected.
        /// </summary>
        void MotionBlurMenuEntrySelected(object sender, EventArgs e)
        {
            motionBlur = !motionBlur;
        }


        /// <summary>
        /// Event handler for when the NeonEffect menu entry is selected.
        /// </summary>
        void NeonEffectMenuEntrySelected(object sender, EventArgs e)
        {
            neonEffect = !neonEffect;
        }


        ///<summary>
        ///Event handler for when the Full Screen menu entry is selected.
        ///
        void FullScreenMenuEntrySelected(object sender, EventArgs e) 
        {
            fullScreen = !fullScreen;
        }

        /// <summary>
        /// When the user cancels the options screen, go back to the main menu.
        /// </summary>
        protected override void OnCancel()
        {
            WorldRules.ScreenRes = (ScreenRes)Enum.Parse(typeof(ScreenRes),
                screenRes[currentScreenRes], true);
            WorldRules.ScoreLimit = scoreLimit;
            WorldRules.MotionBlur = motionBlur;
            WorldRules.NeonEffect = neonEffect;
            WorldRules.FullScreen = fullScreen;

            ExitScreen();
        }


        #endregion
    }
}

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


        static string[] atomDensity = { "None", "Low", "Medium", "High" };
        static int currentAtomDensity = 2;

        static string[] wallStyle = { "None", "One", "Two", "Three" };
        static int currentWallStyle = 0;

        static int scoreLimit = 10;
        static bool motionBlur = true;
        static bool neonEffect = true;
        static bool fullScreen = false;

        MenuEntry scoreLimitMenuEntry = new MenuEntry(String.Empty);
        MenuEntry atomDensityMenuEntry = new MenuEntry(String.Empty);
        MenuEntry wallStyleMenuEntry = new MenuEntry(String.Empty);
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
            currentAtomDensity = (int)WorldRules.AtomDensity;
            motionBlur = WorldRules.MotionBlur;
            neonEffect = WorldRules.NeonEffect;
            fullScreen = WorldRules.FullScreen;
            scoreLimit = WorldRules.ScoreLimit;

            atomDensityMenuEntry.Selected += AtomDensityMenuEntrySelected;
            motionBlurMenuEntry.Selected += MotionBlurMenuEntrySelected;
            neonEffectMenuEntry.Selected += NeonEffectMenuEntrySelected;
            fullScreenMenuEntry.Selected += FullScreenMenuEntrySelected;
            scoreLimitMenuEntry.Selected += ScoreLimitMenuEntrySelected;
            wallStyleMenuEntry.Selected += WallStyleMenuEntrySelected;

            MenuEntries.Add(atomDensityMenuEntry);
            MenuEntries.Add(motionBlurMenuEntry);
            MenuEntries.Add(neonEffectMenuEntry);
            MenuEntries.Add(fullScreenMenuEntry);
            MenuEntries.Add(scoreLimitMenuEntry);
            MenuEntries.Add(wallStyleMenuEntry);
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

            atomDensityMenuEntry.Text = "Atoms Density : " +
                atomDensity[currentAtomDensity];
            motionBlurMenuEntry.Text = "Motion Blur : " + motionBlur.ToString();
            neonEffectMenuEntry.Text = "Neon Effect : " + neonEffect.ToString();
            fullScreenMenuEntry.Text = "Full Screen : " + fullScreen.ToString();
            //scoreLimitMenuEntry.Text = "Score Limit : " + scoreLimit.ToString();
            //wallStyleMenuEntry.Text = "Wall Style : " + wallStyle[currentWallStyle];
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
        /// Event handler for when the Atom Density menu entry is selected.
        /// </summary>
        void AtomDensityMenuEntrySelected(object sender, EventArgs e)
        {
            currentAtomDensity = (currentAtomDensity + 1) %
                atomDensity.Length;
        }


        /// <summary>
        /// Event handler for when the Wall Style menu entry is selected.
        /// </summary>
        void WallStyleMenuEntrySelected(object sender, EventArgs e)
        {
            currentWallStyle = (currentWallStyle + 1) % wallStyle.Length;
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
            WorldRules.AtomDensity = 
                (AtomDensity)Enum.Parse(typeof(AtomDensity), 
                                         atomDensity[currentAtomDensity], true);
            WorldRules.ScoreLimit = scoreLimit;
            WorldRules.MotionBlur = motionBlur;
            WorldRules.NeonEffect = neonEffect;
            WorldRules.FullScreen = fullScreen;

            ExitScreen();
        }


        #endregion
    }
}

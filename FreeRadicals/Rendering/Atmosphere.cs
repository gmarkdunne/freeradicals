#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals.Rendering
{
    /// <summary>
    /// A Atmosphere represented by single-pixel "atoms", with parallax-scrolling.
    /// </summary>
    class Atmosphere
    {
        #region Constants
        /// <summary>
        /// The distance that the atoms move per second.
        /// </summary>
        const float atomVelocity = 1500f;
        #endregion

        #region Fields
        Random random;

        /// <summary>
        /// The positions of each atom.
        /// </summary>
        Vector2[] atomPositions;

        /// <summary>
        /// The depth of each atom, used for parallax.
        /// </summary>
        byte[] atomDepths;

        /// <summary>
        /// The relative "target position" of the Atmosphere, compared with the
        /// currentPosition field to create motion.
        /// </summary>
        Vector2 targetPosition;

        /// <summary>
        /// The relative "current position" of the Atmosphere, compared with the
        /// targetPosition field to create motion.
        /// </summary>
        Vector2 currentPosition;

        static readonly Color[] colors = 
        { 
            Color.Red, Color.Red, Color.Silver, 
            Color.Gray, Color.Orange, Color.Yellow 
        };
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new Atmosphere.
        /// </summary>
        /// <param name="count">The number of atoms in the Atmosphere.</param>
        /// <param name="bounds">The bounds of atoms generation.</param>
        public Atmosphere(int count, Rectangle bounds)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            random = new Random();
            currentPosition = targetPosition = new Vector2(
                bounds.Left + (bounds.Right - bounds.Left) / 2,
                bounds.Top + (bounds.Bottom - bounds.Top) / 2);
            atomPositions = new Vector2[count];
            atomDepths = new byte[count];
            for (int i = 0; i < count; i++)
            {
                atomPositions[i] = new Vector2(
                    random.Next(bounds.Left, bounds.Right),
                    random.Next(bounds.Top, bounds.Bottom));
                atomDepths[i] = (byte)random.Next(1, 1255);
            }
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update the Atmosphere.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public void Update(float elapsedTime)
        {
            if (targetPosition != currentPosition)
            {
                Vector2 movement = targetPosition - currentPosition;
                // if the movement is within 1 square in each direction, 
                // then we're close enough
                if (movement.LengthSquared() > 0f)  // approxmation of sqrt(2)
                {
                    currentPosition = targetPosition;
                    return;
                }
                // move the current position
                movement = Vector2.Normalize(movement) * (atomVelocity * elapsedTime);
                currentPosition += movement;
                // move the atoms, scaled by their depth
                for (int i = 0; i < atomPositions.Length; i++)
                {
                    atomPositions[i] += movement * ((float)atomDepths[i] * 2000f);
                }
            }
        }

        /// <summary>
        /// Render the Atmosphere.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch that renders the atoms.</param>
        /// <param name="spriteTexture">The simple texture used when rendering.</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D spriteTexture)
        {
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (spriteTexture == null)
            {
                throw new ArgumentNullException("spriteTexture");
            }
            for (int i = 0; i < atomPositions.Length; i++)
            {
                Color atomColor = new Color(atomDepths[i], atomDepths[i],
                    atomDepths[i]);
                spriteBatch.Draw(spriteTexture, new Rectangle((int)atomPositions[i].X,
                    (int)atomPositions[i].Y, 1, 1), colors[random.Next(colors.Length)]);
            }
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Assign the target position for the Atmosphere to scroll to.
        /// </summary>
        /// <param name="targetPosition">The target position.</param>
        public void SetTargetPosition(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
        #endregion
    }
}

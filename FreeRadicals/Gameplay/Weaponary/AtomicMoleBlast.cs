#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
#endregion

namespace FreeRadicals.Gameplay.Weaponary
{
    class AtomicMoleBlast : AtomicMoleBlastWeapon
    {
        #region Constants
        /// <summary>
        /// The spread of the second and third laser projectiles' directions, in radians
        /// </summary>
        static readonly float laserSpreadRadians = MathHelper.ToRadians(10f);
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new triple-laser weapon.
        /// </summary>
        /// <param name="owner">The ship that owns this weapon.</param>
        public AtomicMoleBlast(NanoBot owner)
            : base(owner) { }
        #endregion

        #region Interaction
        /// <summary>
        /// Create and spawn the projectile(s) from a firing from this weapon.
        /// </summary>
        /// <param name="direction">The direction that the projectile will move.</param>
        protected override void CreateProjectiles(Vector2 direction)
        {
            // calculate the direction vectors for the second and third projectiles
            float rotation = (float)Math.Acos(Vector2.Dot(new Vector2(0f, -1f), 
                direction));
            rotation *= (Vector2.Dot(new Vector2(0f, -1f), 
                new Vector2(direction.Y, -direction.X)) > 0f) ? 1f : -1f;
            Vector2 direction2 = new Vector2(
                 (float)Math.Sin(rotation - laserSpreadRadians),
                -(float)Math.Cos(rotation - laserSpreadRadians));
            Vector2 direction3 = new Vector2(
                 (float)Math.Sin(rotation + laserSpreadRadians),
                -(float)Math.Cos(rotation + laserSpreadRadians));
            Vector2 direction4 = new Vector2(
                 (float)Math.Sin(rotation - laserSpreadRadians * 2f),
                -(float)Math.Cos(rotation - laserSpreadRadians * 2f));
            Vector2 direction5 = new Vector2(
                 (float)Math.Sin(rotation + laserSpreadRadians * 2f),
                -(float)Math.Cos(rotation + laserSpreadRadians * 2f));

            // create the first projectile
            AtomicMoleBlastProjectile projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction);
            // spawn the projectile
            projectile.Spawn(false);

            // create the second projectile
            projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction2);
            // spawn the projectile
            projectile.Spawn(false);

            // create the third projectile
            projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction3);
            // spawn the projectile
            projectile.Spawn(false);

            // create the forth projectile
            projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction4);
            // spawn the projectile
            projectile.Spawn(false);

            // create the fifth projectile
            projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction5);
            // spawn the projectile
            projectile.Spawn(false);

            // create the six projectile
            projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                -direction);
            // spawn the projectile
            projectile.Spawn(false);
        }
        #endregion
    }
}

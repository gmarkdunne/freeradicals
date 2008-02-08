#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Simulation;
#endregion

namespace FreeRadicals.Gameplay.Weaponary
{
    /// <summary>
    /// A weapon that shoots a rockets.
    /// </summary>
    class AtomicMoleBlastWeapon : Weapon
    {
        #region Initialization
        /// <summary>
        /// Constructs a new laser weapon.
        /// </summary>
        /// <param name="owner">The ship that owns this weapon.</param>
        public AtomicMoleBlastWeapon(NanoBot owner)
            : base(owner)
        {
            fireDelay = 0.15f;        
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Create and spawn the projectile(s) from a firing from this weapon.
        /// </summary>
        /// <param name="direction">The direction that the projectile will move.</param>
        protected override void CreateProjectiles(Vector2 direction)
        {
            // create the new projectile
            AtomicMoleBlastProjectile projectile = new AtomicMoleBlastProjectile(owner.World, owner,
                direction);
            // spawn the projectile
            projectile.Spawn(false);
        }
        #endregion
    }
}

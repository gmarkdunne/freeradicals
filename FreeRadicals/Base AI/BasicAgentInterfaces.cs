using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace FreeRadicals    
{
    /// <summary>
    /// This is a basic interface, inherit from it when creating your own agents and states.
    /// </summary>
    public interface IBaseAgent
    {
        void Patrol();
        void Bond();
        void Flee();
        bool isSafe();
        bool runAway();
        bool Chase();
        bool Seek();
    }
}
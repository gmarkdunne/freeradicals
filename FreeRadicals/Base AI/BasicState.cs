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
    /// This is the base state class, when creating any state this class will be inherited.
    /// Supprise eh....
    /// </summary>
    public class BasicState
    {
        public BasicState()
        { }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        /// <param name="agent">Agent that this state is asociated with</param>
        public virtual void Enter(BasicAgent agent)
        { }
        /// <summary>
        /// Called when the state is executed. 
        /// This is where all your AI logic will be called from.
        /// 
        /// It is important that your interface has the methods you need to access the associated agents
        /// world object members as you will want to use these fields/properties and/or methods to help
        /// weight the desisions your AI will take.
        /// </summary>
        /// <param name="agent">Agent that this state is asociated with</param>
        public virtual void Execute(BasicAgent agent)
        { }
        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        /// <param name="agent">Agent that this state is asociated with</param>
        public virtual void Exit(BasicAgent agent)
        { }
    }
}

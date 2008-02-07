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
    /// This is the base agent, all agents should be built from this template. As I see it, 99.99% of
    /// all NPC's are going to have these basic methods.
    /// 
    /// When creating a new AI you will inherit from this base agent class.
    /// </summary>
    public class BasicAgent
    {
        int id; // Unique id for this agent.
        static int nextValidID;

        // State the agent is currently in
        protected BasicState currentState;

        // The object that this agent is associated with.
        protected Object worldObject;

        /// <summary>
        /// Get this property gets this agents unique ID.
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Agent Contructor
        /// </summary>
        /// <param name="worldObject">Object this agent is associated with.</param>
        /// <param name="startState">Starting State</param>
        public BasicAgent(Object worldObject, BasicState startState)
        {
            this.worldObject = worldObject;
            currentState = startState;
        }

        /// <summary>
        /// Set this agents ID.
        /// </summary>
        private void SetID()
        {
            id = nextValidID++;
        }

        /// <summary>
        /// This method changes the current state to a new state. 
        /// </summary>
        /// <param name="state">State to change to</param>
        public virtual void ChangeState(BasicState state)
        {
            // Exit this state.
            currentState.Exit(this);
            // Set the new state.
            currentState = state;
            // Call the new states Enter method.
            currentState.Enter(this);
        }

        /// <summary>
        /// Run the state we are in. 
        /// </summary>
        public void ExecuteState()
        {
            currentState.Execute(this);
        }

        /// <summary>
        /// Basic check, "Am I safe?" 
        /// </summary>
        /// <returns>true if it's safe, false if not!</returns>
        public virtual bool isSafe()
        {
            return true;
        }

        /// <summary>
        /// Basic check, "Do I need to run away?" 
        /// </summary>
        /// <returns>true if I need to run away, or false if I can handle it</returns>
        public virtual bool runAway()
        {
            return false;
        }

        /// <summary>
        /// Run Away! 
        /// </summary>
        public virtual void Flee()
        { }

        /// <summary>
        /// Bond, Bond, Bond! 
        /// </summary>
        public virtual void Bond()
        { }

        /// <summary>
        /// Where is that target??? 
        /// </summary>
        public virtual void Patrol()
        { }

        /// <summary>
        /// Once target is found, give chase. 
        /// </summary>
        public virtual void Chase()
        { }

        /// <summary>
        /// Seek a certain target. 
        /// </summary>
        public virtual void Seek()
        { }
    }
}
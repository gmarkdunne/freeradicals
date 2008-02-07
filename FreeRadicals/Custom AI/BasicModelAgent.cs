using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FreeRadicals
{
    /// <summary>
    /// Well this is an example of an agent you can create with the base AI classes.
    /// 
    /// This example has just overriden the base methods, you could add new ones, just make sure
    /// you add them to your interface so that the model has them to run.
    /// </summary>
    public class BasicModelAgent : BasicAgent
    {
        public BasicModelAgent(Object worldObject, BasicState startState) : base(worldObject, startState)
        {

        }
        /// <summary>
        /// Method to get the current state.
        /// </summary>
        /// <returns></returns>
        public string State()
        {
            return this.currentState.ToString();
        }
        #region Implementation of the world object/interface methods
        // Our agent is quite basic in that it's AI is dependant on it being safe and weather
        // it needs to run away or not.
        //public override bool isSafe()
        //{
        //    return ((IBaseAgent)worldObject).isSafe();
        //}
        //public override bool runAway()
        //{
        //    return ((IBaseAgent)worldObject).runAway();
        //}
        public override void Patrol()
        {
            ((IBaseAgent)worldObject).Patrol();
        }
        //public override void Bond()
        //{
        //    ((IBaseAgent)worldObject).Bond();
        //}
        //public override void Flee()
        //{
        //    ((IBaseAgent)worldObject).Flee();
        //}
        //public override void Seek()
        //{
        //    ((IBaseAgent)worldObject).Seek();
        //}
        //public override void Chase()
        //{
        //    ((IBaseAgent)worldObject).Chase();
        //}
        #endregion
    }
}

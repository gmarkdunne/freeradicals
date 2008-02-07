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
    public class State_Flee : BasicState
    {
        public override void Execute(BasicAgent agent)
        {
            if (agent.isSafe())
                agent.ChangeState(new State_Patrol());
            else
                agent.Flee();

            base.Execute(agent);
        }
    }

    public class State_Bond : BasicState
    {
        public override void Execute(BasicAgent agent)
        {
            if (agent.isSafe())
                agent.ChangeState(new State_Patrol());
            else
            {
                if (agent.runAway())
                    agent.ChangeState(new State_Flee());
                else
                    agent.Bond();
            }

            base.Execute(agent);
        }
    }
    public class State_Patrol : BasicState
    {
        public override void Execute(BasicAgent agent)
        {
            //if (!agent.isSafe())
            //    agent.ChangeState(new State_Bond());
            //else
                agent.Patrol();

            //base.Execute(agent);
        }
    }
    public class State_Seek : BasicState
    {
        public override void Execute(BasicAgent agent)
        {
            if (!agent.isSafe())
                agent.ChangeState(new State_Seek());
            else
                agent.Patrol();

            base.Execute(agent);
        }
    }
    public class State_Chase : BasicState
    {
        public override void Execute(BasicAgent agent)
        {
            if (agent.isSafe())
                agent.ChangeState(new State_Patrol());
            else
            {
                if (agent.runAway())
                    agent.ChangeState(new State_Flee());
                else
                    agent.Chase();
            }

            base.Execute(agent);
        }
    }
}

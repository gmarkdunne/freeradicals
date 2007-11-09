//Copyright (c) 2007 GfxStorm.com
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.



using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;



namespace SceneGraphLibrary
{
    internal class Spring
    {
        #region Private Fields

        float tightness = 800.0f;
        float damping = 400.0f;
        float mass = 80.0f;
        Vector3 anchorPoint;
        Vector3 endPoint;
        Vector3 length;
        Vector3 velocity;

        #endregion



        #region Public Properties

        public float Tightness
        {
            get { return tightness; }
            set { tightness = value; }
        }

        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Vector3 AnchorPoint
        {
            get { return anchorPoint; }
            set { anchorPoint = value; }
        }

        public Vector3 Length
        {
            get { return length; }
            set { length = value; }
        }
        
        public Vector3 EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        #endregion



        #region Default Constructor

        public Spring()
        {
        }

        #endregion



        #region Public Methods

        public void Update(TimeSpan elapsedTime)
        {
            //Forumla for a spring http://www.gaffer.org/game-physics/spring-physics/
            //  F = -kx-bv
            //
            //Where:
            //  k = tightness of spring
            //  x = vector difference in points of spring
            //  b = coefficent of damping
            //  v = relative velocity between point of spring

            float timeDelta = (float)elapsedTime.TotalSeconds;

            Vector3 difference = endPoint - (anchorPoint + length);
            Vector3 force = -tightness * difference - damping * velocity;
            Vector3 acceleration = force / mass;
            
            velocity += acceleration * timeDelta;
            endPoint += velocity * timeDelta;
        }

        #endregion

    }
}

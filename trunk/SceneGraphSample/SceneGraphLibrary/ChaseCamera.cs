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
    /// <summary>
    /// Defines a spring-physics based camera that follows a target.
    /// </summary>
    public class ChaseCamera : Camera
    {
        #region Private Fields

        SceneNode target;
        Vector3 targetPositionOffset = new Vector3(0, 200.0f, 200.0f);
        Vector3 lookAtOffset = new Vector3(0, 10f, 0);
        Spring spring = new Spring();

        #endregion



        #region Public Properties

        /// <summary>
        /// The tightness of the spring.
        /// </summary>
        public float Tightness
        {
            get { return spring.Tightness; }
            set { spring.Tightness = value; }
        }

        /// <summary>
        /// The coefficient of damping that will be applied to the spring.
        /// </summary>
        public float Damping
        {
            get { return spring.Damping; }
            set { spring.Damping = value; }
        }

        /// <summary>
        /// The mass of the spring.
        /// </summary>
        public float Mass
        {
            get { return spring.Mass; }
            set { spring.Mass = value; }
        }

        /// <summary>
        /// An offset from the targets position where the camera will rest.
        /// </summary>
        public Vector3 TargetPositionOffset
        {
            get { return targetPositionOffset; }
            set { targetPositionOffset = value; }
        }

        /// <summary>
        /// An offset from the targets position where the camera will aim.
        /// </summary>
        public Vector3 LookAtOffset
        {
            get { return lookAtOffset; }
            set { lookAtOffset = value; }
        }

        /// <summary>
        /// The <see cref="SceneNode"/> the camera will follow.
        /// </summary>
        public SceneNode Target
        {
            get { return target; }
            set { target = value; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="ChaseCamera"/> class.
        /// </summary>
        public ChaseCamera()
        {
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="Camera.Update"/> method.
        /// </summary>
        /// <param name="sceneGraph"></param>
        public override void Update(SceneGraph sceneGraph)
        {
            if (target != null)
            {
                spring.AnchorPoint = target.AbsoluteTransform.Translation;
                spring.Length = Vector3.TransformNormal(targetPositionOffset, target.AbsoluteTransform);

                spring.Update(sceneGraph.GameTime.ElapsedGameTime);

                Position = spring.EndPoint;

                LookAt(target.AbsoluteTransform.Translation + Vector3.TransformNormal(lookAtOffset, target.AbsoluteTransform), target.AbsoluteTransform.Up);
            }

            base.Update(sceneGraph);
        }

        #endregion

    }
}

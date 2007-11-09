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
    /// Defines a controller that rotates a scene node around its X axis.
    /// </summary>
    public class XRotationController : ControllerBase
    {
        #region Private Fields

        float radiansPerSecond;

        #endregion



        #region Public Properties

        /// <summary>
        /// The amount of rotation per second in radians.
        /// </summary>
        public float RadiansPerSecond
        {
            get { return radiansPerSecond; }
            set { radiansPerSecond = value; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="XRotationController"/> class.
        /// </summary>
        public XRotationController()
        {
        }

        /// <summary>
        /// Intializes an instance of the <see cref="XRotationController"/> class.
        /// </summary>
        /// <param name="radiansPerSecond">The amount of rotation per second in radians.</param>
        public XRotationController(float radiansPerSecond)
        {
            this.radiansPerSecond = radiansPerSecond;
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="ControllerBase.UpdateSceneNode"/> method.
        /// </summary>
        /// <param name="node">The scene node that will be updated.</param>
        /// <param name="gameTime">The snapshot of the game time state.</param>
        public override void UpdateSceneNode(SceneNode node, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (radiansPerSecond != 0.0f)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float rotation = radiansPerSecond * elapsedTime;

                node.Rotation *= Quaternion.CreateFromYawPitchRoll(0, rotation, 0);
            }
        }

        #endregion

    }
}

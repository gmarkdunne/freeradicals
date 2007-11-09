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



namespace SceneGraphLibrary
{
    /// <summary>
    /// Defines a base class that implements <see cref="IController"/>.
    /// </summary>
    public abstract class ControllerBase : IController 
    {
        #region IController Members

        /// <summary>
        /// When overriden in a derrived class, allows the class to update the state of a scene node.
        /// </summary>
        /// <param name="node">The node that should be updated.</param>
        /// <param name="gameTime">A snapshot of the game time state.</param>
        public abstract void UpdateSceneNode(SceneNode node, Microsoft.Xna.Framework.GameTime gameTime);

        #endregion
    }
}

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
using Microsoft.Xna.Framework.Graphics;



namespace SceneGraphLibrary
{
    /// <summary>
    /// Defines a scene graph.
    /// </summary>
    public class SceneGraph
    {

        #region Private Fields
        
        SceneNode rootNode = new SceneNode();
        GraphicsDevice device;
        Camera camera;
        GameTime gameTime;
        int nodesCulled;
        bool cullingDisabled = false;

        #endregion



        #region Public Properties

        /// <summary>
        /// The <see cref="T:SceneNode"/> that is the root of the graph.
        /// </summary>
        public SceneNode RootNode
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        /// <summary>
        /// The <see cref="Camera"/> that will be used.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        /// <summary>
        /// A snapshot of the game time state.
        /// </summary>
        public GameTime GameTime
        {
            get { return gameTime; }
        }

        /// <summary>
        /// The <see cref="GraphicsDevice"/> that will be used for rendering.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return device; }
        }

        /// <summary>
        /// The number of scene nodes culled during the last draw operation.
        /// </summary>
        public int NodesCulled
        {
            get { return nodesCulled; }
        }

        /// <summary>
        /// Determines whether the scene graph will cull objects.  The default is enabled (false).
        /// </summary>
        public bool CullingDisabled
        {
            get { return cullingDisabled; }
            set { cullingDisabled = value; }
        }

        #endregion



        #region Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="SceneGraph"/> class.
        /// </summary>
        /// <param name="device">The <see cref="GraphicsDevice"/> that will be used for rendering.</param>
        public SceneGraph(GraphicsDevice device)
        {
            this.device = device;
        }

        #endregion



        #region Private Methods

        void CalculateTransformsRecursive(SceneNode node)
        {
            node.AbsoluteTransform = Matrix.CreateTranslation(node.Offset) * Matrix.CreateFromQuaternion(node.Rotation) * node.AbsoluteTransform * Matrix.CreateTranslation(node.Position);

            //Update children recursively
            foreach (SceneNode childNode in node.Children)
            {
                childNode.AbsoluteTransform = node.AbsoluteTransform;
                CalculateTransformsRecursive(childNode);
            }
        }

        void UpdateRecursive(SceneNode node)
        {
            //Update node
            node.Update(this);

            //Update children recursively
            foreach (SceneNode childNode in node.Children)
            {
                UpdateRecursive(childNode);
            }
        }

        void DrawRecursive(SceneNode node)
        {
            //Draw
            if (node.Visible)
            {
                BoundingSphere transformedSphere = new BoundingSphere();

                transformedSphere.Center = Vector3.Transform(node.BoundingSphere.Center, node.AbsoluteTransform);
                transformedSphere.Radius = node.BoundingSphere.Radius;

                if (cullingDisabled || camera.Frustum.Intersects(transformedSphere))
                {
                    node.Draw(this);
                }
                else
                {
                    nodesCulled++;
                }
            }

            foreach (SceneNode childNode in node.Children)
            {
                DrawRecursive(childNode);
            }
        }

        void CalculateTransforms()
        {
            CalculateTransformsRecursive(rootNode);
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Allows the scene graph to update its state.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            gameTime = time;

            if (camera != null)
                camera.Update(this);

            UpdateRecursive(rootNode);
            CalculateTransformsRecursive(rootNode);
        }

        /// <summary>
        /// Allows the scene graph to render.
        /// </summary>
        public void Draw()
        {
            nodesCulled = 0;
            rootNode.AbsoluteTransform = Matrix.Identity;

            DrawRecursive(rootNode);
        }

        #endregion

    }

}

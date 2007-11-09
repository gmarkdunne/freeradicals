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
    /// Defines a scene node that displays a plane along the X and Z axis.
    /// </summary>
    public class GroundPlaneSceneNode : SceneNode
    {
        #region Private Fields

        Texture2D texture;
        int width = 131070;
        int height = 131070;
        int textureRepeats = 1000;
        VertexDeclaration vertexDeclaration;
        BasicEffect basicEffect;
        VertexBuffer vertexBuffer;

        #endregion



        #region Public Properties

        /// <summary>
        /// The length of the plane along the X axis.
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                vertexBuffer = null;
            }
        }

        /// <summary>
        /// The length of the plane along the Z axis.
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                vertexBuffer = null;
            }
        }

        /// <summary>
        /// The texture that will be displayed on the plane.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// The number of times the texture will repeat along each axis.
        /// </summary>
        public int TextureRepeats
        {
            get { return textureRepeats; }
            set
            {
                textureRepeats = value;
                vertexBuffer = null;
            }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="T:GroundPlaneSceneNode"/> class.
        /// </summary>
        public GroundPlaneSceneNode()
        {
        }

        #endregion



        #region Private Methods

        private void CreateGroundPlane(GraphicsDevice device)
        {
            //Create the effect that will be used to render
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device, null);
            }

            //Create the vertex declaration
            if (vertexDeclaration == null)
            {
                vertexDeclaration = new VertexDeclaration(device,
                                VertexPositionTexture.VertexElements);
            }

            //Create the quad's vertices
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];

            vertices[0].Position = new Vector3(width, 0, height);
            vertices[1].Position = new Vector3(-width, 0, height);
            vertices[2].Position = new Vector3(-width, 0, -height);
            vertices[3].Position = new Vector3(width, 0, -height);

            vertices[0].TextureCoordinate = new Vector2(0, 0);
            vertices[1].TextureCoordinate = new Vector2(textureRepeats, 0);
            vertices[2].TextureCoordinate = new Vector2(textureRepeats, textureRepeats);
            vertices[3].TextureCoordinate = new Vector2(0, textureRepeats);

            //Create the vertex buffer from the vertices
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, ResourceUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

        }

        #endregion



        #region Protected Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.GetBoundingSphere"/> method.
        /// </summary>
        /// <returns></returns>
        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.Zero, Math.Max(width, height));
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.Draw"/> method. 
        /// </summary>
        /// <param name="sceneGraph"></param>
        public override void Draw(SceneGraph sceneGraph)
        {
            if (vertexBuffer == null)
                CreateGroundPlane(sceneGraph.GraphicsDevice);

            if (sceneGraph.Camera != null)
            {
                // Set the effect parameters
                basicEffect.Texture = texture;
                basicEffect.TextureEnabled = true;
                basicEffect.World = AbsoluteTransform;
                basicEffect.View = sceneGraph.Camera.View;
                basicEffect.Projection = sceneGraph.Camera.Projection;

                // Draw the quad.
                basicEffect.Begin(SaveStateMode.SaveState);

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    //Set the vertex info
                    sceneGraph.GraphicsDevice.VertexDeclaration = vertexDeclaration;
                    sceneGraph.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);

                    //Draw
                    sceneGraph.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);

                    pass.End();
                }

                basicEffect.End();
            }

        }

        #endregion

    }
}

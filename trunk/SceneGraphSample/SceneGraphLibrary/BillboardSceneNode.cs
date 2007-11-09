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
    /// Defines a scene node with at texture that always faces the camera.
    /// </summary>
    public class BillboardSceneNode : SceneNode
    {
        #region Private Fields

        Texture2D texture;
        Vector2 scale = Vector2.One;
        Vector2 size = Vector2.One;
        VertexDeclaration vertexDeclaration;
        BasicEffect basicEffect;
        VertexBuffer vertexBuffer;

        #endregion



        #region Public Properties

        /// <summary>
        /// The texture that the scene node will displayed.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// The scaling that will be applied to the scene node's size.
        /// </summary>
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// The scene nodes dimensions.
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="BillboardSceneNode"/> class.
        /// </summary>
        public BillboardSceneNode()
        {
        }

        #endregion



        #region Private Methods

        private void CreateQuad(GraphicsDevice device)
        {
            //Create the effect that will be used to render
            basicEffect = new BasicEffect(device, null);
            
            //Create the vertex declaration
            vertexDeclaration = new VertexDeclaration(device,
                            VertexPositionTexture.VertexElements);

            //Create the quad's vertices
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];

            vertices[0].Position = new Vector3(1, 1, 0);
            vertices[1].Position = new Vector3(-1, 1, 0);
            vertices[2].Position = new Vector3(-1, -1, 0);
            vertices[3].Position = new Vector3(1, -1, 0);

            vertices[0].TextureCoordinate = new Vector2(0, 0);
            vertices[1].TextureCoordinate = new Vector2(1, 0);
            vertices[2].TextureCoordinate = new Vector2(1, 1);
            vertices[3].TextureCoordinate = new Vector2(0, 1);

            //Create the vertex buffer from the vertices
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, ResourceUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        #endregion



        #region Protected Methods
        
        /// <summary>
        /// Overrides the <see cref="SceneNode.GetBoundingSphere"/> method.
        /// </summary>
        /// <returns>A BoundingSphere structure that contains the bounds of the scene.</returns>
        protected override BoundingSphere GetBoundingSphere()
        {
            Vector3 actualSize = new Vector3(size.X, size.Y, 0) * new Vector3(scale.X, scale.Y, 0);
            Vector3 middle = actualSize - (actualSize / 2.0f);

            Vector3[] points = new Vector3[2];

            points[0] = -middle;
            points[1] = middle;

            return BoundingSphere.CreateFromPoints(points);
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.Draw"/> method.
        /// </summary>
        /// <param name="sceneGraph"></param>
        public override void Draw(SceneGraph sceneGraph)
        {
            if (texture != null)
            {
                if (vertexDeclaration == null)
                    CreateQuad(sceneGraph.GraphicsDevice);

                if (sceneGraph.Camera != null)
                {
                    Vector2 finalScale = size * scale;

                    Matrix matrix = Matrix.CreateScale(new Vector3(finalScale.X, finalScale.Y, 1)) *
                        Matrix.CreateConstrainedBillboard(AbsoluteTransform.Translation, sceneGraph.Camera.Position, Vector3.Up, null, null);

                    #region Set the effect parameters

                    basicEffect.Texture = texture;
                    basicEffect.TextureEnabled = true;
                    basicEffect.World = matrix;
                    basicEffect.View = sceneGraph.Camera.View;
                    basicEffect.Projection = sceneGraph.Camera.Projection;

                    #endregion

                    #region Draw the quad

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

                    #endregion
                }
            }
        }

        #endregion

    }
}

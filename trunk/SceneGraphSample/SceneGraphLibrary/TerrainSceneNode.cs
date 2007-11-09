//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;




//namespace SceneGraphLibrary
//{
//    public class TerrainSceneNode : SceneNode
//    {

//        VertexDeclaration vertexDeclaration;
//        VertexBuffer vertexBuffer;
//        BasicEffect effect;
//        int primitiveCount;
//        Vector3 scale = Vector3.One;
//        Texture2D texture;



//        public Vector3 Scale
//        {
//            get { return scale; }
//            set { scale = value; }
//        }

//        public Texture2D Texture
//        {
//            get { return texture; }
//            set { texture = value; }
//        }



//        public TerrainSceneNode()
//        {
//        }


//        public void CreateVertices(Texture2D heightMap)
//        {
//            if (heightMap.Format != SurfaceFormat.Color)
//                throw new NotSupportedException("Only the Color format is supported for terrain generation");

//            vertexDeclaration = new VertexDeclaration(heightMap.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

//            Color[] data = new Color[heightMap.Width * heightMap.Height];

//            heightMap.GetData<Color>(data);

//            List<VertexPositionNormalTexture> positions = new List<VertexPositionNormalTexture>();
//            //List<Vector2> uv = new List<Vector2>();
//            float width = heightMap.Width;
//            float height = heightMap.Height;

//            for (int y = 0; y < heightMap.Height - 2; y++)
//            {
//                for (int x = 0; x < heightMap.Width - 2; x++)
//                {
//                    int xCoord = x - (heightMap.Width / 2);
//                    int yCoord = y - (heightMap.Height / 2);

//                    int index00 = (y * heightMap.Width) + x;
//                    int index10 = (y * heightMap.Width) + x + 1;
//                    int index11 = ((y + 1) * heightMap.Width) + x + 1;
//                    int index01 = ((y + 1) * heightMap.Width) + x;

//                    Color sample00 = data[index00];
//                    Color sample10 = data[index10];
//                    Color sample11 = data[index11];
//                    Color sample01 = data[index01];

//                    //Create a quad from clockwise triangles
//                    Vector3 point1 = new Vector3(xCoord, sample00.R, yCoord);
//                    Vector3 point2 = new Vector3(xCoord + 1, sample10.R, yCoord);
//                    Vector3 point3 = new Vector3(xCoord + 1, sample11.R, yCoord + 1);
//                    Vector3 point4 = new Vector3(xCoord, sample01.R, yCoord + 1);

//                    positions.Add(new VertexPositionNormalTexture(point1, Vector3.Up, new Vector2( x / width,  y / height)));
//                    positions.Add(new VertexPositionNormalTexture(point2, Vector3.Up, new Vector2((x + 1) / width, y / height)));
//                    positions.Add(new VertexPositionNormalTexture(point3, Vector3.Up, new Vector2((x + 1) / width, (y + 1) / height)));

//                    positions.Add(new VertexPositionNormalTexture(point1, Vector3.Up, new Vector2(x / width, y / height)));
//                    positions.Add(new VertexPositionNormalTexture(point3, Vector3.Up, new Vector2((x + 1) / width, (y + 1) / height)));
//                    positions.Add(new VertexPositionNormalTexture(point4, Vector3.Up, new Vector2(x / width, (y + 1) / height)));

//                }
//            }

//            vertexBuffer = new VertexBuffer(heightMap.GraphicsDevice, typeof(VertexPositionNormalTexture), positions.Count, ResourceUsage.WriteOnly);
//            vertexBuffer.SetData<VertexPositionNormalTexture>(positions.ToArray());

//            effect = new BasicEffect(heightMap.GraphicsDevice, null);
//            effect.EnableDefaultLighting();

//            primitiveCount = positions.Count / 3;

//        }


//        public override void Draw(SceneGraph sceneGraph)
//        {

//            if (vertexDeclaration != null)
//            {
//                sceneGraph.GraphicsDevice.VertexDeclaration = vertexDeclaration;
//                sceneGraph.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
//                //sceneGraph.GraphicsDevice.RenderState.CullMode = CullMode.None;
//                //sceneGraph.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

//                #region Set the effect parameters

//                effect.Texture = texture;
//                effect.TextureEnabled = true;
//                effect.World = Matrix.CreateScale(scale) * AbsoluteTransform;
//                effect.View = sceneGraph.Camera.View;
//                effect.Projection = sceneGraph.Camera.Projection;

//                #endregion

//                effect.Begin(SaveStateMode.SaveState);

//                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//                {
//                    pass.Begin();

//                    sceneGraph.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, primitiveCount);

//                    pass.End();
//                }

//                effect.End();

//            }

//        }

//    }
//}

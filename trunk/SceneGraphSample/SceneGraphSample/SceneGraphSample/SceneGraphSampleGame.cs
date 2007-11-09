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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using SceneGraphLibrary;



namespace SceneGraphSample
{
    public class SceneGraphSampleGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        SceneGraph sceneGraph;
        TimeSpan lastFrameReportTime = TimeSpan.Zero;
        int frameCount = 0;
        int framesPerSecond = 0;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D background;



        public SceneGraphSampleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
        }



        protected override void Initialize()
        {
            // Draw as fast as possible :)
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("Content/DefaultFont");
            background = content.Load<Texture2D>("Content/Luas");

            LoadDemo();
        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Toggle Full Screen mode
            if (kState.IsKeyDown(Keys.F))
            {
                graphics.ToggleFullScreen();
            }

            // Update the scene-graph
            if (sceneGraph != null)
            {                
                sceneGraph.Update(gameTime);
            }

            // Update other components
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            // Caculate frames per second
            lastFrameReportTime += gameTime.ElapsedGameTime;
            frameCount++;

            if (lastFrameReportTime.TotalSeconds >= 1.0f)
            {
                framesPerSecond = frameCount;
                frameCount = 0;
                lastFrameReportTime = TimeSpan.Zero;
            }

            // Clear the scene
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the background
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Texture, SaveStateMode.SaveState);
            spriteBatch.Draw(background, new Rectangle(0,0,graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
            
            // Draw the scene-graph
            if (sceneGraph != null)
                sceneGraph.Draw();

            // Draw the scene statistics
            string message = string.Format("FPS: {0}\nNodes Culled: {1}", framesPerSecond, sceneGraph.NodesCulled);

            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
            spriteBatch.DrawString(spriteFont, message, Vector2.Zero, Color.WhiteSmoke);
            spriteBatch.End();

            // Draw other game components
            base.Draw(gameTime);
        }

        private void LoadDemo()
        {
            // Creates the following scene graph
            // 
            // root
            //  |
            //  |---Parent
            //  |     |
            //  |     ---Child
            //  |          |
            //  |          ---Grandchild
            //  |               |
            //  |               ---Great Grandchild


            // Create a scene graph
            sceneGraph = new SceneGraph(graphics.GraphicsDevice);

            // Create a node for the Parent
            ModelSceneNode parentNode = new ModelSceneNode(content.Load<Model>("Content/teapot"));
            parentNode.Position = new Vector3(0, 0, 0);
            parentNode.Scale = 150;

            // Create a node for the Child
            ModelSceneNode childNode = new ModelSceneNode(content.Load<Model>("Content/teapot"));
            childNode.Offset = new Vector3(0, 0, 500);
            childNode.Scale = 75f;

            // Create a model for the Grandchild
            ModelSceneNode grandchildNode = new ModelSceneNode(content.Load<Model>("Content/teapot"));
            grandchildNode.Offset = new Vector3(0, 0, 250);
            grandchildNode.Scale = 37.5f;

            // Create a model for the Great Grandchild
            ModelSceneNode greatGrandchildNode = new ModelSceneNode(content.Load<Model>("Content/teapot"));
            greatGrandchildNode.Offset = new Vector3(0, 0, 125);
            greatGrandchildNode.Scale = 18.75f;

            // Attach the Child to the Parent
            parentNode.Children.Add(childNode);

            // Attach the Grandchild to the Child
            childNode.Children.Add(grandchildNode);

            // Attach the Grandchild to the Child
            grandchildNode.Children.Add(greatGrandchildNode);

            // Attach the Parent to the scene graph
            sceneGraph.RootNode.Children.Add(parentNode);

            // Create a rotation controller for the Parent
            YRotationController parentRotation = new YRotationController();
            parentRotation.RadiansPerSecond = (float)(Math.PI / 16.0);

            // Attach it to the Parent
            parentNode.Controller = parentRotation;

            // Create a rotation controller for the Child
            XRotationController childRotation = new XRotationController();
            childRotation.RadiansPerSecond = (float)(Math.PI / 16.0);

            // Attach it to the Child
            childNode.Controller = childRotation;

            // Create a rotation controller for the Grandchild
            YRotationController grandchildRotation = new YRotationController();
            grandchildRotation.RadiansPerSecond = -(float)(Math.PI / 16.0);

            // Attach it to the Grandchild
            grandchildNode.Controller = grandchildRotation;

            // Create a rotation controller for the Great Grandchild
            YRotationController greatGrandRotation = new YRotationController();
            greatGrandRotation.RadiansPerSecond = -(float)(Math.PI / 16.0);

            // Attach it to the Great Grandchild
            greatGrandchildNode.Controller = greatGrandRotation;

            // Create a camera and position it to view the scene
            Camera camera = new Camera();
            camera.Position = new Vector3(0, 0, 1400);
            camera.FarPlane = 4000;

            // Attach the camera to the scene graph
            sceneGraph.Camera = camera;
        }
    }
}

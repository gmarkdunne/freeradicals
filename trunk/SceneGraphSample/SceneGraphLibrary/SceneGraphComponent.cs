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

    public class SceneGraphComponent :  DrawableGameComponent , ISceneGraphService 
    {
        public event EventHandler SceneGraphCreated;

        SceneGraph sceneGraph;
        Game game;


        #region ISceneGraphService Members

        public SceneGraph SceneGraph
        {
            get { return sceneGraph; }
        }

        #endregion




        public SceneGraphComponent(Game game)
            : base(game)
        {
            this.game = game;

            game.Services.AddService(typeof(ISceneGraphService), this);            
        }

        ~SceneGraphComponent()
        {
            game.Services.RemoveService(typeof(ISceneGraphService));
        }



        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            IGraphicsDeviceService iGraphicsDeviceService = game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService ;

            if (iGraphicsDeviceService == null)
                throw new InvalidOperationException("The IGraphicsDeviceService could not be found.");

            sceneGraph = new SceneGraph(iGraphicsDeviceService.GraphicsDevice);

            OnSceneGraphCreated();

            base.LoadGraphicsContent(loadAllContent);
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            sceneGraph = null;

            base.UnloadGraphicsContent(unloadAllContent);
        }

        protected void OnSceneGraphCreated()
        {
            EventHandler handler = SceneGraphCreated;

            if (handler !=null)
                handler.Invoke(this, EventArgs.Empty);
        }

        public override void Update(GameTime gameTime)
        {
            sceneGraph.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sceneGraph.Draw();

            base.Draw(gameTime);
        }

    }
}

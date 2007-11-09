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
    /// Defines a scene node that displays text that always faces the camera.
    /// </summary>
    public class TextBillboardSceneNode : BillboardSceneNode
    {
        #region Private Fields

        string text = string.Empty;
        bool isDirty = true;
        SpriteFont font;
        Color background = Color.Black;
        Color foreground = Color.White;

        #endregion



        #region Public Properties

        /// <summary>
        /// The text that will be displayed.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    text = string.Empty;
                else
                    text = value;

                isDirty = true;
            }
        }

        /// <summary>
        /// The <see cref="SpriteFont"/> that will be used to display the text.
        /// </summary>
        public SpriteFont SpriteFont
        {
            get { return font; }
            set { font = value; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="TextBillboardSceneNode"/> class.
        /// </summary>
        public TextBillboardSceneNode()
        {
        }

        #endregion



        #region Private Methods

        void UpdateTexture(GraphicsDevice device)
        {
            if (String.IsNullOrEmpty(text))
            {
                Texture = null;
            }
            else
            {
                if (font == null)
                    throw new InvalidOperationException("The font has not been set.");

                Vector2 textSize = font.MeasureString(text);
                Size = textSize;

                #region Draw the text to a RenderTarget
                
                SpriteBatch spriteBatch = new SpriteBatch(device);
                RenderTarget2D renderTarget = new RenderTarget2D(device, (int)textSize.X, (int)textSize.Y, 1, SurfaceFormat.Color);
                RenderTarget2D oldTarget = device.GetRenderTarget(0) as RenderTarget2D;

                device.SetRenderTarget(0, renderTarget);

                device.Clear(background);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
                spriteBatch.DrawString(font, text, Vector2.Zero, foreground);
                spriteBatch.End();

                device.ResolveRenderTarget(0);

                Texture = renderTarget.GetTexture();

                device.SetRenderTarget(0, oldTarget);

                #endregion
            }

        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.Draw"/> method.
        /// </summary>
        /// <param name="sceneGraph"></param>
        public override void Draw(SceneGraph sceneGraph)
        {
            //Update the Texture if necessary
            if (isDirty)
            {
                UpdateTexture(sceneGraph.GraphicsDevice);
                isDirty = false;
            }

            //Draw the billboard
            if (Texture != null)
            {
                base.Draw(sceneGraph);
            }
        }

        #endregion

    }
}

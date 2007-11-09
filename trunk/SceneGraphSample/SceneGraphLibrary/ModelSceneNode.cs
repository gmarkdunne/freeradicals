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
    /// Defines a scene node that displays a model.
    /// </summary>
    public class ModelSceneNode : SceneNode
    {
        #region Private Fields

        Model model;
        BoundingSphere modelSphere;
        float scale = 1.0f;

        #endregion


        #region Public Properties

        /// <summary>
        /// The <see cref="Model"/> that will be displayed.
        /// </summary>
        public Model Model
        {
            get { return model; }
            set
            {
                model = value;

                if (model == null)
                    modelSphere = new BoundingSphere(Offset, 0);
                else
                    CalculateBoundingSphere();
            }
        }

        /// <summary>
        ///  The scaling factor to apply to the model.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;

                if (model != null)
                    CalculateBoundingSphere();

            }
        }

        #endregion



        #region Constructors

        /// <summary>
        /// Intializes an instance of the <see cref="ModelSceneNode"/> class.
        /// </summary>
        public ModelSceneNode()
        {
        }

        /// <summary>
        /// Intializes an instance of the <see cref="ModelSceneNode"/> class.
        /// </summary>
        /// <param name="model">The model that will be displayed.</param>
        public ModelSceneNode(Model model)
        {
            this.model = model;
            CalculateBoundingSphere();
        }

        #endregion



        #region Private Methods

        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model

            modelSphere = new BoundingSphere();

            foreach (ModelMesh mesh in model.Meshes)
            {
                modelSphere = Microsoft.Xna.Framework.BoundingSphere.CreateMerged(modelSphere, model.Meshes[0].BoundingSphere);
            }
        }

        #endregion



        #region Protected Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.GetBoundingSphere"/> method.
        /// </summary>
        /// <returns></returns>
        protected override BoundingSphere GetBoundingSphere()
        {
            return modelSphere;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="SceneNode.Draw"/> methods.
        /// </summary>
        /// <param name="sceneGraph"></param>
        public override void Draw(SceneGraph sceneGraph)
        {
            if (sceneGraph.Camera != null)
            {
                if (model != null)
                {
                    //Copy the model heiarchy transforms
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    #region Render each mesh in the model

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        Matrix finalScale = Matrix.CreateScale(scale) * transforms[mesh.ParentBone.Index] * AbsoluteTransform;

                        foreach (Effect effect in mesh.Effects)
                        {
                            BasicEffect basicEffect = effect as BasicEffect;

                            if (basicEffect == null)
                            {
                                IEffectTransforms effectTransforms = effect as IEffectTransforms;

                                if (effectTransforms == null)
                                {
                                    throw new NotSupportedException("Currently only the BasicEffect and effects that implement IEffectTransforms are supported in a model.");
                                }

                                #region Render using an effect that implements IEffectTransforms

                                //Set the matrices
                                effectTransforms.World = finalScale;
                                effectTransforms.View = sceneGraph.Camera.View;
                                effectTransforms.Projection = sceneGraph.Camera.Projection;

                                #endregion
                            }
                            else
                            {
                                #region Render using BasicEffect

                                //Set the matrices
                                basicEffect.World = finalScale; 
                                basicEffect.View = sceneGraph.Camera.View;
                                basicEffect.Projection = sceneGraph.Camera.Projection;
                                basicEffect.EnableDefaultLighting();
                                basicEffect.PreferPerPixelLighting = true;
                                
                                #endregion
                            }
                        }

                        mesh.Draw(SaveStateMode.SaveState);
                    }

                    #endregion
                }
            }

        }

        #endregion

    }
}

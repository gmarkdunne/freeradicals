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
    /// Defines a controller that moves along a path defined by waypoints.
    /// </summary>
    public class PathFollowingController : ControllerBase
    {
        #region Private Fields

        WayPointCollection wayPoints = new WayPointCollection();
        TimeSpan pathTimeLength = new TimeSpan(0, 0, 10); //Default to 10 seconds
        TimeSpan currentTime = TimeSpan.Zero;

        #endregion


        #region Public Properties

        /// <summary>
        /// A collection of waypoints that define the path to follow.
        /// </summary>
        public WayPointCollection WayPoints
        {
            get { return wayPoints; }
        }

        /// <summary>
        /// The amount of time to complete the path.
        /// </summary>
        public TimeSpan PathTimeLength
        {
            get { return pathTimeLength; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="PathFollowingController"/> class.
        /// </summary>
        public PathFollowingController()
        {
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Overrides the <see cref="ControllerBase.UpdateSceneNode"/> method.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="gameTime"></param>
        public override void UpdateSceneNode(SceneNode node, GameTime gameTime)
        {
            currentTime += gameTime.ElapsedGameTime;

            if (currentTime >= pathTimeLength)
                currentTime = TimeSpan.Zero;

            if (wayPoints.Count > 0)
            {
                double pathTime = pathTimeLength.TotalSeconds;
                double timePerWayPoint = pathTime / wayPoints.Count;

                int currentWayPoint = (int)((currentTime.TotalSeconds / pathTime) * (wayPoints.Count));
                int nextWayPoint = currentWayPoint + 1;

                if (nextWayPoint > wayPoints.Count - 1)
                    nextWayPoint = 0;

                //Determine blend amount
                double startTime = timePerWayPoint * currentWayPoint;
                double endTime = startTime + timePerWayPoint;

                float amount = (float)((currentTime.TotalSeconds - startTime) / (endTime - startTime));

                //Lerp the position
                node.Position = Vector3.Lerp(wayPoints[currentWayPoint].Position, wayPoints[nextWayPoint].Position, amount);

                //Slerp the rotation
                node.Rotation = Quaternion.Slerp(wayPoints[currentWayPoint].Rotation, wayPoints[nextWayPoint].Rotation, amount);

            }
        }

        /// <summary>
        /// Resets the current location to the beginning of the path.
        /// </summary>
        public void ResetTime()
        {
            currentTime = TimeSpan.Zero;
        }

        #endregion

    }
}

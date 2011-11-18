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


namespace DGui
{
    /// <summary>
    /// Defines a camera.
    /// </summary>
    class Camera
    {
        #region Private Fields

        Vector3 position = Vector3.Zero;
        Quaternion rotation = Quaternion.Identity;
        float fieldOfView = (float)(System.Math.PI / 4.0);
        float aspectRatio = 4f / 3f;  // Width/Height
        float nearPlane = 1.0f;
        float farPlane = 10500.0f;
        Vector3 direction = Vector3.Forward;
        Matrix view;
        Matrix projection;
        BoundingFrustum frustum;

        Rectangle boundingRectangle;
        bool useRectangleBounding = false;

        #endregion



        #region Public Properties

        /// <summary>
        /// The camera's maximum movement range.
        /// </summary>
        public Rectangle BoudingRectangle
        {
            get { return boundingRectangle; }
            set { boundingRectangle = value; }
        }

        public bool UseRectangleBounding
        {
            get { return useRectangleBounding; }
            set { useRectangleBounding = value; }
        }


        /// <summary>
        /// The camera's location in space.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set
            {
                if (useRectangleBounding)
                {
                    Point camPosition2D = new Point((int)value.X * -1, (int)value.Y * -1);
                    if (boundingRectangle.Contains(camPosition2D))
                    {
                        position = value;
                    }
                    else
                    {
                        // Else, clip axes properly
                        Vector3 newPosition = value;
                        if (camPosition2D.X < boundingRectangle.X)
                            newPosition.X = boundingRectangle.X;
                        else if (camPosition2D.X > (boundingRectangle.X + boundingRectangle.Width))
                            newPosition.X = (boundingRectangle.X + boundingRectangle.Width) * -1;
                        if (camPosition2D.Y < boundingRectangle.Y)
                            newPosition.Y = boundingRectangle.Y;
                        else if (camPosition2D.Y > (boundingRectangle.Y + boundingRectangle.Height))
                            newPosition.Y = (boundingRectangle.Y + boundingRectangle.Height) * -1;

                        // Apply the clipped position
                        position = newPosition;
                    }
                }
                else
                {
                    position = value;
                }
            }
        }

        /// <summary>
        /// The camera's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// The camera's field of view in radians.
        /// </summary>
        public float FieldOfView
        {
            get { return fieldOfView; }
            set { fieldOfView = value; }
        }

        /// <summary>
        /// The cameras width to height ratio.
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }

        /// <summary>
        /// The near clipping plane.
        /// </summary>
        public float NearPlane
        {
            get { return nearPlane; }
            set { nearPlane = value; }
        }

        /// <summary>
        /// The far clipping plane.
        /// </summary>
        public float FarPlane
        {
            get { return farPlane; }
            set { farPlane = value; }
        }

        /// <summary>
        /// A normal indicating the direction the camera is facing.
        /// </summary>
        public Vector3 Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// The camera's view matrix.
        /// </summary>
        public Matrix View
        {
            get { return view; }
        }

        /// <summary>
        /// The camera's projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }

        /// <summary>
        /// The camera's frustum.
        /// </summary>
        public BoundingFrustum Frustum
        {
            get { return frustum; }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera()
        {
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Aims the camera at a point in space.
        /// </summary>
        /// <param name="target">The point in space the camara will face.</param>
        /// <param name="up">A normal indicating the up direction.</param>
        public void LookAt(Vector3 target, Vector3 up)
        {
            view = Matrix.CreateLookAt(position, target, up);

            rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(view));

            direction = target - position;
            direction.Normalize();
        }

        /// <summary>
        /// Allows a camera to update its state.
        /// </summary>
        /// <param name="sceneGraph">The <see cref="SceneGraph"/> that is updating the camera.</param>
        public virtual void Update(DGuiSceneGraph sceneGraph)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);

            Matrix matrix = Matrix.CreateFromQuaternion(rotation);
            matrix.Translation = position;

            view = Matrix.Invert(matrix);

            frustum = new BoundingFrustum( Matrix.Multiply(view, projection));

            direction = -view.Forward;
            direction.Normalize();

        }

        #endregion

    }

}

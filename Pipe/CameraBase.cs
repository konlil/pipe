using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class CameraBase
    {
        private PipeEngine engine;

        private Vector3 position;
        private float fov_x;
        private float near_plane;
        private float far_plane;

        private Matrix view_matrix;
        private Matrix projection_matrix;

        private bool need_update;

        public CameraBase(PipeEngine engine)
        {
            this.engine = engine;

            position = Vector3.Zero;
            fov_x = 45.0f;
            near_plane = 0.1f;
            far_plane = 1000.0f;

            view_matrix = Matrix.Identity;
            projection_matrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov_x), engine.GraphicsDevice.Viewport.AspectRatio, near_plane, far_plane);
        }

        protected virtual void UpdateMatrix()
        {

        }
    }
}

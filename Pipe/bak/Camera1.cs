using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pipe
{
    class Camera1
    {
        private PipeEngine engine;

        private Vector3 position;
        private Matrix rotation;
        private Vector3 target;
        private Vector3 offset;

        private float speed;

        private Matrix view_matrix;
        private Matrix projection_matrix;
       
        public Camera1(PipeEngine engine)
        {
            this.engine = engine;

            ResetCamera();
        }

        private void ResetCamera()
        {
            target = new Vector3(0, 0, 0);
            offset = new Vector3(0, 20, 50);

            position = target + offset;

            speed = .03f;

            rotation = Matrix.Identity;
            view_matrix = Matrix.Identity;
            projection_matrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), engine.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
        }

        private void HandleKeyboard()
        {
            Vector3 dir = new Vector3(0, 0, 0);

            if(Input.Keyboard.IsKeyDown(Keys.W))
                dir.Z += 1.0f;
            if (Input.Keyboard.IsKeyDown(Keys.S))
                dir.Z -= 1.0f;
            if (Input.Keyboard.IsKeyDown(Keys.A))
                dir.X -= 1.0f;
            if (Input.Keyboard.IsKeyDown(Keys.D))
                dir.X += 1.0f;

            if(dir == Vector3.Zero)
                return dir;

            if (dir.Z < 0)
                speed *= 0.5f;

            dir = Vector3.Transform(dir, rotation);
            dir.Y = 0;
            dir.Normalize();
            dir *= speed;
        }

        /// <summary>
        /// 鼠标X方向拖动，绕up方向旋转
        /// 鼠标Y方向拖动，绕right方向旋转
        /// </summary>
        private void HandleMouse()
        {
            float dx = Input.MouseDeltaX;
            float dy = Input.MouseDeltaY;

            Matrix rot = Matrix.CreateFromAxisAngle(Vector3.Up, dx * 3.14159 / 300);
        }
    }
}

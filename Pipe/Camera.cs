using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pipe
{
    public class Camera
    {
        private PipeEngine engine;

        private Vector3 position;
        private Matrix rotation;
        private Vector3 target;
        private Vector3 offset;

        private float rotation_speed;
        private float moving_speed;
        private float zoom_speed;

        private Vector3 moving_dir;
        private float target_yaw;
        private float target_pitch;

        private Matrix view_matrix;
        private Matrix projection_matrix;

        public Matrix ProjectionMatrix
        {
            get { return projection_matrix; }
        }

        public Matrix ViewMatrix
        {
            get { return view_matrix; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Target
        {
            get { return target; }
            set
            {
                target = value;
                UpdateThirdPersion();
            }
        }

        public Camera(PipeEngine engine)
        {
            this.engine = engine;

            ResetCamera();
        }

        private void ResetCamera()
        {
            target = new Vector3(0, 0, 0);
            offset = new Vector3(0, 80, 100);

            target_pitch = 0.0f;
            target_yaw = 0.0f;

            position = target + offset;

            moving_speed = .5f;
            rotation_speed = .02f;
            zoom_speed = 5.0f;

            moving_dir = new Vector3(0, 0, 0);

            rotation = Matrix.Identity;
            view_matrix = Matrix.Identity;
            projection_matrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), engine.GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f);
        }

        private void UpdateTarget()
        {
            if(Input.IsMouseDragging)
            {
                float dx = Input.MouseDeltaX;
                float dy = Input.MouseDeltaY;

                target_yaw += dx * rotation_speed;
                //target_pitch += dy * rotation_speed;
                offset.Y += dy * moving_speed; 
            }

            //engine.DebugConsole.WriteLine(string.Format("wheel delta: {0}", Input.MouseWheelDelta));

            float length = offset.Length();
            if (Input.MouseWheelDelta > 0)
                length -= zoom_speed;
            else if (Input.MouseWheelDelta < 0)
                length += zoom_speed;

            if (length <= 10)
                length = 10.0f;
            offset.Normalize();
            offset *= length;

            if(Input.Keyboard.IsKeyDown(Keys.S))
            {
                Matrix forward_movement = Matrix.CreateRotationY(target_yaw);
                //forward_movement *= Matrix.CreateRotationX(target_pitch);
                Vector3 v = new Vector3(0, 0, moving_speed);
                v = Vector3.Transform(v, forward_movement);
                target.Z += v.Z;
                target.X += v.X;
            }

            if(Input.Keyboard.IsKeyDown(Keys.W))
            {
                Matrix forward_movement = Matrix.CreateRotationY(target_yaw);
                //forward_movement *= Matrix.CreateRotationX(target_pitch);
                Vector3 v = new Vector3(0, 0, -moving_speed);
                v = Vector3.Transform(v, forward_movement);
                target.Z += v.Z;
                target.X += v.X;
            }

            if(Input.Keyboard.IsKeyDown(Keys.D))
            {
                Matrix forward_movement = Matrix.CreateRotationY(target_yaw);
                //forward_movement *= Matrix.CreateRotationX(target_pitch);
                Vector3 v = new Vector3(moving_speed, 0, 0);
                v = Vector3.Transform(v, forward_movement);
                target.Z += v.Z;
                target.X += v.X;
            }

            if(Input.Keyboard.IsKeyDown(Keys.A))
            {
                Matrix forward_movement = Matrix.CreateRotationY(target_yaw);
                //forward_movement *= Matrix.CreateRotationX(target_pitch); 
                Vector3 v = new Vector3(-moving_speed, 0, 0);
                v = Vector3.Transform(v, forward_movement);
                target.Z += v.Z;
                target.X += v.X;
            }
        }

        private void UpdateThirdPersion()
        {
            Matrix rot = Matrix.CreateRotationY(target_yaw);
            Vector3 transformed_offset = Vector3.Transform(offset, rot);

            position = target + transformed_offset;
            view_matrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }

        public void Update(GameTime gametime)
        {
            UpdateTarget();
            UpdateThirdPersion();    
        }

    }
}

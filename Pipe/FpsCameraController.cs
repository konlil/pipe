using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class FpsCameraController //:Controller
    {
   /*     private Camera camera;

        private Vector3 target_position;        //当前相机的目标位置
        private Vector3 desired_position;       //目的点位置
        private Vector3 moving_direction;
        private float moving_speed = 1.0f;


        public FpsCameraController(PipeEngine engine)
            : base(engine)
        {

        }

        public override void Action(GameTime gameTime)
        {
            if(camera == null)
            {
                camera = (Camera)controlled_entity;
            }

            Vector3 prev_pos = this.target_position;
            UpdateMoving();
            ProcessInput();

            Vector3 ofs = this.target_position - prev_pos;

            UpdateCamera(ofs);
        }

        private void UpdateCamera(Vector3 ofs)
        {
            this.camera.Position += ofs;
        }

        private void UpdateMoving()
        {
            Vector3 prev_pos = target_position;
            
            moving_direction = GetMovingDir();
            if(moving_direction.X == 0 && moving_direction.Y == 0 && moving_direction.Z == 0)
            {
                return;
            }

            Vector3 cur_moving_dir = moving_direction;
            if( Move() == null )
            {
                return;
            }

            moving_direction = cur_moving_dir;
            target_position = prev_pos;

            MoveSlide();
        }

        private bool Move()
        {
            if(moving_direction.X == 0 && moving_direction.Y == 0 && moving_direction.Z == 0)
            {
                return null;
            }

            desired_position = target_position + moving_direction;
            bool result = SweepTest(target_position, desired_position);
            if(result == false)
            {
                target_position = desired_position;
                camera.TargetPosition = target_position;

                return false;
            }

            return true;
        }

        private bool Slide()
        {

        }

        private void MoveSlide()
        {

        }

        private Vector3 GetMovingDir()
        {
            float speed = moving_speed;
            Vector3 dir = new Vector3(0, 0, 0);

            if(Input.Keyboard.IsKeyDown(Keys.W))
            {
                dir.Z += 1.0f;
            }

            if(Input.Keyboard.IsKeyDown(Keys.S))
            {
                dir.Z -= 1.0f;
            }

            if(Input.Keyboard.IsKeyDown(Keys.A))
            {
                dir.X -= 1.0f;
            }

            if(Input.Keyboard.IsKeyDown(Keys.D))
            {
                dir.X += 1.0f;
            }

            if (dir.X == 0 && dir.Y == 0 && dir.Z == 0)
                return dir;

            if (dir.Z < 0)
                speed *= 0.5;

            dir = Vector3.Transform(dir, camera.RotationMatrix);
            dir.Y = 0;
            dir.Normalize();
            dir *= speed;

            return dir;
        }

        private void ProcessInput(float ellapsedTime)
        {
            if(Input.IsMouseDragging)
            {
                float pixel_to_angle = 500.0f;
                
                //向量先绕up旋转
                Vector3 dir = camera.Position - desired_position;
                Matrix rot = Matrix.CreateFromAxisAngle(Vector3.Up, Input.MouseDeltaX * 3.14159 / pixel_to_angle);
                dir = Vector3.Transform(dir, rot);

                //绕up旋转之后的相机旋转矩阵
                Matrix cam_rot = camera.RotationMatrix * rot;

                //绕相机的right向量旋转（俯仰）
                rot = Matrix.CreateFromAxisAngle(cam_rot.Right, Input.MouseDeltaY * 3.14159 / pixel_to_angle);
                dir = Vector3.Transform(dir, rot);

                camera.Position = desired_position + dir;
                camera.RotationMatrix = Matrix.CreateLookAt(Vector3.Zero, -dir, cam_rot.Up);
            }
        }
    */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class FreeCameraController:Controller
    {
        private Camera camera;
        private MouseState original_mouse_state;

        public FreeCameraController(PipeEngine engine)
            : base(engine)
        {

        }

        public override void Action(Microsoft.Xna.Framework.GameTime gametime)
        {
            if(camera == null)
            {
                camera = (Camera)controlled_entity;
                Mouse.SetPosition(engine.GraphicsDevice.Viewport.Width / 2, engine.GraphicsDevice.Viewport.Height / 2);
                original_mouse_state = Mouse.GetState();
            }

            float time = (float)gametime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(time);
            original_mouse_state = Mouse.GetState();
        }

        private void ProcessInput(float time)
        {
            float roll = 0;
            float pitch = 0;
            float yaw = 0;
            float movement = 0;

            float rotation = time * camera.RotationSpeed;
            float move = time * camera.MovementSpeed;

            if(engine.IsActive)
            {
                MouseState current_mouse_state = Mouse.GetState();
                if(current_mouse_state != original_mouse_state)
                {
                    float x_delta = current_mouse_state.X - original_mouse_state.X;
                    float y_delta = current_mouse_state.Y - original_mouse_state.Y;

                    if(Input.MouseLButtonPressed)
                    {
                        camera.Pose.position.X += x_delta*move*0.1f;
                        camera.Pose.position.Y += y_delta*move*0.1f;
                    }
                    else if(Input.MouseRButtonPressed)
                    {
                        pitch -= rotation * y_delta*0.1f;
                    }
                    
                    //yaw += rotation * x_delta;
                    //pitch -= rotation * y_delta;
                }
            }

             //键盘控制相机的前后左右移动和翻滚
            if (Input.KeyboardUpPressed || Input.Keyboard.IsKeyDown(Keys.W))
                movement += move;
            if (Input.KeyboardDownPressed || Input.Keyboard.IsKeyDown(Keys.S))
                movement -= move;
            if (Input.KeyboardRightPressed || Input.Keyboard.IsKeyDown(Keys.D))
                yaw += rotation;
            if (Input.KeyboardLeftPressed || Input.Keyboard.IsKeyDown(Keys.A))
                yaw -= rotation;
            if (Input.Keyboard.IsKeyDown(Keys.Q))
                roll -= rotation;
            if (Input.Keyboard.IsKeyDown(Keys.E))
                roll += rotation;

            //如果相机旋转
            if (roll != 0 || pitch != 0 || yaw != 0)
            {
                camera.Yaw += yaw;
                camera.Pitch += pitch;
                camera.Roll += roll;
                //设置相机的旋转
                camera.pose.rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-camera.Yaw), 
                    MathHelper.ToRadians(camera.Pitch), MathHelper.ToRadians(camera.Roll));
                
                //isDirty设为true表示需要更新相机
                NeedUpdate = true;
                //isRotation = true;
            }

            //如果相机移动
            if (movement != 0)
            {
                //isDirty设为true表示需要更新相机
                NeedUpdate = true;

                //设置相机的位置
                camera.pose.position +=camera.GetMoveForwardVector(movement);
            }
            
            if (NeedUpdate)
            {
                camera.UpdateViewMatrix(true);
                NeedUpdate = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class Input
    {
        private static MouseState mouse_state;
        private static MouseState last_mouse_state;
        private static bool mouse_detected;

        private static KeyboardState keyboard_state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        private static List<Keys> last_keyboard_state = new List<Keys>();

        private static Vector2 mouse_delta;
        private static Vector2 last_mouse_delta;

        private static int mousewheel_delta = 0;
        private static int mousewheel_value = 0;

        private static bool is_dragging = false;
        private static Point dragging_start_position;
        private static bool mouse_centered = false;

        public static bool MouseDetected
        {
            get { return mouse_detected; }
        }
    
        public static Point MousePosition
        {
            get { return new Point(mouse_state.X, mouse_state.Y); }
            set { Mouse.SetPosition(value.X, value.Y); }
        }

        public static float MouseDeltaX
        {
            get { return mouse_delta.X; }
        }

        public static float MouseDeltaY
        {
            get { return mouse_delta.Y; }
        }

        public static float MouseWheelDelta
        {
            get { return mousewheel_delta; }
        }

        public static bool MouseLButtonPressed
        {
            get { return mouse_state.LeftButton == ButtonState.Pressed;  }
        }

        public static bool MouseLButtonReleased
        {
            get { return mouse_state.LeftButton == ButtonState.Released;  }
        }

        public static bool MouseRButtonPressed
        {
            get { return mouse_state.RightButton == ButtonState.Pressed; }
        }

        public static bool MouseRButtonReleased
        {
            get { return mouse_state.RightButton == ButtonState.Released; }
        }

        public static bool IsMouseDragging
        {
            get { return is_dragging; }
        }

        public static KeyboardState Keyboard
        {
            get { return keyboard_state; }
        }
        public static bool KeyboardUpPressed
        {
            get { return keyboard_state.IsKeyDown(Keys.Up); }
        }

        public static bool KeyboardDownPressed
        {
            get { return keyboard_state.IsKeyDown(Keys.Down); }
        }

        public static bool KeyboardLeftPressed
        {
            get { return keyboard_state.IsKeyDown(Keys.Left); }
        }

        public static bool KeyboardRightPressed
        {
            get { return keyboard_state.IsKeyDown(Keys.Right); }
        }

        public static void CenterMouse()
        {
            mouse_centered = true;
        }

        internal static void Update()
        {
            last_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();

            Point last_mouse_pos = new Point(last_mouse_state.X, last_mouse_state.Y);
            last_mouse_delta.X += mouse_state.X - last_mouse_state.X;
            last_mouse_delta.Y += mouse_state.Y - last_mouse_state.Y;
            mouse_delta.X = last_mouse_delta.X / 2.0f;
            mouse_delta.Y = last_mouse_delta.Y / 2.0f;
            last_mouse_delta.X -= last_mouse_delta.X / 2.0f;
            last_mouse_delta.Y -= last_mouse_delta.Y / 2.0f;

            if(mouse_centered)
            {
                mouse_centered = false;
                mouse_state = Mouse.GetState();
            }

            if(MouseLButtonPressed)
            {
                is_dragging = true;
            }
            else if(MouseLButtonReleased)
            {
                is_dragging = false;
            }

            mousewheel_delta = mouse_state.ScrollWheelValue - mousewheel_value;
            mousewheel_value = mouse_state.ScrollWheelValue;

            if(mouse_detected == false)
            {
                mouse_detected = mouse_state.X != last_mouse_state.X 
                    || mouse_state.Y != last_mouse_state.Y 
                    || mouse_state.LeftButton != last_mouse_state.LeftButton; 
            }

            last_keyboard_state = new List<Keys>(keyboard_state.GetPressedKeys());
            keyboard_state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }
    }
}

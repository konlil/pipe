using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using DGui.DrawingSystem;

 

namespace DGui
{
    // Delegate functions (for communicating with other classes)
    public delegate void ToggleButtonEventHandler(DButton.DButtonState state);

    // Toggle button
    public class DToggleButton : DButtonBase
    {
        public event ToggleButtonEventHandler OnToggle;

        bool pressed = false; // Ensure clicks only register if we have released first


        #region Public properties
        public DButtonState State
        {
            get
            {
                return buttonState;
            }
            set
            {
                buttonState = value;
            }
        }
        public bool Pressed
        {
            get { return pressed; }
        }
        #endregion




        public DToggleButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height, 
            Color _fillColor, Color _borderColor,
            Color _clickedFillColor, Color _clickedBorderColor)
            : this(guiManager, x, y, _text, _width, _height, _fillColor, _borderColor)
        {
            SetClickedColors(_clickedFillColor, _clickedBorderColor);
        }

        public DToggleButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height, 
            Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _text, _width, _height)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DToggleButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height)
            : this(guiManager, x, y, _text)
        {
            this.Size = new Vector2(_width, _height);
        }

        public DToggleButton(DGuiManager guiManager, float x, float y, string _text)
            : this(guiManager, x, y)
        {
            if (_text != null)
                Text = _text;
        }

        public DToggleButton(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DToggleButton(DGuiManager guiManager)
            : base(guiManager)
        {
            //FillColor = Color.Ivory;
            //BorderColor = new Color(40,40,40);
            //HoverFillColor = Color.Cornsilk;
            //HoverBorderColor = new Color(40, 40, 40);


            OnToggle += new ToggleButtonEventHandler(Toggle);
        }





        public void Toggle(DButtonState state)
        {
            if (state == DButtonState.On)
            {
                UseHoverColor = false;
                BorderWidth = 2;
                buttonState = DButtonState.On;
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, ColorTheme.ClickedFillColor, ColorTheme.ClickedBorderColor);
            
            }
            else
            {
                buttonState = DButtonState.Off;
                BorderWidth = 1;
                UseHoverColor = true;
                RestoreDefaultTexture();
            }


            
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Enabled)
            {
                MouseState ms = Mouse.GetState();

                // Is mouse hovering over?
                if (IsMouseHoveringOver && IsFocused)
                {
                    // Mouse click?
                    if (ms.LeftButton == ButtonState.Pressed && !pressed)
                    {
                        pressed = true;
                        if (buttonState == DButtonState.Off)
                            OnToggle(DButtonState.On);
                        else
                            OnToggle(DButtonState.Off);
                    }

                    //if (UseHoverColor == true && HoverEventsEnabled == false)
                        //HoverEnter();
                }
                //else if (UseHoverColor == true && HoverEventsEnabled == true)
                //    HoverExit();

                if (ms.LeftButton == ButtonState.Released)
                {
                    pressed = false;
                }
            }
        }




        //protected override void LeftMouseUp()
        //{
        //    //base.LeftMouseUp();
        //    //if (buttonState == DButtonState.On)
        //    ///    texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, clickedFillColor, clickedBorderColor);
        //}
    }
}

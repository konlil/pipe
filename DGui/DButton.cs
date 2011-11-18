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
    // Click-release button
    public class DButton : DButtonBase
    {
        /// <summary>
        /// Button is pressed in
        /// </summary>
        public event DButtonEventHandler OnLeftMouseDown;

        /// <summary>
        /// Left mouse is released after button pressed in
        /// </summary>
        public event DButtonEventHandler OnLeftMouseUp;

        /// <summary>
        /// Click-release event
        /// </summary>
        public event DButtonEventHandler OnClick;


        public bool IsPressed
        {
            get { return (buttonState == DButtonState.On); }
        }



        public DButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height, 
            Color _fillColor, Color _borderColor,
            Color _clickedFillColor, Color _clickedBorderColor)
            : this(guiManager, x, y, _text, _width, _height, _fillColor, _borderColor)
        {
            SetClickedColors(_clickedFillColor, _clickedBorderColor);
        }

        public DButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height, 
            Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _text, _width, _height)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DButton(DGuiManager guiManager, float x, float y, string _text, int _width, int _height)
            : this(guiManager, x, y, _text)
        {
            this.Size = new Vector2(_width, _height);
        }

        public DButton(DGuiManager guiManager, float x, float y, string _text)
            : this(guiManager, x, y)
        {
            if (_text != null)
                Text = _text;
        }


        public DButton(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }


        public DButton(DGuiManager guiManager)
            : base(guiManager)
        {
            OnLeftMouseDown += new DButtonEventHandler(LeftMouseDown);
            OnLeftMouseUp += new DButtonEventHandler(LeftMouseUp);
        }



        protected virtual void LeftMouseDown(GameTime gameTime)
        {
            UseHoverColor = false;
            BorderWidth = BORDER_WIDTH;
            buttonState = DButtonState.On;
            _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, ColorTheme.ClickedFillColor, ColorTheme.ClickedBorderColor);
        }

        protected virtual void LeftMouseUp(GameTime gameTime)
        {
            BorderWidth = 1;
            buttonState = DButtonState.Off;
            if (!HoverEventsEnabled)
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, ColorTheme.FillColor, ColorTheme.BorderColor);
            else
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, ColorTheme.HoverFillColor, ColorTheme.HoverBorderColor);
            UseHoverColor = true;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            base.Update(gameTime);

            Vector2 absPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

            // Is mouse hovering over?
            if (Visible)
            {
                if (IsMouseHoveringOver && IsFocused)
                {
                    // Mouse click?
                    if (ms.LeftButton == ButtonState.Pressed && buttonState == DButtonState.Off)
                        OnLeftMouseDown(gameTime);
                    else if (ms.LeftButton == ButtonState.Released && buttonState == DButtonState.On)
                    {
                        OnLeftMouseUp(gameTime);
                        if (OnClick != null)
                            OnClick(gameTime);
                    }
                }
                else
                {
                    // turn it off if the mouse hovers off it
                    if (buttonState == DButtonState.On)
                        OnLeftMouseUp(gameTime);
                }
            }

            base.Update(gameTime);
        }


    }
}

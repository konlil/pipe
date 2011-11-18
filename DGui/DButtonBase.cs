using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DGui.DrawingSystem;

namespace DGui
{
    // Delegates
    public delegate void DButtonEventHandler(GameTime gameTime);


    public abstract class DButtonBase : DPanel
    {
        public enum DButtonState
        {
            On,
            Off
        };


        protected const float LEFTPAD = 10;
        protected const int WIDTH = 140;
        protected const int HEIGHT = 40;
        protected const int BORDER_WIDTH = 2;


        protected DButtonState buttonState = DButtonState.Off;
        protected DText label = null;
        protected string text = string.Empty;



        #region Public properties
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (label != null)
                    label.Text = value;
            }
        }
        public string FontName
        {
            get
            {
                return label.FontName;
            }
            set
            {
                label.FontName = value;
            }
        }
        public Color FontColor
        {
            get
            {
                if (label != null)
                    return label.FontColor;
                else
                    return Color.Black;
            }
            set
            {
                if (label != null)
                    label.FontColor = value;
            }
        }
        #endregion




        public DButtonBase(DGuiManager guiManager)
            : base(guiManager)
        {
            Size = new Vector2(WIDTH, HEIGHT);
            UseHoverColor = true;

            label = new DText(guiManager);
        }



        public void SetClickedColors(Color _clickedFillColor, Color _clickedBorderColor)
        {
            ClickedFillColor = _clickedFillColor;
            ClickedBorderColor = _clickedBorderColor;
        }




        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Overridden to add border width
            base.LoadContent();
            _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, ColorTheme.FillColor, ColorTheme.BorderColor);

            //label = new DText(Game);
            label.Text = text;
            label.HorizontalAlignment = DText.DHorizontalAlignment.Center;
            label.VerticalAlignment = DText.DVerticalAlignment.Center;
            label.Position = new Vector2(Size.X / 2, Size.Y / 2);
            this.AddPanel(label);
        }




        protected override void UnloadContent()
        {
            label.Dispose();
            base.UnloadContent();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

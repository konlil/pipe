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
using System.Text.RegularExpressions;

using DGui.DrawingSystem;


namespace DGui
{
    // Delegate functions (for communicating with other classes)
    public delegate void DListBoxItemSelectedEventHandler(DListBoxItem listBoxItem);

    // Image and text item for the listbox
    public class DListBoxItem : DPanel
    {
        const float PADDING = 3;
        const int WIDTH = 170;
        const int HEIGHT = 35;
        ContentManager content;
        Vector2 imagePos;
        Vector2 textPos;
        bool selected = false;

        protected string key = null;

        public event DListBoxItemSelectedEventHandler OnSelect;

        protected DListBox listBox;
        protected DImage image = null;
        protected DText label = null;

        protected string text = null;
        protected SpriteFont spriteFont;        // Text font
        //protected Texture2D image = null;       // Image
        protected string imageName = null;      // Content path for image
        protected Color fontColor = Color.Black;
        protected Color selectedFillColor = Color.Wheat;
        protected Color selectedBorderColor = Color.Black;
        //protected Rectangle sourceRectangle;

        #region Public properties
        public DListBox ListBox
        {
            get
            {
                return listBox;
            }
            set
            {
                listBox = value;
            }
        }
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    if (selected)
                        Select();
                    else
                        Unselect();
                }
            }
        }
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value;
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return spriteFont;
            }
            set
            {
                spriteFont = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }
        #endregion


        public DListBoxItem(DGuiManager guiManager, string _text, string _imageName)
            : this(guiManager, _text)
        {
            imageName = _imageName;
        }

        public DListBoxItem(DGuiManager guiManager, string _text)
            : base(guiManager)
        {
            text = "";
            if (_text != null)
                text = _text;

            FillColor = Color.LightYellow;
            BorderColor = Color.LemonChiffon;
            HoverFillColor = Color.Ivory;
            HoverBorderColor = Color.Wheat;
            Size = new Vector2(WIDTH, HEIGHT);
            UseHoverColor = true;
            content = new ContentManager(guiManager.Game.Services);
            content.RootDirectory = "Content";
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
            imagePos = new Vector2(PADDING, PADDING);

            if (imageName != null)
            {
                image = new DImage(_guiManager, (int) imagePos.X, (int) imagePos.Y, imageName, (int) (Size.Y - (2*PADDING)), (int) (Size.Y - (2*PADDING)));
                this.AddPanel(image);
            }

            // Align text centered vertically
            if (image != null)
                textPos = new Vector2(Size.Y + PADDING, Size.Y / 2);
            else
                textPos = new Vector2(PADDING + PADDING, Size.Y / 2);


            // Text label
            label = new DText(_guiManager);
            label.Text = text;
            label.FontName = "Miramonte";
            label.HorizontalAlignment = DText.DHorizontalAlignment.Left;
            label.VerticalAlignment = DText.DVerticalAlignment.Center;
            label.Position = textPos;
            //label.Parent = this;
            this.AddPanel(label);


            base.LoadContent();
        }



        protected override void UnloadContent()
        {
            if (image != null)
                image.Dispose();
            label.Dispose();
            base.UnloadContent();
        }





        public void Select()
        {
            UseHoverColor = false;
            selected = true;
            _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, selectedFillColor, selectedBorderColor);
            OnSelect(this);
        }

        public void Unselect()
        {
            UseHoverColor = true;
            selected = false;
            _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, FillColor, BorderColor);
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            Vector2 absPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

            base.Update(gameTime);

            // Is mouse hovering over?
            if (IsMouseHoveringOver && this.IsFocused)
            {
                if (!_greyedOut && !IsTotallyObscured)
                {
                    // Mouse click?
                    if (ms.LeftButton == ButtonState.Pressed) // && !selected)
                        Select();
                }
            } 
                
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }



        /*protected override void DrawWithClipRect()
        {
            base.DrawWithClipRect();

            if (image != null)
            {
                Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);
                drawPos += imagePos;

                // Scaling of rectangle
                float scale = (Size.Y - (2 * PADDING)) / image.Height;
                //Rectangle rect = new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)(image.Width * scale), (int)(image.Height * scale));

                

                // draw the image
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                spriteBatch.Draw(image,
                                drawPos + Offset,
                                SourceRectangle,
                                Color.White,
                                0,
                                Origin,
                                scale,
                                SpriteEffects.None,
                                0);

                spriteBatch.End();
            }
        }*/

    }
}

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
    public delegate void CheckboxEventHandler();

    // Toggle checkbox with label
    public class DCheckbox : DPanel
    {
        const int TEXTPAD = 10;
        const int BOXSIZE = 18;
        //ContentManager content;
        bool mouseReleased = true;  // To allow only one change at a time
        Vector2 textPos;


        protected bool checkboxState = false;

        protected DText label = null;

        protected DText _xText = null;
        
        public event CheckboxEventHandler OnCheck;
        public event CheckboxEventHandler OnUncheck;
        public event CheckboxEventHandler OnToggle;


        protected string text = null;
        protected SpriteFont spriteFont;
        protected Color fontColor = Color.MidnightBlue;


        #region Public properties
        public bool Checked
        {
            get
            {
                return checkboxState;
            }
            set
            {
                checkboxState = value;
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




        public DCheckbox(DGuiManager guiManager, float x, float y, string _text, 
            Color _fillColor, Color _borderColor,
            Color _checkedFillColor, Color _checkedBorderColor)
            : this(guiManager, x, y, _text, _fillColor, _borderColor)
        {
            this.ClickedFillColor = _checkedFillColor;
            this.ClickedBorderColor = _checkedBorderColor;
        }

        public DCheckbox(DGuiManager guiManager, float x, float y, string _text, 
            Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _text)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DCheckbox(DGuiManager guiManager, float x, float y, string _text)
            : this(guiManager, x, y)
        {
            if (_text != null)
                Text = _text;
        }

        public DCheckbox(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DCheckbox(DGuiManager guiManager)
            : base(guiManager)
        {
            FillColor = ColorTheme.InputFillColor;
            BorderColor = ColorTheme.BorderColor;

            Size = new Vector2(BOXSIZE, BOXSIZE);
            //SetCentered(true);

            //content = new ContentManager(game.Services);
            //content.RootDirectory = Path.Combine("Content", "fonts");
            Text = "";
            OnCheck += new CheckboxEventHandler(Check);
            OnUncheck += new CheckboxEventHandler(Uncheck);
        }


        //public void SetCheckedColors(Color _checkedFillColor, Color _checkedBorderColor)
        //{
        //    CheckedFillColor = _checkedFillColor;
        //    CheckedBorderColor = _checkedBorderColor;
        //}


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
            textPos = new Vector2(Size.X + TEXTPAD, Size.Y / 2);

            label = new DText(_guiManager);
            label.Text = text;
            label.FontName = "Miramonte";
            // Left justified
            label.HorizontalAlignment = DText.DHorizontalAlignment.Left;
            label.VerticalAlignment = DText.DVerticalAlignment.Center;
            label.Position = textPos;
            label.ApplyChildClipping = false;
            label.Initialize();
            this.AddPanel(label);


            _xText = new DText(_guiManager);
            _xText.Text = "x";
            _xText.HorizontalAlignment = DText.DHorizontalAlignment.Center;
            _xText.VerticalAlignment = DText.DVerticalAlignment.Center;
            _xText.Position = (this.Size / 2) + new Vector2(0, -1);
            _xText.Initialize();
            _xText.Visible = false;
            _xText.ApplyChildClipping = false;
            this.AddPanel(_xText);

            base.LoadContent();
        }


        protected override void UnloadContent()
        {
            label.Dispose();
            base.UnloadContent();
        }


        protected void Check()
        {
            checkboxState = true;
            _xText.Visible = true;
        }

        protected void Uncheck()
        {
            checkboxState = false;
            _xText.Visible = false;
        }

        protected void Toggle()
        {
            if (OnToggle != null)
                OnToggle();

            if (checkboxState == true)
                Uncheck();
            else
                Check();
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
            if (IsFocused && IsMouseHoveringOver)
            {
                // Mouse click?
                if (ms.LeftButton == ButtonState.Pressed && mouseReleased == true)
                {
                    Toggle();
                    mouseReleased = false;
                }
            }

            if (ms.LeftButton == ButtonState.Released)
                mouseReleased = true;
                
            
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

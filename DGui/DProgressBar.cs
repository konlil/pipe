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
    public delegate void DProgressEventHandler(int tick, int current);

    /// <summary>
    /// Progress bar/meter
    /// </summary>
    public class DProgressBar : DPanel
    {
        const int WIDTH = 220;
        const int HEIGHT = 30;
        const int BAR_PADDING = 3;
        const int BAR_VALUE_MAX = 100;
        const int BAR_VALUE_MIN = 0;
        const int BAR_VALUE_TICK = 5;

        protected Color barColor = Color.DodgerBlue;
        protected bool readOnly = false;
        protected int barPadding = BAR_PADDING;
        protected int valueMax = BAR_VALUE_MAX;
        protected int valueMin = BAR_VALUE_MIN;
        protected int valueTick = BAR_VALUE_TICK;
        protected int barValue = BAR_VALUE_MAX;

        public event DProgressEventHandler OnProgress;


        protected DPanel progressBar;


        #region Public properties
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
            }
        }
        public int BarPadding
        {
            get
            {
                return barPadding;
            }
            set
            {
                barPadding = value;
            }
        }
        public Color BarColor
        {
            get
            {
                return barColor;
            }
            set
            {
                barColor = value;
            }
        }
        public int Value
        {
            get
            {
                return barValue;
            }
            set
            {
                int tick = value - barValue;
                barValue = value;
                if (OnProgress != null)
                    OnProgress(tick, barValue);
            }
        }
        public int ValueMax
        {
            get
            {
                return valueMax;
            }
            set
            {
                valueMax = value;
                if (barValue > valueMax)
                    barValue = valueMax;
            }
        }
        #endregion






        public DProgressBar(DGuiManager guiManager, float x, float y, int _width, int _height, Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _width, _height)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DProgressBar(DGuiManager guiManager, float x, float y, int _width, int _height)
            : this(guiManager, x, y)
        {
            this.Size = new Vector2(_width, _height);
        }

        public DProgressBar(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DProgressBar(DGuiManager guiManager)
            : base(guiManager)
        {
            FillColor = Color.White;
            BorderColor = Color.Black;
            Size = new Vector2(WIDTH, HEIGHT);

            // Internal handler for resize
            OnProgress += new DProgressEventHandler(DProgressBar_OnProgress);
        }

        void DProgressBar_OnProgress(int tick, int current)
        {
            // Progress bar change. Filter for bad input.
            if (current > valueMax)
                current = valueMax;
            else if (current < valueMin)
                current = valueMin;

            barValue = current;

            // Get bar value percentage
            float valuePercent = 0f;
            if (valueMax != 0)
                valuePercent = (float)barValue / (float)valueMax;
            else
                valuePercent = 1f;

            // Apply to progress bar panel
            int maxBarLength = (int)this.Size.X - (2 * BAR_PADDING);
            float barLength = valuePercent * maxBarLength;
            if (barLength < 1f)  // So we don't draw an empty box. TODO: Remove the progress meter entirely! Or invisible
                barLength = 1f;
            progressBar.Size = new Vector2(barLength, progressBar.Size.Y);
            progressBar.RecreateTexture();


            if (Value == 0 && progressBar.Visible)
            {
                progressBar.Visible = false;
                this.Children.Remove(progressBar);
            }
            else if (Value > 0 && !progressBar.Visible)
            {
                progressBar.Visible = true;
                this.Children.Add(progressBar);
            }
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
            // Small panel inside this one
            progressBar = new DPanel(_guiManager,
                BAR_PADDING, 
                BAR_PADDING, 
                (int)Size.X - (BAR_PADDING * 2),
                (int)Size.Y - (BAR_PADDING * 2));
            progressBar.BorderWidth = 0;
            progressBar.FillColor = barColor;
            progressBar.BorderColor = barColor;
            this.AddPanel(progressBar);

            base.LoadContent();
        }


        protected void Increment()
        {
            Value += valueTick;
        }

        protected void Decrement()
        {
            Value -= valueTick;
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

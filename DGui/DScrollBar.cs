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
    public delegate void ScrollBarEventHandler(DScrollBar sender, float _value);

    // Click-release button
    public class DScrollBar : DPanel
    {
        public enum DScrollDirection { Next, Previous };

        const int SCROLLBAR_WIDTH = 24;
        const int WIDTH = 140;
        const int HEIGHT = 140;
        

        const float VALUE = 0;
        const float MIN = 0;
        const float MAX = 100;
        const float STEP = 5;

        protected float _scrollValue = VALUE;
        protected float _maximum = MAX;
        protected float _minimum = MIN;
        protected float _step = STEP;
        protected float _rapidStep = 1;
        protected bool _alignHorizontal = false;

        protected DButton _minButton = null;
        protected DButton _maxButton = null;
        protected DPanel _scrollIndicator = null;

        public event ScrollBarEventHandler OnScroll;


        protected Color buttonFillColor = Color.Wheat;
        protected Color buttonBorderColor = Color.Black;


        TimeSpan _clickTime = TimeSpan.Zero;
        TimeSpan _scrollTimeSpan = new TimeSpan(0, 0, 0, 0, 500);
        bool _rapidScrollDelay = true;
        TimeSpan _scrollTimeSpanSmall = new TimeSpan(0, 0, 0, 0, 50);


        #region Public properties
        /// <summary>
        /// Alignment-aware width getter
        /// </summary>
        public int ScrollbarWidth
        {
            get
            {
                if (_alignHorizontal)
                    return (int)Size.Y;
                else
                    return (int)Size.X;
            }
        }
        /// <summary>
        /// Alignment-aware length getter
        /// </summary>
        public int ScrollbarLength
        {
            get
            {
                if (_alignHorizontal)
                    return (int)Size.X;
                else
                    return (int)Size.Y;
            }
        }
        public Color ButtonFillColor
        {
            get
            {
                return buttonFillColor;
            }
            set
            {
                buttonFillColor = value;
            }
        }
        public Color ButtonBorderColor
        {
            get
            {
                return buttonBorderColor;
            }
            set
            {
                buttonBorderColor = value;
            }
        }
        public bool AlignHorizontal
        {
            get
            {
                return _alignHorizontal;
            }
            set
            {
                _alignHorizontal = value;
            }
        }
        public float Step
        {
            get
            {
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    UpdateScrollIndicatorSize();
                }
            }
        }
        public float Max
        {
            get
            {
                return _maximum;
            }
            set
            {
                if (_maximum != value)
                {
                    if (value < _minimum)
                        _maximum = _minimum;
                    else
                        _maximum = value;

                    if (_scrollValue > _maximum)
                    {
                        Scroll(_maximum - _scrollValue);
                        _scrollValue = _maximum;
                    }

                    UpdateScrollIndicatorSize();
                    UpdateScrollIndicatorPosition();
                }
            }
        }
        public float Min
        {
            get
            {
                return _minimum;
            }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;
                    UpdateScrollIndicatorSize();
                }
            }
        }
        public float Value
        {
            get
            {
                return _scrollValue;
            }
            set
            {
                if (_scrollValue != value)
                {
                    _scrollValue = value;
                    UpdateScrollIndicatorSize();
                }
            }
        }



        protected float IndicatorLength
        {
            get
            {
                if (_scrollIndicator != null)
                {
                    if (_alignHorizontal)
                        return _scrollIndicator.Size.X;
                    else
                        return _scrollIndicator.Size.Y;
                }
                return -1f;
            }
        }
        protected float InteriorScrollLength
        {
            get
            {
                if (_alignHorizontal)
                    return Size.X - (2 * ScrollbarWidth);
                else
                    return Size.Y - (2 * ScrollbarWidth);
            }
        }
        #endregion




        public DScrollBar(DGuiManager guiManager, float x, float y, int _width, int _height, 
            Color _fillColor, Color _borderColor,
            Color _buttonFillColor, Color _buttonBorderColor)
            : this(guiManager, x, y, _width, _height, _fillColor, _borderColor)
        {
            SetButtonColors(_buttonFillColor, _buttonBorderColor);
        }

        public DScrollBar(DGuiManager guiManager, float x, float y, int _width, int _height, 
            Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _width, _height)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DScrollBar(DGuiManager guiManager, float x, float y, int _width, int _height)
            : this(guiManager, x, y)
        {
            this.Size = new Vector2(_width, _height);
        }


        public DScrollBar(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DScrollBar(DGuiManager guiManager)
            : base(guiManager)
        {
            FillColor = Color.White;
            BorderColor = Color.Gainsboro;
            
            //HoverFillColor = Color.Cornsilk;
            //HoverBorderColor = new Color(40, 40, 40);


            //UseHoverColor = true;





            
        }



        public void SetButtonColors(Color _buttonFillColor, Color _buttonBorderColor)
        {
            ButtonFillColor = _buttonFillColor;
            ButtonBorderColor = _buttonBorderColor;
        }


        /// <summary>
        /// Move list to a scroll position
        /// </summary>
        /// <param name="value"></param>
        public void Scroll(float value)
        {
            float diff = _scrollValue;
            _scrollValue += value;
            if (_scrollValue < _minimum)
                _scrollValue = _minimum;
            if (_scrollValue > _maximum)
                _scrollValue = _maximum;
            diff = _scrollValue - diff;

            // Move the indicator
            if (diff != 0)
            {
                UpdateScrollIndicatorPosition();

                if (OnScroll != null)
                    OnScroll(this, diff);
            }
        }


        protected void UpdateScrollIndicatorPosition()
        {
            float scrollPercent = _scrollValue / _maximum;

            float maxPhysicalRange = InteriorScrollLength - IndicatorLength;

            if (_scrollIndicator != null)
            {
                if (_alignHorizontal)
                    _scrollIndicator.Position = new Vector2(ScrollbarWidth + (maxPhysicalRange * scrollPercent), 0);
                else
                    _scrollIndicator.Position = new Vector2(0, ScrollbarWidth + (maxPhysicalRange * scrollPercent));
            }
        }


        /// <summary>
        /// Increment scroll in a direction
        /// </summary>
        /// <param name="direction"></param>
        public void Scroll(DScrollDirection direction)
        {
            if (direction == DScrollDirection.Next)
                Scroll(_step);
            else
                Scroll(-_step);
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


            



            _minButton = new DButton(_guiManager, 0, 0, "", SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
            if (_alignHorizontal)
            {
                _minButton.Text = "<";
                Size = new Vector2(Size.X, SCROLLBAR_WIDTH);
                _maxButton = new DButton(_guiManager, Size.X - SCROLLBAR_WIDTH, 0, "", SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                _scrollIndicator = new DPanel(_guiManager, SCROLLBAR_WIDTH, 0, SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                _maxButton.Text = ">";
            }
            else
            {
                _minButton.Text = "^";
                Size = new Vector2(SCROLLBAR_WIDTH, Size.Y);
                _maxButton = new DButton(_guiManager, 0, Size.Y - SCROLLBAR_WIDTH, "", SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                _scrollIndicator = new DPanel(_guiManager, 0, SCROLLBAR_WIDTH, SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                _maxButton.Text = "v";
            }
            //scrollIndicator.BorderColor = Color.SlateGray;

            //maxButton.Parent = this;
            //minButton.Parent = this;
            AddPanel(_minButton);
            AddPanel(_maxButton);
            AddPanel(_scrollIndicator);
            UpdateScrollIndicatorSize();

            _minButton.OnLeftMouseDown += new DButtonEventHandler(minButton_OnLeftMouseDown);
            _minButton.OnLeftMouseUp += new DButtonEventHandler(minButton_OnLeftMouseUp);
            _maxButton.OnLeftMouseDown += new DButtonEventHandler(maxButton_OnLeftMouseDown);
            _maxButton.OnLeftMouseUp += new DButtonEventHandler(maxButton_OnLeftMouseUp);
        }

        void maxButton_OnLeftMouseUp(GameTime gameTime)
        {
            _rapidScrollDelay = true;
            _clickTime = TimeSpan.Zero;
        }

        void maxButton_OnLeftMouseDown(GameTime gameTime)
        {
            _clickTime = gameTime.TotalGameTime;
            _rapidScrollDelay = true;
            Scroll(DScrollDirection.Next);
        }

        void minButton_OnLeftMouseUp(GameTime gameTime)
        {
            _rapidScrollDelay = true;
            _clickTime = TimeSpan.Zero;
        }

        void minButton_OnLeftMouseDown(GameTime gameTime)
        {
            _clickTime = gameTime.TotalGameTime;
            _rapidScrollDelay = true;
            Scroll(DScrollDirection.Previous);
        }


        protected void UpdateScrollIndicatorSize()
        {
            _rapidStep = _step / 4;

            if (_scrollIndicator != null && _maximum > 0)
            {
                // Calculate scroll indicator size
                float indicatorLength = ScrollbarLength - (2 * ScrollbarWidth);
                indicatorLength *= _step / _maximum;

                // Minimum size
                if (indicatorLength < SCROLLBAR_WIDTH / 4)
                    indicatorLength = SCROLLBAR_WIDTH / 4;

                if (_alignHorizontal)
                    _scrollIndicator.Size = new Vector2(indicatorLength, ScrollbarWidth);
                else
                    _scrollIndicator.Size = new Vector2(ScrollbarWidth, indicatorLength);

                _scrollIndicator.RecreateTexture();
            }
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Overridden to add border width
            base.LoadContent();
            _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice, (int)Size.X, (int)Size.Y, BorderWidth, FillColor, BorderColor);


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

            if (this.Visible && this.Enabled)
            {
                int multiplier = 1;
                if (_minButton.IsPressed)
                    multiplier = -1;

                if (_minButton.IsPressed || _maxButton.IsPressed)
                {
                    if (_rapidScrollDelay && _clickTime != TimeSpan.Zero && (gameTime.TotalGameTime - _clickTime) > _scrollTimeSpan)
                    {
                        _rapidScrollDelay = false;
                        //Scroll(_step * multiplier);
                        _clickTime = gameTime.TotalGameTime;
                    }
                    else if (!_rapidScrollDelay && (gameTime.TotalGameTime - _clickTime) > _scrollTimeSpanSmall)
                    {
                        Scroll(_rapidStep * multiplier);
                        _clickTime = gameTime.TotalGameTime;
                    }
                }

                // Watch for hover over selector
                // Perform drag
                else if (_scrollIndicator.IsFocused) //_scrollIndicator.IsMouseHoveringOver || _indicatorScrollClicked)
                {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        // Scroll with mouse
                        if (!_indicatorScrollClicked)
                        {
                            _indicatorScrollClicked = true;
                            _indicatorScrollAnchor = new Vector2(ms.X, ms.Y);
                        }
                        else
                        {
                            Vector2 scrollAmount = new Vector2(ms.X, ms.Y);
                            scrollAmount -= _indicatorScrollAnchor;
                            _indicatorScrollAnchor = new Vector2(ms.X, ms.Y);

                            // Scale up the scroll so it keeps up with the mouse,
                            // because Scroll scales the move to keep the bar in range
                            float scrollRatio = InteriorScrollLength / IndicatorLength;
                            this.Scroll(scrollAmount.Y * scrollRatio);
                        }
                    }
                }
                else if (this.IsFocused && this.IsMouseHoveringOver &&
                    !_scrollIndicator.IsMouseHoveringOver && 
                    !_maxButton.IsMouseHoveringOver && 
                    !_minButton.IsMouseHoveringOver)
                {
                    if (ms.LeftButton == ButtonState.Pressed && !_emptyAreaClick)
                    {
                        _emptyAreaClick = true;

                        // Must have clicked empty space. Scroll in this direction!
                        if (_scrollIndicator.AbsoluteTransform.Y < ms.Y)
                            Scroll(DScrollDirection.Next);
                        else
                            Scroll(DScrollDirection.Previous);
                    }
                }
            }


            if (ms.LeftButton == ButtonState.Released)
            {
                _emptyAreaClick = false;
                _indicatorScrollClicked = false;
            }
        }

        Vector2 _indicatorScrollAnchor = Vector2.Zero;
        bool _indicatorScrollClicked = false;
        bool _emptyAreaClick = false;


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

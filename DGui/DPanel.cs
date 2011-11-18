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
using System.Reflection;

using DGui.DrawingSystem;

 

namespace DGui
{
    // Trigger this when hovering over
    public delegate void DPanelEventHandler();
    public delegate void DPanelVisibleChangedEventHandler(bool value);
    public delegate void DPanelFocusChangedEventHandler(bool focused);
    public delegate void DPanelHoverHandler();
    public delegate void DPanelResizeHandler(Vector2 newSize);

    
    /// <summary>
    /// Basic panel for XNA.
    /// Has a size, border color, fill color.
    /// </summary>
    public class DPanel : DGuiSceneNode
    {
        const int WIDTH = 400;
        const int HEIGHT = 300;

        protected DGuiManager _guiManager;
        DPanel _parent = null; // Panel that this panel belongs to. Set by AddPanel(). Be careful on manually adding children! 
                               // Depth for click registering is maintained by registering this.

        // Panel geometry and rendering
        protected Texture2D _texture;
        DGuiColorTheme colorTheme = new DGuiColorTheme();
        protected Rectangle? _sourceRectangle; // Clipping rectangle
        Vector2 _size;
        int _borderWidth = 1; // Width of the panel border
        int _alpha = 255; // Transparency
        Vector2 _origin; // Default 0 origin, draw at top left
        Color _tintColor = Color.White; // Tint color, if you're into that sort of thing


        // Panel state flags
        bool _hoverEventsEnabled = false; // Trigger hovering events
        bool _hoverColorsEnabled = false; // Whether we should change the color when the mouse hovers over
        bool _isPartiallyObscured = false; // Are we partially obscured by a parent?
        protected bool _isMouseHoveringOver = false; // Is the mouse hovering over?
        protected bool _applyChildClipping = true; // Whether to calculate child object clipping and assign them _sourceRectangle
        protected bool _useInteriorClipping = false;     // Control whether this panel is clipped at its parent's interior edge or at the absolute edge.
        bool _isTotallyObscured = false;
        protected bool _greyedOut = false;   // Tint grey and allow no update (for input-enabled descendants only)
        /// <summary>
        /// Use form fill color as opposed to regular control fill color
        /// </summary>
        protected bool _isForm = false;


        /// <summary>
        /// Whether the user has first hovered over this component before clicking
        /// </summary>
        bool _hasHoveredBeforeClick = false;

        protected bool _acceptsFocus = true;



        #region Events
        /// <summary>
        /// When the mouse hovers over the panel
        /// </summary>
        public event DPanelHoverHandler OnHoverEnter;

        /// <summary>
        /// When the mouse ceases hovering over the panel
        /// </summary>
        public event DPanelHoverHandler OnHoverExit;

        /// <summary>
        /// When the panel resizes
        /// </summary>
        public event DPanelResizeHandler OnResize;

        /// <summary>
        /// When a left mouse click is fired outside the panel region
        /// </summary>
        public event DPanelEventHandler OnClickOutsidePanel;

        public event DPanelFocusChangedEventHandler OnFocusChanged;

        #endregion


    
        

        #region Public properties

        public bool AcceptsFocus
        {
            get { return _acceptsFocus; }
            set { _acceptsFocus = value; }
        }
        public bool IsFocused
        {
            get
            {
                if (_guiManager != null && _guiManager.FocusedControl == this)
                    return true;
                return false;
            }
            set
            {
                if (_guiManager != null)
                {
                    if (value == true && _guiManager.FocusedControl != this)
                    {
                        _guiManager.FocusedControl = this;
                        if (OnFocusChanged != null)
                            OnFocusChanged(true);
                    }
                    if (value == false && _guiManager.FocusedControl == this)
                    {
                        _guiManager.FocusedControl = null;
                        if (OnFocusChanged != null)
                            OnFocusChanged(false);
                    }
                }
            }
        }

        /// <summary>
        /// Parent control
        /// </summary>
        public DPanel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Whether the control is inactive
        /// </summary>
        public bool GreyedOut
        {
            get
            {
                return _greyedOut;
            }
            set
            {
                _greyedOut = value;

                this.Enabled = !value;

                if (_greyedOut)
                    this.TintColor = Color.Gray;
                else
                    this.TintColor = Color.White;

                foreach (DPanel childPanel in this.Children)
                {
                    childPanel.GreyedOut = value;
                }
            }
        }
        /// <summary>
        /// Clip any child controls at the interior border rather than the absolute edge.
        /// Used for listbox
        /// </summary>
        public bool UseInteriorClipping
        {
            get
            {
                return _useInteriorClipping;
            }
            set
            {
                _useInteriorClipping = value;
            }
        }
        /// <summary>
        /// Is the mouse hovering over the panel?
        /// </summary>
        public bool IsMouseHoveringOver
        {
            get
            {
                return _isMouseHoveringOver;
            }
            set
            {
                _isMouseHoveringOver = value;
            }
        }
        /// <summary>
        /// Whether to apply clipping to child panel draw
        /// </summary>
        public bool ApplyChildClipping
        {
            get
            {
                return _applyChildClipping;
            }
            set
            {
                _applyChildClipping = value;
            }
        }
        /// <summary>
        /// Color theme for the panel
        /// </summary>
        public DGuiColorTheme ColorTheme
        {
            get
            {
                return colorTheme;
            }
            set
            {
                colorTheme = value;
            }
        }
        /// <summary>
        /// X position, convenience for Position.X
        /// </summary>
        public float X
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position = new Vector2(value,Position.Y);
            }
        }
        /// <summary>
        /// Y position, convenience for Position.Y
        /// </summary>
        public float Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position = new Vector2(Position.X, value);
            }
        }

        /// <summary>
        /// Width of the panel
        /// </summary>
        public float Width
        {
            get
            {
                return _size.X;
            }
            set
            {
                _size = new Vector2(value,_size.Y);
                if (OnResize != null)
                    OnResize(_size);
            }
        }

        /// <summary>
        /// Height of the panel
        /// </summary>
        public float Height
        {
            get
            {
                return Size.Y;
            }
            set
            {
                _size = new Vector2(_size.X, value);
                if (OnResize != null)
                    OnResize(_size);
            }
        }

        /// <summary>
        /// Clipping rectangle
        /// </summary>
        public Rectangle ?SourceRectangle
        {
            get
            {
                return _sourceRectangle;
            }
            set
            {
                _sourceRectangle = value;
            }
        }


        /// <summary>
        /// Transparency value, 0-255
        /// </summary>
        public int Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
            }
        }

        /// <summary>
        /// Width of the border of the panel
        /// </summary>
        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

        

        /// <summary>
        /// Use hover color when hovering over this control
        /// </summary>
        public bool UseHoverColor
        {
            get
            {
                return _hoverColorsEnabled;
            }
            set
            {
                _hoverColorsEnabled = value;
                if (!_hoverColorsEnabled)
                    RestoreDefaultTexture();
            }
        }

        public Vector2 Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                if (OnResize != null)
                    OnResize(_size);
            }
        }

        public Vector2 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
            }
        }

        public Color TintColor
        {
            get
            {
                return _tintColor;
            }
            set
            {
                _tintColor = value;
            }
        }

        // Convenience functions for ColorTheme ahead

        public Color FillColor
        {
            get
            {
                return colorTheme.FillColor;
            }
            set
            {
                colorTheme.FillColor = value;
            }
        }

        public Color BorderColor
        {
            get
            {
                return colorTheme.BorderColor;
            }
            set
            {
                colorTheme.BorderColor = value;
            }
        }

        public Color HoverFillColor
        {
            get
            {
                return colorTheme.HoverFillColor;
            }
            set
            {
                colorTheme.HoverFillColor = value;
            }
        }

        public Color HoverBorderColor
        {
            get
            {
                return colorTheme.HoverBorderColor;
            }
            set
            {
                colorTheme.HoverBorderColor = value;
            }
        }

        public Color ClickedFillColor
        {
            get { return colorTheme.ClickedFillColor; }
            set { colorTheme.ClickedFillColor = value; }
        }
        public Color ClickedBorderColor
        {
            get { return colorTheme.ClickedBorderColor; }
            set { colorTheme.ClickedBorderColor = value; }
        }

        //public bool NeedsUpdating { get; set; }

        #endregion


        #region Protected Properties
        /// <summary>
        /// Use hovering behavior for this panel
        /// </summary>
        protected bool HoverEventsEnabled
        {
            get
            {
                return _hoverEventsEnabled;
            }
            set
            {
                _hoverEventsEnabled = value;
            }
        }
        /// <summary>
        /// Whether the control has been clipped by a parent panel
        /// </summary>
        protected bool IsPartiallyObscured
        {
            get
            {
                return _isPartiallyObscured;
            }
            set
            {
                _isPartiallyObscured = value;
            }
        }
        public bool IsTotallyObscured
        {
            get { return _isTotallyObscured; }
            protected set { _isTotallyObscured = value; }
        }
        #endregion



        public DPanel(DGuiManager guiManager)
            : base(guiManager.Game)
        {
            _guiManager = guiManager;
            _origin = new Vector2(0, 0);
            Size = new Vector2(1, 1);

            _guiManager.OnFocusQueueReject += new DGuiManagerFocusQueueHandler(_guiManager_OnFocusQueueReject);
        }

        void _guiManager_OnFocusQueueReject(DPanel panel)
        {
            if (panel == this)
                _hasHoveredBeforeClick = false;
        }


        #region Additional constructors
        public DPanel(DGuiManager guiManager, float x, float y, int width, int height, DGuiColorTheme theme, int borderWidth)
            : this(guiManager, x, y, width, height, theme)
        {
            _borderWidth = borderWidth;
        }

        public DPanel(DGuiManager guiManager, float x, float y, int width, int height, DGuiColorTheme theme)
            : this(guiManager, x, y, width, height)
        {
            colorTheme = theme;
        }

        public DPanel(DGuiManager guiManager, float x, float y, int width, int height)
            : this(guiManager, x, y)
        {
            _size = new Vector2(width, height);
        }

        public DPanel(DGuiManager guiManager, Vector2 position)
            : this(guiManager)
        {
            Position = position;
        }

        public DPanel(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            Position = new Vector2(x, y);
        }
        #endregion


        // Xna

        #region Initialize
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
        #endregion


        #region LoadContent
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // load the box using the Farseer helper!
            RestoreDefaultTexture();
            _sourceRectangle = new Rectangle(0, 0, (int)_size.X, (int)_size.Y);

            // Half dimensions for centered draw origin
            //origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
        #endregion




        protected override void UnloadContent()
        {
            _texture.Dispose();
            base.UnloadContent();
        }


        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            Vector2 absPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

            // Is mouse hovering over?
            if (ms.X > absPos.X && ms.X < (absPos.X + _size.X) &&
                ms.Y > absPos.Y && ms.Y < (absPos.Y + _size.Y))
            {
                _isMouseHoveringOver = true;

                if (_hoverEventsEnabled == false)
                    HoverEnter();

                if (ms.LeftButton == ButtonState.Released)
                    _hasHoveredBeforeClick = true;
                else if (_hasHoveredBeforeClick)
                {
                    _guiManager.FocusListEnqueue(this);
                }
            }
            else
            {
                _isMouseHoveringOver = false;
                _hasHoveredBeforeClick = false;
                if (_hoverEventsEnabled == true)
                    HoverExit();

                if (ms.LeftButton == ButtonState.Pressed && OnClickOutsidePanel != null)
                {
                    OnClickOutsidePanel();
                }
            }


            DoChildPanelClipping();

            
            base.Update(gameTime);
            //NeedsUpdating = false;
        }
        #endregion


        /// <summary>
        /// Look through all the children and compare draw rects.
        /// Set clipping rectangles or visibility as necessary
        /// </summary>
        private void DoChildPanelClipping()
        {
            Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);
            Rectangle rect = new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)Width, (int)Height);

            // Examine source rectangle
            if (_sourceRectangle.HasValue)
            {
                float clipHeight, clipWidth;
                clipWidth = Size.X;
                clipHeight = Size.Y;
                if (_sourceRectangle.Value.Width < Size.X)
                    clipWidth = _sourceRectangle.Value.Width;

                if (_sourceRectangle.Value.Height < Size.Y)
                    clipHeight = _sourceRectangle.Value.Height;

                rect = new Rectangle((int)(rect.X + Offset.X),
                                    (int)(rect.Y + Offset.Y),
                                    (int)clipWidth,
                                    (int)clipHeight);
            }


            foreach (DGuiSceneNode childNode in this.Children)
            {
                DPanel childPanel = (DPanel)childNode;
                Vector2 childPos = new Vector2(childPanel.AbsoluteTransform.X, childPanel.AbsoluteTransform.Y);
                Rectangle childRect = new Rectangle((int)childPos.X, (int)childPos.Y, (int)childPanel.Width, (int)childPanel.Height);
                Rectangle thisRect = rect;


                // Give nodes that have child clipping disabled a full clipbox.
                if (!childPanel.ApplyChildClipping)
                {
                    childPanel.Offset = Vector2.Zero;
                    //childPanel.Visible = true;
                    childPanel.SourceRectangle = null;
                    childPanel.IsPartiallyObscured = false;
                    continue;
                }

                // Control whether this child panel is clipped at the interior edge of our border or at the absolute edge.
                if (childPanel.UseInteriorClipping)
                {
                    Rectangle interiorRect = new Rectangle((int)(rect.X + (_borderWidth * 2)),
                                                 (int)(rect.Y + (_borderWidth * 2)),
                                                 (int)(rect.Width - (_borderWidth * 4)),
                                                 (int)(rect.Height - (_borderWidth * 4)));
                    thisRect = interiorRect;
                }


                // If the child is in any way touching this panel
                if (thisRect.Intersects(childRect))
                {
                    // If the child isn't fully contained within this panel
                    if (!thisRect.Contains(childRect))
                    {
                        // Perform clipping
                        Rectangle drawRect = new Rectangle(0, 0, (int)childRect.Width, (int)childRect.Height);
                        childPanel.Offset = Vector2.Zero;

                        // Texture clipping for negative axes
                        if (childRect.Left < thisRect.Left)
                        {
                            // Alter position and clip rectangle
                            int clip = (thisRect.Left - childRect.Left);
                            drawRect.Width -= clip;
                            drawRect.X += clip;
                            childPanel.Offset = new Vector2(clip, 0);
                        }

                        // If child pokes out the top
                        if (childRect.Top < thisRect.Top)
                        {
                            // Alter position and clip rectangle
                            int clip = (thisRect.Top - childRect.Top);
                            drawRect.Height -= clip;
                            drawRect.Y += clip;
                            childPanel.Offset = new Vector2(0, clip);
                        }

                        // Clipping for positive axes
                        if (childRect.Right > thisRect.Right)
                            drawRect.Width = childRect.Width - (childRect.Right - thisRect.Right);
                        if (childRect.Bottom > thisRect.Bottom)
                            drawRect.Height = childRect.Height - (childRect.Bottom - thisRect.Bottom);

                        childPanel.IsTotallyObscured = false;
                        childPanel.SourceRectangle = drawRect;
                        childPanel.IsPartiallyObscured = true;
                    }
                    else
                    {
                        // Child is fully contained by its parent, draw away!
                        childPanel.Offset = Vector2.Zero;
                        childPanel.IsTotallyObscured = false;
                        childPanel.SourceRectangle = null;
                        childPanel.IsPartiallyObscured = false;
                    }
                }
                else
                {
                    // not clipping, don't draw!
                    childPanel.Offset = Vector2.Zero;
                    childPanel.IsTotallyObscured = true;
                    childPanel.SourceRectangle = null;
                    childPanel.IsPartiallyObscured = true;
                }
            }
        }


        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Visible && !IsTotallyObscured)
                DrawWithClipRect();
        }
        #endregion


        // User functions follow

        protected virtual void DrawWithClipRect()
        {
            Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

            // Protect alpha
            _alpha = (int)Math.Abs(_alpha);
            if (_alpha > 255)
                _alpha = 255;

            // Apply tint and alpha
            _tintColor = new Color(_tintColor.R, _tintColor.G, _tintColor.B, (byte)_alpha);

            // Draw the panel
            _guiManager.SpriteBatch.Draw(_texture,
                                drawPos + Offset,
                                _sourceRectangle,
                                _tintColor,
                                0,
                                _origin,
                                1,
                                SpriteEffects.None,
                                0);
        }




        protected void HoverEnter()
        {
            _hoverEventsEnabled = true;
            if (_hoverColorsEnabled)
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice,
                    (int)_size.X,
                    (int)_size.Y,
                    _borderWidth,
                    colorTheme.HoverFillColor,
                    colorTheme.HoverBorderColor);

            if (OnHoverEnter != null)
                OnHoverEnter();
        }

        protected void HoverExit()
        {
            _hoverEventsEnabled = false;
            if (_hoverColorsEnabled)
                RestoreDefaultTexture();

            if (OnHoverExit != null)
                OnHoverExit();
        }



        /// <summary>
        /// Revert texture to the control fill color value
        /// </summary>
        protected void RestoreDefaultTexture()
        {
            Color fillColor = colorTheme.FillColor;
            if (_isForm)
                fillColor = colorTheme.FormFillColor;

            if (!UseHoverColor || !HoverEventsEnabled)
            {
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice,
                        (int)_size.X,
                        (int)_size.Y,
                        _borderWidth,
                        fillColor,
                        colorTheme.BorderColor);
            }
            else
            {
                _texture = DrawingHelper.CreateRectangleTexture(Game.GraphicsDevice,
                        (int)_size.X,
                        (int)_size.Y,
                        _borderWidth,
                        colorTheme.HoverFillColor,
                        colorTheme.HoverBorderColor);
            }
        }




        public void RecreateTexture()
        {
            // load the box using the Farseer helper!
            RestoreDefaultTexture();
            _sourceRectangle = new Rectangle(0, 0, (int)_size.X, (int)_size.Y);
        }


        public void AddPanel(DPanel p)
        {
            p.Initialize();
            p.Parent = this;
            this.Children.Add(p);
        }

        public bool HasAncestor(DPanel p)
        {
            return HasAncestorRecursive(this, p, false);
        }

        private bool HasAncestorRecursive(DPanel p, DPanel target, bool result)
        {
            if (p == target)
                result = true;
            if (!result)
            {
                if (p.Parent != null)
                    result = HasAncestorRecursive(p.Parent, target, result);
            }

            return result;
        }
    }
}

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
    // Transparent text overlay
    // Uses the free font pack for XNA 2.0 Beta
    public class DText : DPanel
    {
        const int WIDTH = 120;
        const int HEIGHT = 20;

        public enum DHorizontalAlignment
        {
            Left,
            Center,
            Right
        };

        public enum DVerticalAlignment
        {
            Top,
            Center,
            Bottom
        };

        public DHorizontalAlignment horizontalAlignment = DHorizontalAlignment.Center;
        public DVerticalAlignment verticalAlignment = DVerticalAlignment.Center;


        ContentManager content;
        protected string text = string.Empty;
        protected string fontName = "MiramonteBold";
        protected SpriteFont spriteFont;
        protected Color fontColor = Color.Black;

        #region Public properties
        public string FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = value;
                if (spriteFont != null)
                    Size = spriteFont.MeasureString(text);
            }
        }
        public DHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return horizontalAlignment;
            }
            set
            {
                horizontalAlignment = value;
            }
        }
        public DVerticalAlignment VerticalAlignment
        {
            get
            {
                return verticalAlignment;
            }
            set
            {
                verticalAlignment = value;
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
                if (value != null)
                {
                    text = value;
                    if (spriteFont != null && text != string.Empty)
                        Size = spriteFont.MeasureString(text);
                }
                if (String.IsNullOrEmpty(value))
                    Size = new Vector2(1, Size.Y);
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


        public DText(DGuiManager guiManager, float x, float y, int _width, int _height, string _text, Color _fontColor)
            : this(guiManager, x, y, _width, _height, _text)
        {
            fontColor = _fontColor;
        }

        public DText(DGuiManager guiManager, float x, float y, int _width, int _height, string _text)
            : this(guiManager, x, y, _text)
        {
            Size = new Vector2(_width, _height);
        }

        public DText(DGuiManager guiManager, float x, float y, string _text)
            : this(guiManager, x, y)
        {
            text = _text;
        }

        public DText(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DText(DGuiManager guiManager)
            : base(guiManager)
        {
            Size = new Vector2(WIDTH, HEIGHT);

            content = new ContentManager(guiManager.Game.Services);
            content.RootDirectory = "Content";
            text = string.Empty;  // hack to avoid a render-time null check
            _acceptsFocus = false;
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
            base.LoadContent();
            spriteFont = content.Load<SpriteFont>(fontName);

            if (text != null && text.Length > 0)
                Size = spriteFont.MeasureString(text);
            else
                Size = spriteFont.MeasureString("T");

            // Set text alignment by position, not origin (for DPanel interior clipping)
            float xOffset = Position.X;
            float yOffset = Position.Y;

            switch (horizontalAlignment)
            {
                case DHorizontalAlignment.Center:
                    xOffset -= (Size.X / 2f);
                    break;
                case DHorizontalAlignment.Left:
                    break;
                case DHorizontalAlignment.Right:
                    xOffset -= Size.X;
                    break;
                default:
                    break;
            }

            switch (verticalAlignment)
            {
                case DVerticalAlignment.Center:
                    yOffset -= (Size.Y / 2f);
                    break;
                case DVerticalAlignment.Bottom:
                    yOffset -= Size.Y;
                    break;
                case DVerticalAlignment.Top:
                    break;
                default:
                    break;
            }

            Position = new Vector2(xOffset, yOffset);
        }


        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
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

        protected override void DrawWithClipRect()
        {
            if (Visible)
            {
                Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

                if (SourceRectangle.HasValue && IsPartiallyObscured)
                {
                    //Game.GraphicsDevice.ScissorRectangle = SourceRectangle.Value;
                    //Game.GraphicsDevice.RenderState.ScissorTestEnable = true;

                    //Game.GraphicsDevice.SetRenderTarget(0, null);
                    RenderTarget2D renderTarget = new RenderTarget2D(Game.GraphicsDevice, 
                        SourceRectangle.Value.Width, 
                        SourceRectangle.Value.Height, 
                        1,
                        SurfaceFormat.Color, 
                        RenderTargetUsage.PreserveContents);

                    Game.GraphicsDevice.SetRenderTarget(0, renderTarget);

                    Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.TransparentWhite, 1, 0);

                    //draw the text!
                    SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                    spriteBatch.DrawString(spriteFont,
                                            Text,
                                            new Vector2(SourceRectangle.Value.X, -SourceRectangle.Value.Y),
                                            fontColor,
                                            0,
                                            Origin,
                                            1,
                                            SpriteEffects.None,
                                            0);
                    spriteBatch.End();

                    Game.GraphicsDevice.SetRenderTarget(0, null);
                    //Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0);

                    _texture = renderTarget.GetTexture();

                    _guiManager.SpriteBatch.Draw(_texture, 
                        drawPos + new Vector2(SourceRectangle.Value.X, SourceRectangle.Value.Y),
                        new Rectangle(0,0, SourceRectangle.Value.Width, SourceRectangle.Value.Height),
                        fontColor, 
                        0f, 
                        Vector2.Zero, 
                        1f, 
                        SpriteEffects.None, 
                        0);

                }
                else if (!Parent.IsTotallyObscured)
                {
                    //draw the text!
                    _guiManager.SpriteBatch.DrawString(spriteFont,
                                            Text,
                                            drawPos,
                                            fontColor,
                                            0,
                                            Origin,
                                            1,
                                            SpriteEffects.None,
                                            0);
                }
                

                

                //Game.GraphicsDevice.RenderState.ScissorTestEnable = false;
            }
        }
    }
}

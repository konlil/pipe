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
    // Scalable image
    public class DImage : DPanel
    {
        const int WIDTH = 100;
        const int HEIGHT = 100;

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

        ContentManager content;
        protected Texture2D image = null;       // Image
        protected float scale = 1f;             // Image scaling
        protected string imageName = null;      // Content path for image
        public DHorizontalAlignment horizontalAlignment = DHorizontalAlignment.Left;
        public DVerticalAlignment verticalAlignment = DVerticalAlignment.Top;

        Rectangle? scaledRectangle = null;
        Vector2 scaledOrigin = Vector2.Zero;


        #region Public properties
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }
        public Texture2D Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
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
        #endregion


        public DImage(DGuiManager guiManager, float x, float y, string _imageName, int _width, int _height)
            : this(guiManager, x, y, _imageName)
        {
            Size = new Vector2(_width, _height);
        }

        public DImage(DGuiManager guiManager, float x, float y, string _imageName)
            : this(guiManager, x, y)
        {
            imageName = _imageName;
        }

        public DImage(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DImage(DGuiManager guiManager)
            : base(guiManager)
        {
            Size = new Vector2(WIDTH, HEIGHT);

            content = new ContentManager(guiManager.Game.Services);
            content.RootDirectory = "Content";
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

            if (imageName != null)
                image = content.Load<Texture2D>(Path.Combine(content.RootDirectory, imageName));

            if (image != null)
            {
                scale = Size.Y / image.Height;
                Size = new Vector2(image.Width * scale, image.Height * scale); // destructive
            }

            // Use new origin based on alignments
            /*float xOrig, yOrig;
            if (horizontalAlignment == DHorizontalAlignment.Left)
                xOrig = 0f;
            else if (horizontalAlignment == DHorizontalAlignment.Right)
                xOrig = Size.X;
            else
                xOrig = Size.X / 2f;

            if (verticalAlignment == DVerticalAlignment.Top)
                yOrig = 0f;
            else if (verticalAlignment == DVerticalAlignment.Bottom)
                yOrig = Size.Y;
            else
                yOrig = Size.Y / 2f;

            Origin = new Vector2(xOrig, yOrig);*/
        }


        protected override void UnloadContent()
        {
            if (image != null)
                image.Dispose();
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

            // Update scale value
            if (SourceRectangle.HasValue)
                scaledRectangle = new Rectangle((int)(SourceRectangle.Value.X / scale),
                                                (int)(SourceRectangle.Value.Y / scale),
                                                (int)(SourceRectangle.Value.Width / scale),
                                                (int)(SourceRectangle.Value.Height / scale));
            else
                scaledRectangle = null;

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
            //base.DrawWithClipRect();

            if (image != null)
            {
                Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y);

                // draw the image
                _guiManager.SpriteBatch.Draw(image,
                                drawPos + Offset,
                                scaledRectangle,
                                Color.White,
                                0,
                                Origin,
                                scale,
                                SpriteEffects.None,
                                0);
            }
        }
    }
}

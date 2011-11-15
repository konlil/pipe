using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Angella.ConsoleComponent;

namespace Pipe
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PipeEngine : Microsoft.Xna.Framework.Game
    {
#region 成员变量
        private GraphicsDeviceManager graphics;
        private static SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Matrix global_transformation;
        private Vector2 screen_scaling_factor;

        internal Color background_color;
        private Texture2D cursor;

        private float MsCostThisFrame = 0;
        private float MsTotalCost = 0;
        private float MsTickThisFrame = 0;
        private int FrameCountThisScecond = 0;
        private int FrameCountTotal = 0;
        private int fps = 0;

        private readonly SceneManager scm;
        private Effect base_effect;

        private ConsoleComponent console;

        public PipeEngine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            background_color = Color.Black;
            scm = new SceneManager(this);
            
#if DEBUG
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
#endif
        }
#endregion

#region 属性
        public Viewport view_port;
        public SceneManager Scm
        {
            get { return scm; }
        }

        public Color BackgroundColor
        {
            get { return background_color; }
            set { background_color = value; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public Effect BaseEffect
        {
            get { return base_effect; }
        }
        public static SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont DefaultFont
        {
            get { return spriteFont; }
        }

        public Vector2 ScreenScalingFactor
        {
            get { return screen_scaling_factor; }
        }

        public Matrix GlobalTransformation
        {
            get { return global_transformation; }
        }

        public float Fps
        {
            get { return fps;  }
        }

        public ConsoleComponent DebugConsole
        {
            get { return console; }
        }
#endregion
       
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDeviceCapabilities caps = GraphicsDevice.GraphicsDeviceCapabilities;
            if( caps.MaxPixelShaderProfile < ShaderProfile.PS_3_0
                || caps.MaxVertexShaderProfile < ShaderProfile.VS_3_0 )
            {
                throw new InvalidOperationException("引擎至少需要VS3.0和PS3.0的显卡");
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts\\default_font");
            cursor = Content.Load<Texture2D>("Textures\\MouseCursor");
            base_effect = Content.Load<Effect>("Effects\\Generic");

            GraphicsDevice.RenderState.FogEnable = false;
            GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

#if DEBUG
            graphics.PreferMultiSampling = false;
            GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.None;
#else
            graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;
#endif
            graphics.ApplyChanges();
            GraphicsDevice.DeviceReset += new EventHandler(GraphicsDevice_DeviceReset);

            console = new ConsoleComponent(this, 17, Color.White);
            Components.Add(console);

            base.Initialize();
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs args)
        {
            screen_scaling_factor.X = (float)graphics.GraphicsDevice.Viewport.Width / 800f;
            screen_scaling_factor.Y = (float)graphics.GraphicsDevice.Viewport.Height / 600f;

            global_transformation = Matrix.CreateScale(screen_scaling_factor.X, screen_scaling_factor.Y, 1);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            screen_scaling_factor.X = (float)graphics.GraphicsDevice.Viewport.Width / 800f;
            screen_scaling_factor.Y = (float)graphics.GraphicsDevice.Viewport.Height / 600f;

            global_transformation = Matrix.CreateScale(screen_scaling_factor.X, screen_scaling_factor.Y, 1);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            console.Clear();

            Input.Update();
            scm.Update(gameTime);

            base.Update(gameTime);
        }

        private void UpdateFps(GameTime gametime)
        {
            MsCostThisFrame = (float)gametime.ElapsedRealTime.TotalMilliseconds;
            
            if (MsCostThisFrame <= 0)
                MsCostThisFrame = 0.0001f;
            
            MsTotalCost += MsCostThisFrame;
            
            FrameCountThisScecond++;
            FrameCountTotal++;

            if( MsTotalCost - MsTickThisFrame > 1000.0f )
            {
                fps = (int)(FrameCountThisScecond * 1000.0f / (MsTotalCost - MsTickThisFrame));
                MsTickThisFrame = MsTotalCost;
                FrameCountThisScecond = 0;
            }
        }

        public void DrawString(string str, Vector2 pos)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.DrawString(spriteFont, str, pos, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            GraphicsDevice.RenderState.DepthBufferEnable = true;
            
            if(Input.Keyboard.IsKeyDown(Keys.F9))
            {
                GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                GraphicsDevice.RenderState.CullMode = CullMode.None;
            }
            else
            {
                GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            }
            
            scm.Draw(gameTime);
            base.Draw(gameTime);

            UpdateFps(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, Fps.ToString(), new Vector2(2, 2), Color.White);
            spriteBatch.Draw(cursor, new Rectangle(Input.MousePosition.X, Input.MousePosition.Y, 32, 32), Color.White);
            spriteBatch.End();
        }
    }
}

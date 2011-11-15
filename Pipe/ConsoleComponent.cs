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
using Pipe;


namespace Angella.ConsoleComponent
{
    public class ConsoleComponent : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteFont consoleFont;

        private float interline;
        private Color textColor;

        private List<string> messages;
        
        public ConsoleComponent(PipeEngine game, float interline, Color defaultTextColor) : base(game)
        {
            this.interline = interline;
            this.textColor = defaultTextColor;

            this.messages = new List<string>();

            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            consoleFont = game.Content.Load<SpriteFont>("Fonts\\default_font");
        }

        public void Clear()
        {
            messages.Clear();
        }

        public void WriteLine()
        {
            messages.Add("");
        }

        public void WriteLine(string text)
        {
            messages.Add(text);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();

            float y = interline;
            foreach (string text in messages)
            {
                spriteBatch.DrawString(consoleFont, text, new Vector2(interline, y), textColor);
                y += interline;
            }

            spriteBatch.End();
        }
    }
}
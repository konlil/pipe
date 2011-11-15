using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pipe;

namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PipeGame : PipeEngine 
    {
        Scene scene;

        public PipeGame()
        {

        }

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scene = new Scene(this, 0, "test_scene");
            SceneManager.AddScene(scene);

            base.Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
    }
}

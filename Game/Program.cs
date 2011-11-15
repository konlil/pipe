using System;

namespace Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PipeGame game = new PipeGame())
            {
                game.Run();
            }
        }
    }
}


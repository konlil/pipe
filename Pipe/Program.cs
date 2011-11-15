using System;

namespace Pipe
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //using (PipeEngine game = new PipeEngine())
            using (TestGame game = new TestGame())
            {
                game.Run();
            }
        }
    }
}


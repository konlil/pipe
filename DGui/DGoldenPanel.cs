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

 


namespace DGui
{
    public enum DGoldenPanelAlignment { Horizontal, Vertical };
    

    // Panel that uses the golden ratio
    // Specify the longest side, and set the flag whether you want to use vertical or horizontal
    public class DGoldenPanel : DPanel
    {
        DGoldenPanelAlignment alignment = DGoldenPanelAlignment.Horizontal;


        public DGoldenPanel(DGuiManager guiManager, int dimension, DGoldenPanelAlignment _alignment)
            : this(guiManager)
        {
            alignment = _alignment;
            float height = dimension;
            float width = dimension;

            // Use golden ratio to get size
            // The longest side is used to obtain the length of the shorter.
            if (alignment == DGoldenPanelAlignment.Horizontal)
                height = GoldenRatio.ShortFromLong(width);
            else
                width = GoldenRatio.ShortFromLong(height);

            Size = new Vector2(width, height);
        }

        public DGoldenPanel(DGuiManager guiManager)
            : base(guiManager)
        {
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



    }
}

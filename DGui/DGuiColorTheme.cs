using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DGui
{
    /// <summary>
    /// Color themes for DPanel drawing
    /// </summary>
    public class DGuiColorTheme : ICloneable
    {
        /// <summary>
        /// Fill color
        /// </summary>
        public Color FillColor = new Color(229, 238, 255);

        /// <summary>
        /// Fill color for forms, used instead of FillColor when DPanel._isForm is true
        /// </summary>
        public Color FormFillColor = new Color(252, 253, 255);

        /// <summary>
        /// Border color
        /// </summary>
        public Color BorderColor = new Color(20, 20, 20);

        /// <summary>
        /// Fill color for input objects (textbox, combobox, listbox)
        /// </summary>
        public Color InputFillColor = Color.White;

        /// <summary>
        /// Fill color for clickable/selectable objects, when clicked
        /// </summary>
        public Color ClickedFillColor = new Color(112, 162, 255);

        /// <summary>
        /// Border color for clickable/selectable objects, when clicked
        /// </summary>
        public Color ClickedBorderColor = Color.Black;


        /// <summary>
        /// Fill color to be used when mouse is hovering over, when DPanel.UseHoverColor is true
        /// </summary>
        public Color HoverFillColor = new Color(255, 249, 224);

        /// <summary>
        /// Border color to be used when mouse is hovering over, when DPanel.UseHoverColor is true
        /// </summary>
        public Color HoverBorderColor = new Color(10, 10, 10);

        /// <summary>
        /// Main font color for all controls
        /// </summary>
        public Color FontColor = Color.Black;

        /// <summary>
        /// Color for editable text
        /// </summary>
        public Color InputFontColor = Color.Blue;


        public DGuiColorTheme()
        {
        }

        // ICloneable
        #region Clone
        public Object Clone()
        {
            DGuiColorTheme ct = (DGuiColorTheme)this.MemberwiseClone();
            return (Object)ct;
        }
        #endregion
    }

    public static class DGuiColorThemePresets
    {
        //public static DGuiColorTheme BlueTheme = null;
        //public static DGuiColorTheme GreenTheme = null;
        //public static DGuiColorTheme YellowTheme = null;
        //public static DGuiColorTheme OrangeTheme = null;
        //public static DGuiColorTheme RedTheme = null;
        //public static DGuiColorTheme PurpleTheme = null;

        //public static DGuiColorTheme GreyTheme = null;
        //public static DGuiColorTheme WhiteTheme = null;


        //static DGuiColorThemePresets()
        //{
        //    // Setup themes
        //    BlueTheme = new DGuiColorTheme();
        //    BlueTheme.ControlFillColor = new Color(149, 175, 204);
        //    BlueTheme.ClickedFillColor = new Color(66, 106, 152);
        //    BlueTheme.BorderColor = new Color(55, 89, 127);
        //    BlueTheme.HoverFillColor = new Color(88, 142, 204);

        //    GreenTheme = new DGuiColorTheme();
        //    GreenTheme.ControlFillColor = new Color(170, 252, 160);
        //    GreenTheme.ClickedFillColor = new Color(56, 255, 59);
        //    GreenTheme.BorderColor = new Color(20, 20, 20);
        //    GreenTheme.HoverFillColor = new Color(117, 255, 117);

        //    YellowTheme = new DGuiColorTheme();
        //    YellowTheme.ControlFillColor = new Color(255, 252, 214);
        //    YellowTheme.ClickedFillColor = new Color(255, 245, 117);
        //    YellowTheme.BorderColor = new Color(20, 20, 20);
        //    YellowTheme.HoverFillColor = new Color(255, 240, 168);

        //    RedTheme = new DGuiColorTheme();
        //    RedTheme.ControlFillColor = new Color(204, 166, 166);
        //    RedTheme.ClickedFillColor = new Color(76, 43, 43);
        //    RedTheme.BorderColor = new Color(127, 72, 72);
        //    RedTheme.HoverFillColor = new Color(204, 115, 115);

        //    PurpleTheme = new DGuiColorTheme();
        //    PurpleTheme.ControlFillColor = new Color(166, 156, 255);
        //    PurpleTheme.ClickedFillColor = new Color(121, 75, 255);
        //    PurpleTheme.BorderColor = new Color(20, 20, 20);
        //    PurpleTheme.HoverFillColor = new Color(136, 109, 255);

        //    OrangeTheme = new DGuiColorTheme();
        //    OrangeTheme.ControlFillColor = new Color(255, 197, 142);
        //    OrangeTheme.ClickedFillColor = new Color(255, 150, 81);
        //    OrangeTheme.BorderColor = new Color(20, 20, 20);
        //    OrangeTheme.HoverFillColor = new Color(255, 164, 112);

        //    GreyTheme = new DGuiColorTheme();
        //    GreyTheme.ControlFillColor = new Color(209, 209, 209);
        //    GreyTheme.ClickedFillColor = new Color(142, 142, 142);
        //    GreyTheme.BorderColor = new Color(81, 81, 81);
        //    GreyTheme.HoverFillColor = new Color(188, 188, 188);

        //    WhiteTheme = new DGuiColorTheme();
        //    WhiteTheme.ControlFillColor = new Color(255, 255, 255);
        //    WhiteTheme.ClickedFillColor = new Color(255, 150, 81);
        //    WhiteTheme.BorderColor = new Color(20, 20, 20);
        //    WhiteTheme.HoverFillColor = new Color(255, 164, 112);
        //}
    }
}

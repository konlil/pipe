using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DGui;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
    public class MainUI : DrawableGameComponent
    {
        Game1 game;

        DForm form;
        DButton button1;
        DCheckbox check1;

        DGuiManager gui_manager;

        public MainUI(Game1 game)
            :base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            base.Initialize();

            gui_manager = game.GuiManager;

            form = new DForm(gui_manager, "test", null);
            form.Size = new Vector2(800, 600);
            form.Position = new Vector2(0, 0);
            form.Alpha = 0;
            form.Initialize();
            gui_manager.AddControl(form);

            DLayoutFlow layout = new DLayoutFlow(2, 2, 60, 20, DLayoutFlow.DLayoutFlowStyle.Vertically);
            layout.Position = new Vector2(10, 10);

            button1 = new DButton(gui_manager);
            layout.Add(button1);
            button1.Text = "test";
            button1.Initialize();
            form.AddPanel(button1);
            button1.OnClick += new DButtonEventHandler(button_OnClick);

            check1 = new DCheckbox(gui_manager);
            layout.Add(check1);
             check1.Text = "Enable Fog";
            check1.FontColor = Color.White;
            check1.FillColor = Color.Wheat;
            check1.Checked = true;
            check1.Initialize();
            form.AddPanel(check1);
            check1.OnToggle += new CheckboxEventHandler(check_OnCheck);
        }

        void button_OnClick(GameTime gametime)
        {
        }

        void check_OnCheck()
        {
            game.Scm.ActiveScene.FogEnabled = check1.Checked;
        }
    }
}

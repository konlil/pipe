using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Angella.ConsoleComponent;

namespace Pipe
{
    class TestGame : PipeEngine
    {
        public TestScene scene;
        Camera camera;

        Script script;

        public TestGame()
        {
            this.Window.Title = "test scene";

            script = new Script();
            script.DoFile("scripts\\test.lua");
        }

        protected override void Initialize()
        {
            base.Initialize();

            scene = new TestScene(this);
            Scm.AddScene(scene);
            Scm.SetActiveScene(scene);

            /*
            Vector3 camera_position = new Vector3(0, 10, 51);
            Vector3 camera_target = camera_position + Vector3.Forward;
            Camera cam = new Camera();
            cam.Initialize();
            cam.Fov = MathHelper.PiOver4;
            scene.ActiveCamera = cam;
             */
            camera = new Camera(this);
            scene.ActiveCamera = camera;
           

            //camera_ctrl = new FpsCameraController(this);
            //cam.AttachController(camera_ctrl);
            //camera_ctrl.Enabled = true;

            //////////////////////////////////////////////////////////////////////////
            Axis axis = new Axis(this);
            axis.Initialize();
            scene.AddEntity(axis);

            axis.Pose.SetScale(10, 10, 10);

            ////////////////////////////////我是忧郁的分割线//////////////////////////////////////////
            //Terrain terrain = new Terrain(this, "Textures\\heightmap1", null);
            //terrain.Initialize();
            //scene.AddEntity(terrain);

            //camera.Target = new Vector3(terrain.WidthInPixel / 2.0f, 0, -terrain.HeightInPixel / 2.0f);
            camera.Target = new Vector3(0, 0, 0);

            //////////////////////////////////////////////////////////////////////////
            //Quad s1 = new Quad(this);
            //s1.Initialize();
            //scene.AddEntity(s1);

            //s1.Pose.SetPosition(0, 0, 0);
            //s1.Pose.SetScale(4, 4, 1);

            //////////////////////////////////////////////////////////////////////////
            //Avatar a1 = new Avatar(this, "Models\\p1_wedge");
            //a1.Initialize();
            //scene.AddEntity(a1);

            //a1.pose.SetPosition(10, 20, -10);
            //a1.pose.SetScale(0.01f, 0.01f, 0.01f);

            //////////////////////////////////////////////////////////////////////////
            Box b1 = new Box(this);
            b1.Initialize();
            scene.AddEntity(b1);

            b1.pose.SetScale(10.0f, 10.0f, 10.0f);
            b1.pose.SetPosition(0, 10, 0);

        }

        protected override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        protected override void Draw(GameTime gametime)
        {
            base.Draw(gametime);
        }
    }
}

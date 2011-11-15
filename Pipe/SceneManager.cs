using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public sealed class SceneManager : DrawableGameComponent
    {

        private PipeEngine engine;
        List<Scene> scenes = new List<Scene>();
        private ushort sceneid;
        private bool is_initialized;

        private Scene active_scene;

        public const byte SCENE_MAX_LIGHTS = 8;
        public static byte TotalLights = 0;
        public static byte EnabledLights = 0;

        internal SceneManager(PipeEngine engine)
            : base(engine)
        {
            this.engine = engine;
        }

        public Scene ActiveScene
        {
            get { return active_scene; }
        }

#region DrawableGameComponent重载函数
        public override void Initialize()
        {
            base.Initialize();

            foreach(Scene scene in scenes)
            {
                scene.Initialize();
            }

            is_initialized = true;
        }

        protected override void UnloadContent()
        {
            foreach(Scene scene in scenes)
            {
            }
        }

        public override void Update(GameTime gametime)
        {
            foreach (Scene scene in scenes)
            {
                scene.Update(gametime);
            }
        }

        public override void Draw(GameTime gametime)
        {
            if( active_scene != null )
            {
                active_scene.Draw(gametime);
            }
        }
#endregion

        public void AddScene(Scene scene)
        {
            bool already_exist = false;
            foreach(Scene s in scenes)
            {
                if (s.scene_id == scene.scene_id)
                {
                    already_exist = true;
                    break;
                }
            }

            if (already_exist)
                throw new InvalidOperationException("The scene already exist in scenemanager");

            scene.scene_id = sceneid++;
            if (is_initialized)
                scene.Initialize();

            scenes.Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            if (active_scene == scene)
                throw new InvalidOperationException("Attemp to remove active scene");
            
            scenes.Remove(scene);
        }

        public void SetActiveScene(Scene s)
        {
            bool find = false;
            foreach(Scene scene in scenes)
            {
                if(scene.scene_id == s.scene_id)
                {
                    active_scene = scene;
                    find = true;
                    break;
                }
            }

            if (!find)
                throw new InvalidOperationException("Don't exist the scene");
        }

        public void SetActiveScene(int index)
        {
            foreach(Scene scene in scenes)
            {
                if( scene.scene_id == index )
                {
                    active_scene = scene;
                    break;
                }
            }
        }
    }
}

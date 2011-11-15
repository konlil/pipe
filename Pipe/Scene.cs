using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public abstract class Scene
    {
        protected PipeEngine engine;
        protected List<Entity> entities;
        protected Camera active_camera;

        protected bool fog_enabled;
        internal int scene_id;
        private string scene_name;

        private List<Light> lights = new List<Light>(SceneManager.SCENE_MAX_LIGHTS);

        public Scene(PipeEngine engine, ushort id, string name)
        {
            this.engine = engine;
            this.scene_id = id;
            this.scene_name = name;
            this.entities = new List<Entity>();
        }

        public Camera ActiveCamera
        {
            get { return active_camera; }
            set { active_camera = value; }
        }
 
        public bool FogEnabled
        {
            get { return fog_enabled; }
            set { fog_enabled = value; }
        }

        public virtual void Initialize()
        {
            
        }

        public void AddEntity(Entity entity)
        {
            if (engine.GraphicsDevice.IsDisposed)
                return;

            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public virtual void Update(GameTime gametime)
        {
            if (active_camera != null)
            {
                active_camera.Update(gametime);
            }

            foreach (Object3d entity in entities)
                entity.Update(gametime);
        }

        public virtual void Draw(GameTime gametime)
        {
            if (active_camera == null)
                return;

            foreach (Entity entity in entities)
                entity.Draw(gametime, active_camera);

        }
    }
}

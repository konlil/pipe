using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class FogInfo : ICloneable
    {
        public bool enabled;
        public Vector4 color;
        public float start;
        public float end;

        public FogInfo()
        {
            enabled = false;
            color = Vector4.One;
            start = 50;
            end = 500;
        }

        public object Clone()
        {
            FogInfo info = new FogInfo();
            info.enabled = this.enabled;
            info.color = this.color;
            info.end = this.end;
            info.start = this.start;
            return info;
        }
    }

    public class EnvInfo : ICloneable
    {
        public Vector3 camera_position;

        public Vector3 ambient_color;
        public FogInfo fog_info;

        public EnvInfo()
        {
            camera_position = Vector3.Zero;
            ambient_color = Vector3.One;
            fog_info = new FogInfo();
        }

        public object Clone()
        {
            EnvInfo info = new EnvInfo();
            info.camera_position = this.camera_position;
            info.ambient_color = this.ambient_color;
            info.fog_info = fog_info.Clone() as FogInfo;

            return info;
        }
    }

    public abstract class Scene
    {
        protected PipeEngine engine;
        protected List<Entity> entities;
        protected Camera active_camera;

        protected bool fog_enabled;
        internal int scene_id;
        private string scene_name;

        protected EnvInfo env_info;

        private List<Light> lights = new List<Light>(SceneManager.SCENE_MAX_LIGHTS);

        public Scene(PipeEngine engine, ushort id, string name)
        {
            this.engine = engine;
            this.scene_id = id;
            this.scene_name = name;
            this.entities = new List<Entity>();

            this.env_info = new EnvInfo();
            this.env_info.ambient_color = new Vector3(1, 1, 1);
            this.env_info.fog_info.enabled = true;
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

        public void AddLight(Light light)
        {
            //更新光源数量
            SceneManager.TotalLights += 1;
            if (SceneManager.TotalLights >= SceneManager.SCENE_MAX_LIGHTS)
                throw new IndexOutOfRangeException(string.Format("无法添加大于{0}个光源", SceneManager.SCENE_MAX_LIGHTS));

            //在集合中添加光源
            lights.Add(light);
        }

        public virtual void Update(GameTime gametime)
        {
            if (active_camera != null)
            {
                active_camera.Update(gametime);

                env_info.camera_position = active_camera.Position;
            }

            foreach (Object3d entity in entities)
            {
                entity.Update(gametime);
            }
        }

        public virtual void Draw(GameTime gametime)
        {
            if (active_camera == null)
                return;

            foreach (Entity entity in entities)
            {
                entity.ApplyEnvInfo(env_info);
                entity.Draw(gametime, active_camera);
            }

        }
    }
}

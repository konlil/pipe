using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public abstract class Object3d
    {
        PipeEngine engine;
        internal WorldPose pose;
        protected Vector3 yaw_pitch_roll;

        protected bool is_visible;
        protected bool is_initialized;

        internal List<IController> controllers = new List<IController>();

        public Object3d(PipeEngine engine)
        {
            this.engine = engine;
            this.pose = new WorldPose();
        }

        #region 属性
        public bool IsVisible
        {
            get { return is_visible; }
            set { is_visible = value; }
        }
    
        public bool IsInitialized
        {
            get { return is_initialized; }
            set { is_initialized = value; }
        }

        internal PipeEngine PipeEngine { set { engine = value; } }
        public PipeEngine Engine { get { return engine; } }

        public WorldPose Pose
        {
            get { return pose; }
            set { pose = value; }
        }

        public Vector3 YawPitchRoll 
        {
            get { return yaw_pitch_roll; }
            set { yaw_pitch_roll = new Vector3(value.X % 360, value.Y % 360, value.Z % 360); }
        }

        public float Yaw 
        {
            get { return yaw_pitch_roll.X; }
            set { yaw_pitch_roll.X = value % 360; }
        }
        public float Pitch 
        { 
            get { return yaw_pitch_roll.Y; }
            set { yaw_pitch_roll.Y = value % 360; }
        }
        public float Roll 
        { 
            get { return yaw_pitch_roll.Z; }
            set { yaw_pitch_roll.Z = value % 360; }
        }

        public string Name { get; set; }
        #endregion

        public abstract void Initialize();

        public void SetScale(float sx, float sy, float sz)
        {
            this.pose.SetScale(sx, sy, sz);
        }

        public void SetPosition(float px, float py, float pz)
        {
            this.pose.SetPosition(px, py, pz);
        }

        public virtual void Update(GameTime gametime)
        {
            if( controllers.Count > 0 )
            {
                for(int i=0; i<controllers.Count; i++)
                {
                    controllers[i].Update(gametime);
                }
            }
        }

        public void AttachController(IController controller)
        {
            if( !controllers.Contains(controller))
            {
                controllers.Add(controller);
                controller.Attach(this);
            }
        }

        public void DetatchController(IController controller)
        {
            if(controllers.Contains(controller))
            {
                controllers.Remove(controller);
                controller.Detatch();
            }
        }
    }
}

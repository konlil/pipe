using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public enum LightType
    {
        /// <summary>
        /// 方向光
        /// </summary>
        Directional,
        
        /// <summary>
        /// 点光源
        /// </summary>
        Point,

        /// <summary>
        /// 聚光灯
        /// </summary>
        Spot,
    }
    public class Light
    {
        private bool enabled;

        private Vector3 color;
        private Vector3 direction;
        private Vector3 position;

        public Vector4 spot_prop;

        private LightType light_type;

        private EffectParameter position_param;
        private EffectParameter instance_param;

        /// <summary>
        /// 定义一个方向光
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        public Light(PipeEngine engine, Vector3 color, Vector3 direction)
            : this(engine, LightType.Directional, color, -direction, direction, 0, 0, 0, 0)
        {
            if (direction == Vector3.Zero)
                throw new System.ArgumentException("方向光必须设置方向");
        }

        /// <summary>
        /// 定义一个点光源
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="color"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <param name="fallof"></param>
        public Light(PipeEngine engine, Vector3 color, Vector3 position, float range, float fallof)
            : this(engine, LightType.Point, color, position, Vector3.Zero, range, fallof, 0, 0)
        {

        }

        /// <summary>
        /// 定义一个聚光灯
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="color"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="range"></param>
        /// <param name="fallof"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        public Light(PipeEngine engine, Vector3 color, Vector3 position, Vector3 direction, float range, float fallof, float theta, float phi)
            : this(engine, LightType.Spot, color, position, direction, range, fallof, theta, phi)
        {

        }

        public Light(PipeEngine engine, LightType lt, Vector3 color, Vector3 position, Vector3 direction, float range, float fallof, float theta, float phi)
        {
            //instance_param = engine.BaseEffect.Parameters["gLights"].Elements[SceneManager.TotalLights];
           // position_param = instance_param.StructureMembers["position"];

            this.light_type = lt;
            instance_param.StructureMembers["light_type"].SetValue((float)light_type);
            this.color = color;
            this.position = position;
            this.enabled = true;
            this.spot_prop.X = range;
            this.spot_prop.Y = fallof;
            this.spot_prop.Z = theta;
            this.spot_prop.W = phi;
            instance_param.StructureMembers["spot_prop"].SetValue(spot_prop);
        }

        public LightType LightType { get { return light_type; } }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (enabled)
                    instance_param.StructureMembers["enabled"].SetValue(1.0f);
                else
                    instance_param.StructureMembers["enabled"].SetValue(0.0f);
            }
        }

        public Vector3 Color
        {
            get { return color; }
            set
            {
                color = value;
                instance_param.StructureMembers["color"].SetValue(color);
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                this.position_param.SetValue(position);
                if( light_type == LightType.Directional )
                {
                    direction = -position;
                    instance_param.StructureMembers["direction"].SetValue(direction);
                }
            }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                instance_param.StructureMembers["direction"].SetValue(direction);
                if( light_type == LightType.Directional )
                {
                    position = -direction;
                }
            }
        }

        public float Range
        {
            get { return spot_prop.X; }
            set
            {
                spot_prop.X = value;
                instance_param.StructureMembers["spot_prop"].SetValue(spot_prop);
            }
        }

        public float FallOff
        {
            get { return spot_prop.Y; }
            set
            {
                spot_prop.Y = value;
                instance_param.StructureMembers["spot_prop"].SetValue(spot_prop);
            }
        }

        public float Theta
        {
            get { return spot_prop.Z; }
            set
            {
                spot_prop.Z = value;
                instance_param.StructureMembers["spot_prop"].SetValue(spot_prop);
            }
        }

        public float Phi
        {
            get { return spot_prop.W; }
            set
            {
                spot_prop.W = value;
                instance_param.StructureMembers["spot_prop"].SetValue(spot_prop);
            }
        }
    }
}

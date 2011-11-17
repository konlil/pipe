using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class BaseMaterial : IMaterial
    {
        protected PipeEngine engine;

        protected Effect effect;
        protected string current_technique_name;

        //local fog info
        protected bool fog_enabled;
        protected Vector3 fog_color;
        protected float fog_start;
        protected float fog_end;

        protected EnvInfo env_info;

        public BaseMaterial(PipeEngine engine, string effect_name)
        {
            this.engine = engine;

            Effect eff = engine.Content.Load<Effect>(effect_name);
            this.effect = eff.Clone(engine.GraphicsDevice);
            this.current_technique_name = effect.Techniques[0].Name;
            this.effect.CurrentTechnique = effect.Techniques[0];

            this.env_info = new EnvInfo();
        }

        public BaseMaterial(PipeEngine engine, Effect eff)
        {
            this.engine = engine;
            this.effect = eff.Clone(engine.GraphicsDevice);
            this.current_technique_name = effect.Techniques[0].Name;
            this.effect.CurrentTechnique = effect.Techniques[0];

            this.env_info = new EnvInfo();
        }

        #region IMaterial Members

        public Effect EffectInstance
        {
            get { return effect; }
        }

        public string CurrentTechniqueName
        {
            get
            {
                return current_technique_name;
            }
            set
            {
                if (current_technique_name != value)
                {
                    current_technique_name = value;
                    effect.CurrentTechnique = effect.Techniques[current_technique_name];
                }
            }
        }

        public bool FogEnabled
        {
            get { return fog_enabled; }
            set
            {
                fog_enabled = value;
                ApplyEnvInfo();
            }
        }

        public void SetWorldMatrix(Matrix world)
        {
            EffectInstance.Parameters["World"].SetValue(world);
        }

        public void SetViewMatrix(Matrix view)
        {
            EffectInstance.Parameters["View"].SetValue(view);
        }

        public void SetProjectionMatrix(Matrix projection)
        {
            EffectInstance.Parameters["Projection"].SetValue(projection);
        }

        public void SetEnvInfo(EnvInfo info)
        {
            env_info = info;
            ApplyEnvInfo();
        }

        protected void ApplyEnvInfo()
        {
            EffectInstance.Parameters["AmbientColor"].SetValue(env_info.ambient_color);

            EffectInstance.Parameters["FogEnabled"].SetValue(fog_enabled && env_info.fog_info.enabled);
            EffectInstance.Parameters["FogStart"].SetValue(env_info.fog_info.start);
            EffectInstance.Parameters["FogEnd"].SetValue(env_info.fog_info.end);
            EffectInstance.Parameters["FogColor"].SetValue(env_info.fog_info.color);

            EffectParameter cam_pos = EffectInstance.Parameters["CameraPos"];
            if (cam_pos != null)
            {
                cam_pos.SetValue(env_info.camera_position);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class BasicMaterial : IMaterial
    {
        PipeEngine engine;

        BasicEffect basic_effect;
        string current_technique_name;
        string diffuse_texture_name;

        //local fog info
        protected bool fog_enabled;
        protected Vector3 fog_color;
        protected float fog_start;
        protected float fog_end;

        protected EnvInfo env_info;
        protected Light light_info;

        public BasicMaterial(PipeEngine engine)
        {
            this.engine = engine;

            basic_effect = new BasicEffect(engine.GraphicsDevice, null);
            basic_effect.CurrentTechnique = basic_effect.Techniques[0];
            current_technique_name = basic_effect.CurrentTechnique.Name;
        }

        #region IMaterial Members

        public Effect EffectInstance
        {
            get { return basic_effect; }
        }

        public string CurrentTechniqueName
        {
            get
            {
                return current_technique_name; 
            }
            set
            {
                if(current_technique_name != value)
                {
                    current_technique_name = value;
                    basic_effect.CurrentTechnique = basic_effect.Techniques[current_technique_name];
                }
            }
        }

        public string DiffuseTextureName
        {
            get { return diffuse_texture_name; }
            set
            {
                if(diffuse_texture_name != value)
                {
                    diffuse_texture_name = value;
                }

                if(diffuse_texture_name != null)
                {
                    basic_effect.TextureEnabled = true;
                    basic_effect.Texture = engine.Content.Load<Texture2D>(diffuse_texture_name);
                }
                else
                {
                    basic_effect.TextureEnabled = false;
                }
            }
        }

        public Vector3 DiffuseColor
        {
            get { return basic_effect.DiffuseColor; }
            set
            {
                basic_effect.DiffuseColor = value;
            }
        }

        public Vector3 SpecularColor
        {
            get { return basic_effect.SpecularColor; }
            set
            {
                basic_effect.SpecularColor = value;
            }
        }

        public float SpecularPower
        {
            get { return basic_effect.SpecularPower; }
            set
            {
                basic_effect.SpecularPower = value;
            }
        }

        public Vector2 DiffuseUVTile
        {
            get;
            set;
        }

        public bool FogEnabled
        {
            get { return fog_enabled; }
            set
            {
                fog_enabled = value;
                basic_effect.FogEnabled = value && env_info.fog_info.enabled;
            }
        }

        public void SetWorldMatrix(Matrix world)
        {
            basic_effect.World = world; 
        }

        public void SetViewMatrix( Matrix view)
        {
            basic_effect.View = view;
        }

        public void SetProjectionMatrix(Matrix projection)
        {
            basic_effect.Projection = projection;
        }

        public void SetEnvInfo(EnvInfo info)
        {
            env_info = info;
        }

        public void ApplyLight(int index, Light info)
        {
            light_info = info;
        }

        #endregion
    }
}

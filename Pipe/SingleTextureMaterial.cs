using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class SingleTextureMaterial : BaseMaterial
    {
        protected string diffuse_texture_name;
        protected string detail_texture_name;

        public SingleTextureMaterial(PipeEngine engine, string effect_name)
            :base(engine,effect_name)
        {
        }

        public SingleTextureMaterial(PipeEngine engine, Effect eff)
            :base(engine, eff)
        {
        }

        public string DiffuseTextureName
        {
            get { return diffuse_texture_name; }
            set
            {
                diffuse_texture_name = value;
                if(string.IsNullOrEmpty(diffuse_texture_name))
                {
                    EffectInstance.Parameters["DiffuseTexture"].SetValue((Texture2D)null); 
                }
                else
                {
                    Texture2D texture = engine.Content.Load<Texture2D>(diffuse_texture_name);
                    EffectInstance.Parameters["DiffuseTexture"].SetValue(texture);
                }
            }
        }

        public string DetailTextureName
        {
            get { return detail_texture_name; }
            set
            {
                detail_texture_name = value;
                if(string.IsNullOrEmpty(detail_texture_name))
                {
                    EffectInstance.Parameters["DetailTexture"].SetValue((Texture2D)null);
                    EffectInstance.Parameters["DetailEnabled"].SetValue(false);
                }
                else
                {
                    Texture2D texture = engine.Content.Load<Texture2D>(detail_texture_name);
                    EffectInstance.Parameters["DetailTexture"].SetValue(texture);
                    EffectInstance.Parameters["DetailEnabled"].SetValue(true);
                }
            }
        }
    }
}

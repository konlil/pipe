using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class SingleTextureMaterial : IMaterial
    {
        PipeEngine engine;
        
        protected Effect effect;
        protected string current_technique_name;
        protected string texture_name;

        public SingleTextureMaterial(PipeEngine engine, string effect_name)
        {
            this.engine = engine;

            Effect eff = engine.Content.Load<Effect>(effect_name);
            this.effect = eff.Clone(engine.GraphicsDevice);
            this.current_technique_name = effect.Techniques[0].Name;
            this.effect.CurrentTechnique = effect.Techniques[0];
        }

        public SingleTextureMaterial(PipeEngine engine, Effect eff)
        {
            this.engine = engine;
            this.effect = eff.Clone(engine.GraphicsDevice);
            this.current_technique_name = effect.Techniques[0].Name;
            this.effect.CurrentTechnique = effect.Techniques[0];
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
                if(current_technique_name != value)
                {
                    current_technique_name = value;
                    effect.CurrentTechnique = effect.Techniques[current_technique_name];
                }
            }
        }

        public string TextureName
        {
            get { return texture_name; }
            set
            {
                texture_name = value;
                if(string.IsNullOrEmpty(texture_name))
                {
                    EffectInstance.Parameters["Tex0"].SetValue((Texture2D)null); 
                }
                else
                {
                    Texture2D texture = engine.Content.Load<Texture2D>(texture_name);
                    EffectInstance.Parameters["Tex0"].SetValue(texture);
                }
            }
        }

        public void SetWorldMatrix(Matrix world)
        {
            EffectInstance.Parameters["World"].SetValue(world); 
        }

        public void SetViewMatrix( Matrix view)
        {
            EffectInstance.Parameters["View"].SetValue(view); 
        }

        public void SetProjectionMatrix(Matrix projection)
        {
            EffectInstance.Parameters["Projection"].SetValue(projection); 
        }

        #endregion
    }
}

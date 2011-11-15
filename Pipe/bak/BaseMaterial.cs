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
        protected Effect effect;
        protected PipeEngine engine;
        protected EffectParameter g_world;
        protected EffectParameter g_view;

        public BaseMaterial(PipeEngine engine, Effect eff)
        {
            this.engine = engine;
            if(eff == null)
            {
                throw new ArgumentException("effect");
            }

            this.effect = eff.Clone(engine.GraphicsDevice);
            this.effect.CurrentTechnique = this.effect.Techniques[0];

            g_world = this.effect.Parameters["gWorld"];
            g_view = this.effect.Parameters["gView"];
        }

        public Effect EffectInstance
        {
            get { return effect; }
        }

        public string CurrentTechniqueName
        {
            get { return effect.CurrentTechnique.Name; }
            set { effect.CurrentTechnique = effect.Techniques[value]; }
        }

        public virtual void SetEffectParameters(ref Matrix world)
        {
            g_world.SetValue(world);
            g_view.SetValue(engine.Scm.ActiveScene.ActiveCamera.ViewMatrix);
        }
    }
}

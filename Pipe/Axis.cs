using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public class Axis:Entity
    {
        public Axis(PipeEngine engine)
            : base(engine)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Mesh mesh = MeshBuilder.CreateAxis(Engine.GraphicsDevice);
            base.AddMesh(mesh);
            
            BasicMaterial material = new BasicMaterial(Engine);
            base.AddMaterial(material);

            RenderContext ctx = new RenderContext(Engine);
            ctx.Material = material;
            ctx.Mesh = mesh;
            base.AddRenderContext(ctx);

            BasicEffect eff = material.EffectInstance as BasicEffect;
            eff.VertexColorEnabled = true;
        }

        public override int Draw(GameTime gametime, Camera camera) 
        {
            Engine.GraphicsDevice.RenderState.CullMode = CullMode.None;
            return base.Draw(gametime, camera);
        }
    }
}

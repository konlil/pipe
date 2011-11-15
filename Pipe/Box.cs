using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class Box : Entity
    {
        public Box(PipeEngine engine)
            : base(engine)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            
            Mesh mesh = MeshBuilder.CreateBox(Engine.GraphicsDevice);
            base.AddMesh(mesh);

            //BasicMaterial material = new BasicMaterial(Engine);
            SingleTextureMaterial material = new SingleTextureMaterial(Engine, "Effects\\generic");
            material.CurrentTechniqueName = "TNoShader";
            material.TextureName = "Textures\\grid";
            base.AddMaterial(material);

            RenderContext ctx = new RenderContext(Engine);
            ctx.Mesh = mesh;
            ctx.Material = material;
            base.AddRenderContext(ctx);

            //material.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);
            //BasicEffect box_eff = material.EffectInstance as BasicEffect;
            //box_eff.VertexColorEnabled = true;
        }
    }
}

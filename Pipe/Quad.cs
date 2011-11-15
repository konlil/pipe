using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class Quad:Entity
    {
        public Quad(PipeEngine engine)
            : base(engine)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Mesh mesh = MeshBuilder.CreateQuad(Engine.GraphicsDevice);
            base.AddMesh(mesh);

            BasicMaterial material = new BasicMaterial(Engine);
            base.AddMaterial(material);

            RenderContext ctx = new RenderContext(Engine);
            ctx.Mesh = mesh;
            ctx.Material = material;
            base.AddRenderContext(ctx);

            material.DiffuseTextureName = "Textures\\grid";
            material.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f);
            material.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
            material.SpecularPower = 50;
        }
    }
}

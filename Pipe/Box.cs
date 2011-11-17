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
        protected RenderContext m_ctx;
        
        public Box(PipeEngine engine)
            : base(engine)
        {
            m_ctx = new RenderContext(engine);

            AddRenderContext(m_ctx);
        }

        public RenderContext Context
        {
            get { return m_ctx; }
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Mesh mesh = MeshBuilder.CreateBox(Engine.GraphicsDevice);
            base.AddMesh(mesh);

            //BasicMaterial material = new BasicMaterial(Engine);
            SingleTextureMaterial material = new SingleTextureMaterial(Engine, "Effects\\single_texture");
            material.CurrentTechniqueName = "TSingleTexture";
            material.DiffuseTextureName = "Textures\\wood";
            material.DetailTextureName = "Textures\\Detail";
            base.AddMaterial(material);

            m_ctx.Mesh = mesh;
            m_ctx.Material = material;

            //material.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);
            //BasicEffect box_eff = material.EffectInstance as BasicEffect;
            //box_eff.VertexColorEnabled = true;
        }
    }
}

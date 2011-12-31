using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class ModelEntity : Entity
    {
        private Model model;
        private string model_file;

        public ModelEntity(PipeEngine engine) : base(engine)
        {
        }

        public string ModelFile
        {
            get { return model_file; }
            set {
                model_file = value;
                model = Engine.Content.Load<Model>(model_file);
            }
        }

        public override void Initialize()
        {
            //base.Initialize();

        }

        public override void ApplyEnvInfo(EnvInfo info){}
        public override void ClearLights(){}
        public override bool ApplyLight(Light light) { return false; }

        public override int Draw(GameTime gametime, Camera camera)
        {
            Engine.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            int total_primitives = 0;
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DiffuseColor = new Vector3(255, 0, 0);
                    effect.World = pose.world_matrix;
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            Engine.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            return total_primitives;
        }
    }
}

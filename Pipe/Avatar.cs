using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Pipe
{
    public class Avatar : Entity
    {
        protected Model model;
        protected string file_name;

        public Avatar(PipeEngine engine, string filename)
            : base(engine)
        {
            this.file_name = filename;
        }

        public override void Initialize()
        {
            base.Initialize();

            if(file_name != null)
            {
                model = Engine.Content.Load<Model>(file_name);
            }
        }

        public override int Draw(Microsoft.Xna.Framework.GameTime gametime, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Debug.WriteLine(model.Meshes.Count);

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * pose.world_matrix;
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                mesh.Draw();
            }
            return model.Meshes.Count;
        }
    }
}

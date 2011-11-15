using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public class RenderContext
    {
        protected PipeEngine engine;

        protected IMaterial material;
        //protected IRenderStates renderstates;
        protected Mesh mesh;


        public RenderContext(PipeEngine engine)
        {
            this.engine = engine;
        }

        public IMaterial Material
        {
            get { return material; }
            set { material = value; }
        }

        public Mesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }
    }
}

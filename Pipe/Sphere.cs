using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipe
{
    public class Sphere : ModelEntity
    {
        protected RenderContext m_ctx;

        public Sphere(PipeEngine engine) : base(engine)
        {
        }

        public RenderContext Context
        {
            get { return m_ctx; }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public abstract class UIEntity : Object3d
    {
        protected Vector2 position;
        protected Vector2 origin;
        protected float rotation;

        protected float layer_depth;
        private int z_order;

        protected bool is_mouse_in;

        public UIEntity(PipeEngine engine, string texture_name, Vector2 pos)
            : this(engine, texture_name, pos, 0, Vector2.Zero, 0)
        {

        }

        public UIEntity(PipeEngine engine, string texture_name, Vector2 position, float rotation, Vector2 origin, float layer_depth)
            : base(engine)
        {
            this.rotation = rotation;
            this.origin = origin;
            this.layer_depth = layer_depth;
            this.position = position;
            this.z_order = 0;
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}

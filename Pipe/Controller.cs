using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public abstract class Controller : IController
    {
        public Controller(PipeEngine engine)
        {
            this.engine = engine;
        }

        private bool enabled;
        protected PipeEngine engine;
        protected Object3d controlled_entity;
        internal bool need_update;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool NeedUpdate
        {
            get { return need_update; }
            set { need_update = value; }
        }

        public abstract void Action(GameTime gametime);

        public virtual void Attach(Object3d entity)
        {
            if(controlled_entity != null)
            {
                Object3d tmp = controlled_entity;
                controlled_entity = null;
                tmp.DetatchController(this);
            }

            controlled_entity = entity;
            entity.AttachController(this);
        }

        public virtual void Detatch()
        {
            if(controlled_entity != null)
            {
                Object3d tmp = controlled_entity;
                controlled_entity = null;
                tmp.DetatchController(this);
            }
        }

        public void Update(GameTime gametime)
        {
            if(enabled && controlled_entity != null && engine.IsActive )
            {
                Action(gametime);
            }
        }
    }
}

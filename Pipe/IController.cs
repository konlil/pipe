using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public interface IController
    {
        bool Enabled { get; set; }

        void Update(GameTime gametime);
        void Attach(Object3d entity);
        void Detatch();
        void Action(GameTime gametime);
    }
}

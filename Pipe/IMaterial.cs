﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public interface IMaterial
    {
        Effect EffectInstance
        {
            get;
        }

        string CurrentTechniqueName
        {
            get;
            set;
        }

        void SetWorldMatrix(Matrix world);
        void SetViewMatrix(Matrix view);
        void SetProjectionMatrix(Matrix projection);
        
        void SetEnvInfo(EnvInfo fog_info);
        void ApplyLight(int index, Light light);
    }
}

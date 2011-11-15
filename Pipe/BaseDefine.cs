using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    namespace BaseDefine
    {
        public struct VertexPositionNormalTextureColor
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 texcoord;
            public Color color;

            public VertexPositionNormalTextureColor(Vector3 position, Vector3 normal, Vector2 texcoord, Color color)
            {
                this.position = position;
                this.normal = normal;
                this.texcoord = texcoord;
                this.color = color;
            }

            public static VertexElement[] VertexElements =
            {
                 new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                 new VertexElement(0, 3*sizeof(float), VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                 new VertexElement(0, 6*sizeof(float), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(0, 8*sizeof(float), VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
            };

            public static int SizeInBytes = 9 * sizeof(float);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public class Mesh
    {
        private VertexBuffer vb;
        private IndexBuffer ib;
        private VertexDeclaration vd;
        private PrimitiveType primitive_type;
        private DrawMode draw_mode;

        internal int vertices_count;
        internal int primitive_count;

        private bool is_visible;

#region 构造函数
        /// <summary>
        /// 创建一个mesh对象，不使用索引缓冲
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="vb"></param>
        /// <param name="vd"></param>
        public Mesh(PrimitiveType pt, VertexBuffer vb, VertexDeclaration vd)
        {
            this.is_visible = true;
            this.vertices_count = vb.SizeInBytes / vd.GetVertexStrideSize(0);
            this.draw_mode = DrawMode.Primitive;
            this.primitive_type = pt;
            this.vb = vb;
            this.vd = vd;
            this.primitive_count = CalcPrimitiveCount();
        }

        /// <summary>
        /// 创建一个mesh对象，使用索引缓冲
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="vb"></param>
        /// <param name="vd"></param>
        /// <param name="ib"></param>
        public Mesh(PrimitiveType pt, VertexBuffer vb, VertexDeclaration vd, IndexBuffer ib )
        {
            this.is_visible = true;
            this.vertices_count = vb.SizeInBytes / vd.GetVertexStrideSize(0);
            this.draw_mode = DrawMode.Indexed;
            this.primitive_type = pt;
            this.vb = vb;
            this.vd = vd;
            this.ib = ib;
            this.primitive_count = CalcPrimitiveCount();
        }
#endregion

#region 属性
        public int VerticesCount { get { return vertices_count; } }

        public int PrimitivesCount { get { return primitive_count; } }

        public DrawMode DrawMode { get { return draw_mode; } }

        public PrimitiveType PrimitiveType { get { return primitive_type; } }

        public VertexBuffer VertexBuffer { get { return vb; } }

        public IndexBuffer IndexBuffer { get { return ib; } }

        public VertexDeclaration VertexDecl { get { return vd; } }

        public bool Visible
        {
            get { return is_visible; }
            set
            {
                is_visible = value;
            }
        }

#endregion
        private int CalcPrimitiveCount()
        {
            int count = vertices_count / 3;

            if( this.draw_mode == DrawMode.Primitive)
            {
                switch(this.primitive_type)
                {
                    case PrimitiveType.TriangleList:
                        count = vertices_count / 3;
                        break;
                    case PrimitiveType.TriangleStrip:
                        count = vertices_count - 2;
                        break;
                    case PrimitiveType.LineList:
                        count = vertices_count / 2;
                        break;
                    case PrimitiveType.LineStrip:
                        count = vertices_count - 1;
                        break;
                }
            }
            else
            {
                    int size = ib.SizeInBytes;
                    int bytes_per_index = ib.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4;
                    int tmp = size / bytes_per_index;

                    switch(this.primitive_type)
                    {
                        case PrimitiveType.TriangleList:
                            count = tmp / 3;
                            break;
                        case PrimitiveType.TriangleStrip:
                            count = tmp - 2;
                            break;
                        case PrimitiveType.LineList:
                            count = tmp / 2;
                            break;
                        case PrimitiveType.LineStrip:
                            count = tmp - 1;
                            break;
                    }
                }
            
            return count;
        }

        public void PreRender(GraphicsDevice device)
        {
            device.VertexDeclaration = vd;
            device.Vertices[0].SetSource(vb, 0, vd.GetVertexStrideSize(0));
            if(draw_mode == DrawMode.Indexed)
                device.Indices = ib;
        }

        public void Render(GraphicsDevice device)
        {
            //device.RenderState.FillMode = FillMode.WireFrame;
            if (draw_mode == DrawMode.Indexed)
                device.DrawIndexedPrimitives(primitive_type, 0, 0, vertices_count, 0, primitive_count);
            else
                device.DrawPrimitives(primitive_type, 0, primitive_count);
        }
    }
}

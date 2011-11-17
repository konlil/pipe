using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pipe.BaseDefine;

namespace Pipe
{
    public class MeshBuilder
    {
        public static Mesh CreateAxis(GraphicsDevice device)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[6];

            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
            vertices[4] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
            vertices[5] = new VertexPositionColor(new Vector3(0, 0, -1), Color.Blue);

            VertexBuffer vb = new VertexBuffer(device, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            vb.SetData(vertices, 0, 6);
            VertexDeclaration vd = new VertexDeclaration(device, VertexPositionColor.VertexElements);

            return new Mesh(PrimitiveType.LineList, vb, vd);
        }

        public static Mesh CreateQuad(GraphicsDevice device)
        {
            VertexPositionNormalTexture[] vertices;

            vertices = new VertexPositionNormalTexture[4];

            vertices[0].Position = new Vector3(-0.5f, -0.5f, 0);
            vertices[0].Normal = Vector3.Backward;
            vertices[0].TextureCoordinate = new Vector2(0, 1);

            vertices[1].Position = new Vector3(-0.5f, 0.5f, 0);
            vertices[1].Normal = Vector3.Backward;
            vertices[1].TextureCoordinate = new Vector2(0, 0);

            vertices[2].Position = new Vector3(0.5f, -0.5f, 0);
            vertices[2].Normal = Vector3.Backward;
            vertices[2].TextureCoordinate = new Vector2(1, 1);

            vertices[3].Position = new Vector3(0.5f, 0.5f, 0);
            vertices[3].Normal = Vector3.Backward;
            vertices[3].TextureCoordinate = new Vector2(1, 0);

            VertexBuffer vb = new VertexBuffer(device, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            vb.SetData(vertices, 0, 4);
            VertexDeclaration vd = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements);

            return new Mesh(PrimitiveType.TriangleStrip, vb, vd);
        }
        
        //    4+---------+3
        //    /|        /|
        //  1+-|-------+2|
        //   | |       | |
        //   |8+-------|-+7
        //   |/        |/
        //  5+---------+6
        public static Mesh CreateBox(GraphicsDevice device)
        {
#region 写顶点数组

            //创建24个顶点
            VertexPositionNormalTextureColor[] vertices;

            vertices = new VertexPositionNormalTextureColor[24];

            Vector3 v1 = new Vector3(-1.0f, 1.0f, 1.0f);
            Vector3 v2 = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 v3 = new Vector3(1.0f, 1.0f, -1.0f);
            Vector3 v4 = new Vector3(-1.0f, 1.0f, -1.0f);

            Vector3 v5 = new Vector3(-1.0f, -1.0f, 1.0f);
            Vector3 v6 = new Vector3(1.0f, -1.0f, 1.0f);
            Vector3 v7 = new Vector3(1.0f, -1.0f, -1.0f);
            Vector3 v8 = new Vector3(-1.0f, -1.0f, -1.0f);

            #region     //5, 1, 2, 6
            vertices[0].position = v5;
            vertices[0].texcoord = new Vector2(0, 0);
            vertices[0].normal = Vector3.Backward;
            vertices[0].color = Color.White;

            vertices[1].position = v1;
            vertices[1].texcoord = new Vector2(0, 1);
            vertices[1].normal = Vector3.Backward;
            vertices[1].color = Color.White;

            vertices[2].position = v2;
            vertices[2].texcoord = new Vector2(1, 1);
            vertices[2].normal = Vector3.Backward;
            vertices[2].color = Color.White;

            vertices[3].position = v6;
            vertices[3].texcoord = new Vector2(1, 0);
            vertices[3].normal = Vector3.Backward;
            vertices[3].color = Color.White;
            #endregion

            #region            //8, 4, 3, 7
            vertices[4].position = v8;
            vertices[4].texcoord = new Vector2(0, 0);
            vertices[4].normal = Vector3.Forward;
            vertices[4].color = Color.White;

            vertices[5].position = v4;
            vertices[5].texcoord = new Vector2(0, 1);
            vertices[5].normal = Vector3.Forward;
            vertices[5].color = Color.White;

            vertices[6].position = v3;
            vertices[6].texcoord = new Vector2(1, 1);
            vertices[6].normal = Vector3.Forward;
            vertices[6].color = Color.White;

            vertices[7].position = v7;
            vertices[7].texcoord = new Vector2(1, 0);
            vertices[7].normal = Vector3.Forward;
            vertices[7].color = Color.White;
            #endregion

            #region //5, 1, 4, 8
            
            vertices[8].position = v5;
            vertices[8].texcoord = new Vector2(1, 0);
            vertices[8].normal = Vector3.Left;
            vertices[8].color = Color.White;

            vertices[9].position = v1;
            vertices[9].texcoord = new Vector2(1, 1);
            vertices[9].normal = Vector3.Left;
            vertices[9].color = Color.White;

            vertices[10].position = v4;
            vertices[10].texcoord = new Vector2(0, 1);
            vertices[10].normal = Vector3.Left;
            vertices[10].color = Color.White;

            vertices[11].position = v8;
            vertices[11].texcoord = new Vector2(0, 0);
            vertices[11].normal = Vector3.Left;
            vertices[11].color = Color.White;
            #endregion

            #region            //6, 2, 3, 7
            vertices[12].position = v6;
            vertices[12].texcoord = new Vector2(0, 0);
            vertices[12].normal = Vector3.Right;
            vertices[12].color = Color.White;

            vertices[13].position = v2;
            vertices[13].texcoord = new Vector2(0, 1);
            vertices[13].normal = Vector3.Right;
            vertices[13].color = Color.White;

            vertices[14].position = v3;
            vertices[14].texcoord = new Vector2(1, 1);
            vertices[14].normal = Vector3.Right;
            vertices[14].color = Color.White;

            vertices[15].position = v7;
            vertices[15].texcoord = new Vector2(1, 0);
            vertices[15].normal = Vector3.Right;
            vertices[15].color = Color.White;
            #endregion

            #region            //5, 8, 7, 6
            vertices[16].position = v5;
            vertices[16].texcoord = new Vector2(0, 0);
            vertices[16].normal = Vector3.Down;
            vertices[16].color = Color.White;

            vertices[17].position = v8;
            vertices[17].texcoord = new Vector2(0, 1);
            vertices[17].normal = Vector3.Down;
            vertices[17].color = Color.White;

            vertices[18].position = v7;
            vertices[18].texcoord = new Vector2(1, 1);
            vertices[18].normal = Vector3.Down;
            vertices[18].color = Color.White;

            vertices[19].position = v6;
            vertices[19].texcoord = new Vector2(1, 0);
            vertices[19].normal = Vector3.Down;
            vertices[19].color = Color.White;
            #endregion

            #region            //1, 4, 3, 2
            vertices[20].position = v1;
            vertices[20].texcoord = new Vector2(0, 0);
            vertices[20].normal = Vector3.Up;
            vertices[20].color = Color.White;

            vertices[21].position = v4;
            vertices[21].texcoord = new Vector2(0, 1);
            vertices[21].normal = Vector3.Up;
            vertices[21].color = Color.White;

            vertices[22].position = v3;
            vertices[22].texcoord = new Vector2(1, 1);
            vertices[22].normal = Vector3.Up;
            vertices[22].color = Color.White;

            vertices[23].position = v2;
            vertices[23].texcoord = new Vector2(1, 0);
            vertices[23].normal = Vector3.Up;
            vertices[23].color = Color.White;
            #endregion
#endregion

            #region 索引数组
            //创建36个索引
            ushort[] indices = {
                                   0, 1, 3,
                                   1, 2, 3,

                                   4, 5, 7,
                                   5, 6, 7,

                                   8, 9, 11,
                                   9, 10, 11,

                                   12, 13, 15,
                                   13, 14, 15,

                                   16, 17, 19,
                                   17, 18, 19,

                                   20, 21, 23,
                                   21, 22, 23
                               };
#endregion

            VertexBuffer vb = new VertexBuffer(device, typeof(VertexPositionNormalTextureColor), vertices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices, 0, 24);
            VertexDeclaration vd = new VertexDeclaration(device, VertexPositionNormalTextureColor.VertexElements);

            IndexBuffer ib = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            ib.SetData(indices);

            return new Mesh(PrimitiveType.TriangleList, vb, vd, ib);
        }
    }
}

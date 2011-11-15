using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class Terrain:Entity
    {
        private string height_map_name;
        Texture2D height_map;
        private float min_height = float.MaxValue;
        private float max_height = float.MinValue;
        private float height_limit = 0;
        private float grid_size = 8;

        private int terrain_width;
        private int terrain_height;

        float[,] height_data;

        public Terrain(PipeEngine engine, string heightmap, string colormap)
            : this(engine, heightmap, colormap, null, Vector2.Zero, Vector3.One)
        {

        }

        public Terrain(PipeEngine engine, string heightmap, string colormap, string detailmap, Vector2 detailtiles, Vector3 scale)
            : base(engine)
        {
            this.height_map_name = heightmap;

            this.pose.SetScale(ref scale);
        }

        public float[,] HeightData { get { return height_data; } }
        public float MinHeight { get { return min_height; } }
        public float MaxHeight { get { return max_height; } }
        public float WidthInPixel { get { return terrain_width*grid_size; } }
        public float HeightInPixel { get { return terrain_height * grid_size; } }
        public float WidthInGrid { get { return terrain_width; } }
        public float HeightInGrid { get { return terrain_height; } }

        public override void Initialize()
        {
            base.Initialize();
            CreateTerrain();
        }

        internal override void LoadContent()
        {
            base.LoadContent();

            height_map = Engine.Content.Load<Texture2D>(height_map_name);
            height_data = Utility.LoadHeightData(height_map, ref terrain_width, ref terrain_height, ref min_height, ref max_height, height_limit);
        }

        public override int Draw(GameTime gametime, Camera camera)
        {
            return base.Draw(gametime, camera);
        }

        private void CreateTerrain()
        {
            if (!is_initialized)
                return;

            VertexDeclaration vd = new VertexDeclaration(Engine.GraphicsDevice, VertexPositionTexture.VertexElements);

            VertexPositionTexture[] vertices =  CreateTerrainVertices();
            int[] indices = CreateTerrainIndicesForTriangleStrip();

            GenerateNormalForTriangleStrip(ref vertices, ref indices);

            VertexBuffer vb = new VertexBuffer(Engine.GraphicsDevice, VertexPositionTexture.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);

            IndexBuffer ib = new IndexBuffer(Engine.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            ib.SetData(indices);

            Mesh mesh = new Mesh(PrimitiveType.TriangleStrip, vb, vd, ib);
            base.AddMesh(mesh);
    
            BasicMaterial default_material = new BasicMaterial(Engine);
            base.AddMaterial(default_material);

            RenderContext ctx = new RenderContext(Engine);
            ctx.Mesh = mesh;
            ctx.Material = default_material;
            base.AddRenderContext(ctx);

            default_material.DiffuseTextureName = "Textures\\grass";

        }

        private VertexPositionTexture[] CreateTerrainVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[terrain_width * terrain_height];

            int i = 0;
            for(int z=0; z<terrain_height; z++)
            {
                for(int x=0; x<terrain_width; x++)
                {
                    Vector3 position = new Vector3(x*grid_size, height_data[x, z], -z*grid_size);
                    Vector3 normal = new Vector3(0, 1, 0);
                    Vector2 texcoord = new Vector2((float)x / terrain_width, (float)z / terrain_height);

                    //vertices[i++] = new VertexPositionTexture(position, normal, texcoord);
                    vertices[i++] = new VertexPositionTexture(position, texcoord);
                }
            }

            return vertices;
        }

        private int[] CreateTerrainIndicesForTriangleStrip()
        {
            int[] indices = new int[terrain_width * 2 * (terrain_height - 1)];

            int i = 0;
            int z = 0;
            
            while( z < terrain_height - 1)
            {
                //奇数行从左往右建立索引
                for(int x=0; x<terrain_width; x++)
                {
                    indices[i++] = z * terrain_width + x;
                    indices[i++] = (z+1)*terrain_width + x;
                }
                z++;

                //偶数行从右往左
                if( z < terrain_height - 1)
                {
                    for(int x=terrain_width-1; x >= 0; x--)
                    {
                        indices[i++] = (z+1) * terrain_width + x;
                        indices[i++] = (z)*terrain_width + x;
                    }
                    z++;
                }
            }

            return indices;
        }

        private void GenerateNormalForTriangleStrip(ref VertexPositionTexture[] vertices, ref int[] indices)
        {
           /* for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            bool swap_winding = false;
            for(int i=2; i<indices.Length; i++)
            {
                Vector3 vec_1st = vertices[indices[i - 1]].Position - vertices[indices[i]].Position;
                Vector3 vec_2nd = vertices[indices[i - 2]].Position - vertices[indices[i]].Position;
                Vector3 normal = Vector3.Cross(vec_1st, vec_2nd);
                normal.Normalize();

                //逆序的时候翻转法线
                if (swap_winding)
                    normal *= -1;
                
                //当索引处于边界时，三角形处于退化状态（三条边重合），此时法线为NAN
                if(!float.IsNaN(normal.X ) && !float.IsNaN(normal.Y) && !float.IsNaN(normal.Z))
                {
                    vertices[indices[i]].Normal += normal;
                    vertices[indices[i - 1]].Normal += normal;
                    vertices[indices[i - 2]].Normal += normal;
                }
                swap_winding = !swap_winding;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
            */
        }
    }
}

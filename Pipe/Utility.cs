using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public class Utility
    {
        public static Texture2D LoadTexture(PipeEngine engine, string asset_or_filename)
        {
            Texture2D texture;
            string path = System.IO.Path.Combine(engine.Content.RootDirectory, asset_or_filename);
            path = path.Replace(".xnb", "");
            if(System.IO.File.Exists(path + ".xnb"))
            {
                texture = engine.Content.Load<Texture2D>(path);
            }
            else
            {
                path = System.IO.Path.Combine(Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation, "Content\\" + asset_or_filename);
                if (System.IO.File.Exists(path + ".xnb"))
                {
                    texture = engine.Content.Load<Texture2D>(path);
                }
                else
                {
                    if (System.IO.File.Exists(path))
                        texture = Texture2D.FromFile(engine.GraphicsDevice, path);
                    else
                        texture = Texture2D.FromFile(engine.GraphicsDevice, asset_or_filename);
                }
            }
            if (texture != null)
                texture.Name = asset_or_filename;
            return texture;
        }
        public static float [,] LoadHeightData(Texture2D height_map, ref int terrain_width, ref int terrain_height, ref float min_height, ref float max_height, float height_limit)
        {
            terrain_width = height_map.Width;
            terrain_height = height_map.Height;

            Color[] map_colors = new Color[terrain_height * terrain_width];
            height_map.GetData<Color>(map_colors);

            float[,] height_data = new float[terrain_width, terrain_height];
            for(int x=0; x<terrain_width; x++)
            {
                for(int y=0; y<terrain_height; y++)
                {
                    height_data[x, y] = map_colors[x+y*terrain_width].R;
                    if (height_data[x, y] > max_height)
                        max_height = height_data[x, y];
                    if(height_data[x, y] < min_height)
                        min_height = height_data[x, y];
                }
            }

            for (int x = 0; x < terrain_width; x++)
            {
                for(int y=0; y< terrain_height; y++)
                {
                    if (max_height == min_height)
                        height_data[x, y] = Math.Min(min_height, height_limit);
                    else
                        height_data[x, y] = (height_data[x, y] - min_height) / (max_height - min_height) * height_limit;
                }
            }

            return height_data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class WorldPose
    {
        public WorldPose()
        {
            rotation = Quaternion.Identity;
            position = Vector3.Zero;
            scale = Vector3.One;
            UpdateMatrix();
        }

        public Vector3 scale;
        public Vector3 position;
        public Quaternion rotation;
        public Matrix world_matrix;

        internal Matrix scale_matrix;
        internal Matrix rotation_matrix;
        internal Matrix translation_matrix;

        public void SetPosition(ref Vector3 pos)
        {
            position = pos;
            translation_matrix = Matrix.CreateTranslation(position);
            RebuildWorldMatrix();
        }

        public void SetScale(ref Vector3 sc)
        {
            scale = sc;
            scale_matrix = Matrix.CreateScale(scale);
            RebuildWorldMatrix();
        }

        public void SetRotation(ref Quaternion rot)
        {
            rotation = rot;
            rotation_matrix = Matrix.CreateFromQuaternion(rotation);
            RebuildWorldMatrix();
        }

        public void SetRotation(ref Matrix rot)
        {
            rotation = Quaternion.CreateFromRotationMatrix(rot);
            rotation_matrix = rot;
            RebuildWorldMatrix();
        }

        public void SetPosition(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
            translation_matrix = Matrix.CreateTranslation(position);
            RebuildWorldMatrix();
        }

        public void SetScale(float sx, float sy, float sz)
        {
            scale = new Vector3(sx, sy, sz);
            scale_matrix = Matrix.CreateScale(scale);
            RebuildWorldMatrix();
        }

        public void UpdateMatrix()
        {
            scale_matrix = Matrix.CreateScale(scale);
            rotation_matrix = Matrix.CreateFromQuaternion(rotation);
            translation_matrix = Matrix.CreateTranslation(position);
            RebuildWorldMatrix();
        }
        
        private void RebuildWorldMatrix()
        {
            Matrix.Multiply(ref scale_matrix, ref rotation_matrix, out world_matrix);
            world_matrix.Translation = position;
        }
    }
}

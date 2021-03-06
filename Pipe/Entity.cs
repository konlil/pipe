﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public abstract class Entity : Object3d
    {
        protected const int MAX_LIGHTS = 4;

        protected List<Mesh> meshes;
        protected List<IMaterial> materials;
        protected List<RenderContext> render_contexts;
        //protected List<Light> valid_lights;

        protected int light_count;

        public Entity(PipeEngine engine) : base(engine)
        {
            IsVisible = true;
        }

        internal virtual void LoadContent()
        {

        }

        public override  void Initialize()
        {
            if (Engine.GraphicsDevice.IsDisposed)
                return;

            light_count = 0;

            meshes = new List<Mesh>();
            materials = new List<IMaterial>();
            render_contexts = new List<RenderContext>();
            //valid_lights = new List<Light>();

            LoadContent();
            IsInitialized = true;
        }

        public virtual int AddMaterial(IMaterial mat)
        {
            materials.Add(mat);
            return materials.Count - 1;
        }

        public virtual void RemoveMaterialAt(int idx)
        {
            materials.RemoveAt(idx);
        }

        public virtual IMaterial GetMaterial(int idx)
        {
            if(materials[idx] != null)
            {
                return materials[idx];
            }

            return null;
        }

        public virtual int AddMesh(Mesh msh)
        {
            meshes.Add(msh);
            return meshes.Count - 1;
        }

        public virtual void ShowMesh(int idx, bool show)
        {
            if(meshes[idx] != null)
            {
                meshes[idx].Visible = show;
            }
        }

        public virtual Mesh GetMesh(int idx)
        {
            if(meshes[idx] != null)
            {
                return meshes[idx];
            }
            return null;
        }

        public virtual int AddRenderContext(RenderContext ctx)
        {
            render_contexts.Add(ctx);
            return render_contexts.Count - 1;
        }

        public virtual void ApplyEnvInfo(EnvInfo info)
        {
            foreach(RenderContext rc in render_contexts)
            {
                rc.Material.SetEnvInfo(info);
            }
        }

        public virtual void ClearLights()
        {
            light_count = 0;
        }

        public virtual bool ApplyLight(Light light)
        {
            if (light_count > MAX_LIGHTS)
                return false;

            light.Accept(light_count);

            light_count = light_count + 1;
            return true;
        }
        
        public virtual int Draw(GameTime gametime, Camera camera)
        {
            int total_primitives = 0;

            foreach(RenderContext rc in render_contexts)
            {
                IMaterial imaterial = rc.Material;
                Mesh mesh = rc.Mesh;
                
                mesh.PreRender(Engine.GraphicsDevice);
                
                if(imaterial != null)
                {
                    imaterial.SetWorldMatrix(pose.world_matrix);
                    imaterial.SetProjectionMatrix(camera.ProjectionMatrix);
                    imaterial.SetViewMatrix(camera.ViewMatrix);
    
                    imaterial.EffectInstance.Begin();
                    for(int i=0; i< imaterial.EffectInstance.CurrentTechnique.Passes.Count; i++)
                    {
                        EffectPass pass = imaterial.EffectInstance.CurrentTechnique.Passes[i];
                        pass.Begin();
                        mesh.Render(Engine.GraphicsDevice);
                        pass.End();
                        total_primitives += mesh.primitive_count;
                    }
                    imaterial.EffectInstance.End();
                }
            }

            return total_primitives;
        }
    }
}

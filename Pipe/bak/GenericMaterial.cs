using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipe
{
    public class GenericMaterial:BaseMaterial
    {
        protected Vector3 diffuse_color = Vector3.One;
        protected Vector3 emissive_color = Vector3.Zero;
        protected Vector3 specular_color = Vector3.Zero;

        protected float specular_power = 0.01f;

        protected string diffuse_texture_name = null;
        protected Texture2D diffuse_texture = null;
        protected Vector2 diffuse_uvtile = Vector2.One;

        private Texture2D detail_texture = null;
        private string detail_texture_name = null;
        private Vector2 detail_uvtile = Vector2.Zero;

        protected bool fog_enabled = false;

        public GenericMaterial(PipeEngine engine, Effect eff)
            : base(engine, eff)
        {
            effect.Parameters["gDiffuseEnabled"].SetValue(true);
            effect.Parameters["gDiffuseUVTile"].SetValue(diffuse_uvtile);
            effect.Parameters["gDetailUVTile"].SetValue(detail_uvtile);

            effect.Parameters["gMaterialDiffuse"].SetValue(diffuse_color);
            effect.Parameters["gMaterialEmissive"].SetValue(emissive_color);
            effect.Parameters["gMaterialSpecular"].SetValue(specular_color);
            effect.Parameters["gMaterialSpecPower"].SetValue(specular_power);

            effect.Parameters["gFogEnabled"].SetValue(fog_enabled);
        }

#region 属性
        public Vector3 DiffuseColor
        {
            get { return diffuse_color; }
            set
            {
                diffuse_color = value;
                effect.Parameters["gMaterialDiffuse"].SetValue(value);
            }
        }

        public Vector3 EmissiveColor
        {
            get { return emissive_color; }
            set
            {
                emissive_color = value;
                effect.Parameters["gMaterialEmissive"].SetValue(value);
            }
        }

        public Vector3 SpecularColor
        {
            get { return specular_color; }
            set
            {
                specular_color = value;
                effect.Parameters["gMaterialSpecular"].SetValue(value);
            }
        }

        public float SpecularPower
        {
            get { return specular_power; }
            set
            {
                specular_power = value;
                effect.Parameters["gMaterialSpecPower"].SetValue(value);
            }
        }

        public string DiffuseTextureName
        {
            get { return diffuse_texture_name; }
            set
            {
                diffuse_texture_name = value;
                if(diffuse_texture_name == null)
                {
                    diffuse_texture = null;
                    effect.Parameters["gDiffuseTexture"].SetValue((Texture)null);
                    effect.Parameters["gDiffuseEnabled"].SetValue(false);
                }
                else
                {
                    diffuse_texture = Utility.LoadTexture(engine, diffuse_texture_name);
                    effect.Parameters["gDiffuseTexture"].SetValue(diffuse_texture);
                    effect.Parameters["gDiffuseEnabled"].SetValue(true);
                }
            }
        }

        public Vector2 DiffuseUVTitle
        {
            get { return diffuse_uvtile; }
            set
            {
                diffuse_uvtile = value;
                effect.Parameters["gDiffuseUVTile"].SetValue(diffuse_uvtile);
            }
        }

        public string DetailTextureName
        {
            get { return detail_texture_name; }
            set
            {
                detail_texture_name = value;
                if(detail_texture_name == null)
                {
                    detail_texture = null;
                    effect.Parameters["gDetailTexture"].SetValue((Texture)null);
                }
                else
                {
                    detail_texture = Utility.LoadTexture(engine, detail_texture_name);
                    effect.Parameters["gDetailTexture"].SetValue(detail_texture);
                }
            }
        }

        public Vector2 DetailUVTile
        {
            get { return detail_uvtile; }
            set
            {
                detail_uvtile = value;
                effect.Parameters["gDetailUVTile"].SetValue(value);
            }
        }

        public bool FogEnabled
        {
            get { return fog_enabled; }
            set
            {
                fog_enabled = value;
                effect.Parameters["gFogEnabled"].SetValue(engine.Scm.ActiveScene.FogEnabled && fog_enabled);
            }
        }
#endregion

    }
}

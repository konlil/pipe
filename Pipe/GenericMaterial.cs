using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pipe
{
    public class GenericMaterial : BaseMaterial
    {
        #region 成员变量
        /// <summary>
        /// 漫反射颜色。如果材质的漫反射纹理不存在则为顶点颜色，默认为白色。
        /// </summary>
        protected Vector3 diffuseColor = Vector3.One;

        /// <summary>
        /// 物体的自发光颜色。如果设置，就算物体没有被照亮也可见，拥有自发光颜色，默认为黑色。
        /// </summary>
        protected Vector3 emissiveColor = Vector3.Zero;

        /// <summary>
        /// 物体的镜面高光颜色，默认为黑色。
        /// </summary>
        protected Vector3 specularColor = Vector3.Zero;

        /// <summary>
        /// 镜面高光强度，默认为0.01。
        /// </summary>
        protected float specularPower = 0.01f;

        /// <summary>
        /// 漫反射纹理
        /// </summary>
        private Texture2D diffuseTexture = null;

        /// <summary>
        /// 漫反射纹理文件名称，需要包含路径
        /// </summary>
        protected string diffuseTextureName = null;

        /// <summary>
        /// 漫反射纹理UV方向平铺次数，默认平铺一次
        /// </summary>
        protected Vector2 diffuseUVTile = Vector2.One;

        /// <summary>
        /// 细节纹理
        /// </summary>
        private Texture2D detailTexture = null;

        /// <summary>
        /// 细节纹理文件名称，需要包含路径
        /// </summary>
        protected string detailTextureName = null;

        /// <summary>
        /// 细节纹理的平铺次数，默认不平铺   
        /// </summary>      
        protected Vector2 detailUVTile = Vector2.Zero;

        /// <summary>
        /// 是否进行雾化，默认为false。
        /// </summary>
        protected bool fogEnabled = false;

        #endregion

        private void InitEffect()
        {
            effect.Parameters["DiffuseTextureEnabled"].SetValue(false);

            effect.Parameters["MaterialDiffuse"].SetValue(diffuseColor);
            effect.Parameters["MaterialSpecular"].SetValue(specularColor);
            effect.Parameters["MaterialSpecPower"].SetValue(specularPower);
            effect.Parameters["MaterialEmissive"].SetValue(emissiveColor);
        }

        public GenericMaterial(PipeEngine engine, string effect_name)
            :base(engine, effect_name)
        {
            InitEffect();
        }

        public GenericMaterial(PipeEngine engine, Effect eff)
            :base(engine, eff)
        {
            InitEffect();
        }

        #region 属性

        /// <summary>
        /// 获取或设置漫反射颜色。如果材质的漫反射纹理不存在则为顶点颜色，默认为白色。
        /// </summary>
        public Vector3 DiffuseColor
        {
            get { return diffuseColor; }
            set
            {
                diffuseColor = value;
                effect.Parameters["MaterialDiffuse"].SetValue(diffuseColor);
            }
        }

        /// <summary>
        /// 获取或设置物体的自发光颜色。如果设置，就算物体没有被照亮也可见，拥有自发光颜色，默认为黑色。
        /// </summary>
        public Vector3 EmissiveColor
        {
            get { return emissiveColor; }
            set
            {
                emissiveColor = value;
                effect.Parameters["MaterialEmissive"].SetValue(emissiveColor);      
            }
        }

        /// <summary>
        /// 获取或设置物体的镜面高光颜色，默认为黑色。
        /// </summary>
        public Vector3 SpecularColor
        {
            get { return specularColor; }
            set
            {
                specularColor = value;
                effect.Parameters["MaterialSpecular"].SetValue(specularColor);
                
            }
        }

        /// <summary>
        /// 获取或设置镜面高光强度，默认为0.01。
        /// </summary>
        public float SpecularPower
        {
            get { return specularPower; }
            set
            {
                specularPower = value;
                effect.Parameters["MaterialSpecPower"].SetValue(specularPower);
            }
        }      


        /// <summary>
        /// 获取或设置漫反射纹理文件名称，需要包含路径
        /// </summary>
        public string DiffuseTextureName
        {
            get { return diffuseTextureName; }
            set 
            { 
                diffuseTextureName = value;
                if (string.IsNullOrEmpty(diffuseTextureName))
                {
                    diffuseTexture = null;
                    effect.Parameters["DiffuseTexture"].SetValue((Texture)null);
                    effect.Parameters["DiffuseTextureEnabled"].SetValue(false );
                }
                else
                {
                    diffuseTexture = engine.Content.Load<Texture2D>(diffuseTextureName);
                    effect.Parameters["DiffuseTexture"].SetValue(diffuseTexture);
                    effect.Parameters["DiffuseTextureEnabled"].SetValue(true);
                }
            }
        }

        /// <summary>
        /// 获取或设置漫反射纹理UV方向平铺次数
        /// </summary>
        public Vector2 DiffuseUVTile
        {
            get { return diffuseUVTile; }
            set 
            { 
                diffuseUVTile = value;                
                effect.Parameters["DiffuseUVTile"].SetValue(diffuseUVTile);
                
            }
        }

        /// <summary>
        /// 获取或设置细节纹理文件名称，需要包含路径
        /// </summary>
        public string DetailTextureName
        {
            get { return detailTextureName; }
            set 
            {
                detailTextureName = value;
                if (detailTextureName == null)
                {
                    detailTexture = null;
                    effect.Parameters["DetailTexture"].SetValue((Texture)null);
                }
                else
                {
                    detailTexture = engine.Content.Load<Texture2D>(detailTextureName); 
                    effect.Parameters["DetailTexture"].SetValue(detailTexture);
                }
            }
        }

        /// <summary>
        /// 获取或设置细节纹理的平铺次数
        /// </summary>
        public Vector2 DetailUVTile
        {
            get { return detailUVTile; }
            set 
            { 
                detailUVTile = value;
                effect.Parameters["DetailUVTile"].SetValue(detailUVTile);
            }
        }

        /// <summary>
        /// 获取或设置是否进行雾化，默认为false。
        /// </summary>
        public bool FogEnabled
        {
            get { return fogEnabled; }
            set
            {
                fogEnabled = value;
                //只有在场景的FogEnabled开启并且节点本身的fogEnabled开启的情况下才进行雾化计算
                effect.Parameters["FogEnabled"].SetValue(fogEnabled);
            }
        }

        #endregion
    }
}

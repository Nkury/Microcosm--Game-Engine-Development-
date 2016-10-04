using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    public class Material
    {
        public Effect effect;
        public Texture2D DiffuseTexture;
        public float Shininess;

        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }
        public Camera Camera { get; set; }
        public Matrix World { get; set; }
        public Light Light { get; set; }

        public Material(Matrix world, Camera camera, Light light, ContentManager content, String filename, int currentTechnique, float shininess, Texture2D texture)
        {
            effect = content.Load<Effect>(filename);
            World = world;
            Camera = camera;
            Light = light;
            Shininess = shininess;
            CurrentTechnique = currentTechnique;
            DiffuseTexture = texture;
        }

        public virtual void Apply(int currentPass)
        {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.LocalPosition);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.LocalPosition);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["AmbientColor"].SetValue(Light.Ambient.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(Light.Specular.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(Light.Diffuse.ToVector3());
            effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);

            effect.CurrentTechnique.Passes[currentPass].Apply();
        }

    }
}

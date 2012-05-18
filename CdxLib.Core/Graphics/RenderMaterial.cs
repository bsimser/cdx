using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Graphics
{
    public class RenderMaterial
    {
        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="name"></param>
        /// <param name="scale"></param>
        public RenderMaterial(Texture2D texture, string name, float scale)
        {
            Color = Color.White;
            Depth = 0f;
            Texture = texture;
            Scale = scale;
            CenterOnBody = true;
            TextureName = name;

            Materials.AddMaterial(name.ToLower(), texture);
        }

        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="name"></param>
        public RenderMaterial(Texture2D texture, string name)
            : this(texture, name, 1f)
        {
        }

        /// <summary>
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// </summary>
        public bool CenterOnBody { get; set; }

        /// <summary>
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// </summary>
        public object UserData { get; set; }
    }
}
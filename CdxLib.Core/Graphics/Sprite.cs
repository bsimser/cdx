using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Graphics
{
    /// <summary>
    /// A Sprite represents a game entity on the screen and contains
    /// both the location where to draw it on the screen and what it looks like.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// </summary>
        public Vector2 Origin = Vector2.Zero;

        /// <summary>
        /// </summary>
        public Texture2D Texture;

        public int X;

        public int Y;

        public int Width;

        public int Height;

        public int Layer;

        public float Angle;

        /// <summary>
        /// </summary>
        /// <param name = "sprite"></param>
        public Sprite(Texture2D sprite)
        {
            X = 0;
            Y = 0;
            Layer = 1;
            Angle = 0;

            if (sprite != null)
            {
                Texture = sprite;
                Origin = new Vector2(sprite.Width/2f, sprite.Height/2f);
                Width = sprite.Width;
                Height = sprite.Height;
            }
        }
    }
}
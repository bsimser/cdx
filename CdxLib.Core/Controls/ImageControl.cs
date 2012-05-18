using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Controls
{
    /// <summary>
    ///   ImageControl is a control that displays a single sprite. By default it displays an entire texture.
    ///   If a null texture is given, this control will use DrawContext.BlankTexture. This allows it to be
    ///   used to draw solid-colored rectangles.
    /// </summary>
    public class ImageControl : Control
    {
        // Color to modulate the texture with. The default is white, which displays the original unmodified texture.
        public Color Color;
        public Vector2 Origin;
        public Vector2? SourceSize;
        private Texture2D _texture;

        /// <summary>
        /// </summary>
        public ImageControl() : this(null, Vector2.Zero)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name = "texture"></param>
        /// <param name = "position"></param>
        public ImageControl(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            Color = Color.White;
        }

        /// <summary>
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                if (_texture == value) return;
                _texture = value;
                InvalidateAutoSize();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "context"></param>
        public override void Draw(DrawContext context)
        {
            base.Draw(context);
            var drawTexture = _texture ?? context.BlankTexture;

            var actualSourceSize = SourceSize ?? Size;
            var sourceRectangle = new Rectangle
                                      {
                                          X = (int) Origin.X,
                                          Y = (int) Origin.Y,
                                          Width = (int) actualSourceSize.X,
                                          Height = (int) actualSourceSize.Y,
                                      };
            var destRectangle = new Rectangle
                                    {
                                        X = (int) context.DrawOffset.X,
                                        Y = (int) context.DrawOffset.Y,
                                        Width = (int) Size.X,
                                        Height = (int) Size.Y
                                    };
            context.SpriteBatch.Draw(drawTexture, destRectangle, sourceRectangle, Color);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override Vector2 ComputeSize()
        {
            return _texture != null ? new Vector2(_texture.Width, _texture.Height) : Vector2.Zero;
        }
    }
}
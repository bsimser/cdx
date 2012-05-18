using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Controls
{
    /// <summary>
    ///   TextControl is a control that displays a single string of text. By default, the
    ///   size is computed from the given text and spritefont.
    /// </summary>
    public class TextControl : Control
    {
        public Color Color;
        private SpriteFont _font;
        private string _text;

        /// <summary>
        /// </summary>
        public TextControl()
            : this(string.Empty, null, Color.White, Vector2.Zero)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name = "text"></param>
        /// <param name = "font"></param>
        public TextControl(string text, SpriteFont font)
            : this(text, font, Color.White, Vector2.Zero)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name = "text"></param>
        /// <param name = "font"></param>
        /// <param name = "color"></param>
        public TextControl(string text, SpriteFont font, Color color)
            : this(text, font, color, Vector2.Zero)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name = "text"></param>
        /// <param name = "font"></param>
        /// <param name = "color"></param>
        /// <param name = "position"></param>
        public TextControl(string text, SpriteFont font, Color color, Vector2 position)
        {
            _text = text;
            _font = font;
            Position = position;
            Color = color;
        }

        /// <summary>
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                InvalidateAutoSize();
            }
        }

        /// <summary>
        ///   Font to use
        /// </summary>
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                if (_font == value) return;
                _font = value;
                InvalidateAutoSize();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "context"></param>
        public override void Draw(DrawContext context)
        {
            base.Draw(context);
            context.SpriteBatch.DrawString(_font, Text, context.DrawOffset, Color);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override Vector2 ComputeSize()
        {
            return _font.MeasureString(Text);
        }
    }
}
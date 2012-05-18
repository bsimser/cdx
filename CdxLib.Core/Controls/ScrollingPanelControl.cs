using CdxLib.Core.ScreenManager;
using Microsoft.Xna.Framework;

namespace CdxLib.Core.Controls
{
    /// <summary>
    /// </summary>
    public class ScrollingPanelControl : PanelControl
    {
        private readonly ScrollTracker _scrollTracker = new ScrollTracker();

        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            var size = ComputeSize();
            _scrollTracker.CanvasRect.Width = (int) size.X;
            _scrollTracker.CanvasRect.Height = (int) size.Y;
            _scrollTracker.Update(gametime);

            base.Update(gametime);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            _scrollTracker.HandleInput(input);
            base.HandleInput(input);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public override void Draw(DrawContext context)
        {
            // To render the scrolled panel, we just adjust our offset before rendering our child controls as
            // a normal PanelControl
            context.DrawOffset.Y = -_scrollTracker.ViewRect.Y;
            base.Draw(context);
        }
    }
}
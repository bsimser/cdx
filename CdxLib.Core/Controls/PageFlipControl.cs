using CdxLib.Core.ScreenManager;
using Microsoft.Xna.Framework;

namespace CdxLib.Core.Controls
{
    /// <summary>
    ///   This control aligns its child controls horizontally, and allows the user to flick
    ///   through them.
    /// </summary>
    public class PageFlipControl : PanelControl
    {
        // PageFlipTracker handles the logic of scrolling / tracking etc.
        private readonly PageFlipTracker _tracker = new PageFlipTracker();

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        protected override void OnChildAdded(int index, Control child)
        {
            _tracker.PageWidthList.Insert(index, (int) child.Size.X);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        protected override void OnChildRemoved(int index, Control child)
        {
            _tracker.PageWidthList.RemoveAt(index);
        }

        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            _tracker.Update();
            base.Update(gametime);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            _tracker.HandleInput(input);
            if (ChildCount <= 0) return;
            // Only the child that currently has focus gets input
            var current = _tracker.CurrentPage;
            this[current].HandleInput(input);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public override void Draw(DrawContext context)
        {
            var childCount = ChildCount;
            if (childCount < 2)
            {
                // Default rendering behavior if we don't have enough
                // children to flip through.
                base.Draw(context);
                return;
            }
            var origin = context.DrawOffset;
            var iCurrent = _tracker.CurrentPage;

            var horizontalOffset = _tracker.CurrentPageOffset;
            context.DrawOffset = origin + new Vector2 {X = horizontalOffset};
            this[iCurrent].Draw(context);

            if (horizontalOffset > 0)
            {
                // The screen has been dragged to the right, so the edge of another
                // page is visible to the left.
                var iLeft = (iCurrent + childCount - 1)%childCount;
                context.DrawOffset.X = origin.X + horizontalOffset - _tracker.EffectivePageWidth(iLeft);
                this[iLeft].Draw(context);
            }

            if (horizontalOffset + this[iCurrent].Size.X >= context.Device.Viewport.Width) return;

            // The edge of another page is visible to the right.
            // If we have two pages, it's possible that a page will be
            // drawn twice, with parts of it visible on each edge of the screen.
            var iRight = (iCurrent + 1)%childCount;
            context.DrawOffset.X = origin.X + horizontalOffset + _tracker.EffectivePageWidth(iCurrent);
            this[iRight].Draw(context);
        }
    }
}
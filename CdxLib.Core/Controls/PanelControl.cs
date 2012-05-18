using Microsoft.Xna.Framework;

namespace CdxLib.Core.Controls
{
    /// <summary>
    /// </summary>
    public class PanelControl : Control
    {
        /// <summary>
        ///   Position child components in a column, with the given spacing between components
        /// </summary>
        /// <param name = "xMargin"></param>
        /// <param name = "yMargin"></param>
        /// <param name = "ySpacing"></param>
        public void LayoutColumn(float xMargin, float yMargin, float ySpacing)
        {
            var y = yMargin;

            for (var i = 0; i < ChildCount; i++)
            {
                var child = this[i];
                child.Position = new Vector2 {X = xMargin, Y = y};
                y += child.Size.Y + ySpacing;
            }

            InvalidateAutoSize();
        }

        /// <summary>
        ///   Position child components in a row, with the given spacing between components
        /// </summary>
        /// <param name = "xMargin"></param>
        /// <param name = "yMargin"></param>
        /// <param name = "xSpacing"></param>
        public void LayoutRow(float xMargin, float yMargin, float xSpacing)
        {
            var x = xMargin;

            for (var i = 0; i < ChildCount; i++)
            {
                var child = this[i];
                child.Position = new Vector2 {X = x, Y = yMargin};
                x += child.Size.X + xSpacing;
            }

            InvalidateAutoSize();
        }
    }
}
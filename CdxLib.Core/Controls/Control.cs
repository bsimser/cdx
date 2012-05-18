using System;
using System.Collections.Generic;
using CdxLib.Core.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Controls
{
    ///<summary>
    ///  Control is the base class for a simple UI controls framework.
    ///
    ///  Controls are grouped into a heirarchy: each control has a Parent and
    ///  an optional list of Children. They draw themselves
    ///
    ///  Input handling
    ///  ==============
    ///  Call HandleInput() once per update on the root control of the heirarchy; it will then call HandleInput() on
    ///  its children. Controls can override HandleInput() to check for touch events and the like. If you override this
    ///  function, you need to call base.HandleInput() to let your child controls see input.
    ///
    ///  Rendering
    ///  =========
    ///  Override Control.Draw() to render your control. Control.Draw() takes a 'DrawContext' struct, which contains
    ///  the GraphicsDevice and other objects useful for drawing. It also contains a SpriteBatch, which will have had
    ///  Begin() called *before* Control.Draw() is called. This allows batching of sprites across multiple controls
    ///  to improve rendering speed.
    ///
    ///  Layout
    ///  ======
    ///  Controls have a Position and Size, which defines a rectangle. By default, Size is computed
    ///  auotmatically by an internal call to ComputeSize(), which each child control can implement
    ///  as appropriate. For example, TextControl uses the size of the rendered text.
    ///
    ///  If you *write* to the Size property, auto-sizing will be disabled, and the control will
    ///  retain the written size unless you write to it again.
    ///
    ///  There is no dynamic layout system. Instead, container controls (PanelControl in particular)
    ///  contain methods for positioning their child controls into rows, columns, or other arrangements.
    ///  Client code should create the controls needed for a screen, then call one or more of these
    ///  layout functions to position them.
    ///</summary>
    ///<remarks>
    ///  Controls do not have any special system for setting TouchPanel.EnabledGestures; if you're using
    ///  gesture-sensitive controls, you need to set EnabledGestures as appopriate in each GameScreen.
    ///</remarks>
    public class Control
    {
        /// <summary>
        ///   Draw() is not called unless Control.Visible is true (the default).
        /// </summary>
        public bool Visible = true;

        private bool _autoSize = true;
        private List<Control> _children;
        private Vector2 _position;
        private Vector2 _size;
        private bool _sizeValid;

        /// <summary>
        ///   Position of this control within its parent control.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (Parent != null)
                {
                    Parent.InvalidateAutoSize();
                }
            }
        }

        /// <summary>
        ///   Size if this control. See above for a discussion of the layout system.
        /// </summary>
        public Vector2 Size
        {
            // Default behavior is for ComputeSize() to determine the size, and then cache it.
            get
            {
                if (!_sizeValid)
                {
                    _size = ComputeSize();
                    _sizeValid = true;
                }
                return _size;
            }

            // Setting the size overrides whatever ComputeSize() would return, and disables autoSize
            set
            {
                _size = value;
                _sizeValid = true;
                _autoSize = false;
                if (Parent != null)
                {
                    Parent.InvalidateAutoSize();
                }
            }
        }

        /// <summary>
        ///   The control containing this control, if any
        /// </summary>
        public Control Parent { get; private set; }

        /// <summary>
        ///   Number of child controls of this control
        /// </summary>
        public int ChildCount
        {
            get { return _children == null ? 0 : _children.Count; }
        }

        /// <summary>
        ///   Indexed access to the children of this control.
        /// </summary>
        public Control this[int childIndex]
        {
            get { return _children[childIndex]; }
        }

        /// <summary>
        ///   Call this method when a control's content changes so that its size needs to be recomputed. This has no
        ///   effect if autoSize has been disabled.
        /// </summary>
        protected void InvalidateAutoSize()
        {
            if (!_autoSize) return;
            _sizeValid = false;
            if (Parent != null)
            {
                Parent.InvalidateAutoSize();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "child"></param>
        public void AddChild(Control child)
        {
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }
            AddChild(child, ChildCount);
        }

        /// <summary>
        /// </summary>
        /// <param name = "child"></param>
        /// <param name = "index"></param>
        public void AddChild(Control child, int index)
        {
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }
            child.Parent = this;
            if (_children == null)
            {
                _children = new List<Control>();
            }
            _children.Insert(index, child);
            OnChildAdded(index, child);
        }

        /// <summary>
        /// </summary>
        /// <param name = "index"></param>
        public void RemoveChildAt(int index)
        {
            var child = _children[index];
            child.Parent = null;
            _children.RemoveAt(index);
            OnChildRemoved(index, child);
        }

        /// <summary>
        ///   Remove the given control from this control's list of children.
        /// </summary>
        public void RemoveChild(Control child)
        {
            if (child.Parent != this)
                throw new InvalidOperationException();

            RemoveChildAt(_children.IndexOf(child));
        }

        /// <summary>
        /// </summary>
        /// <param name = "context"></param>
        public virtual void Draw(DrawContext context)
        {
            var origin = context.DrawOffset;
            for (var i = 0; i < ChildCount; i++)
            {
                var child = _children[i];
                if (!child.Visible) continue;
                context.DrawOffset = origin + child.Position;
                child.Draw(context);
            }
        }

        /// <summary>
        ///   Called once per frame to update the control; override this method if your control requires custom updates.
        ///   Call base.Update() to update any child controls.
        /// </summary>
        public virtual void Update(GameTime gametime)
        {
            for (var i = 0; i < ChildCount; i++)
            {
                _children[i].Update(gametime);
            }
        }

        /// <summary>
        ///   Called once per frame to update the control; override this method if your control requires custom updates.
        ///   Call base.Update() to update any child controls.
        /// </summary>
        public virtual void HandleInput(InputState input)
        {
            for (var i = 0; i < ChildCount; i++)
            {
                _children[i].HandleInput(input);
            }
        }

        /// <summary>
        ///   Called when the Size property is read and sizeValid is false. Call base.ComputeSize() to compute the
        ///   size (actually the lower-right corner) of all child controls.
        /// </summary>
        public virtual Vector2 ComputeSize()
        {
            if (_children == null || _children.Count == 0)
            {
                return Vector2.Zero;
            }
            var bounds = _children[0].Position + _children[0].Size;
            for (var i = 1; i < _children.Count; i++)
            {
                var corner = _children[i].Position + _children[i].Size;
                bounds.X = Math.Max(bounds.X, corner.X);
                bounds.Y = Math.Max(bounds.Y, corner.Y);
            }
            return bounds;
        }

        /// <summary>
        ///   Called after a child control is added to this control. The default behavior is to call InvalidateAutoSize().
        /// </summary>
        protected virtual void OnChildAdded(int index, Control child)
        {
            InvalidateAutoSize();
        }

        /// <summary>
        ///   Called after a child control is removed from this control. The default behavior is to call InvalidateAutoSize().
        /// </summary>
        protected virtual void OnChildRemoved(int index, Control child)
        {
            InvalidateAutoSize();
        }

        // Call this method once per frame on the root of your control heirarchy to draw all the controls.
        // See ControlScreen for an example.
        public static void BatchDraw(Control control, GraphicsDevice device, SpriteBatch spriteBatch, Vector2 offset,
                                     GameTime gameTime)
        {
            if (control == null || !control.Visible) return;
            spriteBatch.Begin();
            control.Draw(new DrawContext
                             {
                                 Device = device,
                                 SpriteBatch = spriteBatch,
                                 DrawOffset = offset + control.Position,
                                 GameTime = gameTime
                             });
            spriteBatch.End();
        }
    }
}
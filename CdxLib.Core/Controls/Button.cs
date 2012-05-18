using System;
using System.Collections.Generic;
using CdxLib.Core.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CdxLib.Core.Controls
{
    /// <summary>
    ///   Button Control for XNA. Combine it with PanelControl or ScrollingPanelControl for menu like feel.
    /// </summary>
    public class Button : Control
    {
        #region Delegates

        public delegate void ClickHandler(Button sender);

        #endregion

        #region ButtonState enum

        public enum ButtonState
        {
            Normal,
            Clicked
        }

        #endregion

        #region FontSize enum

        public enum FontSize
        {
            Small,
            Medium,
            Big
        }

        #endregion

        #region TextAlign enum

        public enum TextAlign
        {
            Left,
            Right,
            Centre
        }

        #endregion

        private const int TimeToShowClickedTexture = 200; //Miliseconds

        private readonly Game _game;
        public Vector2 TapPosition = Vector2.Zero;
        private Rectangle _clickArea;
        private bool _clickAreaSpecific;
        private Color _clickedForeground = Color.Black;
        private SpriteFont _font;
        private Color _foreground = Color.Black;
        private int _height = 50;
        private Vector2 _lastDrawOffset = Vector2.Zero;
        private TimeSpan _lastTap = new TimeSpan(0);
        private SpriteBatch _sBatch;
        private ButtonState _state;
        private TextAlign _textAlignment = TextAlign.Centre;
        private FontSize _textSize = FontSize.Medium;
        private bool _textVisible = true;
        private Texture2D _txClickedButton;
        private Texture2D _txNormalButton;
        private int _width = 50;

        /// <summary>
        /// </summary>
        /// <param name = "game"></param>
        public Button(Game game)
        {
            _game = game;
        }

        /// <summary>
        ///   Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        /// <remarks>
        /// </remarks>
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;
                _font.LineSpacing = 30;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [text visible].
        /// </summary>
        /// <value><c>true</c> if [text visible]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// </remarks>
        public bool TextVisible
        {
            get { return _textVisible; }
            set { _textVisible = value; }
        }

        /// <summary>
        ///   Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        /// <remarks>
        /// </remarks>
        public string DisplayText { get; set; }

        /// <summary>
        ///   Gets or sets the text rotation.
        /// </summary>
        /// <value>The text rotation.</value>
        /// <remarks>
        /// </remarks>
        public float TextRotation { get; set; }

        /// <summary>
        ///   Gets or sets the size of the text.
        /// </summary>
        /// <value>The size of the text.</value>
        /// <remarks>
        /// </remarks>
        public FontSize TextSize
        {
            get { return _textSize; }
            set { _textSize = value; }
        }

        /// <summary>
        ///   Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        /// <remarks>
        /// </remarks>
        public TextAlign TextAlignment
        {
            get { return _textAlignment; }
            set { _textAlignment = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [click area specific].
        /// </summary>
        /// <value><c>true</c> if [click area specific]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// </remarks>
        public bool ClickAreaSpecific
        {
            get { return _clickAreaSpecific; }
            set { _clickAreaSpecific = value; }
        }

        /// <summary>
        ///   The area where this button can be clicked, if this is not specified it uses the Normal Textures Bounds
        /// </summary>
        public Rectangle ClickArea
        {
            get
            {
                if (!_clickAreaSpecific)
                {
                    if (_txNormalButton != null && _clickArea == Rectangle.Empty)
                    {
                        _clickArea = new Rectangle((int) Position.X, (int) Position.Y, Width, Height);
                    }
                }
                return _clickArea;
            }
            set
            {
                _clickArea = value;
                _clickAreaSpecific = true;
            }
        }

        /// <summary>
        ///   Gets or sets the normal button state texture.
        /// </summary>
        /// <value>The normal button state texture.</value>
        /// <remarks>
        /// </remarks>
        public Texture2D NormalButtonTexture
        {
            get { return _txNormalButton; }
            set { _txNormalButton = value; }
        }

        /// <summary>
        ///   Gets or sets the clicked button state texture.
        /// </summary>
        /// <value>The clicked button state texture.</value>
        /// <remarks>
        /// </remarks>
        public Texture2D ClickedButtonTexture
        {
            get { return _txClickedButton; }
            set { _txClickedButton = value; }
        }

        /// <summary>
        ///   Gets or sets the ButtonState.
        /// </summary>
        /// <value>The ButtonState.</value>
        public ButtonState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// </summary>
        public int Height
        {
            get
            {
                if (_height == 0)
                    _height = TextHeight + 10;
                return _height;
            }
            set { _height = value; }
        }

        /// <summary>
        /// </summary>
        public int Width
        {
            get
            {
                if (_width == 0)
                    _width = TextWidth + 10;
                return _width;
            }
            set { _width = value; }
        }

        /// <summary>
        ///   Gets or sets the clicked forground Color.
        /// </summary>
        /// <value>The clicked forground Color.</value>
        /// <remarks>
        /// </remarks>
        public Color ClickedForeground
        {
            get { return _clickedForeground; }
            set { _clickedForeground = value; }
        }

        /// <summary>
        ///   Gets or sets the forground Color.
        /// </summary>
        /// <value>The forground Color.</value>
        /// <remarks>
        /// </remarks>
        public Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }

        /// <summary>
        ///   Gets the width of the text.
        /// </summary>
        public int TextWidth
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayText))
                    return 0;
                return (int) (Font.MeasureString(DisplayText).X*ScaleText());
            }
        }

        /// <summary>
        ///   Gets the height of the text.
        /// </summary>
        public int TextHeight
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayText))
                    return 0;
                return (int) (Font.MeasureString(DisplayText).Y*ScaleText());
            }
        }

        /// <summary>
        ///   Gets or sets the tag.
        ///   Used to store extra information we need to retrieve when button gets clicked.
        /// </summary>
        /// <value>The tag.</value>
        /// <remarks>
        /// </remarks>
        public object Tag { get; set; }

        /// <summary>
        /// </summary>
        public void Initialize()
        {
            _state = ButtonState.Normal;
        }

        /// <summary>
        ///   Loads the content.
        /// </summary>
        public void LoadContent()
        {
            //Check whether we have all the required textures and fonts available.
            if (NormalButtonTexture == null)
            {
                throw new Exception("No Texture provided for Button.");
            }
            if (ClickedButtonTexture == null)
                ClickedButtonTexture = NormalButtonTexture;

            if (!string.IsNullOrEmpty(DisplayText))
                if (Font == null)
                {
                    throw new Exception("No Font provided for Button.");
                }

            _sBatch = new SpriteBatch(_game.GraphicsDevice);
        }

        /// <summary>
        /// </summary>
        /// <param name = "input"></param>
        public override void HandleInput(InputState input)
        {
            //We need to handle the Tap gesture, to identify whether its clicked inside and should we raise event or not.
            HandleInput(input.Gestures);

            //Handle inputs
            base.HandleInput(input);
        }

        /// <summary>
        /// </summary>
        /// <param name = "gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //TapPosition gets set from HandleInput if tapped in current button's click area.
            //If We have TapPosition then record the time, this time will be used to render the button's clicked texture.
            if (TapPosition != Vector2.Zero)
            {
                _lastTap = gameTime.TotalGameTime;
                TapPosition = Vector2.Zero;
            }

            //If we are in Clicked state and we have shown clicked texture for specified period of time then get back to normal state.
            //Also raise the event now.
            //If we don't do this then button won't look like button as, when user tap on button it raise the event and 
            //clicked texture won't be shown for enough amount of time to be visible.
            if (_state == ButtonState.Clicked &&
                gameTime.TotalGameTime.Subtract(_lastTap).Milliseconds > TimeToShowClickedTexture)
            {
                _state = ButtonState.Normal;
                if (OnClicked != null)
                    OnClicked(this);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// </summary>
        /// <param name = "context"></param>
        public override void Draw(DrawContext context)
        {
            if (Visible)
            {
                SpriteBatch spriteBatch;

                if (context.SpriteBatch == null)
                {
                    spriteBatch = _sBatch;
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    Draw(context);
                    spriteBatch.End();
                }
                else
                {
                    //Get Texture to render based on the ButtonState.
                    var toPaint = StateToTexture();
                    //This gets updated if button is in ScrollablePanel
                    var pos = context.DrawOffset;
                    _lastDrawOffset = context.DrawOffset;

                    //Draw Texture
                    spriteBatch = context.SpriteBatch;
                    spriteBatch.Draw(toPaint, new Rectangle((int) pos.X, (int) pos.Y, Width, Height), Color.White);

                    //Draw Text
                    DrawText(spriteBatch, context.DrawOffset);
                }
            }

            base.Draw(context);
        }

        /// <summary>
        /// </summary>
        /// <param name = "spriteBatch"></param>
        /// <param name = "drawOffset"></param>
        private void DrawText(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            //Draw Text if Visible
            if (!TextVisible) return;

            //We should have some display text to render
            if (string.IsNullOrEmpty(DisplayText)) return;

            //Get Position based on the TextAlignment in horizontal side.
            var pos = new Vector2(StartPositionX(drawOffset), drawOffset.Y + Height/2 - TextHeight/2);

            //If clicked state then draw with ClickedForeground else with normal texture.
            spriteBatch.DrawString(Font, DisplayText, pos,
                                   _state == ButtonState.Clicked ? ClickedForeground : Foreground, TextRotation,
                                   Vector2.Zero,
                                   ScaleText(), SpriteEffects.None, 0);
        }

        /// <summary>
        ///   Called when the Size property is read and sizeValid is false. Call base.ComputeSize() to compute the
        ///   size (actually the lower-right corner) of all child controls.
        /// </summary>
        public override Vector2 ComputeSize()
        {
            return new Vector2(_width, _height);
        }

        /// <summary>
        ///   States to texture.
        /// </summary>
        /// <returns>Texture for the current button state.</returns>
        private Texture2D StateToTexture()
        {
            Texture2D toPaint = null;
            switch (_state)
            {
                case ButtonState.Normal:
                    toPaint = _txNormalButton;
                    break;
                case ButtonState.Clicked:
                    toPaint = _txClickedButton;
                    break;
            }
            return toPaint;
        }

        /// <summary>
        ///   Scales the text.
        /// </summary>
        /// <returns>Size of text according to TextSize property</returns>
        private float ScaleText()
        {
            switch (TextSize)
            {
                case FontSize.Small:
                    return 0.75f;
                case FontSize.Medium:
                    return 1;
                case FontSize.Big:
                    return 1.25f;
                default:
                    return 1;
            }
        }

        /// <summary>
        ///   Starts the position X.
        /// </summary>
        /// <param name = "drawOffset">The draw offset.</param>
        /// <returns>Start Position for X direction based on text alignment property</returns>
        private float StartPositionX(Vector2 drawOffset)
        {
            switch (TextAlignment)
            {
                case TextAlign.Left:
                    return drawOffset.X + 50;
                case TextAlign.Right:
                    return drawOffset.X + Width - TextWidth - 50;
                case TextAlign.Centre:
                    return drawOffset.X + Width/2 - TextWidth/2;
                default:
                    return drawOffset.X + Width/2 - TextWidth/2;
            }
        }

        /// <summary>
        ///   Handles the input.
        /// </summary>
        /// <param name = "gestures">The gestures.</param>
        public void HandleInput(List<GestureSample> gestures)
        {
            //Currently we are only interested in Tap gesture.
            if (gestures.Count <= 0) return;
            switch (gestures[0].GestureType)
            {
                case GestureType.Tap:
                    if (HandleTap(gestures[0].Position))
                        gestures.RemoveAt(0); //After processing the gesture, remove it from the queue.
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "tapPosition"></param>
        /// <returns></returns>
        private bool HandleTap(Vector2 tapPosition)
        {
            TapPosition = tapPosition;

            //Find if the Tap in inside the button's click area.
            var rect = ClickAreaSpecific
                           ? ClickArea
                           : new Rectangle((int) _lastDrawOffset.X, (int) _lastDrawOffset.Y, Width, Height);

            var mousePos = new Point((int) TapPosition.X, (int) TapPosition.Y);

            if (rect.Contains(mousePos))
            {
                //If Tap is inside clickable area then change the state to Clicked.
                //By doing this we are telling the system to render clicked texture and 
                //after some specific timespan raise the click event.
                _state = ButtonState.Clicked;
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Occurs when [on clicked].
        /// </summary>
        /// <remarks>
        /// </remarks>
        public event ClickHandler OnClicked;
    }
}
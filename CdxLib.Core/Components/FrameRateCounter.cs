using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace CdxLib.Core.Components
{
    /// <summary>
    ///   Displays the FPS
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        private readonly NumberFormatInfo _format;
        private readonly Vector2 _position;
        private readonly ScreenManager.ScreenManager _screenManager;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _frameCounter;
        private int _frameRate;

        /// <summary>
        /// </summary>
        /// <param name = "screenManager"></param>
        public FrameRateCounter(ScreenManager.ScreenManager screenManager) : base(screenManager.Game)
        {
            _screenManager = screenManager;
            _format = new NumberFormatInfo {NumberDecimalSeparator = "."};
            _position = new Vector2(30, 25);
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            //If 1 second passed....
            if (_elapsedTime <= TimeSpan.FromSeconds(1)) return;

            //Reset FrameRate Counter
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            var fps = string.Format(_format, "{0} fps", _frameRate);

            _screenManager.SpriteBatch.Begin();
//            _screenManager.SpriteBatch.DrawString(_screenManager.SpriteFonts.FrameRateCounterFont, fps,
//                                                  _position, Color.Red, 0, Vector2.Zero, 1.0f,
//                                                  SpriteEffects.None, 1);
            _screenManager.SpriteBatch.End();
        }
    }
}
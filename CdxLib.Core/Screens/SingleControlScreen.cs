using CdxLib.Core.Controls;
using CdxLib.Core.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CdxLib.Core.Screens
{
    /// <summary>
    /// A screen containing single Control. This class serves as a bridge between the 'Controls'
    /// UI system and the 'ScreenManager' UI system.
    /// </summary>
    public class SingleControlScreen : GameScreen
    {
        /// <summary>
        /// The sole Control in this screen. Derived classes can do what they like with it.
        /// </summary>
        protected Control RootControl;

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (RootControl != null)
            {
                Control.BatchDraw(RootControl, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch,
                                  Vector2.Zero, gameTime);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if(RootControl != null)
            {
                RootControl.Update(gameTime);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="input"></param>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // cancel the current screen if the user presses the back button
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, null, out player))
            {
                ExitScreen();
            }

            if(RootControl != null)
            {
                RootControl.HandleInput(input);
            }

            base.HandleInput(gameTime, input);
        }
    }
}
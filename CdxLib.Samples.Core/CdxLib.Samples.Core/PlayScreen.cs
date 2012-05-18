using CdxLib.Core.ScreenManager;
using CdxLib.Core.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CdxLib.Samples.Core
{
    internal class PlayScreen : GameScreen
    {
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            var pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2f,
                                  ScreenManager.GraphicsDevice.Viewport.Height/2f);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.GameSpriteFont, "PlayScreen", pos,
                                                 Color.White);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.IsNewButtonPress(Buttons.Back) || input.IsNewKeyPress(Keys.Escape))
            {
                ExitScreen();
                var screensToLoad = new GameScreen[]
                                    {
                                        new MainScreen("Main Menu"), 
                                    };
                LoadingScreen.Load(ScreenManager, true, null, screensToLoad);
            }
        }
    }
}
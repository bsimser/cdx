using System;
using CdxLib.Core.ScreenManager;
using CdxLib.Core.Screens;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;

namespace CdxLib.Samples.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly ScreenManager _screenManager;

        public Game1()
        {
            // Most games will probably run horizontally but flip
            // these values here to run vertically if you want
            new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 480,
                PreferredBackBufferWidth = 800,
                IsFullScreen = true
            };

            // Where do we find our game content?
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            // Create the screen manager and handle adding the initial screens
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);
            PhoneApplicationService.Current.Launching += GameLaunching;
        }

        private void GameLaunching(object sender, LaunchingEventArgs e)
        {
            LoadingScreen.Load(_screenManager, true, null, new BackgroundScreen(), new MainScreen("Main Menu"));
        }
    }
}

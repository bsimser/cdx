using CdxLib.Core.Screens;
using Microsoft.Xna.Framework;

namespace CdxLib.Samples.Core
{
    class MainScreen : MenuScreen
    {
        public MainScreen(string menuTitle) : base(menuTitle)
        {
            var menuEntry = new MenuEntry("Screens");
            menuEntry.Selected += ScreensMenuSelected;
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("Controls");
            menuEntry.Selected += ControlMenuSelected;
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("Components");
            menuEntry.Selected += ComponentMenuSelected;
            MenuEntries.Add(menuEntry);
        }

        private void ComponentMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            var screensToLoad = new GameScreen[]
                                    {
                                        this,
                                        new ComponentMenu("Components"), 
                                    };
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, screensToLoad);
        }

        private void ControlMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            var screensToLoad = new GameScreen[]
                                    {
                                        this,
                                        new ControlMenu("Controls"), 
                                    };
            LoadingScreen.Load(ScreenManager, false, null, screensToLoad);
        }

        private void ScreensMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            var screensToLoad = new GameScreen[]
                                    {
                                        this,
                                        new ScreenMenu("Screens"), 
                                    };
            LoadingScreen.Load(ScreenManager, false, null, screensToLoad);
        }

        /// <summary>
        /// When user cancels the main menu exit the game
        /// </summary>
        /// <param name="playerIndex"></param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }
    }
}

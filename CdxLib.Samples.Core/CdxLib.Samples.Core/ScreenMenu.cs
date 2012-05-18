using System;
using CdxLib.Core.Screens;

namespace CdxLib.Samples.Core
{
    class ScreenMenu : MenuScreen
    {
        public ScreenMenu(string menuTitle) : base(menuTitle)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            var menuEntry = new MenuEntry("GameScreen");
            menuEntry.Selected += GameScreenSelected;
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("MessageBoxScreen");
            menuEntry.Selected += MessageBoxScreenSelected;
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("SingleControlScreen");
            menuEntry.Selected += GameScreenSelected;
            MenuEntries.Add(menuEntry);
        }

        private void MessageBoxScreenSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MessageBoxScreen("Hello"), null);
        }

        private void GameScreenSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new PlayScreen(), null);
        }
    }
}
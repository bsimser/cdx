using CdxLib.Core.Screens;

namespace CdxLib.Samples.Core
{
    class ControlMenu : MenuScreen
    {
        public ControlMenu(string menuTitle) : base(menuTitle)
        {
            var menuEntry = new MenuEntry("Button");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("ImageControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("MenuButton");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("PageFlipControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("PanelControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("ImageControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("ScrollingPanelControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("TextControl");
            MenuEntries.Add(menuEntry);

            menuEntry = new MenuEntry("ToggleMenuButton");
            MenuEntries.Add(menuEntry);
        }
    }
}
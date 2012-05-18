using CdxLib.Core.Screens;

namespace CdxLib.Samples.Core
{
    class ComponentMenu : MenuScreen
    {
        public ComponentMenu(string menuTitle) : base(menuTitle)
        {
            var menuEntry = new MenuEntry("FrameRateCounter");
            MenuEntries.Add(menuEntry);
        }
    }
}
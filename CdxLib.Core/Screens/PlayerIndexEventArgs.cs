using System;
using Microsoft.Xna.Framework;

namespace CdxLib.Core.Screens
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        private readonly PlayerIndex _playerIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return _playerIndex; }
        }
    }
}
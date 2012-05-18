using System;
using CdxLib.Core.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.GamerServices.Controls
{
    /// <remarks>
    ///   This class displays a list of high scores, to give an example of presenting
    ///   a list of data that the player can scroll through.
    /// </remarks>
    public class HighScorePanel : ScrollingPanelControl
    {
        private readonly SpriteFont _detailFont;
        private readonly SpriteFont _headerFont;
        private readonly SpriteFont _titleFont;
        private Control _resultListControl;

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        public HighScorePanel(ContentManager content)
        {
            _titleFont = content.Load<SpriteFont>(@"Font\MenuTitle");
            _headerFont = content.Load<SpriteFont>(@"Font\MenuHeader");
            _detailFont = content.Load<SpriteFont>(@"Font\MenuDetail");

            AddChild(new TextControl("High score", _titleFont));
            AddChild(CreateHeaderControl());
            PopulateWithFakeData();
        }

        /// <summary>
        /// </summary>
        private void PopulateWithFakeData()
        {
            var newList = new PanelControl();
            var rng = new Random();
            for (var i = 0; i < 50; i++)
            {
                long score = 10000 - i*10;
                var time = TimeSpan.FromSeconds(rng.Next(60, 3600));
                newList.AddChild(CreateLeaderboardEntryControl("player" + i, score, time));
            }
            newList.LayoutColumn(0, 0, 0);

            if (_resultListControl != null)
            {
                RemoveChild(_resultListControl);
            }
            _resultListControl = newList;
            AddChild(_resultListControl);
            LayoutColumn(0, 0, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected Control CreateHeaderControl()
        {
            var panel = new PanelControl();

            panel.AddChild(new TextControl("Player", _headerFont, Color.Turquoise, new Vector2(0, 0)));
            panel.AddChild(new TextControl("Score", _headerFont, Color.Turquoise, new Vector2(200, 0)));

            return panel;
        }

        // Create a Control to display one entry in a leaderboard. The content is broken out into a parameter
        // list so that we can easily create a control with fake data when running under the emulator.
        // For time leaderboards, this function interprets the time as a count in seconds. The
        // value posted is simply a long, so your leaderboard might actually measure time in ticks, milliseconds,
        // or microfortnights. If that is the case, adjust this function to display appropriately.
        protected Control CreateLeaderboardEntryControl(string player, long rating, TimeSpan time)
        {
            var textColor = Color.White;
            var panel = new PanelControl();

            // Player name
            panel.AddChild(
                new TextControl
                    {
                        Text = player,
                        Font = _detailFont,
                        Color = textColor,
                        Position = new Vector2(0, 0)
                    });

            // Score
            panel.AddChild(
                new TextControl
                    {
                        Text = String.Format("{0}", rating),
                        Font = _detailFont,
                        Color = textColor,
                        Position = new Vector2(200, 0)
                    });

            // Time
            panel.AddChild(
                new TextControl
                    {
                        Text = String.Format("Completed in {0:g}", time),
                        Font = _detailFont,
                        Color = textColor,
                        Position = new Vector2(400, 0)
                    });

            return panel;
        }
    }
}
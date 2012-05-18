using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.ScreenManager
{
    /// <summary>
    ///   Font helper class.
    /// </summary>
    public class SpriteFonts
    {
        private readonly ContentManager _content;
        private readonly Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();

        /// <summary>
        /// </summary>
        /// <param name="contentManager"></param>
        public SpriteFonts(ContentManager contentManager)
        {
            _content = contentManager;
        }

        /// <summary>
        /// </summary>
        public SpriteFont LargeText
        {
            get
            {
                if (!_fonts.ContainsKey("LargeText"))
                    _fonts.Add("LargeText", _content.Load<SpriteFont>("Fonts/LargeText"));
                return _fonts["LargeText"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont FrameRateCounterFont
        {
            get
            {
                if (!_fonts.ContainsKey("FrameRateCounterFont"))
                    _fonts.Add("FrameRateCounterFont", _content.Load<SpriteFont>("Fonts/frameRateCounterFont"));
                return _fonts["FrameRateCounterFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont SegoeBold
        {
            get
            {
                if (!_fonts.ContainsKey("SegoeBold"))
                    _fonts.Add("SegoeBold", _content.Load<SpriteFont>("Fonts/SegoeBold"));
                return _fonts["SegoeBold"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont GameSpriteFont
        {
            get
            {
                if (!_fonts.ContainsKey("GameSpriteFont"))
                    _fonts.Add("GameSpriteFont", _content.Load<SpriteFont>("Fonts/gamefont"));
                return _fonts["GameSpriteFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont GameSpriteSmallFont
        {
            get
            {
                if (!_fonts.ContainsKey("GameSpriteSmallFont"))
                    _fonts.Add("GameSpriteSmallFont", _content.Load<SpriteFont>("Fonts/gamefontsmall"));
                return _fonts["GameSpriteSmallFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont GameSpriteMediumFont
        {
            get
            {
                if (!_fonts.ContainsKey("GameSpriteMediumFont"))
                    _fonts.Add("GameSpriteMediumFont", _content.Load<SpriteFont>("Fonts/gamefontMedium"));
                return _fonts["GameSpriteMediumFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont MenuSpriteFont
        {
            get
            {
                if (!_fonts.ContainsKey("MenuSpriteFont"))
                    _fonts.Add("MenuSpriteFont", _content.Load<SpriteFont>("Fonts/menufont"));
                return _fonts["MenuSpriteFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont DetailsFont
        {
            get
            {
                if (!_fonts.ContainsKey("DetailsFont"))
                    _fonts.Add("DetailsFont", _content.Load<SpriteFont>("Fonts/detailsfont"));
                return _fonts["DetailsFont"];
            }
        }

        /// <summary>
        /// </summary>
        public SpriteFont DebugFont
        {
            get
            {
                if (!_fonts.ContainsKey("DebugFont"))
                    _fonts.Add("DebugFont", _content.Load<SpriteFont>("Fonts/debugfont"));
                return _fonts["DebugFont"];
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Graphics
{
    /// <summary>
    ///   Moving Background for game screens.
    /// </summary>
    public class ParallaxBackground
    {
        private readonly ParallaxDirection _direction;
        private Vector2 _defaultMovementSpeed;
        private Vector2 _position;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ParallaxBackground" /> class.
        /// </summary>
        /// <param name = "scrollingTextures">The scrolling textures. Order of texture should be maintained.</param>
        /// <param name = "direction">The direction.</param>
        /// <remarks>
        /// </remarks>
        public ParallaxBackground(List<Texture2D> scrollingTextures, ParallaxDirection direction)
        {
            SpeedX = 1;
            SpeedY = 1;
            _direction = direction;
            ScrollingTextures = scrollingTextures;
            Initialize();
        }

        /// <summary>
        ///   Gets or sets the speed X.
        /// </summary>
        /// <value>The speed X.</value>
        /// <remarks>
        /// </remarks>
        public float SpeedX { get; set; }

        /// <summary>
        ///   Gets or sets the speed Y.
        /// </summary>
        /// <value>The speed Y.</value>
        /// <remarks>
        /// </remarks>
        public float SpeedY { get; set; }

        /// <summary>
        ///   Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks>
        /// </remarks>
        public int Height { get; set; }

        /// <summary>
        ///   Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        /// <remarks>
        /// </remarks>
        public int Width { get; set; }

        /// <summary>
        ///   Gets or sets the scrolling textures.
        /// </summary>
        /// <value>The scrolling textures.</value>
        /// <remarks>
        /// </remarks>
        public List<Texture2D> ScrollingTextures { get; set; }

        /// <summary>
        /// </summary>
        private List<TextureDetails> Textures { get; set; }

        /// <summary>
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        ///   Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            //Set Speed according to Direction
            switch (_direction)
            {
                case ParallaxDirection.Horizontal:
                    SpeedX = 1;
                    SpeedY = 0;
                    break;
                case ParallaxDirection.Vertical:
                    SpeedX = 0;
                    SpeedY = 1;
                    break;
            }

            Position = Vector2.Zero;
            _defaultMovementSpeed = new Vector2(SpeedX, SpeedY);

            //Extract data from ScrollingTextures
            int lastWidth = 0, lastHeight = 0;
            Textures = new List<TextureDetails>();

            var i = 1;
            foreach (var texture in ScrollingTextures)
            {
                Textures.Add(new TextureDetails(texture, texture.Width, texture.Height, lastWidth, lastHeight,
                                                i.ToString()));
                lastWidth += texture.Width;
                lastHeight += texture.Height;
                i++;
            }
        }

        /// <summary>
        ///   Moves the specified distance.
        /// </summary>
        /// <param name = "distance">The distance.</param>
        /// <remarks>
        /// </remarks>
        public void Move(Vector2 distance)
        {
            //Change the CurrentPosition according to the Distance and the Direction we are moving.
            _position.X += SpeedX*distance.X;
            _position.Y += SpeedY*distance.Y;

            if (Position.X >=
                (Textures[Textures.Count - 1].BeltPositionX + Textures[Textures.Count - 1].Width))
                _position.X = 0;

            if (Position.Y >=
                (Textures[Textures.Count - 1].BeltPositionY + Textures[Textures.Count - 1].Height))
                _position.Y = 0;
        }

        /// <summary>
        ///   Moves by default speed and distance.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void Move()
        {
            Move(_defaultMovementSpeed);
        }

        /// <summary>
        /// </summary>
        /// <param name = "spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Iterate through all texture
            //If Current position fall into current texture then render it and
            //Increment the current location by width (min(texturewidth and windowWidth)

            var positionX = Position.X;
            var positionY = Position.Y;
            for (var i = 0; i < Textures.Count; i++)
            {
                if (_direction == ParallaxDirection.Horizontal)
                    if (positionX >= Textures[i].BeltPositionX &&
                        positionX <= (Textures[i].BeltPositionX + Textures[i].Width))
                    {
                        positionX += Math.Min(Textures[i].Width, Width);
                        spriteBatch.Draw(Textures[i].Texture,
                                         new Vector2(Textures[i].BeltPositionX - Position.X, 0), Color.White);
                    }

                if (_direction == ParallaxDirection.Vertical)
                    if (positionY >= Textures[i].BeltPositionY &&
                        positionY <= (Textures[i].BeltPositionY + Textures[i].Height))
                    {
                        positionY += Math.Min(Textures[i].Height, Height);
                        spriteBatch.Draw(Textures[i].Texture,
                                         new Vector2(0, Textures[i].BeltPositionY - Position.Y), Color.White);
                    }
            }

            //Means last texture reached and we can't reset Position to zero unless untill we completely render last texture,
            //So render first texture just by the last texture
            // |-------------|--------------------------|
            // | Last        | First                    |
            // | Texture     | Texture                  |
            // |-------------|--------------------------|
            if (_direction == ParallaxDirection.Horizontal)
                if (Position.X >=
                    (Textures[Textures.Count - 1].BeltPositionX + Textures[Textures.Count - 1].Width - Width))
                {
                    spriteBatch.Draw(Textures[0].Texture,
                                     new Vector2(
                                         (Textures[Textures.Count - 1].BeltPositionX +
                                          Textures[Textures.Count - 1].Width - Position.X), 0), Color.White);
                }
            if (_direction == ParallaxDirection.Vertical)
                if (Position.Y >=
                    (Textures[Textures.Count - 1].BeltPositionY + Textures[Textures.Count - 1].Height - Height))
                {
                    spriteBatch.Draw(Textures[0].Texture,
                                     new Vector2(0,
                                                 (Textures[Textures.Count - 1].BeltPositionY +
                                                  Textures[Textures.Count - 1].Height - Position.Y)),
                                     Color.White);
                }
        }

        #region Nested type: TextureDetails

        /// <summary>
        ///   DataStructure to store information about the scrolling tectures. Used in current class only.
        /// </summary>
        private struct TextureDetails
        {
            public readonly int BeltPositionX;
            public readonly int BeltPositionY;
            public readonly int Height;
            public readonly Texture2D Texture;
            public readonly int Width;
            public string Name;

            public TextureDetails(Texture2D texture, int width, int height, int posX, int posY, string name)
            {
                Texture = texture;
                Width = width;
                Height = height;
                BeltPositionX = posX;
                BeltPositionY = posY;
                Name = name;
            }
        }

        #endregion
    }
}
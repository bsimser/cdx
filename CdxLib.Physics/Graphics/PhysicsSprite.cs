using CdxLib.Core.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Physics.Graphics
{
    /// <summary>
    /// </summary>
    public class PhysicsSprite : Sprite
    {
        private Body _body;

        /// <summary>
        /// </summary>
        /// <param name="sprite"></param>
        public PhysicsSprite(Texture2D sprite) : base(sprite)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="body"></param>
        public void AttachBody(Body body)
        {
            _body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="screenCenter"></param>
        public void Update(Vector2 screenCenter)
        {
            // Update our sprite to match the location where the physics engine has determined where it is
            X = (int) (ConvertUnits.ToDisplayUnits(_body.Position.X) + screenCenter.X);
            Y = (int) (ConvertUnits.ToDisplayUnits(_body.Position.Y) + screenCenter.Y);
            Angle = _body.Rotation;
        }

        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                new Rectangle(X, Y, Width, Height),
                null,
                Color.White,
                Angle,
                Origin,
                SpriteEffects.None,
                Layer);
        }
    }
}

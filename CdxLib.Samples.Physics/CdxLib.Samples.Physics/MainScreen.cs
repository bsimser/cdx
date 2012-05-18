using System;
using System.Collections.Generic;
using CdxLib.Physics.Graphics;
using CdxLib.Physics.Screens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Samples.Physics
{
    public class MainScreen : PhysicsGameScreen
    {
        private readonly IList<PhysicsSprite> _sprites = new List<PhysicsSprite>();
        private readonly Random _random = new Random();
        private double _timer;

        public override void LoadContent()
        {
            base.LoadContent();

            // Make objects fall at 10 meters per second
            World.Gravity = Vector2.UnitY*10;

            // Create some walls
            InitializeSpace();

            // Create our crate
            InitializeCrate();

            // Turn on debug view to draw entities
            DebugView.Enabled = true;
            DebugView.AppendFlags(DebugViewFlags.DebugPanel);
        }

        private void InitializeSpace()
        {
            var floor = BodyFactory.CreateRectangle(World, 
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width), 
                ConvertUnits.ToSimUnits(20), 0);
            floor.Position = new Vector2(
                ConvertUnits.ToSimUnits(0), 
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height/2));
            floor.BodyType = BodyType.Static;

            var left = BodyFactory.CreateRectangle(World,
                ConvertUnits.ToSimUnits(20),
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height), 0);
            left.Position = new Vector2(
                ConvertUnits.ToSimUnits(-ScreenManager.GraphicsDevice.Viewport.Width/2),
                ConvertUnits.ToSimUnits(0));
            left.BodyType = BodyType.Static;

            var right = BodyFactory.CreateRectangle(World, 
                ConvertUnits.ToSimUnits(20), 
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height), 0);
            right.Position = new Vector2(
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width / 2),
                ConvertUnits.ToSimUnits(0));
            right.BodyType = BodyType.Static;
        }

        private void InitializeCrate()
        {
            // Create the crate as a sprite for display
            var sprite = new PhysicsSprite(ScreenManager.ContentManager.Load<Texture2D>("Sprites/crate"))
            {
                X = _random.Next(-400, 400),
                Y = -512,
                Width = _random.Next(40, 80),
                Height = _random.Next(40, 80),
                Layer = 1,
            };

            // Create a crate to fall
            var crate = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(sprite.Width),
                                                 ConvertUnits.ToSimUnits(sprite.Height), 0.8f);

            // Give it a little spin
            crate.Rotation = MathHelper.ToRadians(_random.Next(0, 45));

            // And make it a little rough
            crate.Friction = 0.3f;

            // Add some bounce
            crate.Restitution = 0.6f;

            // Make it non-static so it interacts with other objects
            crate.BodyType = BodyType.Dynamic;

            // Drop the crate from somewhere above the screen
            crate.Position = new Vector2(ConvertUnits.ToSimUnits(_random.Next(-ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Width/2)),
                                          ConvertUnits.ToSimUnits(-ScreenManager.GraphicsDevice.Viewport.Height));

            // Finally attach it to our sprite
            sprite.AttachBody(crate);

            _sprites.Add(sprite);
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // The base physics screen handles stepping the world (which then updates all of our objects in it)
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update our sprite (which will sync the physics object to the graphics)
            var screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2f,
                                           ScreenManager.GraphicsDevice.Viewport.Height/2f);
            foreach (var physicsSprite in _sprites)
            {
                physicsSprite.Update(screenCenter);
            }

            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(_timer > 200 && World.BodyList.Count < 80)
            {
                _timer = 0;
                InitializeCrate();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            foreach (var sprite in _sprites)
            {
                sprite.Draw(ScreenManager.SpriteBatch);
            }

            ScreenManager.SpriteBatch.End();

            if (DebugView.Enabled)
                base.Draw(gameTime);
        }
    }
}
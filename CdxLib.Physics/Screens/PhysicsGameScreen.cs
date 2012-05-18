using System;
using CdxLib.Core.Screens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace CdxLib.Physics.Screens
{
    public class PhysicsGameScreen : GameScreen
    {
        protected Camera2D Camera;
        protected World World;
        protected DebugViewXna DebugView;

        /// <summary>
        /// </summary>
        protected PhysicsGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            EnableCameraControl = true;
            World = null;
            Camera = null;
            DebugView = null;
        }

        /// <summary>
        /// </summary>
        public bool EnableCameraControl { get; set; }

        /// <summary>
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            //We enable diagnostics to show get values for our performance counters.
            Settings.EnableDiagnostics = true;

            if (World == null)
            {
                World = new World(Vector2.Zero);
            }
            else
            {
                World.Clear();
            }

            if (DebugView == null)
            {
                DebugView = new DebugViewXna(World);
                DebugView.LoadContent(ScreenManager);
            }

            if (Camera == null)
            {
                Camera = new Camera2D(ScreenManager.GraphicsDevice);
            }
            else
            {
                Camera.ResetCamera();
            }

            // Loading may take a while... so prevent the game from "catching up" once we finished loading
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// </summary>
        /// <param name = "gameTime"></param>
        /// <param name = "otherScreenHasFocus"></param>
        /// <param name = "coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!coveredByOtherScreen && !otherScreenHasFocus)
            {
                // variable time step but never less then 30 Hz
                World.Step((float) Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds*0.001f, (1f/30f)));
            }
            else
            {
                World.Step(0f);
            }

            Camera.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// </summary>
        /// <param name = "gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            var projection = Camera.SimProjection;
            var view = Camera.SimView;
            DebugView.RenderDebugData(ref projection, ref view);
            base.Draw(gameTime);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using CdxLib.Core.Graphics;
using CdxLib.Core.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CdxLib.Core.ScreenManager
{
    /// <summary>
    ///   The screen manager is a component which manages one or more GameScreen
    ///   instances. It maintains a stack of screens, calls their Update and Draw
    ///   methods at the appropriate times, and automatically routes input to the
    ///   topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        private readonly InputState _input = new InputState();
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private bool _isInitialized;
        private bool _traceEnabled;
        private readonly List<RenderTarget2D> _transitions = new List<RenderTarget2D>();

        /// <summary>
        ///   Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            ContentManager = game.Content;
            ContentManager.RootDirectory = "Content";
        }

        /// <summary>
        /// </summary>
        public ContentManager ContentManager { get; set; }

        /// <summary>
        ///   A default SpriteBatch shared by all the screens. This saves
        ///   each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        ///   A blank white texture shared by all the screens, useful
        ///   for drawing solid lines and rectangles with SpriteBatch.
        ///   This saves each screen from having to bother loading
        ///   their own local copy.
        /// </summary>
        public Texture2D BlankTexture { get; private set; }

        /// <summary>
        ///   If true, the manager prints out a list of all the screens
        ///   each time it is updated. This can be useful for making sure
        ///   everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return _traceEnabled; }
            set { _traceEnabled = value; }
        }

        /// <summary>
        ///   Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            SpriteFonts = new SpriteFonts(ContentManager);
            base.Initialize();
            _isInitialized = true;
        }

        /// <summary>
        /// </summary>
        public SpriteFonts SpriteFonts { get; set; }

        /// <summary>
        ///   Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            var content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO how to load common locations?
//            Font = content.Load<SpriteFont>("Font\\MenuTitle");

            BlankTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            BlankTexture.SetData(new[] {Color.White});

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        ///   Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        ///   Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            _input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState != ScreenState.TransitionOn && screen.ScreenState != ScreenState.Active)
                    continue;

                // If this is the first active screen we came across,
                // give it a chance to handle input.
                if (!otherScreenHasFocus)
                {
                    screen.HandleInput(gameTime, _input);

                    otherScreenHasFocus = true;
                }

                // If this is an active non-popup, inform any subsequent
                // screens that they are covered by it.
                if (!screen.IsPopup)
                    coveredByOtherScreen = true;
            }

            // Print debug trace?
            if (_traceEnabled)
                TraceScreens();
        }

        /// <summary>
        /// </summary>
        public void ResetTargets()
        {
            _transitions.Clear();
        }

        /// <summary>
        ///   Prints a list of all the screens, for debugging.
        /// </summary>
        private void TraceScreens()
        {
            Debug.WriteLine(string.Join(", ", _screens.Select(screen => screen.GetType().Name).ToArray()));
        }

        /// <summary>
        ///   Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            int transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.TransitionOff)
                {
                    ++transitionCount;
                    if (_transitions.Count < transitionCount)
                    {
                        PresentationParameters _pp = GraphicsDevice.PresentationParameters;
                        _transitions.Add(new RenderTarget2D(GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight,
                                                            false,
                                                            SurfaceFormat.Color, DepthFormat.Depth24Stencil8, _pp.MultiSampleCount,
                                                            RenderTargetUsage.DiscardContents));
                    }
                    GraphicsDevice.SetRenderTarget(_transitions[transitionCount - 1]);
                    GraphicsDevice.Clear(Color.Transparent);
                    screen.Draw(gameTime);
                    GraphicsDevice.SetRenderTarget(null);
                }
            }

            GraphicsDevice.Clear(Color.Transparent);

            transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.TransitionOff)
                {
                    if (screen.ScreenState != ScreenState.TransitionOff)
                    {
                        SpriteBatch.Begin();
                        SpriteBatch.Draw(_transitions[transitionCount], Vector2.Zero, Color.White * screen.TransitionAlpha);
                        SpriteBatch.End();

                    }
                    ++transitionCount;
                }
                else
                {
                    screen.Draw(gameTime);
                }
            }
        }

        /// <summary>
        ///   Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
            {
                screen.LoadContent();
            }

            _screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;

            screen.FirstRun = false;
        }

        /// <summary>
        ///   Removes a screen from the screen manager. You should normally
        ///   use GameScreen.ExitScreen instead of calling this directly, so
        ///   the screen can gradually transition off rather than just being
        ///   instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (_screens.Count > 0)
            {
                TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
            }
        }

        /// <summary>
        ///   Expose an array holding all the screens. We return a copy rather
        ///   than the real master list, because screens should only ever be added
        ///   or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        /// <summary>
        ///   Helper draws a translucent black fullscreen sprite, used for fading
        ///   screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();

            SpriteBatch.Draw(BlankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black*alpha);

            SpriteBatch.End();
        }

        /// <summary>
        ///   Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void SerializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("ScreenManager"))
                {
                    DeleteState(storage);
                }
                else
                {
                    // otherwise just create the directory
                    storage.CreateDirectory("ScreenManager");
                }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (GameScreen screen in _screens.Where(screen => screen.IsSerializable))
                        {
                            writer.Write(screen.GetType().AssemblyQualifiedName);
                        }
                    }
                }

                // now we create a new file stream for each screen so it can save its state
                // if it needs to. we name each file "ScreenX.dat" where X is the index of
                // the screen in the stack, to ensure the files are uniquely named
                int screenIndex = 0;
                foreach (GameScreen screen in _screens)
                {
                    if (!screen.IsSerializable) continue;
                    string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                    // open up the stream and let the screen serialize whatever state it wants
                    using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                    {
                        screen.Serialize(stream);
                    }

                    screenIndex++;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool DeserializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // see if our saved state directory exists
                if (storage.DirectoryExists("ScreenManager"))
                {
                    try
                    {
                        // see if we have a screen list
                        if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                        {
                            // load the list of screen types
                            using (
                                IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat",
                                                                                    FileMode.Open,
                                                                                    FileAccess.Read))
                            {
                                using (var reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a screen from it
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type screenType = Type.GetType(line);
                                            var screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen, PlayerIndex.One);
                                        }
                                    }
                                }
                            }
                        }

                        // next we give each screen a chance to deserialize from the disk
                        for (int i = 0; i < _screens.Count; i++)
                        {
                            string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                            using (
                                IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open,
                                                                                    FileAccess.Read))
                            {
                                _screens[i].Deserialize(stream);
                            }
                        }

                        return true;
                    }
                    catch (Exception)
                    {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///   Deletes the saved state files from isolated storage.
        /// </summary>
        private static void DeleteState(IsolatedStorageFile storage)
        {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("ScreenManager", file));
            }
        }
    }
}
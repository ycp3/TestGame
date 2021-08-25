using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame
{
    public class Game1 : Game
    {
        private const int N = 64;
        private const int Height = 500;
        private const int Width = 500;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;

        private SmokeEmitter _smokeEmitter;
        private Texture2D _smokeTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.ApplyChanges();

            _device = _graphics.GraphicsDevice;
            _smokeEmitter = new SmokeEmitter(N);
            _smokeTexture = new Texture2D(_device, N, N, false, SurfaceFormat.Color);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // MouseState mouseState = Mouse.GetState();
            // int mouseX = Math.Clamp(mouseState.X * N / Width, 0, N - 1);
            // int mouseY = Math.Clamp(mouseState.Y * N / Height, 0, N - 1);

            KeyboardState keyboardState = Keyboard.GetState();

            _smokeEmitter.HandleInput(keyboardState);
            _smokeEmitter.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _smokeTexture.SetData(_smokeEmitter.ToColors());

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // _spriteBatch.Begin();
            _spriteBatch.Draw(_smokeTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, (float) Height / N,
                SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
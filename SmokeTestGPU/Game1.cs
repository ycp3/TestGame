using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SmokeTestGPU
{
    public class Game1 : Game
    {
        private const int WindowWidth = 1920;
        private const int WindowHeight = 1080;
        private const int TargetWidth = 320;
        private const int TargetHeight = TargetWidth * 9 / 16;
        private const int Scale = WindowWidth / TargetWidth;
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;

        private RenderTarget2D _testTarget;
        private Effect _testEffect;

        private Texture2D _rocket;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "SmokeTestGPU";
            
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _device = _graphics.GraphicsDevice;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _testEffect = Content.Load<Effect>("Shaders/Smoke");
            _rocket = Content.Load<Texture2D>("rocket");

            _testTarget = new RenderTarget2D(_device, TargetWidth, TargetHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetRenderTarget(_testTarget);
            _spriteBatch.Begin(effect: _testEffect);
            _spriteBatch.Draw(_rocket, new Rectangle(0, 0, TargetWidth, TargetHeight), Color.White);
            _spriteBatch.End();
            
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_testTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Enes_TasciGameDev.States;

namespace Enes_TasciGameDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState = GameState.StartMenu;
        private SpriteFont font;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("DefaultFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (currentState == GameState.StartMenu)
            {
                if (keyboard.IsKeyDown(Keys.Enter))
                {
                    currentState = GameState.Playing;
                }
            }
            else if (currentState == GameState.GameOver)
            {
                if (keyboard.IsKeyDown(Keys.Enter))
                {
                    currentState = GameState.StartMenu;
                }
            }
            else if (currentState == GameState.Playing)
            {
                // hier later game-logica
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (currentState == GameState.StartMenu)
            {
                _spriteBatch.DrawString(font, "Press ENTER to Start", new Vector2(100, 100), Color.White);
            }
            else if (currentState == GameState.Playing)
            {
                _spriteBatch.DrawString(font, "Game Running...", new Vector2(100, 100), Color.White);
            }
            else if (currentState == GameState.GameOver)
            {
                _spriteBatch.DrawString(font, "Game Over! Press ENTER", new Vector2(100, 100), Color.Red);
            }

            _spriteBatch.End();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

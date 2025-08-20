using Enes_TasciGameDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class StartScreen : IGameState
{
    private Game1 game;
    private SpriteFont titleFont;
    private SpriteFont buttonFont;
    private Rectangle startButton;
    private bool isMouseOver;
    private Texture2D rectTexture;

    public StartScreen(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        titleFont = game.Content.Load<SpriteFont>("TitleFont");   // Big title font
        buttonFont = game.Content.Load<SpriteFont>("DefaultFont"); // Normal button font

        // Centered start button
        startButton = new Rectangle(
            game.GraphicsDevice.Viewport.Width / 2 - 100,
            game.GraphicsDevice.Viewport.Height / 2,
            200,
            60
        );

        // Simple gray rectangle texture for button
        rectTexture = new Texture2D(game.GraphicsDevice, 1, 1);
        rectTexture.SetData(new[] { Color.Gray });
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        isMouseOver = startButton.Contains(mouse.Position);

        if (isMouseOver && mouse.LeftButton == ButtonState.Pressed)
        {
            game.ChangeState(new Level1(game));
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        game.GraphicsDevice.Clear(Color.LightBlue);

        // Draw the centered title
        string titleText = "The Coin Game";
        Vector2 titleSize = titleFont.MeasureString(titleText);
        spriteBatch.DrawString(titleFont, titleText,
            new Vector2((game.GraphicsDevice.Viewport.Width - titleSize.X) / 2, 100),
            Color.Black);

        // Draw start button rectangle
        Color buttonColor = isMouseOver ? Color.DarkGray : Color.Gray;
        spriteBatch.Draw(rectTexture, startButton, buttonColor);

        // Draw start button text
        string buttonText = "Start Game";
        Vector2 buttonSize = buttonFont.MeasureString(buttonText);
        spriteBatch.DrawString(buttonFont, buttonText,
            new Vector2(startButton.X + (startButton.Width - buttonSize.X) / 2,
                        startButton.Y + (startButton.Height - buttonSize.Y) / 2),
            Color.Black);

        spriteBatch.End();
    }
}

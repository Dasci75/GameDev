using Enes_TasciGameDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Level1 : IGameState
{
    private Game1 game;
    private Player player;  // Your Player class
    private Texture2D playerTexture;
    private Texture2D background;
    private KeyboardState previousKeyboardState;

    public Level1(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        // Load player texture (make sure the file exists in Content)
        background = game.Content.Load<Texture2D>("bgLevel1"); // Naam zonder extensie
        playerTexture = game.Content.Load<Texture2D>("player"); // your sprite sheet
        player = new Player(new Vector2(400, 240), playerTexture, rows: 4, columns: 4); // adjust rows/columns to your sprite sheet
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime);
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Teken de achtergrond
        spriteBatch.Draw(background, new Rectangle(0, 0,
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height), Color.White);

        // Teken de speler
        player.Draw(spriteBatch);

        spriteBatch.End();
    }
}

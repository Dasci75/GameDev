using Enes_TasciGameDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Level1 : IGameState
{
    private Player player;
    private Texture2D playerTexture;

    private Game1 game; // need the game instance to access Content

    public Level1(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        playerTexture = game.Content.Load<Texture2D>("player"); // your sprite sheet
        player = new Player(playerTexture, new Vector2(400, 240), 4, 4); // 8 columns, 13 rows
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        game.GraphicsDevice.Clear(Color.CornflowerBlue);
        player.Draw(spriteBatch);
    }
}


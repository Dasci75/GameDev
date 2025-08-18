using Enes_TasciGameDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Level1 : IGameState
{
    private Game1 game;
    private Player player;
    private Texture2D playerTexture;

    public Level1(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        playerTexture = game.Content.Load<Texture2D>("player"); // Make sure player.xnb exists
        player = new Player(playerTexture, new Vector2(400, 240));
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        game.GraphicsDevice.Clear(Color.CornflowerBlue);
        player.Draw(spriteBatch);
        spriteBatch.End();
    }
}

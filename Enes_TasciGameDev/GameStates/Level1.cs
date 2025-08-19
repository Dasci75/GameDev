using Enes_TasciGameDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Level1 : IGameState
{
    private Game1 game;
    private Player player;  // Your Player class
    private Texture2D playerTexture;
    private Texture2D background;
    private KeyboardState previousKeyboardState;
    private List<Coin> coins;
    private Texture2D coinTexture;
    private Random random = new Random();

    public Level1(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        // Background & player
        background = game.Content.Load<Texture2D>("bgLevel1");
        playerTexture = game.Content.Load<Texture2D>("player");
        player = new Player(new Vector2(400, 240), playerTexture, 4, 4);

        // Coins
        coinTexture = game.Content.Load<Texture2D>("coin"); // voeg coin.png toe aan Content
        coins = new List<Coin>();

        for (int i = 0; i < 10; i++) // spawn 10 coins random
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - coinTexture.Width),
                random.Next(0, game.GraphicsDevice.Viewport.Height - coinTexture.Height)
            );
            coins.Add(new Coin(coinTexture, pos));
        }
    }

    public void Update(GameTime gameTime)
    {
        player.Update(gameTime, game.GraphicsDevice);

        Rectangle playerBounds = player.GetBounds();

        for (int i = coins.Count - 1; i >= 0; i--)
        {
            if (playerBounds.Intersects(coins[i].GetBounds()))
            {
                coins.RemoveAt(i);
                // eventueel score verhogen of geluid afspelen
            }
        }
    }




    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Teken de achtergrond
        spriteBatch.Draw(background, new Rectangle(0, 0,
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height), Color.White);

        // Teken de coins
        foreach (var coin in coins)
        {
            coin.Draw(spriteBatch);
        }

        // Teken de speler
        player.Draw(spriteBatch);

        spriteBatch.End();
    }
}

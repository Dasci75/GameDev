using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public interface IGameState
{
    void LoadContent(); // Laad sprites, fonts etc.
    void Update(GameTime gameTime); // Logica
    void Draw(SpriteBatch spriteBatch); // Tekenen
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bratalian
{
    public class Player
    {
        public Vector2 Position;
        private Texture2D _texture;
        private float _speed = 100f;
        private float _scale;

        public Player(Texture2D tex, Vector2 start, int tileSize)
        {
            _texture = tex;
            Position = start;
            // escala para caber no tile
            _scale = tileSize / (float)_texture.Width;
        }

        public void Update(GameTime gt)
        {
            var kb = Keyboard.GetState();
            Vector2 mv = Vector2.Zero;
            if (kb.IsKeyDown(Keys.W)) mv.Y -= 1;
            if (kb.IsKeyDown(Keys.S)) mv.Y += 1;
            if (kb.IsKeyDown(Keys.A)) mv.X -= 1;
            if (kb.IsKeyDown(Keys.D)) mv.X += 1;
            if (mv != Vector2.Zero) mv.Normalize();
            Position += mv * _speed * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb)
        {
            // usamos o centro do sprite como origem para a escala ficar proporcional
            var origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
            sb.Draw(
                _texture,
                Position + origin * _scale,  // centraliza dentro do tile
                null,
                Color.White,
                0f,
                origin,
                _scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}

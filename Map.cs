using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bratalian
{
    public class Map
    {
        private Texture2D _tileset;
        private int[,] _data;
        private int _tileSize;
        private int _tilesPerRow;

        public int Width => _data.GetLength(1) * _tileSize;
        public int Height => _data.GetLength(0) * _tileSize;

        public Map(Texture2D tileset, int[,] mapData, int tileSize)
        {
            _tileset = tileset;
            _data = mapData;
            _tileSize = tileSize;
            _tilesPerRow = tileset.Width / tileSize;  // no teu caso = 10
        }

        public void Draw(SpriteBatch sb)
        {
            int rows = _data.GetLength(0);
            int cols = _data.GetLength(1);

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                {
                    int type = _data[y, x];            // 1..10
                    int idx = type - 1;               // 0..9
                    float rotation = 0f;               // em radianos
                    Vector2 origin = new Vector2(_tileSize / 2f, _tileSize / 2f);

                    // para os tipos “rotacionáveis”, 
                    // mapeamos sempre o mesmo idx e ajustamos o ângulo:
                    switch (type)
                    {
                        // --- Relva ---
                        case 4: // canto superior direito (use idx=3 sem rotação)
                            idx = 3; rotation = 0f; break;
                        case 5: // canto superior esquerdo -> gira 90° CCW o tile 4
                            idx = 3; rotation = -MathHelper.PiOver2; break;
                        case 6: // aresta entre cantos (tile 6 no sheet)
                            idx = 5; // atenção: idx=5 corresponde ao tile6 no spritesheet
                                     // decide rotação pelo vizinho (exemplo simplificado):
                                     // se tiver relva à esquerda ou direita, gira 90°:
                                     // se não, mantém vertical.
                                     // (aqui sempre vertical; ajuste conforme seu mapData)
                            rotation = 0f;
                            break;

                        // --- Obstáculo grande ---
                        case 7: // lateral entre cantos
                            idx = 6; rotation = 0f; break;
                        case 8: // interior do obstáculo
                            idx = 7; rotation = 0f; break;
                        case 9: // canto inferior esquerdo -> usa tile9 como está
                            idx = 8; rotation = 0f; break;
                        case 10:// canto superior esquerdo -> gira tile9 180°
                            idx = 8; rotation = MathHelper.Pi; break;

                        default:
                            idx = type - 1; rotation = 0f; break;
                    }

                    // calcula onde “fatia” no spritesheet:
                    int srcX = (idx % _tilesPerRow) * _tileSize;
                    int srcY = (idx / _tilesPerRow) * _tileSize;
                    var srcRect = new Rectangle(srcX, srcY, _tileSize, _tileSize);

                    // onde desenhar na tela:
                    var dstRect = new Rectangle(
                        x * _tileSize + _tileSize / 2,
                        y * _tileSize + _tileSize / 2,
                        _tileSize,
                        _tileSize
                    );

                    sb.Draw(
                        _tileset,
                        new Vector2(dstRect.X, dstRect.Y),
                        srcRect,
                        Color.White,
                        rotation,
                        origin,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
        }
    }
}

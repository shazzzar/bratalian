using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bratalian
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Player _player;
        private Map _map;
        private Camera2D _camera;

        private Texture2D _tilesetTex, _playerTex;
        private SpriteFont _font;

        private int[,] _mapData;
        private int _tileSize = 12;

        private enum GameState { Overworld, Battle }
        private GameState _state = GameState.Overworld;
        private Random _rng = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // janela 800×600 fixa
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            int cols = _graphics.PreferredBackBufferWidth / _tileSize; // ≃66
            int rows = _graphics.PreferredBackBufferHeight / _tileSize; // =50
            _mapData = new int[rows, cols];

            // preenche: 1=chão, 2=relva nas bordas
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    _mapData[y, x] = (y == 0 || y == rows - 1 || x == 0 || x == cols - 1) ? 2 : 1;

            // cria um obstáculo central de 8×6 tiles
            int w = 8, h = 6;
            int sx = cols / 2 - w / 2, sy = rows / 2 - h / 2;
            for (int y = sy; y < sy + h; y++)
                for (int x = sx; x < sx + w; x++)
                {
                    bool top = y == sy;
                    bool bot = y == sy + h - 1;
                    bool left = x == sx;
                    bool right = x == sx + w - 1;

                    if (!top && !bot && !left && !right)
                        _mapData[y, x] = 8;            // interior (tile 8)
                    else if ((top || bot) && (left || right))
                        _mapData[y, x] = (bot && left) ? 9 : 10; // cantos 9 e 10
                    else if (top || bot)
                        _mapData[y, x] = 3;            // obstáculo chão (tile 3)
                    else
                        _mapData[y, x] = 7;            // lateral (tile 7)
                }

            base.Initialize();
            _camera = new Camera2D(GraphicsDevice.Viewport);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _tilesetTex = Content.Load<Texture2D>("tileset");
            _playerTex = Content.Load<Texture2D>("player");
            _font = Content.Load<SpriteFont>("File");

            _map = new Map(_tilesetTex, _mapData, _tileSize);

            // posiciona player no centro do mapa
            float px = (_mapData.GetLength(1) * _tileSize) / 2f;
            float py = (_mapData.GetLength(0) * _tileSize) / 2f;
            _player = new Player(_playerTex, new Vector2(px, py), _tileSize);
        }

        protected override void Update(GameTime gt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (_state == GameState.Overworld)
            {
                _player.Update(gt);

                // centraliza câmera e limita nos bordas do mapa
                var halfW = _graphics.PreferredBackBufferWidth / (2f * _camera.Zoom);
                var halfH = _graphics.PreferredBackBufferHeight / (2f * _camera.Zoom);
                var cp = _player.Position;
                cp.X = MathHelper.Clamp(cp.X, halfW, _map.Width - halfW);
                cp.Y = MathHelper.Clamp(cp.Y, halfH, _map.Height - halfH);
                _camera.Position = cp;

                // encontro só em relva (2)
                int tx = (int)(_player.Position.X / _tileSize);
                int ty = (int)(_player.Position.Y / _tileSize);
                if (ty >= 0 && ty < _mapData.GetLength(0)
                 && tx >= 0 && tx < _mapData.GetLength(1)
                 && _mapData[ty, tx] == 2
                 && _rng.NextDouble() < 0.005)
                    _state = GameState.Battle;
            }
            else if (_state == GameState.Battle)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    _state = GameState.Overworld;
            }

            base.Update(gt);
        }

        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            if (_state == GameState.Overworld)
            {
                _map.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            if (_state == GameState.Battle)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(
                    _font,
                    "A wild Pokemon appeared!\n(Press SPACE to continue)",
                    new Vector2(100, 100),
                    Color.White
                );
                _spriteBatch.End();
            }

            base.Draw(gt);
        }
    }
}

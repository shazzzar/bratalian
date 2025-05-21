using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bratalian
{
    public class Camera2D
    {
        private Viewport _vp;
        public Vector2 Position;
        public float Zoom = 1f;

        public Camera2D(Viewport vp)
        {
            _vp = vp;
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                Matrix.CreateScale(Zoom, Zoom, 1f) *
                Matrix.CreateTranslation(new Vector3(_vp.Width / 2f, _vp.Height / 2f, 0f));
        }
    }
}

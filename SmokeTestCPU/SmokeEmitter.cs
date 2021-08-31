using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestGame
{
    public class SmokeEmitter
    {
        private readonly int N;
        private Smoke _smoke;

        private int _side;

        private Vector2 _position;
        private Vector2 _speed;
        private const float Acceleration = 0.5f;
        private const float MaxSpeed = 1.5f;
        private const float SpeedDecay = 0.2f;
        private const float FadeRate = 0.01f;
        private const int DensityAmount = 10;

        public float X
        {
            get => _position.X;
            set => _position.X = value;
        }

        public float Y
        {
            get => _position.Y;
            set => _position.X = value;
        }

        public SmokeEmitter(int n)
        {
            _side = 0;
            N = n;
            _smoke = new Smoke(n, 1, 0.000001f, 0, 4);
            _position = new Vector2(0, 0);
            _speed = new Vector2(0, 0);
        }

        public void Update()
        {
            UpdatePosition();
            _smoke.AddDensity((int)X, (int)Y, DensityAmount);
            _smoke.Step();
            _smoke.Fade(FadeRate);
        }

        public Color[] ToColors()
        {
            return _smoke.ToColors();
        }

        private void UpdatePosition()
        {
            _position.X = Math.Clamp(_position.X + _speed.X, 1, N - 2);
            _position.Y = Math.Clamp(_position.Y + _speed.Y, 1, N - 2);
            _speed.X = _speed.X < 0 ? Math.Min(_speed.X + SpeedDecay, 0) : Math.Max(_speed.X - SpeedDecay, 0);
            _speed.Y = _speed.Y < 0 ? Math.Min(_speed.Y + SpeedDecay, 0) : Math.Max(_speed.Y - SpeedDecay, 0);
        }

        public void HandleInput(KeyboardState keyboardState)
        {
            bool noKeys = true;

            if (keyboardState.IsKeyDown(Keys.D))
            {
                _speed.X = Math.Min(_speed.X + Acceleration,  MaxSpeed + SpeedDecay);
                _smoke.AddVelocity((int)X, (int)Y, -0.1f, 0);
                noKeys = false;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                _speed.X = Math.Max(_speed.X - Acceleration, -(MaxSpeed + SpeedDecay));
                _smoke.AddVelocity((int)X, (int)Y, 0.1f, 0);
                noKeys = false;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                _speed.Y = Math.Max(_speed.Y - Acceleration, -(MaxSpeed + SpeedDecay));
                _smoke.AddVelocity((int)X, (int)Y, 0, 0.1f);
                noKeys = false;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                _speed.Y = Math.Min(_speed.Y + Acceleration,  MaxSpeed + SpeedDecay);
                _smoke.AddVelocity((int)X, (int)Y, 0, -0.1f);
                noKeys = false;
            }

            if (noKeys)
            {
                switch (_side)
                {
                    case 0:
                        _smoke.AddVelocity(Math.Min((int)X, N - 1), (int)Y, 0.01f, 0);
                        break;
                    case 1:
                        _smoke.AddVelocity((int)X, Math.Min((int)Y, N - 1), 0, 0.01f);
                        break;
                    case 2:
                        _smoke.AddVelocity(Math.Max((int)X, 0), (int)Y, -0.01f, 0);
                        break;
                    case 3:
                        _smoke.AddVelocity((int)X, Math.Max((int)Y, 0), 0, -0.01f);
                        break;
                }

                _side = _side == 3 ? 0 : _side + 1;
            }
        }
    }
}
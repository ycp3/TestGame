using System;
using Color = Microsoft.Xna.Framework.Color;

namespace TestGame
{
    public class Smoke
    {
        private readonly int N;
        private readonly int Size;

        private float[] _s;
        private float[] _density;
        private float[] _vx;
        private float[] _vy;
        private float[] _vx0;
        private float[] _vy0;

        private readonly int _iterations;
        private readonly float _diff;
        private readonly float _visc;
        private readonly float _timestep;

        public Smoke(int n, float timestep, float diff, float visc, int iterations)
        {
            N = n;
            Size = N * N;
            _diff = diff;
            _visc = visc;
            _timestep = timestep;
            _iterations = iterations;

            _s = new float[Size];
            _density = new float[Size];
            _vx = new float[Size];
            _vy = new float[Size];
            _vx0 = new float[Size];
            _vy0 = new float[Size];
        }

        public Color[] ToColors()
        {
            var array = new Color[Size];
            for (int i = 0; i < Size; i++)
            {
                array[i] = new Color(Color.Black, _density[i]);
            }

            return array;
        }

        public void AddDensity(int x, int y, float amount)
        {
            _density[Index(x, y)] = Math.Min(_density[Index(x, y)] + amount, 255);
        }

        public void AddVelocity(int x, int y, float amountX, float amountY)
        {
            int index = Index(x, y);
            _vx[index] += amountX;
            _vy[index] += amountY;
        }

        public void Fade(float amount)
        {
            for (int i = 0; i < Size; i++)
            {
                _density[i] = Math.Max(_density[i] - amount, 0);
            }
        }

        public void Step()
        {
            Diffuse(1, _vx0, _vx, _visc);
            Diffuse(2, _vy0, _vy, _visc);

            Project(_vx0, _vy0, _vx, _vy);

            Advect(1, _vx, _vx0, _vx0, _vy0);
            Advect(2, _vy, _vy0, _vx0, _vy0);

            Project(_vx, _vy, _vx0, _vy0);

            Diffuse(0, _s, _density, _diff);
            Advect(0, _density, _s, _vx, _vy);
        }

        private void Diffuse(int b, float[] x, float[] x0, float diff)
        {
            float a = _timestep * diff * (N - 2) * (N - 2);
            LinSolve(b, x, x0, a, 1 + 4 * a);
        }

        private void LinSolve(int b, float[] x, float[] x0, float a, float c)
        {
            float cRecip = 1.0f / c;
            for (int k = 0; k < _iterations; k++)
            {
                for (int j = 1; j < N - 1; j++)
                    for (int i = 1; i < N - 1; i++)
                    {
                        x[Index(i, j)] = (x0[Index(i, j)] +
                                          a * (x[Index(i - 1, j)] +
                                               x[Index(i + 1, j)] +
                                               x[Index(i, j - 1)] +
                                               x[Index(i - 1, j + 1)])) * cRecip;
                    }

                SetBounds(b, x);
            }
        }

        private void Project(float[] vx, float[] vy, float[] p, float[] div)
        {
            for (int j = 1; j < N - 1; j++)
                for (int i = 1; i < N - 1; i++)
                {
                    div[Index(i, j)] = -0.5f *
                            (vx[Index(i + 1, j)] -
                             vx[Index(i - 1, j)] +
                             vy[Index(i, j + 1)] -
                             vy[Index(i, j - 1)]) / N;
                    p[Index(i, j)] = 0;
                }

            SetBounds(0, div);
            SetBounds(0, p);
            LinSolve(0, p, div, 1, 4);

            for (int j = 1; j < N - 1; j++)
                for (int i = 1; i < N - 1; i++)
                {
                    vx[Index(i, j)] -= 0.5f * (p[Index(i + 1, j)] - p[Index(i - 1, j)]) * N;
                    vy[Index(i, j)] -= 0.5f * (p[Index(i, j + 1)] - p[Index(i, j - 1)]) * N;
                }

            SetBounds(1, vx);
            SetBounds(2, vy);
        }

        private void Advect(int b, float[] d, float[] d0, float[] vx, float[] vy)
        {
            float dtx = _timestep * (N - 2);
            float dty = _timestep * (N - 2);

            float nFloat = N - 2;

            for (int j = 1; j < N - 1; j++)
                for (int i = 1; i < N - 1; i++)
                {
                    float tmp1 = dtx * vx[Index(i, j)];
                    float tmp2 = dty * vy[Index(i, j)];
                    float x = Math.Clamp(i - tmp1, 0.5f, nFloat - 0.5f);
                    float y = Math.Clamp(j - tmp2, 0.5f, nFloat - 0.5f);
                    int i0 = (int) x;
                    int i1 = i0 + 1;
                    int j0 = (int) y;
                    int j1 = j0 + 1;

                    float s1 = x - i0;
                    float s0 = 1.0f - s1;
                    float t1 = y - j0;
                    float t0 = 1.0f - t1;

                    d[Index(i, j)] = s0 * (t0 * d0[Index(i0, j0)] + t1 * d0[Index(i0, j1)]) +
                                     s1 * (t0 * d0[Index(i1, j0)] + t1 * d0[Index(i1, j1)]);
                }

            SetBounds(b, d);
        }

        private void SetBounds(int b, float[] array)
        {
            for (int i = 1; i < N - 1; i++)
            {
                array[Index(0, i)] = b == 1 ? -array[Index(1, i)] : array[Index(1, i)];
                array[Index(N - 1, i)] = b == 1 ? -array[Index(N - 2, i)] : array[Index(N - 2, i)];
            }

            for (int i = 1; i < N - 1; i++)
            {
                array[Index(i, 0)] = b == 2 ? -array[Index(i, 1)] : array[Index(i, 1)];
                array[Index(i, N - 1)] = b == 2 ? -array[Index(i, N - 2)] : array[Index(i, N - 2)];
            }

            array[Index(0, 0)] = 0.5f * (array[Index(1, 0)] + array[Index(0, 1)]);
            array[Index(0, N - 1)] = 0.5f * (array[Index(1, N - 1)] + array[Index(0, N - 2)]);
            array[Index(N - 1, 0)] = 0.5f * (array[Index(N - 2, 0)] + array[Index(N - 1, 1)]);
            array[Index(N - 1, N - 1)] = 0.5f * (array[Index(N - 2, N - 1)] + array[Index(N - 1, N - 2)]);
        }

        private int Index(int x, int y)
        {
            return x + y * N;
        }
    }
}
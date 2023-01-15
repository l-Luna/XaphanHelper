using System;
using System.Linq;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class ByteArray2D
    {
        public int Width { get; }
        public int Height { get; }

        private readonly byte[] data;
        public byte[] Data => data;

        public byte this[int x, int y]
        {
            get => data[x + y * Width];
            set => data[x + y * Width] = value;
        }

        public bool TryGet(int x, int y, out byte value)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                value = this[x, y];
                return true;
            }

            value = default;
            return false;
        }

        public ByteArray2D(int width, int height)
        {
            Width = width;
            Height = height;
            data = new byte[width * height];
        }

        public ByteArray2D(int width, int height, byte defaultValue)
        {
            Width = width;
            Height = height;
            data = Enumerable.Repeat(defaultValue, width * height).ToArray();
        }

        public void Min(ByteArray2D other, int dx, int dy)
        {
            int minX = Math.Max(dx, 0);
            int minY = Math.Max(dy, 0);
            int maxX = Math.Min(dx + other.Width, Width);
            int maxY = Math.Min(dy + other.Height, Height);

            for (int y = minY, sy = minY - dy; y < maxY; y++, sy++)
            {
                for (int x = minX, sx = minX - dx; x < maxX; x++, sx++)
                {
                    var dest = data[x + y * Width];
                    var src = other.data[sx + sy * other.Width];
                    data[x + y * Width] = Math.Min(dest, src);
                }
            }
        }
    }
}

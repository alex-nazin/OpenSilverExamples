using System;
using System.Windows.Media;

namespace MigrationExamples.OpenSilver.Pages.WriteableBitmapWorkaround
{
    internal static class BitmapPixelsGenerator
    {
        internal static int[] GeneratePixels(int size, Color[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentNullException(nameof(colors));
            }

            var pixels = new int[size * size];

            int count = colors.Length;
            int step = size / colors.Length;

            for (int row = 0; row < count; ++row)
            {
                for (int col = 0; col < count; ++col)
                {
                    int fromX = col * step;
                    int fromY = row * step;
                    int toX = col != count - 1 ? (col + 1) * step : size;
                    int toY = row != count - 1 ? (row + 1) * step : size;

                    Color color = colors[(row + col) % colors.Length];
                    int value = color.B | (color.G << 8) | (color.R << 16) | (color.A << 24);

                    for (int y = fromY; y < toY; ++y)
                    {
                        for (int x = fromX; x < toX; ++x)
                        {
                            int index = x + y * size;
                            pixels[index] = value;
                        }
                    }
                }
            }

            return pixels;
        }
    }
}

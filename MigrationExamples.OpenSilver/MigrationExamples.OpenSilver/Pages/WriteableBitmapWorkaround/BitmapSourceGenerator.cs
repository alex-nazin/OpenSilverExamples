using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace MigrationExamples.OpenSilver.Pages.WriteableBitmapWorkaround
{
    internal static class BitmapSourceGenerator
    {
        internal static BitmapSource GetBitmapSource(int width, int height, int[] pixels)
        {
            if (pixels == null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (pixels.Length != width * height)
            {
                throw new ArgumentException(nameof(pixels));
            }

            // 14 - size of BITMAPFILEHEADER structure
            // 108 - size of BITMAPV4HEADER structure
            // _width * _height * 4 - size of ARGB pixels
            int fileSize = 14 + 108 + width * height * 4;
            byte[] bmpFileHeader = new byte[] {
                    0, 0,     /// signature
                    0, 0, 0, 0, /// image file size in bytes
                    0, 0, 0, 0, /// reserved
                    0, 0, 0, 0, /// start of pixel array
                };
            bmpFileHeader[0] = (byte)'B';
            bmpFileHeader[1] = (byte)'M';
            bmpFileHeader[2] = (byte)((fileSize) & 0xFF);
            bmpFileHeader[3] = (byte)((fileSize >> 8) & 0xFF);
            bmpFileHeader[4] = (byte)((fileSize >> 16) & 0xFF);
            bmpFileHeader[5] = (byte)((fileSize >> 24) & 0xFF);
            bmpFileHeader[10] = 14 + 108;

            byte[] bmpInfoHeader = new byte[108]
            {
                    0, 0, 0, 0, /// header size
                    0, 0, 0, 0, /// image width
                    0, 0, 0, 0, /// image height
                    1, 0,       /// number of color planes = 1
                    32, 0,       /// bits per pixel = 32
                    3, 0, 0, 0, /// compression = BI_BITFIELDS
                    0, 0, 0, 0, /// image size
                    0, 0, 0, 0, /// horizontal resolution
                    0, 0, 0, 0, /// vertical resolution
                    0, 0, 0, 0, /// colors in color table
                    0, 0, 0, 0, /// important color count
                    0, 0, 0xFF, 0, /// red mask
                    0, 0xFF, 0, 0, /// green mask
                    0xFF, 0, 0, 0, /// blue mask
                    0, 0, 0, 0xFF, /// alpha mask
                    0x20, 0x6E, 0x69, 0x57, /// color space
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /// xyz for red endpoint
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /// xyz for green endpoint
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /// xyz for blue endpoint
                    0, 0, 0, 0, /// red gamma
                    0, 0, 0, 0, /// green gamma
                    0, 0, 0, 0, /// blue gamma
            };

            bmpInfoHeader[0] = 108; // size of structure
            bmpInfoHeader[4] = (byte)((width) & 0xFF);
            bmpInfoHeader[5] = (byte)((width >> 8) & 0xFF);
            bmpInfoHeader[6] = (byte)((width >> 16) & 0xFF);
            bmpInfoHeader[7] = (byte)((width >> 24) & 0xFF);
            bmpInfoHeader[8] = (byte)((height) & 0xFF);
            bmpInfoHeader[9] = (byte)((height >> 8) & 0xFF);
            bmpInfoHeader[10] = (byte)((height >> 16) & 0xFF);
            bmpInfoHeader[11] = (byte)((height >> 24) & 0xFF);

            using (MemoryStream ms = new MemoryStream(fileSize))
            {
                ms.Write(bmpFileHeader, 0, bmpFileHeader.Length);
                ms.Write(bmpInfoHeader, 0, bmpInfoHeader.Length);
                for (int y = height - 1; y >= 0; --y)
                {
                    for (int x =0; x < width; ++x)
                    {
                        int pixel = pixels[y * height + x];
                        ms.WriteByte((byte)((pixel) & 0xFF));
                        ms.WriteByte((byte)((pixel >> 8) & 0xFF));
                        ms.WriteByte((byte)((pixel >> 16) & 0xFF));
                        ms.WriteByte((byte)((pixel >> 24) & 0xFF));
                    }
                }

                byte[] bytes = ms.ToArray();
                string base64String = Convert.ToBase64String(bytes);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(string.Format("data:image/bmp;base64, {0}", base64String));
                return bitmapImage;
            }
        }
    }
}

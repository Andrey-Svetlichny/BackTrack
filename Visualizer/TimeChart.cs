using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Visualizer
{
    public class TimeChart
    {
        public int Width { get; }
        public int Height { get; }
        public WriteableBitmap Bitmap { get; }

        public TimeChart(int width, int height)
        {
            Width = width;
            Height = height;
            Bitmap = new WriteableBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Bgr32,
                null);
        }

        public void DrawPixel(int x, int y)
        {
            // Reserve the back buffer for updates.
            Bitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer.
                int pBackBuffer = (int)Bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * Bitmap.BackBufferStride;
                pBackBuffer += x * 4;

                // Compute the pixel's color.
                int colorData = 255 << 16; // R
                colorData |= 128 << 8;   // G
                colorData |= 255 << 0;   // B

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = colorData;
            }

            // Specify the area of the bitmap that changed.
            Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));

            // Release the back buffer and make it available for display.
            Bitmap.Unlock();
        }

        public void DrawLineVertical(int x, int y1, int y2, int color)
        {
            Bitmap.Lock();
            unsafe
            {
                // Get a pointer to the back buffer.
                int pBackBuffer = (int)Bitmap.BackBuffer;

                for (int y = y1; y < y2; y++)
                {
                    // pixel address
                    int p = pBackBuffer + y * Bitmap.BackBufferStride + x * 4;
                    *((int*)p) = color;
                }
            }
            Bitmap.AddDirtyRect(new Int32Rect(x, y1, 1, y2 - y1));
            Bitmap.Unlock();
        }

        public void DrawLineHorizontal(int x1, int x2, int y, int color)
        {
            Bitmap.Lock();
            unsafe
            {
                // Get a pointer to the back buffer.
                int pBackBuffer = (int)Bitmap.BackBuffer;

                for (int x = x1; x < x2; x++)
                {
                    // pixel address
                    int p = pBackBuffer + y * Bitmap.BackBufferStride + x * 4;
                    *((int*)p) = color;
                }
            }
            Bitmap.AddDirtyRect(new Int32Rect(x1, y, x2-x1, 1));
            Bitmap.Unlock();
        }

        public void DrawRect(int x1, int x2, int y1, int y2, int color)
        {
            Bitmap.Lock();
            unsafe
            {
                // Get a pointer to the back buffer.
                int pBackBuffer = (int)Bitmap.BackBuffer;

                for (int x = x1; x < x2; x++)
                for (int y = y1; y < y2; y++)
                {
                    // pixel address
                    int p = pBackBuffer + y * Bitmap.BackBufferStride + x * 4;
                    *((int*)p) = color;
                }
            }
            Bitmap.AddDirtyRect(new Int32Rect(x1, y1, x2 - x1, y2 - y1));
            Bitmap.Unlock();
        }

    }
}

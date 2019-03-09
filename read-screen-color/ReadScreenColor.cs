using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenColor
{
    public class ReadScreenColor
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        public delegate void OnUpdate(long ElapsedMilliseconds);
        public static event OnUpdate onUpdate;

        public static float PerSecond = 5f;

        private Stopwatch stopwatch;
        public static Size ScreenSize = new Size(1920, 1080);
        public static int PartSize = 100;
        public static Size AverageSize = new Size(16 * 120, 9 * 120);
        //private Bitmap[,] screenPixels;
        private Bitmap screen = new Bitmap(ScreenSize.Width, ScreenSize.Height, PixelFormat.Format32bppArgb);
        private Bitmap screenTop = new Bitmap(ScreenSize.Width, PartSize, PixelFormat.Format32bppArgb);
        private Bitmap screenBot = new Bitmap(ScreenSize.Width, PartSize, PixelFormat.Format32bppArgb);
        private Bitmap screenLeft = new Bitmap(PartSize, ScreenSize.Height - (PartSize * 2), PixelFormat.Format32bppArgb);
        private Bitmap screenRight = new Bitmap(PartSize, ScreenSize.Height - (PartSize * 2), PixelFormat.Format32bppArgb);
        private Bitmap average = new Bitmap(AverageSize.Width, AverageSize.Height, PixelFormat.Format32bppArgb);

        public Bitmap Result
        {
            get { return this.average; }
        }

        public ReadScreenColor(/*Size grid*/)
        {
            //TODO make sure not losing pixel on sides

            //this.screenPixels = new Bitmap[grid.Width, grid.Height];

            //var width = screenSize.Width / grid.Width;
            //var height = screenSize.Height / grid.Height;

            //for (int i = 0; i < grid.Width; i++)
            //{
            //    for (int j = 0; j < grid.Height; j++)
            //    {
            //        this.screenPixels[i, j] = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //    }
            //}
        }

        public UpdateResult Update(bool withStopWatch = false)
        {
            var result = new UpdateResult();
            if (withStopWatch)
            {
                this.stopwatch = new Stopwatch();
                this.stopwatch.Start();
            }

            this.checkScreen();

            if (withStopWatch)
            {
                this.stopwatch.Stop();
                result.elapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
                Debug.WriteLine("UpdateTime:" + result.elapsedMilliseconds);
                onUpdate?.Invoke(result.elapsedMilliseconds);
            }

            return result;
        }

        private void checkScreen()
        {
            this.CopyFromScreen();

            this.GraphicsDrawImage();
        }

        #region read from screen
        public void ReadScreen()
        {
            using (Graphics gdest = Graphics.FromImage(this.screen))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(
                        hDC, 0, 0, ScreenSize.Width, ScreenSize.Height,
                        hSrcDC, 0, 0, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
        }
        public void CopyFromScreen()
        {
            using (Graphics gsrc = Graphics.FromImage(this.screen))
            {
                gsrc.CopyFromScreen(0, 0, 0, 0, ScreenSize, CopyPixelOperation.SourceCopy);
            }
        }
        public void CopyFromScreenParts()
        {
            using (Graphics gsrc = Graphics.FromImage(this.screen))
            {
                copyPart(gsrc, 0, 0, this.screen.Width, PartSize);
                copyPart(gsrc, 0, this.screen.Height - PartSize, this.screen.Width, PartSize);
                copyPart(gsrc, 0, PartSize, PartSize, this.screen.Height);
                copyPart(gsrc, this.screen.Width - PartSize, PartSize, PartSize, this.screen.Height);
            }
        }

        private static void copyPart(Graphics gsrc, int sourceX, int sourceY, int targetWidth, int targetHeight)
        {
            gsrc.CopyFromScreen(sourceX, sourceY, sourceX, sourceY, new Size(targetWidth, targetHeight));
        }
        public void ReadScreenParts()
        {
            using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hSrcDC = gsrc.GetHdc();

                readScreenPart(hSrcDC, this.screenTop, 0, 0, this.screen.Width, PartSize);
                readScreenPart(hSrcDC, this.screenBot, 0, this.screen.Height - PartSize, this.screen.Width, PartSize);
                readScreenPart(hSrcDC, this.screenLeft, 0, PartSize, PartSize, this.screen.Height);
                readScreenPart(hSrcDC, this.screenRight, this.screen.Width - PartSize, PartSize, PartSize, this.screen.Height);

                gsrc.ReleaseHdc();
            }
        }

        private static void readScreenPart(IntPtr hSrcDC, Bitmap target, int sourceX, int sourceY, int targetWidth, int targetHeight)
        {
            using (Graphics gdest = Graphics.FromImage(target))
            {
                IntPtr hDC = gdest.GetHdc();
                int retval = BitBlt(
                    hDC, 0, 0, targetWidth, targetHeight,
                    hSrcDC, sourceX, sourceY, (int)CopyPixelOperation.SourceCopy);

                gdest.ReleaseHdc();
            }
        }
        public void ReadScreenPartsv2()
        {
            using (Graphics gdest = Graphics.FromImage(this.screen))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();

                    blitPart(hSrcDC, hDC, 0, 0, this.screen.Width, PartSize);
                    blitPart(hSrcDC, hDC, 0, this.screen.Height - PartSize, this.screen.Width, PartSize);
                    blitPart(hSrcDC, hDC, 0, PartSize, PartSize, this.screen.Height);
                    blitPart(hSrcDC, hDC, this.screen.Width - PartSize, PartSize, PartSize, this.screen.Height);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
        }

        private static void blitPart(IntPtr hSrcDC, IntPtr hDC, int sourceX, int sourceY, int targetWidth, int targetHeight)
        {
            int retval = BitBlt(
                                    hDC, sourceX, sourceY, targetWidth, targetHeight,
                                    hSrcDC, sourceX, sourceY, (int)CopyPixelOperation.SourceCopy);
        }
        #endregion

        #region process image
        public Color[] GraphicsDrawImage()
        {
            using (Graphics gavg = Graphics.FromImage(this.average))
            {
                gavg.DrawImage(this.screen, 0, 0, AverageSize.Width, 1);
            }

            var colors = new Color[AverageSize.Width];
            for (int i = 0; i < AverageSize.Width; i++)
            {
                colors[i] = this.average.GetPixel(i, 0);
            }
            return colors;
        }
        public Color[] LoopSumAverage()
        {
            var colorSums = new ColorSum[AverageSize.Width];

            for (int x = 0; x < ScreenSize.Width; x++)
            {
                for (int y = 0; y < ScreenSize.Height; y++)
                {
                    int i = x * AverageSize.Width / ScreenSize.Width;

                    // x * 1920 / 30

                    colorSums[i] += this.screen.GetPixel(x, y);
                }
            }

            var colors = new Color[AverageSize.Width];
            for (int i = 0; i < AverageSize.Width; i++)
            {
                colors[i] = colorSums[i].Average();
            }

            return colors;
        }


        public Color[] LoopSumAverageUnsafe()
        {
            return processBitMap(this.screen);
        }

        private static Color[] processBitMap(Bitmap bitmap)
        {
            var colorSums = new ColorSum[AverageSize.Width];

#if USE_UNSAFE
            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];

                        int i = x / bytesPerPixel * AverageSize.Width / ScreenSize.Width;

                        colorSums[i].Add(red, green, blue);
                    }
                }
                bitmap.UnlockBits(bitmapData);
            }
#endif

            var colors = new Color[AverageSize.Width];
            for (int i = 0; i < AverageSize.Width; i++)
            {
                colors[i] = colorSums[i].Average();
            }

            return colors;
        }
        private struct ColorSum
        {
            public int r;
            public int g;
            public int b;

            public int count;

            public ColorSum(int r, int g, int b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.count = 1;
            }
            public ColorSum(int r, int g, int b, int count)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.count = count;
            }

            public void Add(int r, int g, int b)
            {
                this.r += r;
                this.g += g;
                this.b += b;
                this.count++;
            }

            public static ColorSum operator +(ColorSum left, ColorSum right)
            {
                return new ColorSum(
                    left.r + right.r,
                    left.g + right.g,
                    left.b + right.b,
                    left.count + right.count
                    );
            }
            public static ColorSum operator +(ColorSum sum, Color color)
            {
                return new ColorSum(
                    sum.r + color.R,
                    sum.g + color.G,
                    sum.b + color.B,
                    sum.count + 1
                    );
            }

            public Color Average()
            {
                if (this.count == 0)
                {
                    return Color.Black;
                }
                else
                {
                    return Color.FromArgb(
                    this.r / this.count,
                    this.g / this.count,
                    this.b / this.count
                    );
                }
            }
        }
        #endregion
    }
    public struct UpdateResult
    {
        public long elapsedMilliseconds;
    }
}

using ArgbConverter.DataModels;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArgbConverter.ProcessingClasses
{
    public class ArgbProcessor
    {
        private string filePath = "";

        public ArgbProcessor(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new Exception("ścieżka pliku jest pusta!");

            if (!File.Exists(filePath))
                throw new Exception($"plik [{filePath}] nie istnieje!");

            this.filePath = filePath;
        }

        #region Task startery

        public Task<PacketObjMsg> CreateOpacityMapAndNonTransparentImageTaskStart(string destinationPath)
        {
            var t = new Task<PacketObjMsg>
            (
                () => CreateOpacityMapAndNonTransparentImage(destinationPath)
            );
            t.Start();
            return t;
        }

        public static Task<PacketObjMsg> CreateGrayScaleImageTaskStart(Bitmap sourceBitmap)
        {
            var t = new Task<PacketObjMsg>
            (
                () => CreateGrayScaleImage(sourceBitmap)
            );
            t.Start();
            return t;
        }

        #endregion

        #region Metody tworzące pakiety zwrotne

        public static PacketObjMsg CreateGrayScaleImage(Bitmap sourceBitmap)
        {
            PacketObjMsg packet = new PacketObjMsg();
            Bitmap resultBitmap = (Bitmap)sourceBitmap.Clone();
            string msg = "";

            try
            {
                int bmpWidth = sourceBitmap.Width;
                int bmpHeight = sourceBitmap.Height;

                Color pixelColor;

                for (int y = 0; y < bmpHeight; y++)
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        pixelColor = sourceBitmap.GetPixel(x, y);
                        int alpha = pixelColor.A;
                        int red = pixelColor.R;
                        int green = pixelColor.G;
                        int blue = pixelColor.B;
                        int avg = (red + green + blue) / 3;
                        decimal percentAlpha = DataProcessor.CalculatePercent(255M, Convert.ToDecimal(alpha));
                        int avgAlpha = Convert.ToInt32(avg * (percentAlpha / 100));
                        //Console.WriteLine($"% [{percentAlpha}] avg.A [{avgAlpha}]");
                        resultBitmap.SetPixel(x, y, Color.FromArgb(255, avgAlpha, avgAlpha, avgAlpha));
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Błąd tworzenia mapy w skali szarości: {exception.Message}";
            }

            packet.Obj = resultBitmap;
            packet.Msg = msg;
            return packet;
        }

        public PacketObjMsg CreateOpacityMapAndNonTransparentImage(string destinationPath)
        {
            PacketObjMsg packet = new PacketObjMsg();
            bool success = false;
            string msg = "";

            try
            {
                //Thread.Sleep(1000);
                
                #region Wstępne

                if (String.IsNullOrEmpty(this.filePath))
                    throw new Exception("ścieżka pliku jest pusta!");

                if (!File.Exists(this.filePath))
                    throw new Exception($"plik nie istnieje!");

                string extension = Path.GetExtension(this.filePath);
                if (extension.ToLower() != ".png" && extension.ToLower() != "png")
                    throw new Exception($"plik ma nieprawidłowy format, powinien być .png! Format [{extension}].");

                if (!Directory.Exists(destinationPath))
                    throw new Exception($"ścieżka docelowa [{destinationPath}] nie istnieje!");

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.filePath);

                #endregion

                #region Przetwarzanie

                Bitmap bmp = new Bitmap(this.filePath);
                Bitmap bmpOpacity;
                Bitmap bmpNonTransparent;

                object opacityBmpObj = bmp.Clone();
                object nonTransparentBmpObj = bmp.Clone();

                if (opacityBmpObj.GetType() == typeof(Bitmap) || nonTransparentBmpObj.GetType() == typeof(Bitmap))
                {
                    bmpOpacity = (Bitmap)opacityBmpObj;
                    bmpNonTransparent = (Bitmap)nonTransparentBmpObj;
                }
                else
                {
                    throw new Exception($"conajmniej jedna z kopii bitmapy ma nieprawidłowy typ, powinien być Bitmap. Kopia 1 [{opacityBmpObj.GetType().ToString()}] kopia 2 [{nonTransparentBmpObj.GetType().ToString()}]");
                }

                int bmpWidth = bmp.Width;
                int bmpHeight = bmp.Height;

                Color pixelColor;

                for (int y = 0; y < bmpHeight; y++)
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        pixelColor = bmp.GetPixel(x, y);
                        int alpha = pixelColor.A;
                        int red = pixelColor.R;
                        int green = pixelColor.G;
                        int blue = pixelColor.B;
                        int avg = (red + green + blue) / 3;

                        bmpOpacity.SetPixel(x, y, Color.FromArgb(255, alpha, alpha, alpha));
                        bmpNonTransparent.SetPixel(x, y, Color.FromArgb(255, red, green, blue));
                    }
                }

                #endregion

                #region Zapis

                string opacityMapPath = $"{destinationPath}\\{fileNameWithoutExtension}_opacity.png";
                string nonTransparentImagePath = $"{destinationPath}\\{fileNameWithoutExtension}_nonTransparent.png";
                bmpOpacity.Save(opacityMapPath);
                bmpNonTransparent.Save(nonTransparentImagePath);

                #endregion

                success = true;
            }
            catch (Exception exception)
            {
                msg = $"Błąd tworzenia mapy przezroczystości dla obrazu [{this.filePath}]: {exception.Message}";
            }

            packet.Obj = success;
            packet.Msg = msg;
            return packet;
        }

        #endregion
    }
}

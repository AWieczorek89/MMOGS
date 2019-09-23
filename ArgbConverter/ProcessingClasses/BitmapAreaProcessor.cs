using ArgbConverter.AdvancedTools;
using ArgbConverter.DataModels;
using ArgbConverter.ProcessingClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArgbConverter.ProcessingClasses
{
    public class BitmapAreaProcessor : IDisposable
    {
        public enum AreaAxis
        {
            X,
            Y,
            Both,
            None
        }

        public HairTexGeneratorForm parentForm = null;
        public Bitmap SourceBitmap { get; private set; } = null;
        public PixelAreaInfo[,] PixelsInfoArr { get; private set; } = null;
        public int PixelAreaCount { get; private set; } = 0;

        private static readonly Random getRandom = new Random();

        public BitmapAreaProcessor (HairTexGeneratorForm parentForm)
        {
            this.parentForm = parentForm;
        }
        
        #region Task startery

        /// <summary>
        /// Inicjowanie danych do przetworzenia w celu weryfikacji rozkładu stref
        /// </summary>
        public Task<PacketObjMsg> LoadBitmapTaskStart(Bitmap bitmap)
        {
            var t = new Task<PacketObjMsg>(() => LoadBitmap(bitmap));
            t.Start();
            return t;
        }

        /// <summary>
        /// Oznaczanie stref wpływu i stref martwych
        /// </summary>
        public Task<PacketObjMsg> GenerateAreasTaskStart(int toleranceR, int toleranceG, int toleranceB)
        {
            var t = new Task<PacketObjMsg>(() => GenerateAreas(toleranceR, toleranceG, toleranceB));
            t.Start();
            return t;
        }

        /// <summary>
        /// Przypisywanie poszczególnym pikselom ID strefy, do której przynależy
        /// </summary>
        public Task<PacketObjMsg> GenerateAreaIndexesTaskStart()
        {
            var t = new Task<PacketObjMsg>(() => GenerateAreaIndexes());
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GenerateAreaPreviewImageTaskStart()
        {
            var t = new Task<PacketObjMsg>(() => GenerateAreaPreviewImage());
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GenerateAreaSettingsTaskStart()
        {
            var t = new Task<PacketObjMsg>(() => GenerateAreaSettings());
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GetAreaBoundingsTaskStart(int areaId)
        {
            var t = new Task<PacketObjMsg>(() => GetAreaBoundings(areaId));
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GenerateTransparentBitmapTaskStart(int width, int height)
        {
            var t = new Task<PacketObjMsg>(() => GenerateTransparentBitmap(width, height));
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GetAreaLineRangeTaskStart(AreaAxis axis, int pointOnAxis, int areaId)
        {
            var t = new Task<PacketObjMsg>(() => GetAreaLineRange(axis, pointOnAxis, areaId));
            t.Start();
            return t;
        }

        public Task<PacketObjMsg> GenerateHairTexGlossinessAndOpacityTaskStart
        (
            AreaBoundings areaBoundings, 
            Bitmap textureBitmap, 
            Color backgroundColor, 
            bool glossEnabled, 
            bool opacityEnabled,
            //double[] marginMpls,
            //AreaAxis marginAxis
            bool boundsChecker
        )
        {
            var t = new Task<PacketObjMsg>
            (
                () => GenerateHairTexGlossinessAndOpacity
                (
                    areaBoundings, 
                    textureBitmap, 
                    backgroundColor, 
                    glossEnabled, 
                    opacityEnabled,
                    //marginMpls,
                    //marginAxis
                    boundsChecker
                )
            );
            t.Start();
            return t; 
        }

        #endregion

        #region Metody tworzące pakiety zwrotne
        
        private PacketObjMsg GenerateHairTexGlossinessAndOpacity
        (
            AreaBoundings areaBoundings, 
            Bitmap textureBitmap, 
            Color backgroundColor, 
            bool glossEnabled, 
            bool opacityEnabled,
            //double[] marginMpls,
            //AreaAxis marginAxis
            bool boundsChecker
        )
        {
            PacketObjMsg packet = new PacketObjMsg();
            packet.Obj = textureBitmap;
            string msg = "";

            if (!glossEnabled && !opacityEnabled)
                return packet;
            
            try
            {
                int areaMinX = areaBoundings.BoundsX.X;
                int areaMaxX = areaBoundings.BoundsX.Y;
                int areaMinY = areaBoundings.BoundsY.X;
                int areaMaxY = areaBoundings.BoundsY.Y;

                #region Poszerzanie marginesów - ANULOWANE

                //double marginMpl;
                //int rangeX;
                //int rangeY;

                //for (int i = 0; i < marginMpls.Length; i++)
                //{
                //    marginMpl = marginMpls[i];
                //    rangeX = areaMaxX - areaMinX;
                //    rangeY = areaMaxY - areaMinY;
                    
                //    //POSZERZANIE O MARGINESY
                //    switch (marginAxis)
                //    {
                //        case AreaAxis.Both:
                //            areaMinX -= Convert.ToInt32(rangeX * marginMpl);
                //            areaMaxX += Convert.ToInt32(rangeX * marginMpl);
                //            areaMinY -= Convert.ToInt32(rangeY * marginMpl);
                //            areaMaxY += Convert.ToInt32(rangeY * marginMpl);
                //            break;
                //        case AreaAxis.X:
                //            areaMinX -= Convert.ToInt32(rangeX * marginMpl);
                //            areaMaxX += Convert.ToInt32(rangeX * marginMpl);
                //            break;
                //        case AreaAxis.Y:
                //            areaMinY -= Convert.ToInt32(rangeY * marginMpl);
                //            areaMaxY += Convert.ToInt32(rangeY * marginMpl);
                //            break;
                //    }
                //}
                
                //areaMinX = (areaMinX < 0 ? 0 : areaMinX);
                //areaMinY = (areaMinY < 0 ? 0 : areaMinY);
                //areaMaxX = (areaMaxX > (textureBitmap.Width - 1) ? (textureBitmap.Width - 1) : areaMaxX);
                //areaMaxY = (areaMaxY > (textureBitmap.Height - 1) ? (textureBitmap.Height - 1) : areaMaxY);

                #endregion

                Color sourcePixelColor;
                Color texturePixelColor;

                int srcAvg;
                double mpl;
                int resultR;
                int resultG;
                int resultB;
                int resultA;

                for (int y = areaMinY; y <= areaMaxY; y++)
                {
                    for (int x = areaMinX; x <= areaMaxX; x++)
                    {
                        sourcePixelColor = this.SourceBitmap.GetPixel(x, y);
                        texturePixelColor = textureBitmap.GetPixel(x, y);
                        resultR = texturePixelColor.R;
                        resultG = texturePixelColor.G;
                        resultB = texturePixelColor.B;
                        resultA = texturePixelColor.A;

                        srcAvg = (sourcePixelColor.R + sourcePixelColor.G + sourcePixelColor.B) / 3;
                        mpl = DataProcessor.CalculatePercent(255.00, (double)srcAvg) / 100.00;

                        if (glossEnabled)
                        {
                            resultR = Convert.ToInt32(DataProcessor.Lerp(backgroundColor.R, texturePixelColor.R, mpl));
                            resultG = Convert.ToInt32(DataProcessor.Lerp(backgroundColor.G, texturePixelColor.G, mpl));
                            resultB = Convert.ToInt32(DataProcessor.Lerp(backgroundColor.B, texturePixelColor.B, mpl));
                        }

                        if (opacityEnabled)
                        {
                            resultA = Convert.ToInt32(DataProcessor.Lerp(0, texturePixelColor.A, mpl));
                        }

                        textureBitmap.SetPixel(x, y, Color.FromArgb(resultA, resultR, resultG, resultB));
                    }
                }

                if(boundsChecker)
                {
                    #region Sprawdzanie marginesów - ramka
                    //SPRAWDZANIE MARGINESÓW
                    using (var gr = Graphics.FromImage(textureBitmap))
                    {
                        Pen p = new Pen(Color.Red);
                        Point upperLeftPoint = new Point(areaMinX, areaMinY);
                        Point upperRightPoint = new Point(areaMaxX, areaMinY);
                        Point lowerLeftPoint = new Point(areaMinX, areaMaxY);
                        Point lowerRightPoint = new Point(areaMaxX, areaMaxY);

                        gr.DrawLine(p, upperLeftPoint, upperRightPoint);
                        gr.DrawLine(p, lowerLeftPoint, lowerRightPoint);
                        gr.DrawLine(p, upperLeftPoint, lowerLeftPoint);
                        gr.DrawLine(p, upperRightPoint, lowerRightPoint);
                    }

                    #endregion
                }
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd generowania przezroczystości i połysku tekstury: {exception.Message}";
            }

            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg GetAreaLineRange(AreaAxis axis, int pointOnAxis, int areaId)
        {
            PacketObjMsg packet = new PacketObjMsg();
            AreaLineRange range = new AreaLineRange();
            string msg = "";

            range.AreaId = areaId;

            try
            {
                PixelAreaInfo pixelInfo;
                int min = -1;
                int max = -1;

                if (axis == AreaAxis.X)
                {
                    for (int y = 0; y < this.PixelsInfoArr.GetLength(1); y++)
                    {
                        pixelInfo = this.PixelsInfoArr[pointOnAxis, y];

                        if (pixelInfo.AreaId == areaId && !pixelInfo.IsDeadZone)
                        {
                            if (min == -1)
                            {
                                min = y;
                                max = y;
                            }
                            else
                            if (y > max)
                            {
                                max = y;
                            }
                        }
                    }
                }
                else
                if (axis == AreaAxis.Y)
                {
                    for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                    {
                        pixelInfo = this.PixelsInfoArr[x, pointOnAxis];

                        if (pixelInfo.AreaId == areaId && !pixelInfo.IsDeadZone)
                        {
                            if (min == -1)
                            {
                                min = x;
                                max = x;
                            }
                            else
                            if (x > max)
                            {
                                max = x;
                            }
                        }
                    }
                }

                range.RangePointFrom = min;
                range.RangePointTo = max;
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd wyodrębniania zakresu liniowego strefy [{areaId}] na osi [{axis.ToString()}] w punkcie [{pointOnAxis}]: {exception.Message}";
            }

            packet.Obj = range;
            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg GenerateTransparentBitmap(int width, int height)
        {
            PacketObjMsg packet = new PacketObjMsg();
            Bitmap bmp = new Bitmap(width, height);
            string msg = "";

            try
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255));
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd generowania przezroczystej bitmapy [{width} x {height}]: {exception.Message}";
            }

            packet.Obj = bmp;
            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg GetAreaBoundings(int areaId)
        {
            PacketObjMsg packet = new PacketObjMsg();
            AreaBoundings boundings = new AreaBoundings(areaId);
            string msg = "";

            try
            {
                if (this.PixelsInfoArr == null)
                    throw new Exception("tablica informacji o pikselach jest NULL!");

                int minX = -1;
                int maxX = -1;
                int minY = -1;
                int maxY = -1;
                PixelAreaInfo pixelInfo;

                for (int y = 0; y < this.PixelsInfoArr.GetLength(1); y++)
                {
                    for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                    {
                        pixelInfo = this.PixelsInfoArr[x, y];

                        if (pixelInfo.AreaId != areaId)
                            continue;

                        if (minX == -1 || x < minX)
                            minX = x;

                        if (maxX == -1 || x > maxX)
                            maxX = x;

                        if (minY == -1 || y < minY)
                            minY = y;

                        if (maxY == -1 || y > maxY)
                            maxY = y;
                    }
                }

                if (minX == -1 || maxX == -1 || minY == -1 || maxY == -1)
                    throw new Exception($"odczytano błędne dane ograniczeń: X [{minX},{maxX}] Y [{minY},{maxY}]");

                boundings.BoundsX = new Point(minX, maxX);
                boundings.BoundsY = new Point(minY, maxY);
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd generowania obiektu danych ograniczeń strefy [{areaId}]: {exception.Message}";
            }
            
            packet.Obj = boundings;
            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg LoadBitmap(Bitmap bitmap)
        {
            PacketObjMsg packet = new PacketObjMsg();
            bool success = false;
            string msg = "";

            try
            {
                if (bitmap == null)
                    throw new Exception("bitmapa jest NULL!");

                int bmpWidth = bitmap.Width;
                int bmpHeight = bitmap.Height;

                if (bmpWidth < 1 || bmpHeight < 1)
                    throw new Exception($"nieprawidłowe wymiary przekazanej bitmapy [{bmpWidth}][{bmpHeight}]!");

                this.PixelAreaCount = 0;
                this.SourceBitmap = bitmap;
                this.PixelsInfoArr = new PixelAreaInfo[bmpWidth, bmpHeight];

                for (int y = 0; y < this.PixelsInfoArr.GetLength(1); y++)
                {
                    for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                    {
                        this.PixelsInfoArr[x, y] = new PixelAreaInfo();
                    }
                }

                success = true;
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd ładowania: {exception.Message}";
            }

            packet.Obj = success;
            packet.Msg = msg;
            return packet;
        }
        
        private PacketObjMsg GenerateAreaSettings()
        {
            PacketObjMsg packet = new PacketObjMsg();
            List<AreaSettings> areaSettingsList = new List<AreaSettings>();
            string msg = "";

            try
            {
                List<int> areaIds = GetAllAreaIds();
                
                foreach (int areaId in areaIds)
                {
                    AreaSettings areaSettings = new AreaSettings(areaId);
                    areaSettingsList.Add(areaSettings);
                }
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd generowania ustawień stref: {exception.Message}";
            }

            packet.Obj = areaSettingsList;
            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg GenerateAreaPreviewImage()
        {
            PacketObjMsg packet = new PacketObjMsg();
            Bitmap previewBitmap = (this.SourceBitmap != null ? (Bitmap)this.SourceBitmap.Clone() : null);
            string msg = "";

            try
            {
                int bmpWidth = this.SourceBitmap.Width;
                int bmpHeight = this.SourceBitmap.Height;

                int areaId = -1;
                bool isDeadZone = true;
                //bool areaExists = false;

                #region Odczyt listy stref
                //ODCZYT ID WSZYSTKICH STREF
                List<int> areaIds = GetAllAreaIds();
                
                #endregion

                #region Oznaczanie stref na bitmapie
                //OZNACZANIE STREF NA BITMAPIE
                Color randomColor;
                Color pixelColor;
                Graphics gr = Graphics.FromImage(previewBitmap);
                Font textFont = new Font("Tahoma", 10);
                Brush brush = Brushes.White;
                
                int avgR = -1;
                int avgG = -1;
                int avgB = -1;

                int textX = -1;
                int textY = -1;

                PixelAreaInfo undefinedPixelInfo = new PixelAreaInfo(-1, true);

                PixelAreaInfo upperPixInfo;
                PixelAreaInfo leftPixInfo;
                PixelAreaInfo upperLeftPixInfo;
                PixelAreaInfo upperRightPixInfo;
                PixelAreaInfo rightPixInfo;
                PixelAreaInfo bottomLeftPixInfo;
                PixelAreaInfo bottomPixInfo;
                PixelAreaInfo bottomRightInfo;

                bool isBoundary = false;
                int areaCount = areaIds.Count;
                int areaCounter = 0;

                foreach (int areaIdFromList in areaIds)
                {
                    areaCounter++;
                    this.parentForm.UpdateAsyncInfo($"Ilość stref [{areaCount}] wykonano [{areaCounter}]");

                    textX = -1;
                    textY = -1;

                    randomColor = Color.FromArgb
                    (
                        getRandom.Next(50, 200),
                        getRandom.Next(50, 200),
                        getRandom.Next(50, 200)
                    );

                    for (int y = 0; y < bmpHeight; y++)
                    {
                        for (int x = 0; x < bmpWidth; x++)
                        {
                            areaId = this.PixelsInfoArr[x, y].AreaId;
                            isDeadZone = this.PixelsInfoArr[x, y].IsDeadZone;

                            if (isDeadZone || areaId != areaIdFromList)
                                continue;

                            upperPixInfo = (y > 0 ? this.PixelsInfoArr[x, y - 1] : undefinedPixelInfo);
                            leftPixInfo = (x > 0 ? this.PixelsInfoArr[x - 1, y] : undefinedPixelInfo);
                            upperLeftPixInfo = (x > 0 && y > 0 ? this.PixelsInfoArr[x - 1, y - 1] : undefinedPixelInfo);
                            upperRightPixInfo = (y > 0 && x < bmpWidth - 1 ? this.PixelsInfoArr[x + 1, y - 1] : undefinedPixelInfo);
                            rightPixInfo = (x < bmpWidth - 1 ? this.PixelsInfoArr[x + 1, y] : undefinedPixelInfo);
                            bottomLeftPixInfo = (x > 0 && y < bmpHeight - 1 ? this.PixelsInfoArr[x - 1, y + 1] : undefinedPixelInfo);
                            bottomPixInfo = (y < bmpHeight - 1 ? this.PixelsInfoArr[x, y + 1] : undefinedPixelInfo);
                            bottomRightInfo = (x < bmpWidth - 1 && y < bmpHeight - 1 ? this.PixelsInfoArr[x + 1, y + 1] : undefinedPixelInfo);

                            isBoundary = false;

                            if 
                            (
                                upperLeftPixInfo.AreaId != areaId || upperPixInfo.AreaId != areaId || upperRightPixInfo.AreaId != areaId ||
                                leftPixInfo.AreaId != areaId || rightPixInfo.AreaId != areaId ||
                                bottomLeftPixInfo.AreaId != areaId || bottomPixInfo.AreaId != areaId || bottomRightInfo.AreaId != areaId
                            )
                            {
                                isBoundary = true;
                            }

                            pixelColor = this.SourceBitmap.GetPixel(x, y);
                            avgR = (pixelColor.R + randomColor.R) / 2;
                            avgG = (pixelColor.G + randomColor.G) / 2;
                            avgB = (pixelColor.B + randomColor.B) / 2;
                            previewBitmap.SetPixel(x, y, (!isBoundary ? Color.FromArgb(pixelColor.A, avgR, avgG, avgB) : Color.Red));

                            if (textX < 0 && textY < 0)
                            {
                                textX = x;
                                textY = y;
                            }
                        }
                    }

                    gr.DrawString
                    (
                        $"Strefa {areaIdFromList}",
                        textFont,
                        brush,
                        new Point
                        (
                            textX, textY
                        )
                    );
                }

                #endregion
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd przetwarzania obrazu podglądu stref: {exception.Message}";
            }

            this.parentForm.UpdateAsyncInfo("");

            packet.Obj = previewBitmap;
            packet.Msg = msg;
            return packet;
        }

        private PacketObjMsg GenerateAreaIndexes()
        {
            PacketObjMsg packet = new PacketObjMsg();
            bool success = false;
            string msg = "";

            try
            {
                #region Wstępne
                //WSTĘPNE
                if (this.SourceBitmap == null || this.PixelsInfoArr == null)
                    throw new Exception("bitmapa źródłowa lub mapa pikseli jest NULL! Należy wpierw wywołać metodę LoadBitmap()!");
                
                int bmpWidth = this.SourceBitmap.Width;
                int bmpHeight = this.SourceBitmap.Height;

                if (bmpWidth < 1 || bmpHeight < 1)
                    throw new Exception("nieprawidłowe wymiary bitmapy!");

                PixelAreaInfo undefinedPixelInfo = new PixelAreaInfo(-1, true);

                #endregion

                #region Tworzenie tablicy list zasięgu liniowego stref
                //TWORZENIE TABLICY LIST ZASIĘGU LINIOWEGO STREF
                this.parentForm.UpdateAsyncInfo("Tworzenie tablicy list zasięgu liniowego...");
                List<AreaLineRange>[] areaLineRangeArr = new List<AreaLineRange>[bmpHeight];

                for (int y = 0; y < bmpHeight; y++)
                {
                    areaLineRangeArr[y] = new List<AreaLineRange>();
                }

                #endregion
                
                #region Przypisywanie stref liniowych - do dołu
                //PRZYPISYWANIE STREF LINIOWYCH - DO DOŁU
                this.PixelAreaCount = 0;
                int rangeXFrom = -1;
                int minAreaId = -1;
                PixelAreaInfo currentPixInfo;
                List<AreaLineRange> previousLineAreas = null;
                bool areaIdChanged = false;

                for (int y = 0; y < bmpHeight; y++)
                {
                    if ((y + 1) % 100 == 0)
                        this.parentForm.UpdateAsyncInfo($"Przypisywanie stref liniowych ({(y + 1)} z {bmpHeight})...");

                    rangeXFrom = -1;
                    minAreaId = -1;

                    for (int x = 0; x < bmpWidth; x++)
                    {
                        currentPixInfo = this.PixelsInfoArr[x, y];

                        if (y > 0)
                            previousLineAreas = areaLineRangeArr[y - 1];
                        
                        if (!currentPixInfo.IsDeadZone && x == (bmpWidth - 1))
                        {
                            #region Warunek #1
                            AreaLineRange areaRange = new AreaLineRange();
                            areaIdChanged = false;

                            if (rangeXFrom > -1)
                            {
                                areaRange.RangePointFrom = rangeXFrom;
                            }
                            else
                            {
                                areaRange.RangePointFrom = x;
                            }

                            areaRange.RangePointTo = x;

                            if (previousLineAreas != null)
                            {
                                foreach (AreaLineRange prevAreaRange in previousLineAreas)
                                {
                                    if
                                    (
                                        (
                                            ( //zawiera się w przedziale
                                                areaRange.RangePointFrom - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z prawej
                                                areaRange.RangePointFrom - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointFrom + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z lewej
                                                areaRange.RangePointTo - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z obu stron
                                                areaRange.RangePointFrom - 1 < prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 > prevAreaRange.RangePointTo
                                            )
                                        )
                                        &&
                                        (
                                            minAreaId < 0 ||
                                            prevAreaRange.AreaId < minAreaId
                                        )
                                    )
                                    {
                                        areaRange.AreaId = prevAreaRange.AreaId;
                                        areaIdChanged = true;
                                    }
                                }
                            }

                            if (!areaIdChanged)
                            {
                                this.PixelAreaCount++;
                                areaRange.AreaId = this.PixelAreaCount;
                            }

                            areaLineRangeArr[y].Add(areaRange); //PRZYPISYWANIE DO TABLICY LIST

                            rangeXFrom = -1;
                            minAreaId = -1;

                            #endregion
                        }
                        else
                        if (!currentPixInfo.IsDeadZone && rangeXFrom < 0)
                        {
                            rangeXFrom = x;
                        }
                        else
                        if (currentPixInfo.IsDeadZone && rangeXFrom > 0)
                        {
                            #region Warunek #3
                            AreaLineRange areaRange = new AreaLineRange();
                            areaRange.RangePointFrom = rangeXFrom;
                            areaRange.RangePointTo = x - 1;
                            areaIdChanged = false;

                            if (previousLineAreas != null)
                            {
                                foreach (AreaLineRange prevAreaRange in previousLineAreas)
                                {
                                    if 
                                    (
                                        (
                                            ( //zawiera się w przedziale
                                                areaRange.RangePointFrom - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z prawej
                                                areaRange.RangePointFrom - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointFrom + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z lewej
                                                areaRange.RangePointTo - 1 >= prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 <= prevAreaRange.RangePointTo
                                            ) ||
                                            ( //przedział otwarty z obu stron
                                                areaRange.RangePointFrom - 1 < prevAreaRange.RangePointFrom &&
                                                areaRange.RangePointTo + 1 > prevAreaRange.RangePointTo
                                            )
                                        )
                                        &&
                                        (
                                            minAreaId < 0 ||
                                            prevAreaRange.AreaId < minAreaId
                                        )
                                    )
                                    {
                                        areaRange.AreaId = prevAreaRange.AreaId;
                                        areaIdChanged = true;
                                    }
                                }
                            }
                            
                            if (!areaIdChanged)
                            {
                                this.PixelAreaCount++;
                                areaRange.AreaId = this.PixelAreaCount;
                            }

                            areaLineRangeArr[y].Add(areaRange); //PRZYPISYWANIE DO TABLICY LIST

                            rangeXFrom = -1;
                            minAreaId = -1;

                            #endregion
                        }
                        
                    }
                }

                #endregion

                #region Wcielanie sąsiednich stref - do góry
                //WCIELANIE SĄSIEDNICH STREF - DO GÓRY
                previousLineAreas = null;
                List<AreaLineRange> currentLineAreas = null;
                
                for (int y = bmpHeight - 1; y >= 1; y--)
                {
                    if ((y + 1) % 100 == 0)
                        this.parentForm.UpdateAsyncInfo($"Wcielanie stref (pozostało linii weryfikacji {(y + 1)})");

                    previousLineAreas = areaLineRangeArr[y - 1];
                    currentLineAreas = areaLineRangeArr[y];

                    foreach (AreaLineRange currentArea in currentLineAreas)
                    {
                        foreach (AreaLineRange prevArea in previousLineAreas)
                        {
                            if
                            (
                                prevArea.AreaId != currentArea.AreaId
                                &&
                                (
                                    ( //zawiera się w przedziale
                                        currentArea.RangePointFrom - 1 >= prevArea.RangePointFrom &&
                                        currentArea.RangePointTo + 1 <= prevArea.RangePointTo
                                    ) ||
                                    ( //przedział otwarty z prawej
                                        currentArea.RangePointFrom - 1 >= prevArea.RangePointFrom &&
                                        currentArea.RangePointFrom + 1 <= prevArea.RangePointTo
                                    ) ||
                                    ( //przedział otwarty z lewej
                                        currentArea.RangePointTo - 1 >= prevArea.RangePointFrom &&
                                        currentArea.RangePointTo + 1 <= prevArea.RangePointTo
                                    ) ||
                                    ( //przedział otwarty z obu stron
                                        currentArea.RangePointFrom - 1 < prevArea.RangePointFrom &&
                                        currentArea.RangePointTo + 1 > prevArea.RangePointTo
                                    )
                                )
                            )
                            {
                                //prevArea.AreaId = currentArea.AreaId; //działało tylko w odniesieniu do linii wyżej, a nie bardziej skomplikowanego obszaru

                                for (int yBis = bmpHeight - 1; yBis >= 1; yBis--)
                                {
                                    foreach (AreaLineRange areaCheck in areaLineRangeArr[yBis])
                                    {
                                        if (areaCheck.AreaId == prevArea.AreaId)
                                            areaCheck.AreaId = currentArea.AreaId;
                                    }
                                }
                            }
                        }
                    }
                }
                
                #endregion

                #region Nadawanie stref pikselom
                //NADAWANIE STREF PIKSELOM WG STREF LINIOWYCH
                List<AreaLineRange> currentAreaRangeList;

                for (int y = 0; y < bmpHeight; y++)
                {
                    this.parentForm.UpdateAsyncInfo($"Indeksowanie stref ({(y + 1)} z {bmpHeight})...");

                    currentAreaRangeList = areaLineRangeArr[y];
                    
                    foreach (AreaLineRange areaRange in currentAreaRangeList)
                    {
                        if (areaRange.RangePointFrom < 0 || areaRange.RangePointTo < 0)
                            continue;

                        for (int x = areaRange.RangePointFrom; x <= areaRange.RangePointTo; x++)
                        {
                            currentPixInfo = this.PixelsInfoArr[x, y];
                            currentPixInfo.AreaId = areaRange.AreaId;
                        }
                    }
                }

                #endregion

                #region Kasowanie stref jednopikselowych
                //KASOWANIE STREF JEDNOPIKSELOWYCH
                PixelAreaInfo upperPixelInfo;
                PixelAreaInfo lowerPixelInfo;
                PixelAreaInfo leftPixelInfo;
                PixelAreaInfo rightPixelInfo;

                for (int y = 1; y < bmpHeight - 1; y++)
                {
                    this.parentForm.UpdateAsyncInfo($"Niwelowanie szumów ({(y + 1)} z {bmpHeight})...");

                    for (int x = 1; x < bmpWidth - 1; x++)
                    {
                        currentPixInfo = this.PixelsInfoArr[x, y];
                        upperPixelInfo = this.PixelsInfoArr[x, y - 1];
                        lowerPixelInfo = this.PixelsInfoArr[x, y + 1];
                        leftPixelInfo = this.PixelsInfoArr[x - 1, y];
                        rightPixelInfo = this.PixelsInfoArr[x + 1, y];

                        if
                        (
                            (leftPixelInfo.IsDeadZone && rightPixelInfo.IsDeadZone) ||
                            (upperPixelInfo.IsDeadZone && lowerPixelInfo.IsDeadZone)
                        )
                        {
                            currentPixInfo.AreaId = -1;
                            currentPixInfo.IsDeadZone = true;
                        }
                    }
                }

                #endregion

                success = true;
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd wyodrębniania indeksów stref #2: {exception.Message} | {exception.StackTrace}";
            }

            this.parentForm.UpdateAsyncInfo("");

            packet.Obj = success;
            packet.Msg = msg;
            return packet;
        }

        #region Stare
        private PacketObjMsg GenerateAreaIndexes_Old()
        {
            PacketObjMsg packet = new PacketObjMsg();
            bool success = false;
            string msg = "";

            try
            {
                if (this.SourceBitmap == null || this.PixelsInfoArr == null)
                    throw new Exception("bitmapa źródłowa lub mapa pikseli jest NULL! Należy wpierw wywołać metodę LoadBitmap()!");

                this.PixelAreaCount = 0;
                
                int bmpWidth = this.SourceBitmap.Width;
                int bmpHeight = this.SourceBitmap.Height;
                PixelAreaInfo undefinedPixelInfo = new PixelAreaInfo(-1, true);

                PixelAreaInfo currentPixInfo;
                //PixelAreaInfo[] neighborhoodPixInfo = new PixelAreaInfo[3];
                PixelAreaInfo upperPixInfo;
                PixelAreaInfo leftPixInfo;
                PixelAreaInfo upperLeftPixInfo;
                PixelAreaInfo upperRightPixInfo;
                List<int> detectedIds = new List<int>();
                int minAreaId = -1;
                
                for (int y = 0; y < bmpHeight; y++)
                {
                    if ((y + 1) % 100 == 0)
                        this.parentForm.UpdateAsyncInfo($"Analiza sąsiedztwa (linia {(y + 1)} z {bmpHeight})...");

                    for (int x = 0; x < bmpWidth; x++)
                    {
                        currentPixInfo = this.PixelsInfoArr[x, y];

                        if (currentPixInfo.IsDeadZone)
                            continue;

                        detectedIds.Clear();

                        minAreaId = -1;

                        upperPixInfo = (y > 0 ? this.PixelsInfoArr[x, y - 1] : undefinedPixelInfo);
                        leftPixInfo = (x > 0 ? this.PixelsInfoArr[x - 1, y] : undefinedPixelInfo);
                        upperLeftPixInfo = (x > 0 && y > 0 ? this.PixelsInfoArr[x - 1, y - 1] : undefinedPixelInfo);
                        upperRightPixInfo = (x < bmpWidth - 1 && y > 0 ? this.PixelsInfoArr[x + 1, y - 1] : undefinedPixelInfo);

                        if (!upperPixInfo.IsDeadZone)
                            detectedIds.Add(upperPixInfo.AreaId);

                        if (!leftPixInfo.IsDeadZone)
                            detectedIds.Add(leftPixInfo.AreaId);

                        if (!upperLeftPixInfo.IsDeadZone)
                            detectedIds.Add(upperLeftPixInfo.AreaId);

                        if (!upperRightPixInfo.IsDeadZone)
                            detectedIds.Add(upperRightPixInfo.AreaId);

                        if (detectedIds.Count > 0)
                        {
                            minAreaId = detectedIds.Min();

                            //ChangeAreaIdOfAllPixels(detectedIds, minAreaId, y); //ZBYT WOLNE
                            AreaGroupingInfo.AddAssignment(detectedIds, minAreaId);
                        }
                        
                        if (minAreaId == -1)
                        {
                            this.PixelAreaCount++;
                            currentPixInfo.AreaId = this.PixelAreaCount++;
                        }
                        else
                        {
                            currentPixInfo.AreaId = minAreaId;
                        }
                    }
                }

                this.parentForm.UpdateAsyncInfo("Unifikowanie stref...");
                AreaGroupingInfo.AnalyzeAreaRelationships();
                List<AreaGroupingInfo> areaGroupingList = AreaGroupingInfo.GetGroupingInfo();

                #region Test
                //DO TESTU
                //File.WriteAllText("test.txt", String.Empty);
                //foreach (var areaGrouping in areaGroupingList)
                //{
                //    File.AppendAllText("test.txt", $"FROM [{areaGrouping.IdFrom}] TO [{areaGrouping.IdTo}]{Environment.NewLine}");
                //}

                #endregion
                
                AreaGroupingInfo groupingInfo;
                for (int i = 0; i < areaGroupingList.Count; i++)
                {
                    this.parentForm.UpdateAsyncInfo($"Reindeksowanie (grupa {(i + 1)} z {areaGroupingList.Count})...");
                    groupingInfo = areaGroupingList[i];
                    ChangeAreaIdOfAllPixels(groupingInfo.IdFrom, groupingInfo.IdTo);
                }

                success = true;
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaInfo) Błąd wyodrębniania indeksów stref #1: {exception.Message} | {exception.StackTrace}";
            }

            this.parentForm.UpdateAsyncInfo("");
            AreaGroupingInfo.Clear();

            packet.Obj = success;
            packet.Msg = msg;
            return packet;
        }
        #endregion

        private PacketObjMsg GenerateAreas(int toleranceR, int toleranceG, int toleranceB)
        {
            PacketObjMsg packet = new PacketObjMsg();
            bool success = false;
            string msg = "";
            
            try
            {
                if (this.SourceBitmap == null || this.PixelsInfoArr == null)
                    throw new Exception("bitmapa źródłowa lub mapa pikseli jest NULL! Należy wpierw wywołać metodę LoadBitmap()!");
                
                toleranceR = DataProcessor.Clamp(0, 255, toleranceR);
                toleranceG = DataProcessor.Clamp(0, 255, toleranceG);
                toleranceB = DataProcessor.Clamp(0, 255, toleranceB);

                int bmpWidth = this.SourceBitmap.Width;
                int bmpHeight = this.SourceBitmap.Height;
                
                Color pixelColor;
                
                for (int y = 0; y < bmpHeight; y++)
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        pixelColor = this.SourceBitmap.GetPixel(x, y);
                        
                        if ( pixelColor.R > toleranceR || pixelColor.G > toleranceG || pixelColor.B > toleranceB )
                        {
                            this.PixelsInfoArr[x, y].IsDeadZone = false;
                        }
                        else
                        {
                            this.PixelsInfoArr[x, y].IsDeadZone = true;
                        }
                    }
                }
                
                success = true;
            }
            catch (Exception exception)
            {
                msg = $"(BitmapAreaProcessor) Błąd generowania stref: {exception.Message} | {exception.StackTrace}";
            }

            packet.Obj = success;
            packet.Msg = msg;
            return packet;
        }

        #endregion

        #region Metody zwykłe

        public static void DrawBezierLineOnBitmap
        (
            List<Point> pointList,
            Bitmap bmp,
            bool controlPointOnQuadraticBezier,
            double tStep,
            Point3 basicColorPalette,
            bool cutOnStart,
            bool cutOnEnd,
            double randomCuttingPercent,
            ref AreaBoundings extendedAreaBoundings
        )
        {
            try
            {
                if (tStep <= 0)
                    throw new Exception("przekazany parametr t nie może być mniejszy lub równy 0!");

                if (pointList.Count < 3)
                    throw new Exception($"zła ilość przekazanych punktów [{pointList.Count}]");

                List<Point> pointsToDrawList = new List<Point>();
                int pointCount = pointList.Count;
                Point pFinal = new Point(0, 0);
                double cuttingPercent = DataProcessor.GetRandomNumber(0, randomCuttingPercent);

                #region Wypełnianie listy punktów do narysowania
                //TWORZENIE LISTY PUNKTÓW LINII BEZIERA
                if (pointCount > 4)
                {
                    pointsToDrawList = DataProcessor.MultiBezierCurve
                    (
                        pointList.ToArray(),
                        tStep
                    );
                }
                else
                {
                    for (double t = 0; t <= 1; t += tStep)
                    {
                        if (pointCount == 3)
                        {
                            if (controlPointOnQuadraticBezier)
                            {
                                DataProcessor.QuadraticBezierCp
                                (
                                    pointList[0],
                                    pointList[1],
                                    pointList[2],
                                    t,
                                    ref pFinal
                                );
                            }
                            else
                            {
                                DataProcessor.QuadraticBezier
                                (
                                    pointList[0],
                                    pointList[1],
                                    pointList[2],
                                    t,
                                    ref pFinal
                                );
                            }
                        }
                        else
                        if (pointCount == 4)
                        {
                            DataProcessor.CubicBezier
                            (
                                pointList[0],
                                pointList[1],
                                pointList[2],
                                pointList[3],
                                t,
                                ref pFinal
                            );
                        }

                        pointsToDrawList.Add(pFinal);
                    }
                }

                #endregion

                //ROZSZERZANIE GRANIC STREFY
                extendedAreaBoundings.ExtendAreaBoundings(pointsToDrawList, bmp.Width - 1, bmp.Height - 1);

                //NANOSZENIE NA BITMAPĘ - OBSŁUGA LISTY PUNKTÓW
                Pen pen = new Pen(Color.FromArgb(basicColorPalette.X, basicColorPalette.Y, basicColorPalette.Z));
                Point currentPoint;
                Point nextPoint;
                double currentPositionPercent = 0;
                double nextPositionPercent = 0;

                using (var gr = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < pointsToDrawList.Count - 1; i++)
                    {
                        //PRZYCINANIE NA KOŃCACH
                        currentPositionPercent = ((double)i / (double)pointsToDrawList.Count) * 100.00;
                        nextPositionPercent = ((double)(i + 1) / (double)pointsToDrawList.Count) * 100.00;

                        if (cutOnStart && nextPositionPercent <= cuttingPercent)
                            continue;

                        if (cutOnEnd && (100.00 - currentPositionPercent) <= cuttingPercent)
                            continue;

                        //NANOSZENIE NA BITMAPĘ
                        currentPoint = pointsToDrawList[i];
                        nextPoint = pointsToDrawList[i + 1];
                        gr.DrawLine(pen, currentPoint, nextPoint);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"(BitmapAreaProcessor) Błąd rysowania linii Beziera na bitmapie: {exception.Message}");
            }
        }

        public List<int> GetAllAreaIds()
        {
            List<int> areaIds = new List<int>();
            int bmpWidth = this.SourceBitmap.Width;
            int bmpHeight = this.SourceBitmap.Height;

            int areaId = -1;
            bool isDeadZone = true;
            bool areaExists = false;

            for (int y = 0; y < bmpHeight; y++)
            {
                for (int x = 0; x < bmpWidth; x++)
                {
                    areaId = this.PixelsInfoArr[x, y].AreaId;
                    isDeadZone = this.PixelsInfoArr[x, y].IsDeadZone;

                    if (areaId < 0 || isDeadZone)
                        continue;

                    areaExists = false;

                    foreach (int areaIdFromList in areaIds)
                    {
                        if (areaIdFromList == areaId)
                        {
                            areaExists = true;
                            break;
                        }
                    }

                    if (!areaExists)
                    {
                        areaIds.Add(areaId);
                    }
                }
            }

            return areaIds;
        }

        private void ChangeAreaIdOfAllPixels(int idForChange, int resultId, int boundY)
        {
            bool isDeadZone = true;
            int currentAreaId = -1;

            for (int y = 0; y <= boundY; y++)
            {
                for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                {
                    currentAreaId = this.PixelsInfoArr[x, y].AreaId;
                    isDeadZone = this.PixelsInfoArr[x, y].IsDeadZone;

                    if (isDeadZone || currentAreaId == resultId)
                        continue;

                    if (currentAreaId == idForChange)
                        this.PixelsInfoArr[x, y].AreaId = resultId;
                }
            }
        }

        private void ChangeAreaIdOfAllPixels(int idForChange, int resultId)
        {
            bool isDeadZone = true;
            int currentAreaId = -1;

            for (int y = 0; y < this.PixelsInfoArr.GetLength(1); y++)
            {
                for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                {
                    currentAreaId = this.PixelsInfoArr[x, y].AreaId;
                    isDeadZone = this.PixelsInfoArr[x, y].IsDeadZone;

                    if (isDeadZone || currentAreaId == resultId)
                        continue;
                    
                    if (currentAreaId == idForChange)
                        this.PixelsInfoArr[x, y].AreaId = resultId;
                }
            }
        }

        private void ChangeAreaIdOfAllPixels(List<int> idsForChange, int resultId, int boundY)
        {
            bool areaForChange = false;
            bool isDeadZone = true;
            int currentAreaId = -1;
            
            for (int y = 0; y <= boundY; y++)
            {
                for (int x = 0; x < this.PixelsInfoArr.GetLength(0); x++)
                {
                    currentAreaId = this.PixelsInfoArr[x, y].AreaId;
                    isDeadZone = this.PixelsInfoArr[x, y].IsDeadZone;

                    if (isDeadZone || currentAreaId == resultId)
                        continue;

                    areaForChange = false;

                    foreach (int idForChange in idsForChange)
                    {
                        if (currentAreaId == idForChange)
                        {
                            areaForChange = true;
                            break;
                        }
                    }

                    if (!areaForChange)
                        continue;

                    this.PixelsInfoArr[x, y].AreaId = resultId;
                }
            }
        }

        public void Clear()
        {
            //this.parentForm = null;
            this.PixelAreaCount = 0;
            this.SourceBitmap = null;
            this.PixelsInfoArr = null;
        }

        public void Dispose()
        {
            Clear();
            this.parentForm = null;
        }

        #endregion
    }
}

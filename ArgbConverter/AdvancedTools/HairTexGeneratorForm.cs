using ArgbConverter.DataModels;
using ArgbConverter.ProcessingClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ArgbConverter.AdvancedTools
{
    public partial class HairTexGeneratorForm : Form
    {
        private enum PreviewType
        {
            HairMap,
            Result
        }

        private readonly object infoLock = new object();

        private Bitmap sourceBitmap = null;
        private Bitmap hairMapInfoBitmap = null;
        private Bitmap resultBitmap = null;

        private BitmapAreaProcessor bitmapAreaInfo = null;
        private HairGenUiController uiController = null;

        private bool loadingInProgress = false;
        private string asyncInfo = "";

        Random random = new Random();

        public HairTexGeneratorForm()
        {
            InitializeComponent();
            infoLabel.Text = "";
            info2Label.Text = "";
            this.bitmapAreaInfo = new BitmapAreaProcessor(this);
            this.uiController = new HairGenUiController(this, editPanelMainSplitContainer.Panel2, boundsCheckerCheckBox);

            this.asyncInfoTimer.Enabled = true;
        }

        public async void GenerateTexture(bool boundsChecker, int areaId = -1) //-1 dla wszystkich
        {
            if (this.loadingInProgress)
                return;

            this.loadingInProgress = true;
            editPanelMainSplitContainer.Panel2.Enabled = false;

            try
            {
                BitmapAreaProcessor bitmapProc = this.bitmapAreaInfo;

                if (bitmapProc == null)
                    throw new Exception($"Obiekt przetwarzania mapy bitowej jest NULL!");

                List<int> areaIdsList = (areaId == -1 ? bitmapProc.GetAllAreaIds() : new List<int>() { areaId });
                int bmpWidth = this.sourceBitmap.Width;
                int bmpHeight = this.sourceBitmap.Height;

                if (bmpWidth < 1 || bmpHeight < 1)
                    throw new Exception($"odczytano błędne wymiary bitmapy źródłowej [{bmpWidth},{bmpHeight}]!");

                #region Wstępne wypełnianie bitmapy rezultatu - przezroczystość
                //WSTĘPNE WYPEŁNIANIE BITMAPY REZULTATU
                UpdateInfo($"Wstępne tworzenie bitmapy rezultatu");

                PacketObjMsg transparentBmpPacket = await bitmapProc.GenerateTransparentBitmapTaskStart(bmpWidth, bmpHeight);
                this.resultBitmap = (Bitmap)transparentBmpPacket.Obj;

                if (!String.IsNullOrEmpty(transparentBmpPacket.Msg))
                    MessageBox.Show(transparentBmpPacket.Msg);

                #endregion

                foreach (int areaIdFromList in areaIdsList)
                {
                    UpdateInfo($"Generowanie tekstury dla ID strefy [{areaIdFromList}]");

                    #region Ograniczenia strefy
                    //POBIERANIE OGRANICZEŃ STREFY
                    PacketObjMsg boundingsPacket = await bitmapProc.GetAreaBoundingsTaskStart(areaIdFromList);
                    AreaBoundings areaBoundings = (AreaBoundings)boundingsPacket.Obj;

                    if (!String.IsNullOrEmpty(boundingsPacket.Msg))
                        MessageBox.Show(boundingsPacket.Msg);

                    int areaMinX = areaBoundings.BoundsX.X;
                    int areaMaxX = areaBoundings.BoundsX.Y;
                    int areaMinY = areaBoundings.BoundsY.X;
                    int areaMaxY = areaBoundings.BoundsY.Y;

                    //obiekt, który będzie przechowywał poszerzone ograniczenia strefy w przypadku, w którym rysowanie wyjdzie poza ograniczenia (np. losowe punkty krzywej)
                    AreaBoundings processingAreaBoundings = areaBoundings.Copy(); 

                    #endregion

                    #region Test ograniczeń stref
                    //TEST
                    //Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
                    //using (Graphics gr = Graphics.FromImage(this.resultBitmap))
                    //{
                    //    gr.FillRectangle(brush, areaMinX, areaMinY, (areaMaxX - areaMinX), (areaMaxY - areaMinY));
                    //}
                    #endregion

                    #region Ustawienia użytkownika
                    //USTAWIENIA UŻYTKOWNIKA DLA STREFY
                    AreaSettings userAreaSettings = this.uiController.GetAreaSettings(areaIdFromList);

                    AreaSettings.HairDrawingDirection hairDirection = userAreaSettings.Direction;
                    AreaSettings.HairCuttingSide hairCuttingSide = userAreaSettings.CuttingSide;

                    int drawingSteps = userAreaSettings.DrawingSteps;
                    int drawingIterations = userAreaSettings.DrawingIterations;
                    decimal iterationLossPercent = userAreaSettings.DrawingStepIterationLossPercent;
                    decimal bezierRandomRangePercent = userAreaSettings.BezierRandomRangePercent;
                    decimal bezierMarginPercent = userAreaSettings.BezierMarginPercent;
                    double cuttingRangePercent = Convert.ToDouble(userAreaSettings.CuttingRangePercent);

                    int bezierPointCount = userAreaSettings.MultiBezierPointCount;

                    if (userAreaSettings.BezierLineType == AreaSettings.BezierType.Quadratic || userAreaSettings.BezierLineType == AreaSettings.BezierType.QuadraticCp)
                    { bezierPointCount = 3; }
                    else
                    if (userAreaSettings.BezierLineType == AreaSettings.BezierType.Cubic)
                    { bezierPointCount = 4; }

                    Point3 backgroundRgbPalette = userAreaSettings.BackgroundRgbPalette;
                    Point3 foregroundRgbPalette = userAreaSettings.ForegroundRgbPalette;

                    bool hairMapDefinesOpacity = userAreaSettings.HairMapDefinesOpacity;
                    bool hairMapDefinesBrightness = userAreaSettings.HairMapDefinesBrightness;

                    #endregion

                    #region Dane wynikowe
                    //DANE WYNIKOWE
                    int iterationLossCount = Convert.ToInt32(drawingIterations * (iterationLossPercent / 100));
                    int currentIterationCount = drawingIterations;

                    int areaRangeX = areaMaxX - areaMinX;
                    int areaRangeY = areaMaxY - areaMinY;

                    #endregion

                    #region Dane przetwarzane
                    //DANE PRZETWARZANE W PĘTLACH
                    int currentAxisPosition;
                    double stepPercent;
                    double positionPercent;
                    double bezierPositionPercent;
                    PacketObjMsg lineRangePacket;
                    AreaLineRange lineRange;
                    int rangePointFrom;
                    int rangePointTo;
                    int resultPoint;
                    
                    int boundRangeMin;
                    int boundRangeMax;

                    List<Point> hairBezierPoints = new List<Point>();

                    Point3 currentColorPalette = new Point3(0, 0, 0);
                    int currentR = 0;
                    int currentG = 0;
                    int currentB = 0;

                    bool cutOnStart = false;
                    bool cutOnEnd = false;

                    if (hairDirection == AreaSettings.HairDrawingDirection.TopToBottom || hairDirection == AreaSettings.HairDrawingDirection.LeftToRight)
                    {
                        cutOnStart = (hairCuttingSide == AreaSettings.HairCuttingSide.Beginning || hairCuttingSide == AreaSettings.HairCuttingSide.BothSides);
                        cutOnEnd = (hairCuttingSide == AreaSettings.HairCuttingSide.End || hairCuttingSide == AreaSettings.HairCuttingSide.BothSides);
                    }
                    else
                    if (hairDirection == AreaSettings.HairDrawingDirection.BottomToTop || hairDirection == AreaSettings.HairDrawingDirection.RightToLeft)
                    {
                        cutOnEnd = (hairCuttingSide == AreaSettings.HairCuttingSide.Beginning || hairCuttingSide == AreaSettings.HairCuttingSide.BothSides);
                        cutOnStart = (hairCuttingSide == AreaSettings.HairCuttingSide.End || hairCuttingSide == AreaSettings.HairCuttingSide.BothSides);
                    }

                    #endregion

                    for (int drawingStep = 0; drawingStep < drawingSteps; drawingStep++) //PĘTLA TONÓW - KROKI
                    {
                        if (currentIterationCount <= 0)
                            break;

                        stepPercent = DataProcessor.CalculatePercent(Convert.ToDouble(drawingSteps - 1), Convert.ToDouble(drawingStep));

                        #region Kolor podstawowy (ton)
                        //WYODRĘBNIANIE PODSTAWOWEGO TONU KOLORU
                        currentR = Convert.ToInt32(DataProcessor.Lerp(backgroundRgbPalette.X, foregroundRgbPalette.X, (stepPercent / 100)));
                        currentG = Convert.ToInt32(DataProcessor.Lerp(backgroundRgbPalette.Y, foregroundRgbPalette.Y, (stepPercent / 100)));
                        currentB = Convert.ToInt32(DataProcessor.Lerp(backgroundRgbPalette.Z, foregroundRgbPalette.Z, (stepPercent / 100)));
                        currentR = DataProcessor.Clamp(0, 255, currentR);
                        currentG = DataProcessor.Clamp(0, 255, currentG);
                        currentB = DataProcessor.Clamp(0, 255, currentB);

                        currentColorPalette = new Point3(currentR, currentG, currentB);

                        #endregion

                        for (int drawingIteration = 0; drawingIteration < currentIterationCount; drawingIteration++) //PĘTLA ITERACJI - SZTUK WŁOSA
                        {
                            hairBezierPoints.Clear();
                            positionPercent = DataProcessor.CalculatePercent(Convert.ToDouble(currentIterationCount - 1), Convert.ToDouble(drawingIteration));
                            
                            for (int bezierPointIteration = 0; bezierPointIteration < bezierPointCount; bezierPointIteration++)
                            {
                                bezierPositionPercent = DataProcessor.CalculatePercent(Convert.ToDouble(bezierPointCount - 1), Convert.ToDouble(bezierPointIteration));
                                
                                if (hairDirection == AreaSettings.HairDrawingDirection.TopToBottom || hairDirection == AreaSettings.HairDrawingDirection.BottomToTop)
                                {
                                    #region Pionowo

                                    currentAxisPosition = Convert.ToInt32(DataProcessor.Lerp(areaMinY, areaMaxY, bezierPositionPercent / 100));

                                    lineRangePacket = await this.bitmapAreaInfo.GetAreaLineRangeTaskStart
                                    (
                                        BitmapAreaProcessor.AreaAxis.Y,
                                        currentAxisPosition,
                                        areaIdFromList
                                    );
                                    lineRange = (AreaLineRange)lineRangePacket.Obj;

                                    //POBIERANIE PUNKTÓW GRANICZNYCH DLA STREFY
                                    rangePointFrom = lineRange.RangePointFrom;
                                    rangePointTo = lineRange.RangePointTo;

                                    //MODYFIKACJA O MARGINESY
                                    rangePointFrom = Convert.ToInt32(rangePointFrom - (rangePointTo - rangePointFrom) * (bezierMarginPercent / 100));
                                    rangePointTo = Convert.ToInt32(rangePointTo + (rangePointTo - rangePointFrom) * (bezierMarginPercent / 100));

                                    if (rangePointFrom < 0)
                                        rangePointFrom = 0;

                                    if (rangePointTo > this.resultBitmap.Width - 1)
                                        rangePointTo = this.resultBitmap.Width - 1;

                                    
                                    //WSTĘPNE TWORZENIE PUNKTU WYNIKOWEGO
                                    resultPoint = Convert.ToInt32(DataProcessor.Lerp(rangePointFrom, rangePointTo, positionPercent / 100));

                                    //ZAKRES LOSOWANIA / LOSOWANIE PUNKTU
                                    boundRangeMin = Convert.ToInt32(resultPoint - ((rangePointTo - rangePointFrom) * (bezierRandomRangePercent / 100)));
                                    boundRangeMax = Convert.ToInt32(resultPoint + ((rangePointTo - rangePointFrom) * (bezierRandomRangePercent / 100)));

                                    resultPoint = this.random.Next(boundRangeMin, boundRangeMax + 1);
                                    resultPoint = DataProcessor.Clamp(0, this.resultBitmap.Width, resultPoint);

                                    //PRZYPISYWANIE PUNKTU WYNIKOWEGO

                                    hairBezierPoints.Add
                                    (
                                        new Point
                                        (
                                            resultPoint,
                                            currentAxisPosition
                                        )
                                    );

                                    #endregion
                                }
                                else
                                if (hairDirection == AreaSettings.HairDrawingDirection.LeftToRight || hairDirection == AreaSettings.HairDrawingDirection.RightToLeft)
                                {
                                    #region Poziomo

                                    currentAxisPosition = Convert.ToInt32(DataProcessor.Lerp(areaMinX, areaMaxX, bezierPositionPercent / 100));

                                    lineRangePacket = await this.bitmapAreaInfo.GetAreaLineRangeTaskStart
                                    (
                                        BitmapAreaProcessor.AreaAxis.X,
                                        currentAxisPosition,
                                        areaIdFromList
                                    );
                                    lineRange = (AreaLineRange)lineRangePacket.Obj;

                                    //POBIERANIE PUNKTÓW GRANICZNYCH DLA STREFY
                                    rangePointFrom = lineRange.RangePointFrom;
                                    rangePointTo = lineRange.RangePointTo;

                                    //MODYFIKACJA O MARGINESY
                                    rangePointFrom = Convert.ToInt32(rangePointFrom - (rangePointTo - rangePointFrom) * (bezierMarginPercent / 100));
                                    rangePointTo = Convert.ToInt32(rangePointTo + (rangePointTo - rangePointFrom) * (bezierMarginPercent / 100));

                                    if (rangePointFrom < 0)
                                        rangePointFrom = 0;

                                    if (rangePointTo > this.resultBitmap.Height - 1)
                                        rangePointTo = this.resultBitmap.Height - 1;

                                    //WSTĘPNE TWORZENIE PUNKTU WYNIKOWEGO
                                    resultPoint = Convert.ToInt32(DataProcessor.Lerp(rangePointFrom, rangePointTo, positionPercent / 100));

                                    //ZAKRES LOSOWANIA / LOSOWANIE PUNKTU
                                    boundRangeMin = Convert.ToInt32(resultPoint - ((rangePointTo - rangePointFrom) * (bezierRandomRangePercent / 100)));
                                    boundRangeMax = Convert.ToInt32(resultPoint + ((rangePointTo - rangePointFrom) * (bezierRandomRangePercent / 100)));

                                    resultPoint = this.random.Next(boundRangeMin, boundRangeMax + 1);
                                    resultPoint = DataProcessor.Clamp(0, this.resultBitmap.Height, resultPoint);

                                    //PRZYPISYWANIE PUNKTU WYNIKOWEGO

                                    hairBezierPoints.Add
                                    (
                                        new Point
                                        (
                                            currentAxisPosition,
                                            resultPoint
                                        )
                                    );

                                    #endregion
                                }
                                else
                                if (hairDirection == AreaSettings.HairDrawingDirection.Radial)
                                {

                                }
                                else
                                if (hairDirection == AreaSettings.HairDrawingDirection.LinearX)
                                {

                                }
                                else
                                if (hairDirection == AreaSettings.HairDrawingDirection.LinearY)
                                {

                                }






                            }
                            
                            //RYSOWANIE

                            BitmapAreaProcessor.DrawBezierLineOnBitmap
                            (
                                hairBezierPoints,
                                this.resultBitmap,
                                (userAreaSettings.BezierLineType == AreaSettings.BezierType.QuadraticCp),
                                0.02,
                                currentColorPalette,
                                cutOnStart,
                                cutOnEnd,
                                cuttingRangePercent,
                                ref processingAreaBoundings
                            );

                            //POSZERZANIE GRANIC STREFY - DLA PRZETWARZANIA PRZEZROCZYSTOŚCI / POŁYSKU
                            //processingAreaBoundings.ExtendAreaBoundings(hairBezierPoints, this.resultBitmap.Width - 1, this.resultBitmap.Height - 1);
                        }

                        currentIterationCount -= iterationLossCount;
                    }

                    //PRZEZROCZYSTOŚĆ / POŁYSK
                    BitmapAreaProcessor.AreaAxis glossOpacAxis = BitmapAreaProcessor.AreaAxis.None;
                    if (hairDirection == AreaSettings.HairDrawingDirection.TopToBottom || hairDirection == AreaSettings.HairDrawingDirection.BottomToTop)
                    {
                        glossOpacAxis = BitmapAreaProcessor.AreaAxis.X;
                    }
                    else
                    if (hairDirection == AreaSettings.HairDrawingDirection.LeftToRight || hairDirection == AreaSettings.HairDrawingDirection.RightToLeft)
                    {
                        glossOpacAxis = BitmapAreaProcessor.AreaAxis.Y;
                    }

                    PacketObjMsg glossOpacityPacket = await bitmapProc.GenerateHairTexGlossinessAndOpacityTaskStart
                    (
                        processingAreaBoundings, //areaBoundings,
                        this.resultBitmap,
                        Color.FromArgb(255, backgroundRgbPalette.X, backgroundRgbPalette.Y, backgroundRgbPalette.Z),
                        hairMapDefinesBrightness,
                        hairMapDefinesOpacity,
                        //new double[] { Convert.ToDouble(bezierMarginPercent / 100), Convert.ToDouble(bezierRandomRangePercent / 100) },
                        //glossOpacAxis
                        boundsChecker
                    );
                    this.resultBitmap = (Bitmap)glossOpacityPacket.Obj;

                    if (!String.IsNullOrEmpty(glossOpacityPacket.Msg))
                        MessageBox.Show(glossOpacityPacket.Msg);
                }

                //WYŚWIETLANIE OBRAZU REZULTATU
                SetPreview(PreviewType.Result, this.resultBitmap);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd generowania tekstury: {exception.Message}");
            }

            UpdateInfo("");
            editPanelMainSplitContainer.Panel2.Enabled = true;
            this.loadingInProgress = false;
        }

        private async void LoadImage()
        {
            if (this.loadingInProgress)
                return;

            this.loadingInProgress = true;
            editPanelMainSplitContainer.Panel2.Enabled = false;

            try
            {
                int deadZoneRangeR = Convert.ToInt32(deadZoneRangeRNumericUpDown.Value);
                int deadZoneRangeG = Convert.ToInt32(deadZoneRangeGNumericUpDown.Value);
                int deadZoneRangeB = Convert.ToInt32(deadZoneRangeBNumericUpDown.Value);
                
                #region Ścieżka - pobieranie / sprawdzanie
                //POBIERANIE ŚCIEŻKI
                string path = "";

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        path = ofd.FileName;
                    }
                }

                if (String.IsNullOrWhiteSpace(path))
                {
                    this.loadingInProgress = false;
                    editPanelMainSplitContainer.Panel2.Enabled = true;
                    UpdateInfo("Nie wybrano pliku!");
                    return;
                }

                if (!File.Exists(path))
                    throw new Exception($"plik [{path}] nie istnieje!");

                string extension = Path.GetExtension(path);
                bool extensionIsValid = false;

                foreach (string ext in GlobalData.ImageFormats)
                {
                    if (ext.ToLower() == extension || $".{ext.ToLower()}" == extension)
                    {
                        extensionIsValid = true;
                    }
                }

                if (!extensionIsValid)
                    throw new Exception($"plik [{path}] ma nieprawidłowy format [{extension}]");

                #endregion

                #region Przetwarzanie bitmapy źródłowej
                //PRZETWARZANIE BITMAPY ŹRÓDŁOWEJ
                UpdateInfo("Przetwarzanie mapy rozkładu...");
                Bitmap originalBitmap = new Bitmap(path);
                PacketObjMsg grayScaleImagePacket = await ArgbProcessor.CreateGrayScaleImageTaskStart(originalBitmap);
                this.sourceBitmap = (Bitmap)grayScaleImagePacket.Obj;

                if (!String.IsNullOrEmpty(grayScaleImagePacket.Msg))
                    MessageBox.Show(grayScaleImagePacket.Msg);

                if (this.sourceBitmap == null)
                    throw new Exception("bitmapa źródłowa jest NULL!");

                //SetPreview(PreviewType.HairMap, (Bitmap)this.sourceBitmap.Clone()); //PODGLĄD #1

                #endregion
                
                this.bitmapAreaInfo.Clear();

                #region Inicjalizacja stref
                //INICJALIZACJA MAPY STREF
                UpdateInfo("Inicjalizacja mapy stref...");

                PacketObjMsg bmpAreaLoadingPacket = await this.bitmapAreaInfo.LoadBitmapTaskStart(this.sourceBitmap);
                bool bmpAreaLoadingSuccess = (bool)bmpAreaLoadingPacket.Obj;

                if (!String.IsNullOrEmpty(bmpAreaLoadingPacket.Msg))
                    MessageBox.Show(bmpAreaLoadingPacket.Msg);

                if (!bmpAreaLoadingSuccess)
                    throw new Exception("metoda inicjalizująca dane stref zwróciła wartość FALSE!");

                #endregion

                #region Wyodrębnianie typu stref
                //WYODRĘBNIANIE TYPU STREF
                UpdateInfo("Wyodrębnianie stref martwych i stref wpływu...");

                PacketObjMsg areaMarkingPacket = await this.bitmapAreaInfo.GenerateAreasTaskStart(deadZoneRangeR, deadZoneRangeG, deadZoneRangeB);
                bool areaExtractingSuccess = (bool)areaMarkingPacket.Obj;

                if (!String.IsNullOrEmpty(areaMarkingPacket.Msg))
                    MessageBox.Show(areaMarkingPacket.Msg);

                if (!areaExtractingSuccess)
                    throw new Exception("metoda wyodrębniająca strefy zwróciła wartość FALSE!");

                #endregion

                #region Indeksowanie stref
                //WYODRĘBNIANIE INDEKSÓW STREF
                UpdateInfo("Indeksowanie stref...");

                PacketObjMsg areaIndexingPacket = await this.bitmapAreaInfo.GenerateAreaIndexesTaskStart();
                bool areaIndexingSuccess = (bool)areaIndexingPacket.Obj;

                if (!String.IsNullOrEmpty(areaIndexingPacket.Msg))
                    MessageBox.Show(areaIndexingPacket.Msg);

                if (!areaIndexingSuccess)
                    throw new Exception("metoda indeksująca strefy zwróciła wartość FALSE!");

                #endregion

                #region Zwracanie obrazu rezultatu
                //ZWRACANIE OBRAZU REZULTATU
                UpdateInfo("Generowanie obrazu mapy rozłożenia...");
                PacketObjMsg areaPreviewPacket = await this.bitmapAreaInfo.GenerateAreaPreviewImageTaskStart();
                this.hairMapInfoBitmap = (Bitmap)areaPreviewPacket.Obj;

                if (!String.IsNullOrEmpty(areaPreviewPacket.Msg))
                    MessageBox.Show(areaPreviewPacket.Msg);

                if (this.hairMapInfoBitmap == null)
                    throw new Exception("referencja do obrazu podglądu jest NULL!");

                SetPreview(PreviewType.HairMap, this.hairMapInfoBitmap); //PODGLĄD #2

                #endregion

                #region Tworzenie wstępne ustawień stref
                //TWORZENIE USTAWIEŃ STREF/UPDATE UI
                UpdateInfo("Inicjowanie danych ustawień stref...");
                PacketObjMsg areaSettingsPacket = await this.bitmapAreaInfo.GenerateAreaSettingsTaskStart();
                List<AreaSettings> areaSettingsList = (List<AreaSettings>)areaSettingsPacket.Obj;

                if (!String.IsNullOrEmpty(areaSettingsPacket.Msg))
                    MessageBox.Show(areaSettingsPacket.Msg);

                if (areaSettingsList.Count > 100)
                {
                    MessageBox.Show("Ilość odczytanych stref jest zbyt duża! Należy wybrać obraz o mniej zróżnicowanych obszarach jasności lub zmienić zakres martwej strefy!");
                    areaSettingsList.Clear();
                }
                
                #endregion

                #region Wypełnianie panelu
                //WYPEŁNIANIE PENELU
                UpdateInfo("Dynamiczne generowanie panelu ustawień stref...");
                await GlobalProcessor.SleepTaskStart(200);
                this.uiController.LoadAreaPanel(areaSettingsList);

                #endregion
                
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd odczytu obrazu: {exception.Message}");
            }
            
            UpdateInfo("");
            editPanelMainSplitContainer.Panel2.Enabled = true;
            this.loadingInProgress = false;
        }

        private void SetPreview(PreviewType type, Bitmap image)
        {
            PictureBox picBox = (type == PreviewType.Result ? resultPictureBox : hairMapPreviewPictureBox);
            picBox.Image = image;
            picBox.Width = image.Width;
            picBox.Height = image.Height;

            if (type == PreviewType.Result)
            {
                resultPictureBox.BackgroundImage = Properties.Resources.checker;
                resultPictureBox.BackgroundImageLayout = ImageLayout.Tile;
            }
        }

        private void UpdateInfo(string text)
        {
            infoLabel.Text = text;
        }

        public void UpdateAsyncInfo(string text)
        {
            lock (this.infoLock)
            {
                this.asyncInfo = text;
            }
        }
        
        private void SaveResultImage()
        {
            try
            {
                if (this.resultBitmap == null)
                    throw new Exception("nie wygenerowano obrazu (NULL)!");

                string resultDirPath = GlobalData.ResultFileDirectory;
                FileDataManager.CreateDirectoryIfNotExists(resultDirPath);
                string filePath = $"{resultDirPath}\\hairTex_{DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss")}.png";
                this.resultBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show("Pomyślnie zapisano obraz!");
                Process.Start(resultDirPath);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd zapisu obrazu wynikowego: {exception.Message}");
            }
        }

        #region Eventy

        private void HairTexGeneratorForm_Load(object sender, EventArgs e)
        {
        }

        private void asyncInfoTimer_Tick(object sender, EventArgs e)
        {
            lock (this.infoLock)
            {
                info2Label.Text = this.asyncInfo;
            }
        }

        private void loadFileButton_Click(object sender, EventArgs e)
        {
            LoadImage();
        }

        private void HairTexGeneratorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.bitmapAreaInfo = null;
            this.uiController = null;
        }

        private void saveResultImageButton_Click(object sender, EventArgs e)
        {
            SaveResultImage();
        }
        
        #endregion
        
    }
}

using ArgbConverter.AdvancedTools;
using ArgbConverter.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ArgbConverter.ProcessingClasses
{
    public class HairGenUiController
    {
        private HairTexGeneratorForm parentForm = null;
        private SplitterPanel areaMenuPanel = null;
        private List<AreaSettings> settingsList = new List<AreaSettings>();
        private List<HairGenUiReferenceContainer> referenceContainerList = new List<HairGenUiReferenceContainer>();
        private CheckBox boundsCheckerCheckBox = null;

        private static int areaPanelMargin = 5;

        public HairGenUiController(HairTexGeneratorForm parentForm, SplitterPanel areaMenuPanel, CheckBox boundsCheckerCheckBox)
        {
            this.parentForm = parentForm;
            this.areaMenuPanel = areaMenuPanel;
            this.boundsCheckerCheckBox = boundsCheckerCheckBox;
        }

        public AreaSettings GetAreaSettings (int areaId)
        {
            AreaSettings areaSettings = null;

            foreach (AreaSettings item in this.settingsList)
            {
                if (item.AreaId == areaId)
                {
                    areaSettings = item;
                    break;
                }
            }

            return areaSettings;
        }

        #region Metody czyszczenia danych

        private void ClearAll()
        {
            foreach (HairGenUiReferenceContainer container in this.referenceContainerList)
            {
                Cleanup(container.MainAreaPanel);
            }

            this.settingsList.Clear();
            this.referenceContainerList.Clear();
        }

        private void Cleanup(Control c)
        {
            foreach (Control child in c.Controls)
                Cleanup(child);

            if (c.Parent != null)
            {
                c.Parent.Controls.Remove(c);
                c.Dispose();
            }
        }

        #endregion

        #region Metody przeładowania UI

        public void ReloadAreaPanel()
        {
            if (this.settingsList == null)
                return;

            List<AreaSettings> settingsListClone = new List<AreaSettings>(this.settingsList);
            LoadAreaPanel(settingsListClone);
        }

        public bool LoadAreaPanel(List<AreaSettings> settingsList)
        {
            bool success = false;

            try
            {
                ClearAll();
                this.settingsList = settingsList;
                AreaSettings areaSettings;
                int posY = areaPanelMargin;

                Font controlFont = new Font("Microsoft Sans Serif", 8);

                for (int i = 0; i < settingsList.Count; i++)
                {
                    areaSettings = settingsList[i];

                    #region Tworzenie kontrolek
                    //TWORZENIE KONTROLEK

                    //panel główny
                    Panel mainAreaPanel = new Panel();
                    mainAreaPanel.Name = $"mainAreaPanel_{i}";
                    mainAreaPanel.BorderStyle = BorderStyle.FixedSingle;
                    mainAreaPanel.BackColor = Color.Silver;
                    mainAreaPanel.AutoScroll = true;
                    mainAreaPanel.Width = 1880;
                    mainAreaPanel.Height = 150;
                    mainAreaPanel.Font = controlFont;
                    mainAreaPanel.Location = new Point
                    (
                        areaPanelMargin,
                        posY
                    );

                    posY += (mainAreaPanel.Height + areaPanelMargin);

                    //etykieta ID
                    Label areaIdLabel = new Label();
                    areaIdLabel.Name = $"areaIdLabel_{i}";
                    areaIdLabel.Text = $"ID strefy: {areaSettings.AreaId}";
                    areaIdLabel.AutoSize = false;
                    areaIdLabel.BorderStyle = BorderStyle.FixedSingle;
                    areaIdLabel.Location = new Point(5, 5);
                    areaIdLabel.Width = mainAreaPanel.Width - (areaPanelMargin * 2);
                    areaIdLabel.Font = controlFont;
                    areaIdLabel.BackColor = Color.FromArgb(224, 224, 224);
                    areaIdLabel.TextAlign = ContentAlignment.MiddleLeft;
                    areaIdLabel.Height = 19;

                    #region Sekcja palety kolorów

                    //paleta kolorów - group box
                    GroupBox colorPaletteGroupBox = new GroupBox();
                    colorPaletteGroupBox.Name = $"colorPaletteGroupBox_{i}";
                    colorPaletteGroupBox.Text = "Paleta kolorów";
                    colorPaletteGroupBox.Width = 120;
                    colorPaletteGroupBox.Height = 110;
                    colorPaletteGroupBox.Font = controlFont;
                    colorPaletteGroupBox.Location = new Point
                    (
                        5,
                        areaIdLabel.Location.Y + areaIdLabel.Height + 5
                    );

                    //paleta kolorów - etykieta kolor1
                    Label colorPaletteColor1Label = new Label();
                    colorPaletteColor1Label.Name = $"colorPaletteColor1Label_{i}";
                    colorPaletteColor1Label.Text = "Kolor 1:";
                    colorPaletteColor1Label.TextAlign = ContentAlignment.MiddleLeft;
                    colorPaletteColor1Label.Font = controlFont;
                    colorPaletteColor1Label.AutoSize = false;
                    colorPaletteColor1Label.Width = 50;
                    colorPaletteColor1Label.Height = 20;
                    colorPaletteColor1Label.BorderStyle = BorderStyle.None;
                    colorPaletteColor1Label.Location = new Point(10, 20);

                    //paleta kolorów - etykieta kolor 2
                    Label colorPaletteColor2Label = new Label();
                    colorPaletteColor2Label.Name = $"colorPaletteColor2Label_{i}";
                    colorPaletteColor2Label.Text = "Kolor 2:";
                    colorPaletteColor2Label.TextAlign = ContentAlignment.MiddleLeft;
                    colorPaletteColor2Label.Font = controlFont;
                    colorPaletteColor2Label.AutoSize = false;
                    colorPaletteColor2Label.Width = 50;
                    colorPaletteColor2Label.Height = 20;
                    colorPaletteColor2Label.BorderStyle = BorderStyle.None;
                    colorPaletteColor2Label.Location = new Point(10, 50);

                    //paleta kolorów - button kolor1
                    Button colorPaletteColor1Button = new Button();
                    colorPaletteColor1Button.Name = $"colorPaletteColor1Button_{i}";
                    colorPaletteColor1Button.Text = "";
                    colorPaletteColor1Button.Font = controlFont;
                    colorPaletteColor1Button.FlatStyle = FlatStyle.Flat;
                    colorPaletteColor1Button.BackColor = Color.FromArgb
                    (
                        areaSettings.BackgroundRgbPalette.X,
                        areaSettings.BackgroundRgbPalette.Y,
                        areaSettings.BackgroundRgbPalette.Z
                    );
                    colorPaletteColor1Button.Location = new Point
                    (
                        colorPaletteColor1Label.Location.X + colorPaletteColor1Label.Width + 5,
                        colorPaletteColor1Label.Location.Y
                    );
                    colorPaletteColor1Button.Width = 20;
                    colorPaletteColor1Button.Height = 20;

                    //paleta kolorów - button kolor 2
                    Button colorPaletteColor2Button = new Button();
                    colorPaletteColor2Button.Name = $"colorPaletteColor2Button_{i}";
                    colorPaletteColor2Button.Text = "";
                    colorPaletteColor2Button.Font = controlFont;
                    colorPaletteColor2Button.FlatStyle = FlatStyle.Flat;
                    colorPaletteColor2Button.BackColor = Color.FromArgb
                    (
                        areaSettings.ForegroundRgbPalette.X,
                        areaSettings.ForegroundRgbPalette.Y,
                        areaSettings.ForegroundRgbPalette.Z
                    );
                    colorPaletteColor2Button.Location = new Point
                    (
                        colorPaletteColor2Label.Location.X + colorPaletteColor2Label.Width + 5,
                        colorPaletteColor2Label.Location.Y
                    );
                    colorPaletteColor2Button.Width = 20;
                    colorPaletteColor2Button.Height = 20;

                    //paleta kolorów - button zmiany
                    Button colorPaletteChangeEverywhereButton = new Button();
                    colorPaletteChangeEverywhereButton.Name = $"colorPaletteChangeEverywhereButton_{i}";
                    colorPaletteChangeEverywhereButton.Text = "Zmień wszędzie";
                    colorPaletteChangeEverywhereButton.Font = controlFont;
                    colorPaletteChangeEverywhereButton.BackColor = Color.FromArgb(255, 224, 192);
                    colorPaletteChangeEverywhereButton.Width = 100;
                    colorPaletteChangeEverywhereButton.Height = 20;
                    colorPaletteChangeEverywhereButton.Location = new Point(10, 80);

                    #endregion

                    #region Sekcja kierunku

                    //kierunek - group box
                    GroupBox directionGroupBox = new GroupBox();
                    directionGroupBox.Name = $"directionGroupBox_{i}";
                    directionGroupBox.Text = "Ustawienia kierunku";
                    directionGroupBox.Width = 500;
                    directionGroupBox.Height = 110;
                    directionGroupBox.Font = controlFont;
                    directionGroupBox.Location = new Point
                    (
                        colorPaletteGroupBox.Location.X + colorPaletteGroupBox.Width + 5,
                        colorPaletteGroupBox.Location.Y
                    );

                    //kierunek - label
                    Label hairDirectionLabel = new Label();
                    hairDirectionLabel.Name = $"hairDirectionLabel_{i}";
                    hairDirectionLabel.Text = "Kierunek rysowania:";
                    hairDirectionLabel.TextAlign = ContentAlignment.MiddleLeft;
                    hairDirectionLabel.Font = controlFont;
                    hairDirectionLabel.AutoSize = false;
                    hairDirectionLabel.Width = 120;
                    hairDirectionLabel.Height = 20;
                    hairDirectionLabel.BorderStyle = BorderStyle.None;
                    hairDirectionLabel.Location = new Point(10, 20);

                    //kierunek - przycinanie - label
                    Label hairCuttingLabel = new Label();
                    hairCuttingLabel.Name = $"hairCuttingLabel_{i}";
                    hairCuttingLabel.Text = "Przycinanie:";
                    hairCuttingLabel.TextAlign = ContentAlignment.MiddleLeft;
                    hairCuttingLabel.Font = controlFont;
                    hairCuttingLabel.AutoSize = false;
                    hairCuttingLabel.Width = 120;
                    hairCuttingLabel.Height = 20;
                    hairCuttingLabel.BorderStyle = BorderStyle.None;
                    hairCuttingLabel.Location = new Point(10, 50);

                    //kierunek - combo box
                    ComboBox hairDirectionComboBox = new ComboBox();
                    hairDirectionComboBox.Name = $"hairDirectionComboBox_{i}";
                    hairDirectionComboBox.Font = controlFont;
                    hairDirectionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    hairDirectionComboBox.Width = 140;
                    hairDirectionComboBox.Location = new Point
                    (
                        hairDirectionLabel.Location.X + hairDirectionLabel.Width + 5,
                        hairDirectionLabel.Location.Y
                    );
                    
                    hairDirectionComboBox.Items.Add("Z góry na dół");
                    hairDirectionComboBox.Items.Add("Z lewej do prawej");
                    hairDirectionComboBox.Items.Add("Z prawej do lewej");
                    hairDirectionComboBox.Items.Add("Z dołu do góry");
                    hairDirectionComboBox.Items.Add("Promieniście");
                    hairDirectionComboBox.Items.Add("Liniowo względem osi X");
                    hairDirectionComboBox.Items.Add("Liniowo względem osi Y");

                    switch (areaSettings.Direction)
                    {
                        case AreaSettings.HairDrawingDirection.TopToBottom:
                            hairDirectionComboBox.SelectedIndex = 0;
                            break;
                        case AreaSettings.HairDrawingDirection.LeftToRight:
                            hairDirectionComboBox.SelectedIndex = 1;
                            break;
                        case AreaSettings.HairDrawingDirection.RightToLeft:
                            hairDirectionComboBox.SelectedIndex = 2;
                            break;
                        case AreaSettings.HairDrawingDirection.BottomToTop:
                            hairDirectionComboBox.SelectedIndex = 3;
                            break;
                        case AreaSettings.HairDrawingDirection.Radial:
                            hairDirectionComboBox.SelectedIndex = 4;
                            break;
                        case AreaSettings.HairDrawingDirection.LinearX:
                            hairDirectionComboBox.SelectedIndex = 5;
                            break;
                        case AreaSettings.HairDrawingDirection.LinearY:
                            hairDirectionComboBox.SelectedIndex = 6;
                            break;
                    }

                    //kierunek - przycinanie - combo box
                    ComboBox hairCuttingComboBox = new ComboBox();
                    hairCuttingComboBox.Name = $"hairCuttingComboBox_{i}";
                    hairCuttingComboBox.Font = controlFont;
                    hairCuttingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    hairCuttingComboBox.Width = 140;
                    hairCuttingComboBox.Location = new Point
                    (
                        hairCuttingLabel.Location.X + hairCuttingLabel.Width + 5,
                        hairCuttingLabel.Location.Y
                    );

                    hairCuttingComboBox.Items.Add("Wyłączone");
                    hairCuttingComboBox.Items.Add("Od początku");
                    hairCuttingComboBox.Items.Add("Od końca");
                    hairCuttingComboBox.Items.Add("Od obu stron");

                    switch (areaSettings.CuttingSide)
                    {
                        case AreaSettings.HairCuttingSide.None:
                            hairCuttingComboBox.SelectedIndex = 0;
                            break;
                        case AreaSettings.HairCuttingSide.Beginning:
                            hairCuttingComboBox.SelectedIndex = 1;
                            break;
                        case AreaSettings.HairCuttingSide.End:
                            hairCuttingComboBox.SelectedIndex = 2;
                            break;
                        case AreaSettings.HairCuttingSide.BothSides:
                            hairCuttingComboBox.SelectedIndex = 3;
                            break;
                    }

                    //kierunek - offset - label
                    Label hairDirectionOffsetLabel = new Label();
                    hairDirectionOffsetLabel.Name = $"hairDirectionOffsetLabel_{i}";
                    hairDirectionOffsetLabel.Text = "Offset X,Y (px):";
                    hairDirectionOffsetLabel.TextAlign = ContentAlignment.MiddleLeft;
                    hairDirectionOffsetLabel.Font = controlFont;
                    hairDirectionOffsetLabel.AutoSize = false;
                    hairDirectionOffsetLabel.Width = 90;
                    hairDirectionOffsetLabel.Height = 20;
                    hairDirectionOffsetLabel.BorderStyle = BorderStyle.None;
                    hairDirectionOffsetLabel.Location = new Point(hairDirectionComboBox.Location.X + hairDirectionComboBox.Width + 5, 20);

                    //kierunek - offsetX - numeric up down
                    NumericUpDown hairDirectionOffsetXNumericUpDown = new NumericUpDown();
                    hairDirectionOffsetXNumericUpDown.Name = $"hairDirectionOffsetXNumericUpDown_{i}";
                    hairDirectionOffsetXNumericUpDown.Width = 50;
                    hairDirectionOffsetXNumericUpDown.DecimalPlaces = 0;
                    hairDirectionOffsetXNumericUpDown.Increment = 1.00M;
                    hairDirectionOffsetXNumericUpDown.Minimum = 0.00M;
                    hairDirectionOffsetXNumericUpDown.Maximum = 99999.00M;
                    hairDirectionOffsetXNumericUpDown.Font = controlFont;
                    hairDirectionOffsetXNumericUpDown.Location = new Point
                    (
                        hairDirectionOffsetLabel.Location.X + hairDirectionOffsetLabel.Width + 5,
                        20
                    );
                    hairDirectionOffsetXNumericUpDown.Value = areaSettings.HairDirectionOffset.X;
                    hairDirectionOffsetXNumericUpDown.Enabled = 
                    (
                        areaSettings.Direction == AreaSettings.HairDrawingDirection.Radial ||
                        areaSettings.Direction == AreaSettings.HairDrawingDirection.LinearY
                    );

                    //kierunek - offsetY - numeric up down
                    NumericUpDown hairDirectionOffsetYNumericUpDown = new NumericUpDown();
                    hairDirectionOffsetYNumericUpDown.Name = $"hairDirectionOffsetYNumericUpDown_{i}";
                    hairDirectionOffsetYNumericUpDown.Width = 50;
                    hairDirectionOffsetYNumericUpDown.DecimalPlaces = 0;
                    hairDirectionOffsetYNumericUpDown.Increment = 1.00M;
                    hairDirectionOffsetYNumericUpDown.Minimum = 0.00M;
                    hairDirectionOffsetYNumericUpDown.Maximum = 99999.00M;
                    hairDirectionOffsetYNumericUpDown.Font = controlFont;
                    hairDirectionOffsetYNumericUpDown.Location = new Point
                    (
                        hairDirectionOffsetXNumericUpDown.Location.X + hairDirectionOffsetXNumericUpDown.Width + 5,
                        hairDirectionOffsetXNumericUpDown.Location.Y
                    );
                    hairDirectionOffsetYNumericUpDown.Value = areaSettings.HairDirectionOffset.Y;
                    hairDirectionOffsetYNumericUpDown.Enabled =
                    (
                        areaSettings.Direction == AreaSettings.HairDrawingDirection.Radial ||
                        areaSettings.Direction == AreaSettings.HairDrawingDirection.LinearX
                    );

                    //kierunek - % przycięcia - label
                    Label cuttingPercentLabel = new Label();
                    cuttingPercentLabel.Name = $"cuttingPercentLabel_{i}";
                    cuttingPercentLabel.Text = "Przycięcie (%):";
                    cuttingPercentLabel.TextAlign = ContentAlignment.MiddleLeft;
                    cuttingPercentLabel.Font = controlFont;
                    cuttingPercentLabel.AutoSize = false;
                    cuttingPercentLabel.Width = hairDirectionOffsetLabel.Width;
                    cuttingPercentLabel.Height = hairDirectionOffsetLabel.Height;
                    cuttingPercentLabel.BorderStyle = BorderStyle.None;
                    cuttingPercentLabel.Location = new Point(hairDirectionOffsetLabel.Location.X, 50);

                    //kierunek - % przycięcia - numeric up down
                    NumericUpDown cuttingPercentNumericUpDown = new NumericUpDown();
                    cuttingPercentNumericUpDown.Name = $"cuttingPercentNumericUpDown_{i}";
                    cuttingPercentNumericUpDown.Width = 70;
                    cuttingPercentNumericUpDown.DecimalPlaces = 2;
                    cuttingPercentNumericUpDown.Increment = 1.00M;
                    cuttingPercentNumericUpDown.Minimum = 0.00M;
                    cuttingPercentNumericUpDown.Maximum = 50.00M;
                    cuttingPercentNumericUpDown.Font = controlFont;
                    cuttingPercentNumericUpDown.Location = new Point
                    (
                        cuttingPercentLabel.Location.X + cuttingPercentLabel.Width + 5,
                        cuttingPercentLabel.Location.Y
                    );
                    cuttingPercentNumericUpDown.Value = areaSettings.CuttingRangePercent;
                    cuttingPercentNumericUpDown.Enabled = (areaSettings.CuttingSide != AreaSettings.HairCuttingSide.None);

                    //kierunek - button zmiany
                    Button dirChangeEverywhereButton = new Button();
                    dirChangeEverywhereButton.Name = $"dirChangeEverywhereButton_{i}";
                    dirChangeEverywhereButton.Text = "Zmień wszędzie";
                    dirChangeEverywhereButton.Font = controlFont;
                    dirChangeEverywhereButton.BackColor = Color.FromArgb(255, 224, 192);
                    dirChangeEverywhereButton.Width = 100;
                    dirChangeEverywhereButton.Height = 20;
                    dirChangeEverywhereButton.Location = new Point(cuttingPercentNumericUpDown.Location.X, 80);

                    #endregion

                    #region Sekcja linii beziera

                    //bezier - group box
                    GroupBox bezierGroupBox = new GroupBox();
                    bezierGroupBox.Name = $"bezierGroupBox_{i}";
                    bezierGroupBox.Text = "Krzywe beziera";
                    bezierGroupBox.Width = 500;
                    bezierGroupBox.Height = 110;
                    bezierGroupBox.Font = controlFont;
                    bezierGroupBox.Location = new Point
                    (
                        directionGroupBox.Location.X + directionGroupBox.Width + 5,
                        directionGroupBox.Location.Y
                    );

                    //bezier typ - label
                    Label bezierTypeLabel = new Label();
                    bezierTypeLabel.Name = $"bezierTypeLabel_{i}";
                    bezierTypeLabel.Text = "Typ:";
                    bezierTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
                    bezierTypeLabel.Font = controlFont;
                    bezierTypeLabel.AutoSize = false;
                    bezierTypeLabel.Width = 50;
                    bezierTypeLabel.Height = 20;
                    bezierTypeLabel.BorderStyle = BorderStyle.None;
                    bezierTypeLabel.Location = new Point(10, 20);

                    //bezier typ - combo box
                    ComboBox bezierTypeComboBox = new ComboBox();
                    bezierTypeComboBox.Name = $"bezierTypeComboBox_{i}";
                    bezierTypeComboBox.Font = controlFont;
                    bezierTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    bezierTypeComboBox.Width = 200;
                    bezierTypeComboBox.Location = new Point
                    (
                        bezierTypeLabel.Location.X + bezierTypeLabel.Width + 5,
                        bezierTypeLabel.Location.Y
                    );

                    bezierTypeComboBox.Items.Add("Kwadratowy");
                    bezierTypeComboBox.Items.Add("Kwadratowy (z punktem przejścia linii)");
                    bezierTypeComboBox.Items.Add("Sześcienny");
                    bezierTypeComboBox.Items.Add("Wielopunktowy");

                    switch (areaSettings.BezierLineType)
                    {
                        case AreaSettings.BezierType.Quadratic:
                            bezierTypeComboBox.SelectedIndex = 0;
                            break;
                        case AreaSettings.BezierType.QuadraticCp:
                            bezierTypeComboBox.SelectedIndex = 1;
                            break;
                        case AreaSettings.BezierType.Cubic:
                            bezierTypeComboBox.SelectedIndex = 2;
                            break;
                        case AreaSettings.BezierType.Multi:
                            bezierTypeComboBox.SelectedIndex = 3;
                            break;
                    }

                    //ilość punktów - label
                    Label bezierPointsCountLabel = new Label();
                    bezierPointsCountLabel.Name = $"bezierPtsCountLabel_{i}";
                    bezierPointsCountLabel.Text = "Ilość punktów:";
                    bezierPointsCountLabel.TextAlign = ContentAlignment.MiddleLeft;
                    bezierPointsCountLabel.Font = controlFont;
                    bezierPointsCountLabel.AutoSize = false;
                    bezierPointsCountLabel.Width = 100;
                    bezierPointsCountLabel.Height = 20;
                    bezierPointsCountLabel.BorderStyle = BorderStyle.None;
                    bezierPointsCountLabel.Location = new Point
                    (
                        bezierTypeComboBox.Location.X + bezierTypeComboBox.Width + 5,
                        bezierTypeComboBox.Location.Y
                    );

                    //ilość punktów - numeric up down
                    NumericUpDown bezierPointsCountNmrUpDown = new NumericUpDown();
                    bezierPointsCountNmrUpDown.Name = $"bezierPtsNmrUpDown_{i}";
                    bezierPointsCountNmrUpDown.Width = 70;
                    bezierPointsCountNmrUpDown.DecimalPlaces = 0;
                    bezierPointsCountNmrUpDown.Increment = 1;
                    bezierPointsCountNmrUpDown.Minimum = 4;
                    bezierPointsCountNmrUpDown.Maximum = 99999;
                    bezierPointsCountNmrUpDown.Font = controlFont;
                    bezierPointsCountNmrUpDown.Location = new Point
                    (
                        bezierPointsCountLabel.Location.X + bezierPointsCountLabel.Width + 5,
                        bezierPointsCountLabel.Location.Y
                    );
                    bezierPointsCountNmrUpDown.Value = (areaSettings.MultiBezierPointCount < 4 ? 4 : areaSettings.MultiBezierPointCount);
                    bezierPointsCountNmrUpDown.Enabled = (areaSettings.BezierLineType == AreaSettings.BezierType.Multi);

                    //zakres odchylenia losowego - label
                    Label bezierRandomRangeLabel = new Label();
                    bezierRandomRangeLabel.Name = $"bezierRandomRangeLabel_{i}";
                    bezierRandomRangeLabel.Text = "Zakres odchylenia losowego (%):";
                    bezierRandomRangeLabel.TextAlign = ContentAlignment.MiddleLeft;
                    bezierRandomRangeLabel.Font = controlFont;
                    bezierRandomRangeLabel.AutoSize = false;
                    bezierRandomRangeLabel.Width = 180;
                    bezierRandomRangeLabel.Height = 20;
                    bezierRandomRangeLabel.BorderStyle = BorderStyle.None;
                    bezierRandomRangeLabel.Location = new Point(10, 50);

                    //zakres odchylenia losowego - numeric up down
                    NumericUpDown bezierRandomRangeNmrUpDown = new NumericUpDown();
                    bezierRandomRangeNmrUpDown.Name = $"bezierRandomRangeNmrUpDown_{i}";
                    bezierRandomRangeNmrUpDown.Width = 70;
                    bezierRandomRangeNmrUpDown.DecimalPlaces = 2;
                    bezierRandomRangeNmrUpDown.Increment = 1.00M;
                    bezierRandomRangeNmrUpDown.Minimum = 0.00M;
                    bezierRandomRangeNmrUpDown.Maximum = 100.00M;
                    bezierRandomRangeNmrUpDown.Font = controlFont;
                    bezierRandomRangeNmrUpDown.Location = new Point
                    (
                        bezierRandomRangeLabel.Location.X + bezierRandomRangeLabel.Width + 5,
                        bezierRandomRangeLabel.Location.Y
                    );
                    bezierRandomRangeNmrUpDown.Value = areaSettings.BezierRandomRangePercent;

                    //margines - label
                    Label bezierMarginLabel = new Label();
                    bezierMarginLabel.Name = $"bezierMarginLabel_{i}";
                    bezierMarginLabel.Text = "Margines (%):";
                    bezierMarginLabel.TextAlign = ContentAlignment.MiddleLeft;
                    bezierMarginLabel.Font = controlFont;
                    bezierMarginLabel.AutoSize = false;
                    bezierMarginLabel.Width = 100;
                    bezierMarginLabel.Height = 20;
                    bezierMarginLabel.BorderStyle = BorderStyle.None;
                    bezierMarginLabel.Location = new Point
                    (
                        bezierRandomRangeNmrUpDown.Location.X + bezierRandomRangeNmrUpDown.Width + 5,
                        bezierRandomRangeNmrUpDown.Location.Y
                    );

                    //margines - numeric up down
                    NumericUpDown bezierMarginNumericUpDown = new NumericUpDown();
                    bezierMarginNumericUpDown.Name = $"bezierMarginNmrUpDown_{i}";
                    bezierMarginNumericUpDown.Width = 70;
                    bezierMarginNumericUpDown.DecimalPlaces = 2;
                    bezierMarginNumericUpDown.Increment = 1.00M;
                    bezierMarginNumericUpDown.Minimum = -50.00M;
                    bezierMarginNumericUpDown.Maximum = 50.00M;
                    bezierMarginNumericUpDown.Font = controlFont;
                    bezierMarginNumericUpDown.Location = new Point
                    (
                        bezierMarginLabel.Location.X + bezierMarginLabel.Width + 5,
                        bezierMarginLabel.Location.Y
                    );
                    bezierMarginNumericUpDown.Value = areaSettings.BezierMarginPercent;

                    //bezier - button zmiany
                    Button bezierChangeEverywhereButton = new Button();
                    bezierChangeEverywhereButton.Name = $"bezierChangeEverywhereButton_{i}";
                    bezierChangeEverywhereButton.Text = "Zmień wszędzie";
                    bezierChangeEverywhereButton.Font = controlFont;
                    bezierChangeEverywhereButton.BackColor = Color.FromArgb(255, 224, 192);
                    bezierChangeEverywhereButton.Width = 100;
                    bezierChangeEverywhereButton.Height = 20;
                    bezierChangeEverywhereButton.Location = new Point(bezierMarginNumericUpDown.Location.X, 80);

                    #endregion

                    #region Sekcja parametrów

                    //parametry - group box
                    GroupBox paramsGroupBox = new GroupBox();
                    paramsGroupBox.Name = $"paramsGroupBox_{i}";
                    paramsGroupBox.Text = "Parametry";
                    paramsGroupBox.Width = 470;
                    paramsGroupBox.Height = 110;
                    paramsGroupBox.Font = controlFont;
                    paramsGroupBox.Location = new Point
                    (
                        bezierGroupBox.Location.X + bezierGroupBox.Width + 5,
                        bezierGroupBox.Location.Y
                    );

                    //parametry - przezroczystość mapy - check box
                    CheckBox hairMapOpacityCheckBox = new CheckBox();
                    hairMapOpacityCheckBox.Name = $"hairMapOpacityCheckBox_{i}";
                    hairMapOpacityCheckBox.Text = "Mapa rozkładu definiuje przezroczystość";
                    hairMapOpacityCheckBox.Font = controlFont;
                    hairMapOpacityCheckBox.Checked = areaSettings.HairMapDefinesOpacity;
                    hairMapOpacityCheckBox.AutoSize = false;
                    hairMapOpacityCheckBox.Width = 250;
                    hairMapOpacityCheckBox.Height = 20;
                    hairMapOpacityCheckBox.Location = new Point(10, 20);

                    //parametry - jasność - check box
                    CheckBox hairMapBrightnessCheckBox = new CheckBox();
                    hairMapBrightnessCheckBox.Name = $"hairMapBrightnessCheckBox_{i}";
                    hairMapBrightnessCheckBox.Text = "Mapa rozkładu definiuje jasność";
                    hairMapBrightnessCheckBox.Font = controlFont;
                    hairMapBrightnessCheckBox.Checked = areaSettings.HairMapDefinesBrightness;
                    hairMapBrightnessCheckBox.AutoSize = false;
                    hairMapBrightnessCheckBox.Width = 250;
                    hairMapBrightnessCheckBox.Height = 20;
                    hairMapBrightnessCheckBox.Location = new Point(10, 50);

                    //parametry - iteracje - label
                    Label paramIterationsLabel = new Label();
                    paramIterationsLabel.Name = $"paramIterationsLabel_{i}";
                    paramIterationsLabel.Text = "Iteracje:";
                    paramIterationsLabel.TextAlign = ContentAlignment.MiddleLeft;
                    paramIterationsLabel.Font = controlFont;
                    paramIterationsLabel.AutoSize = false;
                    paramIterationsLabel.Width = 60;
                    paramIterationsLabel.Height = 20;
                    paramIterationsLabel.BorderStyle = BorderStyle.None;
                    paramIterationsLabel.Location = new Point
                    (
                        hairMapOpacityCheckBox.Location.X + hairMapOpacityCheckBox.Width + 5,
                        hairMapOpacityCheckBox.Location.Y
                    );

                    //parametry - iteracje - numeric up down
                    NumericUpDown paramIterationsNumericUpDown = new NumericUpDown();
                    paramIterationsNumericUpDown.Name = $"paramIterationsNmrUpDown_{i}";
                    paramIterationsNumericUpDown.Width = 70;
                    paramIterationsNumericUpDown.DecimalPlaces = 0;
                    paramIterationsNumericUpDown.Increment = 1;
                    paramIterationsNumericUpDown.Minimum = 1;
                    paramIterationsNumericUpDown.Maximum = 999999;
                    paramIterationsNumericUpDown.Font = controlFont;
                    paramIterationsNumericUpDown.Location = new Point
                    (
                        paramIterationsLabel.Location.X + paramIterationsLabel.Width + 5,
                        paramIterationsLabel.Location.Y
                    );
                    paramIterationsNumericUpDown.Value = areaSettings.DrawingIterations;

                    //parametry - kroki rysowania - label
                    Label paramDrawingStepsLabel = new Label();
                    paramDrawingStepsLabel.Name = $"paramDrawingStepsLabel_{i}";
                    paramDrawingStepsLabel.Text = "Kroki:";
                    paramDrawingStepsLabel.TextAlign = ContentAlignment.MiddleLeft;
                    paramDrawingStepsLabel.Font = controlFont;
                    paramDrawingStepsLabel.AutoSize = false;
                    paramDrawingStepsLabel.Width = 60;
                    paramDrawingStepsLabel.Height = 20;
                    paramDrawingStepsLabel.BorderStyle = BorderStyle.None;
                    paramDrawingStepsLabel.Location = new Point
                    (
                        hairMapBrightnessCheckBox.Location.X + hairMapBrightnessCheckBox.Width + 5,
                        hairMapBrightnessCheckBox.Location.Y
                    );

                    //parametry - kroki rysowania - numeric up down
                    NumericUpDown paramDrawingStepsNumericUpDown = new NumericUpDown();
                    paramDrawingStepsNumericUpDown.Name = $"paramDrawingStepsNmrUpDown_{i}";
                    paramDrawingStepsNumericUpDown.Width = 70;
                    paramDrawingStepsNumericUpDown.DecimalPlaces = 0;
                    paramDrawingStepsNumericUpDown.Increment = 1;
                    paramDrawingStepsNumericUpDown.Minimum = 1;
                    paramDrawingStepsNumericUpDown.Maximum = 999999;
                    paramDrawingStepsNumericUpDown.Font = controlFont;
                    paramDrawingStepsNumericUpDown.Location = new Point
                    (
                        paramDrawingStepsLabel.Location.X + paramDrawingStepsLabel.Width + 5,
                        paramDrawingStepsLabel.Location.Y
                    );
                    paramDrawingStepsNumericUpDown.Value = areaSettings.DrawingSteps;

                    //parametry - procent strat - label
                    Label paramStepIterLossLabel = new Label();
                    paramStepIterLossLabel.Name = $"paramStepIterLossLabel_{i}";
                    paramStepIterLossLabel.Text = "Procent strat iteracji w kolejnych krokach:";
                    paramStepIterLossLabel.TextAlign = ContentAlignment.MiddleLeft;
                    paramStepIterLossLabel.Font = controlFont;
                    paramStepIterLossLabel.AutoSize = false;
                    paramStepIterLossLabel.Width = 220;
                    paramStepIterLossLabel.Height = 20;
                    paramStepIterLossLabel.BorderStyle = BorderStyle.None;
                    paramStepIterLossLabel.Location = new Point(10, 80);

                    //parametry - procent strat - numeric up down
                    NumericUpDown paramStepIterLossNumericUpDown = new NumericUpDown();
                    paramStepIterLossNumericUpDown.Name = $"paramStepIterLossNmrUpDown_{i}";
                    paramStepIterLossNumericUpDown.Width = 70;
                    paramStepIterLossNumericUpDown.DecimalPlaces = 2;
                    paramStepIterLossNumericUpDown.Increment = 1.00M;
                    paramStepIterLossNumericUpDown.Minimum = 0.00M;
                    paramStepIterLossNumericUpDown.Maximum = 50.00M;
                    paramStepIterLossNumericUpDown.Font = controlFont;
                    paramStepIterLossNumericUpDown.Location = new Point
                    (
                        paramStepIterLossLabel.Location.X + paramStepIterLossLabel.Width + 5,
                        paramStepIterLossLabel.Location.Y
                    );
                    paramStepIterLossNumericUpDown.Value = areaSettings.DrawingStepIterationLossPercent;

                    //parametry - button zmiany
                    Button paramsChangeEverywhereButton = new Button();
                    paramsChangeEverywhereButton.Name = $"paramChangeEverywhereButton_{i}";
                    paramsChangeEverywhereButton.Text = "Zmień wszędzie";
                    paramsChangeEverywhereButton.BackColor = Color.FromArgb(255, 224, 192);
                    paramsChangeEverywhereButton.Width = 100;
                    paramsChangeEverywhereButton.Height = 20;
                    paramsChangeEverywhereButton.Font = controlFont;
                    paramsChangeEverywhereButton.Location = new Point
                    (
                        paramStepIterLossNumericUpDown.Location.X + paramStepIterLossNumericUpDown.Width + 25, 
                        80
                    );

                    #endregion

                    #region Sekcja generowania

                    //generowanie - group box
                    GroupBox generationGroupBox = new GroupBox();
                    generationGroupBox.Name = $"generationGroupBox_{i}";
                    generationGroupBox.Text = "Generowanie";
                    generationGroupBox.Width = 250;
                    generationGroupBox.Height = 110;
                    generationGroupBox.Font = controlFont;
                    generationGroupBox.Location = new Point
                    (
                        paramsGroupBox.Location.X + paramsGroupBox.Width + 5,
                        paramsGroupBox.Location.Y
                    );

                    //generowanie - button strefy
                    Button generateForAreaButton = new Button();
                    generateForAreaButton.Name = $"generateForAreaButton_{i}";
                    generateForAreaButton.Text = "Generuj teksturę dla strefy";
                    generateForAreaButton.Font = controlFont;
                    generateForAreaButton.BackColor = Color.FromArgb(255, 224, 192);
                    generateForAreaButton.Width = 230;
                    generateForAreaButton.Height = 30;
                    generateForAreaButton.Location = new Point(10, 25);

                    //generowanie - button wszystkich stref
                    Button generateEverywhereButton = new Button();
                    generateEverywhereButton.Name = $"generateEverywhereButton_{i}";
                    generateEverywhereButton.Text = "Generuj teksturę dla wszystkich stref";
                    generateEverywhereButton.Font = controlFont;
                    generateEverywhereButton.BackColor = Color.FromArgb(255, 224, 192);
                    generateEverywhereButton.Width = 230;
                    generateEverywhereButton.Height = 30;
                    generateEverywhereButton.Location = new Point
                    (
                        generateForAreaButton.Location.X,
                        generateForAreaButton.Location.Y + generateForAreaButton.Height + 5
                    );

                    #endregion

                    #endregion

                    #region Przypisywanie eventów do kontrolek
                    //PRZYPISYWANIE EVENTÓW
                    colorPaletteColor1Button.Click += new EventHandler(colorPaletteColor1Button_Click);
                    colorPaletteColor2Button.Click += new EventHandler(colorPaletteColor2Button_Click);
                    colorPaletteChangeEverywhereButton.Click += new EventHandler(colorPaletteChangeEverywhereButton_Click);

                    hairDirectionComboBox.SelectedIndexChanged += new EventHandler(hairDirectionComboBox_SelectedIndexChanged);
                    hairDirectionOffsetXNumericUpDown.ValueChanged += new EventHandler(hairDirectionOffsetNumericUpDown_ValueChanged);
                    hairDirectionOffsetYNumericUpDown.ValueChanged += new EventHandler(hairDirectionOffsetNumericUpDown_ValueChanged);
                    hairCuttingComboBox.SelectedIndexChanged += new EventHandler(hairCuttingComboBox_SelectedIndexChanged);
                    cuttingPercentNumericUpDown.ValueChanged += new EventHandler(cuttingPercentNumericUpDown_ValueChanged);
                    dirChangeEverywhereButton.Click += new EventHandler(dirChangeEverywhereButton_Click);

                    bezierTypeComboBox.SelectedIndexChanged += new EventHandler(bezierTypeComboBox_SelectedIndexChanged);
                    bezierPointsCountNmrUpDown.ValueChanged += new EventHandler(bezierPointsCountNmrUpDown_ValueChanged);
                    bezierRandomRangeNmrUpDown.ValueChanged += new EventHandler(bezierRandomRangeNmrUpDown_ValueChanged);
                    bezierMarginNumericUpDown.ValueChanged += new EventHandler(bezierMarginNumericUpDown_ValueChanged);
                    bezierChangeEverywhereButton.Click += new EventHandler(bezierChangeEverywhereButton_Click);

                    hairMapOpacityCheckBox.CheckedChanged += new EventHandler(hairMapOpacityCheckBox_CheckedChanged);
                    hairMapBrightnessCheckBox.CheckedChanged += new EventHandler(hairMapBrightnessCheckBox_CheckedChanged);
                    paramIterationsNumericUpDown.ValueChanged += new EventHandler(paramIterationsNumericUpDown_ValueChanged);
                    paramDrawingStepsNumericUpDown.ValueChanged += new EventHandler(paramDrawingStepsNumericUpDown_ValueChanged);
                    paramStepIterLossNumericUpDown.ValueChanged += new EventHandler(paramStepIterLossNumericUpDown_ValueChanged);
                    paramsChangeEverywhereButton.Click += new EventHandler(paramsChangeEverywhereButton_Click);

                    generateForAreaButton.Click += new EventHandler(generateForAreaButton_Click);
                    generateEverywhereButton.Click += new EventHandler(generateEverywhereButton_Click);

                    #endregion

                    #region Dodawanie kontrolek na panel
                    //DODAWANIE KONTROLEK NA PANEL
                    mainAreaPanel.Controls.Add(areaIdLabel);
                    
                    colorPaletteGroupBox.Controls.Add(colorPaletteColor1Label);
                    colorPaletteGroupBox.Controls.Add(colorPaletteColor1Button);
                    colorPaletteGroupBox.Controls.Add(colorPaletteColor2Label);
                    colorPaletteGroupBox.Controls.Add(colorPaletteColor2Button);
                    colorPaletteGroupBox.Controls.Add(colorPaletteChangeEverywhereButton);
                    mainAreaPanel.Controls.Add(colorPaletteGroupBox);

                    directionGroupBox.Controls.Add(hairDirectionLabel);
                    directionGroupBox.Controls.Add(hairDirectionComboBox);
                    directionGroupBox.Controls.Add(hairDirectionOffsetLabel);
                    directionGroupBox.Controls.Add(hairDirectionOffsetXNumericUpDown);
                    directionGroupBox.Controls.Add(hairDirectionOffsetYNumericUpDown);
                    directionGroupBox.Controls.Add(hairCuttingLabel);
                    directionGroupBox.Controls.Add(hairCuttingComboBox);
                    directionGroupBox.Controls.Add(cuttingPercentLabel);
                    directionGroupBox.Controls.Add(cuttingPercentNumericUpDown);
                    directionGroupBox.Controls.Add(dirChangeEverywhereButton);
                    mainAreaPanel.Controls.Add(directionGroupBox);

                    bezierGroupBox.Controls.Add(bezierTypeLabel);
                    bezierGroupBox.Controls.Add(bezierTypeComboBox);
                    bezierGroupBox.Controls.Add(bezierPointsCountLabel);
                    bezierGroupBox.Controls.Add(bezierPointsCountNmrUpDown);
                    bezierGroupBox.Controls.Add(bezierRandomRangeLabel);
                    bezierGroupBox.Controls.Add(bezierRandomRangeNmrUpDown);
                    bezierGroupBox.Controls.Add(bezierMarginLabel);
                    bezierGroupBox.Controls.Add(bezierMarginNumericUpDown);
                    bezierGroupBox.Controls.Add(bezierChangeEverywhereButton);
                    mainAreaPanel.Controls.Add(bezierGroupBox);

                    paramsGroupBox.Controls.Add(hairMapOpacityCheckBox);
                    paramsGroupBox.Controls.Add(hairMapBrightnessCheckBox);
                    paramsGroupBox.Controls.Add(paramIterationsLabel);
                    paramsGroupBox.Controls.Add(paramIterationsNumericUpDown);
                    paramsGroupBox.Controls.Add(paramDrawingStepsLabel);
                    paramsGroupBox.Controls.Add(paramDrawingStepsNumericUpDown);
                    paramsGroupBox.Controls.Add(paramStepIterLossLabel);
                    paramsGroupBox.Controls.Add(paramStepIterLossNumericUpDown);
                    paramsGroupBox.Controls.Add(paramsChangeEverywhereButton);
                    mainAreaPanel.Controls.Add(paramsGroupBox);

                    generationGroupBox.Controls.Add(generateForAreaButton);
                    generationGroupBox.Controls.Add(generateEverywhereButton);
                    mainAreaPanel.Controls.Add(generationGroupBox);

                    this.areaMenuPanel.Controls.Add(mainAreaPanel);

                    #endregion

                    #region Przypisywanie referencji kontrolek do kontenera
                    //PRZYPISYWANIE KONTROLEK DO KONTENERA
                    HairGenUiReferenceContainer referenceContainer = new HairGenUiReferenceContainer();

                    referenceContainer.MainAreaPanel = mainAreaPanel;
                    referenceContainer.HairDirOffsetXNumericUpDown = hairDirectionOffsetXNumericUpDown;
                    referenceContainer.HairDirOffsetYNumericUpDown = hairDirectionOffsetYNumericUpDown;
                    referenceContainer.HairCuttingPercentNumericUpDown = cuttingPercentNumericUpDown;
                    referenceContainer.BezierPointsCountNumericUpDown = bezierPointsCountNmrUpDown;

                    this.referenceContainerList.Add(referenceContainer);

                    #endregion
                }
                
                areaSettings = null;
                success = true;
            }
            catch (Exception exception)
            {
                throw new Exception($"(HairGenUiController) Błąd ładowania panelu ustawień stref: {exception.Message}");
            }

            return success;
        }

        #endregion

        #region Eventy
        
        private void generateEverywhereButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Rozpocząć?", $"Generowanie dla wszystkich stref", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;

                #endregion

                this.parentForm.GenerateTexture(boundsCheckerCheckBox.Checked);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(generateEverywhereButton_Click) Błąd: {exception.Message}");
            }
        }

        private void generateForAreaButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                int areaId = this.settingsList[index].AreaId;

                if (areaId < 0)
                    throw new Exception($"nie odczytano ID strefy dla indeksu [{index}]");

                #endregion

                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Rozpocząć?", $"Generowanie dla strefy [{areaId}]", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;

                #endregion

                this.parentForm.GenerateTexture(boundsCheckerCheckBox.Checked, areaId);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(generateForAreaButton_Click) Błąd: {exception.Message}");
            }
        }

        private void paramsChangeEverywhereButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Jesteś pewien?", "Parametry", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                bool hairMapDefinesOpacity = this.settingsList[index].HairMapDefinesOpacity;
                bool hairMapDefinesBrightness = this.settingsList[index].HairMapDefinesBrightness;
                int drawingIterations = this.settingsList[index].DrawingIterations;
                int drawingSteps = this.settingsList[index].DrawingSteps;
                decimal drawingStepIterationLossPercent = this.settingsList[index].DrawingStepIterationLossPercent;

                foreach (AreaSettings areaSettings in this.settingsList)
                {
                    areaSettings.HairMapDefinesOpacity = hairMapDefinesOpacity;
                    areaSettings.HairMapDefinesBrightness = hairMapDefinesBrightness;
                    areaSettings.DrawingIterations = drawingIterations;
                    areaSettings.DrawingSteps = drawingSteps;
                    areaSettings.DrawingStepIterationLossPercent = drawingStepIterationLossPercent;
                }

                ReloadAreaPanel();

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(paramsChangeEverywhereButton_Click) Błąd: {exception.Message}");
            }
        }

        private void paramStepIterLossNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].DrawingStepIterationLossPercent = Convert.ToInt32(nmrUpDown.Value);

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(paramStepIterLossNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void paramDrawingStepsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].DrawingSteps = Convert.ToInt32(nmrUpDown.Value);

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(paramDrawingStepsNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void paramIterationsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].DrawingIterations = Convert.ToInt32(nmrUpDown.Value);

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(paramIterationsNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void hairMapBrightnessCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(CheckBox))
                    throw new Exception($"przekazano kontrolkę innego typu niż CheckBox. Typ [{sender.GetType().ToString()}]");

                CheckBox chkBox = (CheckBox)sender;
                string chkBoxName = chkBox.Name;
                int index = GetIndexFromName(chkBoxName);
                bool isChecked = chkBox.Checked;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{chkBoxName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].HairMapDefinesBrightness = isChecked;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(hairMapBrightnessCheckBox_CheckedChanged) Błąd: {exception.Message}");
            }
        }

        private void hairMapOpacityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(CheckBox))
                    throw new Exception($"przekazano kontrolkę innego typu niż CheckBox. Typ [{sender.GetType().ToString()}]");

                CheckBox chkBox = (CheckBox)sender;
                string chkBoxName = chkBox.Name;
                int index = GetIndexFromName(chkBoxName);
                bool isChecked = chkBox.Checked;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{chkBoxName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].HairMapDefinesOpacity = isChecked;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(hairMapOpacityCheckBox_Click) Błąd: {exception.Message}");
            }
        }

        private void bezierChangeEverywhereButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Jesteś pewien?", "Krzywe Beziera", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                AreaSettings.BezierType bezierType = this.settingsList[index].BezierLineType;
                int bezierPointCount = this.settingsList[index].MultiBezierPointCount;
                decimal randomRangePercent = this.settingsList[index].BezierRandomRangePercent;
                decimal marginPercent = this.settingsList[index].BezierMarginPercent;

                foreach (AreaSettings areaSettings in this.settingsList)
                {
                    areaSettings.BezierLineType = bezierType;
                    areaSettings.MultiBezierPointCount = bezierPointCount;
                    areaSettings.BezierRandomRangePercent = randomRangePercent;
                    areaSettings.BezierMarginPercent = marginPercent;
                }

                ReloadAreaPanel();

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(bezierChangeEverywhereButton_Click) Błąd: {exception.Message}");
            }
        }

        private void bezierMarginNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);
                decimal value = nmrUpDown.Value;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].BezierMarginPercent = value;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(bezierMarginNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void bezierRandomRangeNmrUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);
                decimal value = nmrUpDown.Value;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].BezierRandomRangePercent = value;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(bezierRandomRangeNmrUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void bezierPointsCountNmrUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);
                int value = Convert.ToInt32(nmrUpDown.Value);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].MultiBezierPointCount = value;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(bezierPointsCountNmrUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void bezierTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(ComboBox))
                    throw new Exception($"przekazano kontrolkę innego typu niż ComboBox. Typ [{sender.GetType().ToString()}]");

                ComboBox combo = (ComboBox)sender;
                string comboName = combo.Name;
                int index = GetIndexFromName(comboName);
                int selectedIndex = combo.SelectedIndex;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{comboName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                AreaSettings.BezierType bezierType = AreaSettings.BezierType.QuadraticCp;

                switch(selectedIndex)
                {
                    case 0:
                        bezierType = AreaSettings.BezierType.Quadratic;
                        break;
                    case 1:
                        bezierType = AreaSettings.BezierType.QuadraticCp;
                        break;
                    case 2:
                        bezierType = AreaSettings.BezierType.Cubic;
                        break;
                    case 3:
                        bezierType = AreaSettings.BezierType.Multi;
                        break;
                }

                this.settingsList[index].BezierLineType = bezierType;

                #endregion

                #region Aktywacja kontrolek
                //AKTYWACJA KONTROLEK
                this.referenceContainerList[index].BezierPointsCountNumericUpDown.Enabled = (bezierType == AreaSettings.BezierType.Multi);

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(bezierTypeComboBox_SelectedIndexChanged) Błąd: {exception.Message}");
            }
        }

        private void colorPaletteColor1Button_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Podmiana koloru
                //PODMIANA KOLORU
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.Color = button.BackColor;
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.SolidColorOnly = false;

                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        int colorR = colorDialog.Color.R;
                        int colorG = colorDialog.Color.G;
                        int colorB = colorDialog.Color.B;

                        this.settingsList[index].BackgroundRgbPalette = new Point3(colorR, colorG, colorB);
                        button.BackColor = Color.FromArgb(colorR, colorG, colorB);
                        //ReloadAreaPanel(); //do testu
                    }
                }

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(colorPaletteColor1Button_Click) Błąd: {exception.Message}");
            }
        }

        private void colorPaletteColor2Button_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Podmiana koloru
                //PODMIANA KOLORU
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.Color = button.BackColor;
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.SolidColorOnly = false;

                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        int colorR = colorDialog.Color.R;
                        int colorG = colorDialog.Color.G;
                        int colorB = colorDialog.Color.B;

                        settingsList[index].ForegroundRgbPalette = new Point3(colorR, colorG, colorB);
                        button.BackColor = Color.FromArgb(colorR, colorG, colorB);
                        //ReloadAreaPanel(); //do testu
                    }
                }

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(colorPaletteColor2Button_Click) Błąd: {exception.Message}");
            }
        }

        private void colorPaletteChangeEverywhereButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Jesteś pewien?", "Kolory", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;
                
                #endregion

                #region Podmiana kolorów
                //PODMIANA KOLORÓW
                int backgroundR = this.settingsList[index].BackgroundRgbPalette.X;
                int backgroundG = this.settingsList[index].BackgroundRgbPalette.Y;
                int backgroundB = this.settingsList[index].BackgroundRgbPalette.Z;

                int foregroundR = this.settingsList[index].ForegroundRgbPalette.X;
                int foregroundG = this.settingsList[index].ForegroundRgbPalette.Y;
                int foregroundB = this.settingsList[index].ForegroundRgbPalette.Z;

                foreach (AreaSettings areaSettings in this.settingsList)
                {
                    areaSettings.BackgroundRgbPalette = new Point3(backgroundR, backgroundG, backgroundB);
                    areaSettings.ForegroundRgbPalette = new Point3(foregroundR, foregroundG, foregroundB);
                }

                ReloadAreaPanel();

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(colorPaletteChangeEverywhereButton_Click) Błąd: {exception.Message}");
            }
        }

        private void hairDirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(ComboBox))
                    throw new Exception($"przekazano kontrolkę innego typu niż ComboBox. Typ [{sender.GetType().ToString()}]");

                ComboBox comboBox = (ComboBox)sender;
                string comboBoxName = comboBox.Name;
                int index = GetIndexFromName(comboBoxName);
                int selectedIndex = comboBox.SelectedIndex;

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{comboBoxName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                AreaSettings.HairDrawingDirection hairDrawingDirection = AreaSettings.HairDrawingDirection.BottomToTop;
                switch (selectedIndex)
                {
                    case 0:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.TopToBottom;
                        break;
                    case 1:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.LeftToRight;
                        break;
                    case 2:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.RightToLeft;
                        break;
                    case 3:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.BottomToTop;
                        break;
                    case 4:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.Radial;
                        break;
                    case 5:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.LinearX;
                        break;
                    case 6:
                        hairDrawingDirection = AreaSettings.HairDrawingDirection.LinearY;
                        break;
                }

                settingsList[index].Direction = hairDrawingDirection;

                #endregion

                #region Aktywacja kontrolek
                //AKTYWACJA KONTROLEK
                if (hairDrawingDirection == AreaSettings.HairDrawingDirection.Radial || hairDrawingDirection == AreaSettings.HairDrawingDirection.LinearY)
                {
                    this.referenceContainerList[index].HairDirOffsetXNumericUpDown.Enabled = true;
                }
                else
                {
                    this.referenceContainerList[index].HairDirOffsetXNumericUpDown.Enabled = false;
                }

                if (hairDrawingDirection == AreaSettings.HairDrawingDirection.Radial || hairDrawingDirection == AreaSettings.HairDrawingDirection.LinearX)
                {
                    this.referenceContainerList[index].HairDirOffsetYNumericUpDown.Enabled = true;
                }
                else
                {
                    this.referenceContainerList[index].HairDirOffsetYNumericUpDown.Enabled = false;
                }

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(hairDirectionComboBox_SelectedIndexChanged) Błąd: {exception.Message}");
            }
        }

        private void hairDirectionOffsetNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                decimal offsetX = this.referenceContainerList[index].HairDirOffsetXNumericUpDown.Value;
                decimal offsetY = this.referenceContainerList[index].HairDirOffsetYNumericUpDown.Value;

                this.settingsList[index].HairDirectionOffset = new Point
                (
                    Convert.ToInt32(offsetX),
                    Convert.ToInt32(offsetY)
                );

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(hairDirectionOffsetNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void dirChangeEverywhereButton_Click(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(Button))
                    throw new Exception($"przekazano kontrolkę innego typu niż Button. Typ [{sender.GetType().ToString()}]");

                Button button = (Button)sender;
                string buttonName = button.Name;
                int index = GetIndexFromName(buttonName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{buttonName}]");

                #endregion

                #region Okno dialogowe
                //OKNO DIALOGOWE
                DialogResult dialogRes = MessageBox.Show("Jesteś pewien?", "Ustawienia kierunku", MessageBoxButtons.YesNo);

                if (dialogRes != DialogResult.Yes)
                    return;

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                AreaSettings.HairDrawingDirection drawingDirection = this.settingsList[index].Direction;
                Point hairDirectionOffset = this.settingsList[index].HairDirectionOffset;
                AreaSettings.HairCuttingSide cuttingSide = this.settingsList[index].CuttingSide;
                decimal cuttingRangePercent = this.settingsList[index].CuttingRangePercent;

                foreach (AreaSettings areaSettings in this.settingsList)
                {
                    areaSettings.Direction = drawingDirection;
                    areaSettings.HairDirectionOffset = hairDirectionOffset;
                    areaSettings.CuttingSide = cuttingSide;
                    areaSettings.CuttingRangePercent = cuttingRangePercent;
                }

                ReloadAreaPanel();

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(dirChangeEverywhereButton_Click) Błąd: {exception.Message}");
            }
        }
        
        private void cuttingPercentNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(NumericUpDown))
                    throw new Exception($"przekazano kontrolkę innego typu niż NumericUpDown. Typ [{sender.GetType().ToString()}]");

                NumericUpDown nmrUpDown = (NumericUpDown)sender;
                string nmrUpDownName = nmrUpDown.Name;
                int index = GetIndexFromName(nmrUpDownName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{nmrUpDownName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                this.settingsList[index].CuttingRangePercent = nmrUpDown.Value;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(cuttingPercentNumericUpDown_ValueChanged) Błąd: {exception.Message}");
            }
        }

        private void hairCuttingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wstępne
                //WSTĘPNE
                if (sender.GetType() != typeof(ComboBox))
                    throw new Exception($"przekazano kontrolkę innego typu niż ComboBox. Typ [{sender.GetType().ToString()}]");

                ComboBox comboBox = (ComboBox)sender;
                string comboName = comboBox.Name;
                int index = GetIndexFromName(comboName);

                if (index < 0)
                    throw new Exception($"odczytano błędny indeks [{index}] z kontrolki [{comboName}]");

                #endregion

                #region Podmiana danych
                //PODMIANA DANYCH
                AreaSettings.HairCuttingSide hairCuttingSide = AreaSettings.HairCuttingSide.None;

                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        hairCuttingSide = AreaSettings.HairCuttingSide.None;
                        break;
                    case 1:
                        hairCuttingSide = AreaSettings.HairCuttingSide.Beginning;
                        break;
                    case 2:
                        hairCuttingSide = AreaSettings.HairCuttingSide.End;
                        break;
                    case 3:
                        hairCuttingSide = AreaSettings.HairCuttingSide.BothSides;
                        break;
                }

                this.settingsList[index].CuttingSide = hairCuttingSide;
                this.referenceContainerList[index].HairCuttingPercentNumericUpDown.Enabled = (hairCuttingSide != AreaSettings.HairCuttingSide.None);

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"(hairCuttingComboBox_SelectedIndexChanged) Błąd: {exception.Message}");
            }
        }

        #endregion
        
        private static int GetIndexFromName(string name)
        {
            int index = -1;

            try
            {
                if (String.IsNullOrWhiteSpace(name))
                    return index;

                if (!name.Contains('_'))
                    return index;

                string[] nameElements = name.Split('_');

                if (nameElements.Length != 2)
                    throw new Exception($"elementy oddzielone separatorem mają nieprawidłową ilość [{nameElements.Length}]");

                if (!Int32.TryParse(nameElements[1], out index))
                {
                    throw new Exception($"nie udało się przekonwertować elementu [{nameElements[1]}]");
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"błąd pozyskiwania indeksu z nazwy [{name}]: {exception.Message}");
            }

            return index;
        }
    }
}

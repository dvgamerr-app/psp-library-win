using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Reflection;

namespace PSPlibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String _ConfigName = "setting.cfg";
        String _ConfigFolder = "";
        String _ConfigStore = "";
        String _ConfigTemp = Path.GetTempPath();
        ConfigDialog _DialogConfig = new ConfigDialog();
        InitializeStore _DialogAbout = new InitializeStore();
        String _DataDriectory;
        SfoFile _ConfigDefault = new SfoFile();
        
        public MainWindow()
        {
            _ConfigFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\";
            _ConfigName = _ConfigFolder + _ConfigName;
            _ConfigStore = _ConfigFolder + "PSPStore\\";
            try
            {
                _ConfigDefault.Load(File.ReadAllBytes(_ConfigName));
            }
            catch
            {
                File.Delete(_ConfigName);
            }

            if (!File.Exists(_ConfigName))
            {
                _ConfigDefault.Param("X", "0");
                _ConfigDefault.Param("Y", "0");
                _ConfigDefault.Param("Width", "1050");
                _ConfigDefault.Param("Height", "687");
                _ConfigDefault.Param("WidthCatalog", "80");
                _ConfigDefault.Param("WidthName", "120");
                _ConfigDefault.Param("WidthLaguage", "80");
                _ConfigDefault.Param("WidthFirmware", "80");
                _ConfigDefault.Param("Mode", 0);
                _ConfigDefault.Param("State", 0);
                _ConfigDefault.Param("PSPGame", "Select PSP Games Directory.");
                _ConfigDefault.SaveAs(_ConfigName);
            }

            

            if (Double.Parse(_ConfigDefault.Param("X")) == 0 && Double.Parse(_ConfigDefault.Param("Y")) == 0)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                this.Top = Double.Parse(_ConfigDefault.Param("Y"));
                this.Left = Double.Parse(_ConfigDefault.Param("X"));
            }
            this.Width = Double.Parse(_ConfigDefault.Param("Width"));
            this.Height = Double.Parse(_ConfigDefault.Param("Height"));          
            
            WindowState configState = System.Windows.WindowState.Normal;
            if(Int32.Parse(_ConfigDefault.Param("State"))==1) {
                configState = System.Windows.WindowState.Maximized;
            }
            this.WindowState = configState;

            InitializeComponent();
        }

        public void GetLibraryData(Item[] data)
        {
            DataGame.Items.Clear();
            Int32 LenguageEnglish = 0;
            Int32 LenguageJapan = 0;
            Int32 LenguageOther = 0;
            Int64 TotalSize = 0;
            Int64 TotalGame = 0;
            foreach (Item rows in data)
            {
                if (rows != null)
                {
                    TotalGame++;
                    DataGame.Items.Add(rows);
                    if (rows.Region == "USA" || rows.Region == "Europe") LenguageEnglish++;
                    else if (rows.Region == "Japan") LenguageJapan++;
                    else LenguageOther++;
                    TotalSize += rows.Detail.Size;
                }
            }
            lblTotalGame.Content = TotalGame + " Games";
            lblTotalSize.Content = PSPGame.GetSize(TotalSize);
            lblLanguage.Content = "";
            if (LenguageEnglish != 0) lblLanguage.Content += "English(" + LenguageEnglish + ")  ";
            if (LenguageJapan != 0) lblLanguage.Content += "Japan(" + LenguageJapan + ")  ";
            if (LenguageOther != 0) lblLanguage.Content += "Other(" + LenguageOther + ")  ";
            DataGame.Columns[1].SortDirection = ListSortDirection.Ascending;
        }        

        #region Method Function Other
        void ShowBackgoundDialog()
        {
            DialogBackground.Width = this.Width;
            DialogBackground.Height = this.Height;
            DialogBackground.Margin = new Thickness(-1);
            DialogBackground.Opacity = 0.5;
            DialogBackground.Visibility = Visibility.Visible;
        }
        void HideBackgoundDialog()
        {
            DialogBackground.Visibility = Visibility.Hidden;
        }
        void log(String message)
        {
            Console.WriteLine(message);
        }
        public static ImageSource GetImage(String name)
        {
            return BitmapFrame.Create(new Uri("pack://application:,,,/PSPlibrary;component/Images/" + name));
        }
        #endregion

        #region Method Event MainWindow

        private void ImageSnap1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot1 != null) ImageBackground.Source = ImageSnap1.Source;
        }
        private void ImageSnap2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot2 != null) ImageBackground.Source = ImageSnap2.Source;
        }
        private void ImageSnap3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot3 != null) ImageBackground.Source = ImageSnap3.Source;
        }
        private void ImageSnap4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot4 != null) ImageBackground.Source = ImageSnap4.Source;
        }
        private void ImageSnap5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot5 != null) ImageBackground.Source = ImageSnap5.Source;
        }
        private void ImageSnap6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Item)DataGame.SelectedValue).Detail.Snapshot6 != null) ImageBackground.Source = ImageSnap6.Source;
        }

        Brush _BorderMouseMove = new SolidColorBrush(Color.FromRgb(60, 127, 177));
        Brush _BorderMouseLeave = new SolidColorBrush(Color.FromRgb(204, 204, 204));
        private void BorderSS1_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS1.BorderBrush = _BorderMouseLeave;
        }
        private void BorderSS1_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS1.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS2_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS2.BorderBrush = _BorderMouseLeave;
        }
        private void BorderSS2_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS2.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS3_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS3.BorderBrush = _BorderMouseLeave;
        }
        private void BorderSS3_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS3.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS4_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS4.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS4_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS4.BorderBrush = _BorderMouseLeave;
        }
        private void BorderSS5_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS5.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS5_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS5.BorderBrush = _BorderMouseLeave;
        }
        private void BorderSS6_MouseMove(object sender, MouseEventArgs e)
        {
            BorderSS6.BorderBrush = _BorderMouseMove;
        }
        private void BorderSS6_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderSS6.BorderBrush = _BorderMouseLeave;
        }
        
        private void psplibrary_Loaded(object sender, RoutedEventArgs e)
        {
            DataGame.Columns[0].Width = Int32.Parse(_ConfigDefault.Param("WidthCatalog"));
            DataGame.Columns[1].Width = Int32.Parse(_ConfigDefault.Param("WidthName"));
            DataGame.Columns[2].Width = Int32.Parse(_ConfigDefault.Param("WidthLaguage"));
            DataGame.Columns[3].Width = Int32.Parse(_ConfigDefault.Param("WidthFirmware"));

            btnMode.Content = "Editor Mode";
            GridEditor.Visibility = Visibility.Hidden;
            btnSaveLibrary.Visibility = Visibility.Hidden;

            ShowBackgoundDialog();
            if (!Directory.Exists(_ConfigDefault.Param("PSPGame")))
            {
                _DialogConfig.Viewed = true;
                _DialogConfig.Position(this);
                _DialogConfig.ShowDialog();
                _DialogAbout.Show(this, false);
            }
            else
            {
                _DialogAbout.Show(this, false);
            }
            HideBackgoundDialog();
        }
        private void psplibrary_Closing(object sender, CancelEventArgs e)
        {
            Byte state = 0;
            Byte mode = Byte.Parse(_ConfigDefault.Param("Mode"));
            if (this.WindowState == WindowState.Maximized) state = 1;

            _ConfigDefault.Load(File.ReadAllBytes(_ConfigName));
            _ConfigDefault.Param("X", psplibrary.Left.ToString());
            _ConfigDefault.Param("Y", psplibrary.Top.ToString());
            _ConfigDefault.Param("Width", psplibrary.Width.ToString());
            _ConfigDefault.Param("Height", psplibrary.Height.ToString());
            _ConfigDefault.Param("WidthCatalog", Math.Round(DataGame.Columns[0].Width.Value,0).ToString());
            _ConfigDefault.Param("WidthName", Math.Round(DataGame.Columns[1].Width.Value,0).ToString());
            _ConfigDefault.Param("WidthLaguage", Math.Round(DataGame.Columns[2].Width.Value,0).ToString());
            _ConfigDefault.Param("WidthFirmware", Math.Round(DataGame.Columns[3].Width.Value, 0).ToString());
            _ConfigDefault.Param("State", state);
            _ConfigDefault.Param("Mode", mode);
            _ConfigDefault.SaveAs(_ConfigName);


        }
        private void psplibrary_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void psplibrary_Activated(object sender, EventArgs e)
        {
            if (_DialogConfig.Viewed)
            {
                _DialogConfig.Topmost = true;
                _DialogConfig.Activate();
            }
            if (_DialogAbout.Viewed)
            {
                _DialogAbout.Topmost = true;
                _DialogAbout.Activate();
            }
        }
        private void psplibrary_Deactivated(object sender, EventArgs e)
        {
            if (_DialogConfig.Viewed) _DialogConfig.Topmost = false;
            if (_DialogAbout.Viewed) _DialogAbout.Topmost = false;
        }

        #endregion

        #region Method MouseClick() Event
        private void SettingItem_Click(object sender, RoutedEventArgs e)
        {
            ShowBackgoundDialog();
            _DialogConfig.Viewed = true;
            _DialogConfig.Position(this);
            _DialogConfig.ShowDialog();
            if (_DialogConfig.Reload) _DialogAbout.Show(this, false);
            HideBackgoundDialog();
        }

        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            ShowBackgoundDialog();
            _DialogAbout.Show(this, true);
            HideBackgoundDialog();
        }

        private void btnMode_Click(object sender, RoutedEventArgs e)
        {
            switch (_ConfigDefault.Param("Mode"))
            {
                case "0":
                    //btnMode.Content = "Viewer Mode";
                    //GridEditor.Visibility = Visibility.Visible;
                    //btnSaveLibrary.Visibility = Visibility.Visible;
                    //_ConfigDefault.Param("Mode", 1);

                    break;
                case "1":
                    //btnMode.Content = "Editor Mode";
                    //GridEditor.Visibility = Visibility.Hidden;
                    //btnSaveLibrary.Visibility = Visibility.Hidden;
                    //_ConfigDefault.Param("Mode", 0);
                    break;
            }
            this.SelectedDataGame();
        }

        private void btnSaveLibrary_Click(object sender, RoutedEventArgs e)
        {
            DataGame.IsEnabled = false;
            btnSaveLibrary.IsEnabled = false;

            lblPlayers.Content = "";
            lblRelease.Content = ((DateTime)EditRelease.SelectedDate).ToBinary();
            if (EditPlayer1.IsChecked == true) lblPlayers.Content = 1;
            if (EditPlayer2.IsChecked == true) lblPlayers.Content = 2;
            if (EditPlayer3.IsChecked == true) lblPlayers.Content = 3;
            if (EditPlayer4.IsChecked == true) lblPlayers.Content = 4;
            if (EditPlayer5.IsChecked == true) lblPlayers.Content = 5;
            
            lblGenre.Content = ((ComboBoxItem)EditGenre.SelectedItem).Content;
            lblPublisher.Content = EditPublisher.Text.Trim();
            lblDeveloper.Content = EditDeveloper.Text.Trim();
            
            OneArgDelegate _ThreadMethod = new OneArgDelegate(SavingSFAFile);
            _ThreadMethod.BeginInvoke(new Item()
            {
                Filename = EditName.Text.Trim(),
                Detail = new ItemData()
                {
                    Release= lblRelease.Content.ToString(),
                    Player = lblPlayers.Content.ToString(),
                    Genre = lblGenre.Content.ToString(),
                    Publisher = lblPublisher.Content.ToString(),
                    Developer = lblDeveloper.Content.ToString(),
                    Description = EditDescription.Text.Trim()
                }
            }, null, null);
            if (EditPublisher.Text.Trim() == "") lblPublisher.Content = "N/A";
            if (EditDeveloper.Text.Trim() == "") lblDeveloper.Content = "N/A";
            if (lblPlayers.Content.ToString().Trim() == "") lblPlayers.Content = "N/A";
            lblRelease.Content = ((DateTime)EditRelease.SelectedDate).ToShortDateString();
        }
        #endregion

        private void DataGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                EditGenre.SelectedIndex = 0;
                EditPlayer1.IsChecked = false;
                EditPlayer2.IsChecked = false;
                EditPlayer3.IsChecked = false;
                EditPlayer4.IsChecked = false;
                EditPlayer5.IsChecked = false;

                btnSaveLibrary.IsEnabled = true;
                //btnMode.IsEnabled = true;
                if (DataGame.SelectedIndex > -1)
                {
                    this.SelectedDataGame();
                }
                else
                {
                    btnMode.IsEnabled = false;
                }
            }
            catch
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Corrupt Library, Please Restart PSPlibrary.", "PSPlibrary", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    LibrarySaveMethod _ThreadDeleteDirectory = new LibrarySaveMethod(DeleteAllDirectory);
                    _ThreadDeleteDirectory.BeginInvoke(null, null);
                }
            }
        }

        void SelectedDataGame()
        {
            Item Selected = (Item)DataGame.SelectedValue;
            _DataDriectory = _ConfigStore + Selected.DiscID + "\\";
            switch (Int32.Parse(_ConfigDefault.Param("Mode")))
            {
                case 0:
                    ImageIcon.Source = GetImage("none.jpg");
                    ImageBackground.Source = GetImage("ScreenShot.png");
                    lblTitleGame.Content = Selected.Detail.Title;
                    lblFilename.Content = Selected.Detail.Name;
                    lblVersion.Content = Selected.Detail.Version;
                    lblFirmware.Content = Selected.Firmware;
                    lblParental.Content = Selected.Detail.Parental;
                    lblRegion.Content = Selected.Region;
                    lblRelease.Content = DateTime.FromBinary(Int64.Parse(Selected.Detail.Release)).ToShortDateString();
                    lblPlayers.Content = Selected.Detail.Player;
                    lblGenre.Content = Selected.Detail.Genre;
                    lblCatalogory.Content = Selected.Detail.Catagory + "-" + Selected.DiscID;
                    lblType.Content = Selected.Detail.Type;
                    lblCreated.Content = Selected.Detail.Created;
                    lblSize.Content = PSPGame.GetSize(Selected.Detail.Size);
                    lblPublisher.Content = Selected.Detail.Publisher;
                    lblDeveloper.Content = Selected.Detail.Developer;
                    // Preview
                    ImageSnap1.Source = GetImage("none.jpg");
                    ImageSnap2.Source = GetImage("none.jpg");
                    ImageSnap3.Source = GetImage("none.jpg");
                    ImageSnap4.Source = GetImage("none.jpg");
                    ImageSnap5.Source = GetImage("none.jpg");
                    ImageSnap6.Source = GetImage("none.jpg");

                    if (Selected.Detail.Release == "0") lblRelease.Content = "N/A";
                    if (Selected.Detail.Player == "0") lblPlayers.Content = "N/A";
                    if (Selected.Detail.Genre == "0") lblGenre.Content = "N/A";
                    if (Selected.Detail.Publisher == "0") lblPublisher.Content = "N/A";
                    if (Selected.Detail.Developer == "0") lblDeveloper.Content = "N/A";

                    if (Selected.Detail.Icon != null) ImageIcon.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Icon));
                    if (Selected.Detail.Background != null) ImageBackground.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Background));
                    if (Selected.Detail.Snapshot1 != null) ImageSnap1.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot1));
                    if (Selected.Detail.Snapshot2 != null) ImageSnap2.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot2));
                    if (Selected.Detail.Snapshot3 != null) ImageSnap3.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot3));
                    if (Selected.Detail.Snapshot4 != null) ImageSnap4.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot4));
                    if (Selected.Detail.Snapshot5 != null) ImageSnap5.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot5));
                    if (Selected.Detail.Snapshot6 != null) ImageSnap6.Source = BitmapFrame.Create(new Uri(_DataDriectory + Selected.Detail.Snapshot6));
                    break;
                case 1:
                    EditName.Text = Selected.Detail.Name;
                    EditPublisher.Text = Selected.Detail.Publisher;
                    EditDeveloper.Text = Selected.Detail.Developer;
                    EditDescription.Text = Selected.Detail.Description;
                    EditRelease.SelectedDate = DateTime.FromBinary(Int64.Parse(Selected.Detail.Release));

                    if (Selected.Detail.Release == "0") EditRelease.SelectedDate = DateTime.Now;
                    if (Selected.Detail.Publisher == "0") EditPublisher.Text = "";
                    if (Selected.Detail.Developer == "0") EditDeveloper.Text = "";
                    if (Selected.Detail.Description == "0") EditDescription.Text = "";

                    foreach (ComboBoxItem item in EditGenre.Items)
                    {
                        if (item.Content.ToString() == Selected.Detail.Genre.ToString())
                        {
                            EditGenre.SelectedItem = item;
                            break;
                        }
                    }
                    switch (Int32.Parse(Selected.Detail.Player))
                    {
                        case 1: EditPlayer1.IsChecked = true; break;
                        case 2: EditPlayer2.IsChecked = true; break;
                        case 3: EditPlayer3.IsChecked = true; break;
                        case 4: EditPlayer4.IsChecked = true; break;
                        case 5: EditPlayer5.IsChecked = true; break;
                    }
                    break;
            }
        }
        delegate void OneArgDelegate(Item edit);
        delegate void LibrarySaveMethod();
        #region DeleteAllDirectory
        void DeleteAllDirectory()
        {
            try
            {
                foreach (String folder in Directory.GetDirectories(_ConfigStore))
                {
                    foreach (String file in Directory.GetFiles(folder)) File.Delete(file);
                    Directory.Delete(folder);
                }
                // Update Thread
                this.Dispatcher.BeginInvoke(new LibrarySaveMethod(RestartApplication), DispatcherPriority.Normal, null);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "DeleteAllDirectory()", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        void RestartApplication()
        {
            Application.Current.Shutdown();
        } 
        #endregion

        #region SavingSFAFile
        void SavingSFAFile(Item edit)
        {
            String GameDataFile = _DataDriectory + "\\GAME.SFO";
            try
            {
                SfoFile GameSFO = new SfoFile();

                GameSFO.Load(File.ReadAllBytes(GameDataFile));
                GameSFO.Param("RELEASE", edit.Detail.Release);
                GameSFO.Param("PLAYERS", edit.Detail.Player);
                GameSFO.Param("GENRE", edit.Detail.Genre);
                GameSFO.Param("DEVELOPER", edit.Detail.Developer);
                GameSFO.Param("PUBLISHER", edit.Detail.Publisher);
                GameSFO.Param("DESCRIPTION", edit.Detail.Description);                

                GameSFO.SaveAs(_DataDriectory + "\\GAME.SFO");

            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "SavingSFAFile()", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            // Update Thread
            Console.WriteLine("Complated Saved.");
            this.Dispatcher.BeginInvoke(new LibrarySaveMethod(SavedSFAFile), DispatcherPriority.Normal, null);
        }

        void SavedSFAFile()
        {
            SfoFile GameSFO = new SfoFile();
            GameSFO.Load(File.ReadAllBytes(_DataDriectory + "GAME.SFO"));

            ((Item)DataGame.SelectedValue).Detail.Release = GameSFO.Param("RELEASE");
            ((Item)DataGame.SelectedValue).Detail.Player = GameSFO.Param("PLAYERS");
            ((Item)DataGame.SelectedValue).Detail.Genre = GameSFO.Param("GENRE");
            ((Item)DataGame.SelectedValue).Detail.Developer = GameSFO.Param("DEVELOPER");
            ((Item)DataGame.SelectedValue).Detail.Publisher = GameSFO.Param("PUBLISHER");
            ((Item)DataGame.SelectedValue).Detail.Description = GameSFO.Param("DESCRIPTION");

            DataGame.IsEnabled = true;
            btnSaveLibrary.IsEnabled = true;
        } 
        #endregion
    }


}

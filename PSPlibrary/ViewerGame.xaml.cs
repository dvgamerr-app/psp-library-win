using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PSPlibrary
{
    /// <summary>
    /// Interaction logic for ViewerGame.xaml
    /// </summary>
    public partial class ViewerGame : Window
    {
        String _ConfigTemp = Path.GetTempPath();
        BackgroundWorker _SyncISO = new BackgroundWorker();
        String _ConfigFolder = "";
        String _ConfigStore = "";

        public String _ISOGame;
        Item _ItemGame = new Item();
        Boolean _CloseDialog = false;
        Point _MouseDirection = new Point(0, 0);

        public ViewerGame(String pathfile)
        {
            _ISOGame = pathfile;
            _SyncISO.WorkerReportsProgress = true;

            _SyncISO.WorkerSupportsCancellation = false;

            _SyncISO.DoWork += new DoWorkEventHandler(SyncISO_DoWork);
            _SyncISO.ProgressChanged += new ProgressChangedEventHandler(SyncISO_ProgressChanged);
            _SyncISO.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SyncISO_RunWorkerCompleted);
            InitializeComponent();
        }

        private void Viewer_Loaded(object sender, RoutedEventArgs e)
        {
            _ConfigFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
            _ConfigStore = _ConfigFolder + "PSPStore\\";
            GridDetails.Visibility = Visibility.Hidden;
            GridScreenshot.Visibility = Visibility.Hidden;
            btnExit.Visibility = Visibility.Hidden;
            lblFilename.Content = Path.GetFileName(_ISOGame);
            _SyncISO.RunWorkerAsync();
        }

        private void SyncISO_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch(Path.GetExtension(_ISOGame).ToLower())
                {
                    case ".iso":
                        PSPGame FileGame = new PSPGame(_ISOGame);
                        if (FileGame.Readable)
                        {
                            SfoFile GameSFO = new SfoFile();
                            GameSFO.Load(FileGame.ReadAllByte("PSP_GAME\\PARAM.SFO"));
                            _ConfigTemp += GameSFO.Param("DISC_ID") + "\\";
                            _ItemGame = InitializeStore.GetLibrary(FileGame, _ConfigStore + GameSFO.Param("DISC_ID") + "\\", 0);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    case ".pbp":



                        break;
                }

            }
            catch
            {
                _SyncISO.ReportProgress(-1);
                e.Cancel = true;
            }
        }

        private void SyncISO_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case -1: lblPreload.Content = "Corrupt ISO"; break;

            }
        }

        private void SyncISO_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImagePreview.Cursor = Cursors.Arrow;

            if (!(e.Error == null))
            {

            }
            else if (e.Cancelled)
            {
                lblPreload.Cursor = Cursors.Arrow;
                lblFilename.Cursor = Cursors.Arrow;
                CaptionClose.Visibility = Visibility.Hidden;
                btnExit.Visibility = Visibility.Visible;

            }
            else
            {
                String DataDirectory = _ConfigStore + _ItemGame.DiscID + "\\";
                ImageSource _IconGame = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Icon));
                Console.WriteLine(_IconGame.Height + "==" + _IconGame.Width);
                //IconBorder.Width = _IconGame.Width;
                IconImage.Source = _IconGame;
                if (_ItemGame.Detail.Snapshot1 != null) SSImage1.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot1));
                else SSBorder1.Visibility = Visibility.Hidden;
                if (_ItemGame.Detail.Snapshot2 != null) SSImage2.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot2));
                else SSBorder2.Visibility = Visibility.Hidden;
                if (_ItemGame.Detail.Snapshot3 != null) SSImage3.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot3));
                else SSBorder3.Visibility = Visibility.Hidden;
                if (_ItemGame.Detail.Snapshot4 != null) SSImage4.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot4));
                else SSBorder4.Visibility = Visibility.Hidden;
                if (_ItemGame.Detail.Snapshot5 != null) SSImage5.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot5));
                else SSBorder5.Visibility = Visibility.Hidden;
                if (_ItemGame.Detail.Snapshot6 != null) SSImage6.Source = BitmapFrame.Create(new Uri(DataDirectory + _ItemGame.Detail.Snapshot6));
                else SSBorder6.Visibility = Visibility.Hidden;

                txtTitleGame.Text = _ItemGame.Detail.Title;
                lblFilename1.Content = Path.GetFileNameWithoutExtension(_ISOGame);
                lblCreated.Content = _ItemGame.Detail.Created;
                lblSize.Content = PSPGame.GetSize(_ItemGame.Detail.Size);
                lblRegion.Content = _ItemGame.Detail.Type;
                lblGenre.Content = _ItemGame.Detail.Genre;
                lblRelease.Content = DateTime.FromBinary(Int64.Parse(_ItemGame.Detail.Release)).ToShortDateString();
                lblPlayers.Content = _ItemGame.Detail.Player;
                lblPublisher.Content = _ItemGame.Detail.Publisher;
                lblDeveloper.Content = _ItemGame.Detail.Developer;
                lblDescription.Text = _ItemGame.Detail.Description;
                lblCatalogory.Content = _ItemGame.Detail.Catagory + "-" + _ItemGame.DiscID;
                lblVersion.Content = _ItemGame.Detail.Version;
                lblFirmware.Content = _ItemGame.Firmware;
                lblParental.Content = _ItemGame.Detail.Parental;

                if (lblGenre.Content.ToString() == "0") lblGenre.Content = "N/A";
                if (lblRegion.Content.ToString() == "0") lblRegion.Content = "N/A";
                if (lblRelease.Content.ToString() == "0") lblRelease.Content = "N/A";
                if (lblPlayers.Content.ToString() == "0") lblPlayers.Content = "N/A";
                if (lblPublisher.Content.ToString() == "0") lblPublisher.Content = "N/A";
                if (lblDeveloper.Content.ToString() == "0") lblDeveloper.Content = "N/A";
                if (lblDescription.Text.ToString() == "0") lblDescription.Text = "N/A";


                // View Details
                GridPreload.Visibility = Visibility.Hidden;
                GridDetails.Visibility = Visibility.Visible;
                GridScreenshot.Visibility = Visibility.Visible;
                _CloseDialog = true;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #region From Move Method.
        private void Viewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _MouseDirection = e.GetPosition(this);
        }

        private void Viewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _MouseDirection = new Point(0, 0);
        }

        private void Viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_MouseDirection != new Point(0, 0))
            {
                if (_MouseDirection.X > e.GetPosition(this).X) this.Left += (e.GetPosition(this).X - _MouseDirection.X);
                if (_MouseDirection.X < e.GetPosition(this).X) this.Left -= (_MouseDirection.X - e.GetPosition(this).X);
                if (_MouseDirection.Y > e.GetPosition(this).Y) this.Top += (e.GetPosition(this).Y - _MouseDirection.Y);
                if (_MouseDirection.Y < e.GetPosition(this).Y) this.Top -= (_MouseDirection.Y - e.GetPosition(this).Y);
            }
        }

        private void Viewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_CloseDialog) Application.Current.Shutdown();
        }

        #endregion

        #region Screenshot Mouse Event Method.
        private void GridScreenshot_MouseMove(object sender, MouseEventArgs e)
        {
            GridScreenshot.Margin = new Thickness(0, GridScreenshot.Margin.Top, GridScreenshot.Margin.Right, GridScreenshot.Margin.Bottom);
            GridScreenshot.Background = new SolidColorBrush(Color.FromArgb(25, 0, 0, 0));
        }
        private void GridScreenshot_MouseLeave(object sender, MouseEventArgs e)
        {
            GridDetails.Visibility = Visibility.Visible;
            GridScreenshot.Margin = new Thickness(-95, GridScreenshot.Margin.Top, GridScreenshot.Margin.Right, GridScreenshot.Margin.Bottom);
            GridScreenshot.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
            ImagePreview.Source = MainWindow.GetImage("ViewerBackground.png");
        }

        private void SSImage1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage1.Source);
        }
        private void SSImage2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage2.Source);
        }
        private void SSImage3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage3.Source);
        }
        private void SSImage4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage4.Source);
        }
        private void SSImage5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage5.Source);
        }
        private void SSImage6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenshotClick(SSImage6.Source);
        }
        void ScreenshotClick(ImageSource source)
        {
            GridDetails.Visibility = Visibility.Hidden;
            ImagePreview.Source = source;           
        }

        #endregion
    }
}

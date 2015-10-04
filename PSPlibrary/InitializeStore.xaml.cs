using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using DiscUtils.Iso9660;
using DiscUtils;
using System.Windows.Media;

namespace PSPlibrary
{
    /// <summary>
    /// Interaction logic for InitializeStore.xaml
    /// </summary>
    public partial class InitializeStore : Window
    {
        BackgroundWorker _SyncGame;
        String _PreloadTitle, _PreloadSubject;
        Int32 _PreloadMaxinium, _PreloadLength;
        MainWindow _MainWindow;

        Item[] GameLibrary = new Item[1];
        public Boolean Viewed { get; set; }
        public Boolean Abouted { get; set; }

        #region Method Control From
        public InitializeStore()
        {
            InitializeComponent();
        }

        public void Position(MainWindow me)
        {
            if (me.WindowState != WindowState.Maximized)
            {
                this.Top = (me.Top + me.Height / 2) - (this.Height / 2);
                this.Left = (me.Left + me.Width / 2) - (this.Width / 2);
            }
            else
            {
                this.Top = (Screen.PrimaryScreen.WorkingArea.Height / 2) - (this.Height / 2);
                this.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (this.Width / 2);
            }
        }

        public void Show(MainWindow me, Boolean about)
        {
            this._MainWindow = me;
            this.Abouted = about;
            this.Viewed = true;
            this.Position(me);
            if (!about)
            {
                ImageAbout.Source = MainWindow.GetImage("Initialize.png");
                GridAbout.Visibility = Visibility.Hidden;
                GridPreload.Visibility = Visibility.Visible;

                _SyncGame = new BackgroundWorker();
                _SyncGame.WorkerReportsProgress = true;
                _SyncGame.WorkerSupportsCancellation = false;
                _SyncGame.DoWork += new DoWorkEventHandler(_SyncGame_DoWork);
                _SyncGame.ProgressChanged += new ProgressChangedEventHandler(_SyncGame_ProgressChanged);
                _SyncGame.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_SyncGame_RunWorkerCompleted);
                _SyncGame.RunWorkerAsync();
            }
            else
            {
                ImageAbout.Source = MainWindow.GetImage("About.png");
                GridAbout.Visibility = Visibility.Visible;
                GridPreload.Visibility = Visibility.Hidden;
            }
            this.Topmost = true;
            this.ShowDialog();
        }

        private void ImageAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Abouted)
            {
                this.Viewed = false;
                this.Hide();
            }
        }
        #endregion

        public static Item GetLibrary(PSPGame isoGame, String dataDirectory, Int32 index)
        {
            if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);
            if (!File.Exists(dataDirectory + "GAME.SFO")) isoGame.ExtractFile("PSP_GAME\\PARAM.SFO", dataDirectory + "GAME.SFO");
            if (!File.Exists(dataDirectory + "ICON.PNG")) isoGame.ExtractFile("PSP_GAME\\ICON0.PNG", dataDirectory + "ICON.PNG");
            if (!File.Exists(dataDirectory + "PIC.PNG")) isoGame.ExtractFile("PSP_GAME\\PIC1.PNG", dataDirectory + "PIC.PNG");

            Item Param = new Item();
            SfoFile GameSFO = new SfoFile();
            GameSFO.Load(File.ReadAllBytes(dataDirectory + "GAME.SFO"));
            String[] DirectoryName = Path.GetDirectoryName(isoGame.FileName).Split('\\');

            Param.DiscID = GameSFO.Param("DISC_ID");
            Param.Filename = Path.GetFileName(isoGame.FileName);
            Param.Region = Item.GetRegion(GameSFO.Param("DISC_ID"));
            Param.Firmware = GameSFO.Param("PSP_SYSTEM_VER");
            Param.Index = index;

            Param.Detail = new ItemData()
            {
                GameDirectory = Path.GetDirectoryName(isoGame.FileName),
                Name = DirectoryName[DirectoryName.Length - 1],
                Icon = null,
                Background = null,
                Title = GameSFO.Param("TITLE"),
                Version = GameSFO.Param("DISC_VERSION"),
                Parental = GameSFO.Param("PARENTAL_LEVEL"),
                Catagory = GameSFO.Param("CATEGORY"),
                Type = Item.GetType(GameSFO.Param("DISC_ID")),
                Size = isoGame.FileSize,
                Created = File.GetCreationTime(isoGame.FileName).ToString().Split(' ')[0],
                Release = GameSFO.Param("RELEASE"),
                Player = GameSFO.Param("PLAYERS"),
                Genre = GameSFO.Param("GENRE"),
                Developer = GameSFO.Param("DEVELOPER"),
                Publisher = GameSFO.Param("PUBLISHER"),
                Description = GameSFO.Param("DESCRIPTION"),
                Snapshot1 = null,
                Snapshot2 = null,
                Snapshot3 = null,
                Snapshot4 = null,
                Snapshot5 = null,
                Snapshot6 = null,
            };

            // IF Image Exists Change
            if (File.Exists(dataDirectory + "ICON.PNG")) Param.Detail.Icon = "ICON.PNG";
            if (File.Exists(dataDirectory + "PIC.PNG"))
            {
                Param.Detail.Background = "PIC.PNG";
                Param.Detail.Snapshot1 = "PIC.PNG";
            }
            if (File.Exists(dataDirectory + "SS1.PNG")) Param.Detail.Snapshot1 = "SS1.PNG";
            if (File.Exists(dataDirectory + "SS2.PNG")) Param.Detail.Snapshot2 = "SS2.PNG";
            if (File.Exists(dataDirectory + "SS3.PNG")) Param.Detail.Snapshot3 = "SS3.PNG";
            if (File.Exists(dataDirectory + "SS4.PNG")) Param.Detail.Snapshot4 = "SS4.PNG";
            if (File.Exists(dataDirectory + "SS5.PNG")) Param.Detail.Snapshot5 = "SS5.PNG";
            if (File.Exists(dataDirectory + "SS6.PNG")) Param.Detail.Snapshot6 = "SS6.PNG";

            return Param;
        }

        private Boolean PermissionDirectory(string path)
        {
            try
            {
                Directory.GetFiles(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        #region SyncGame_DoWork Method Event
        void _SyncGame_DoWork(object sender, DoWorkEventArgs e)
        {
            String _ConfigFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
            String _ConfigStore = _ConfigFolder + "PSPStore\\";
            SfoFile _ConfigDefault = new SfoFile();
            String pathConfigFile = _ConfigFolder + "setting.cfg";
            try
            {
                _ConfigDefault.Load(File.ReadAllBytes(pathConfigFile));

                if (Directory.Exists(_ConfigDefault.Param("PSPGame")))
                {
                    _PreloadTitle = "Create Directory";
                    _PreloadSubject = "Please wait...";
                    _PreloadLength = 0;
                    _PreloadMaxinium = Directory.GetFiles(_ConfigDefault.Param("PSPGame")).Length;
                    _SyncGame.ReportProgress(0);

                    foreach (String name in Directory.GetFiles(_ConfigDefault.Param("PSPGame")))
                    {
                        string extens = Path.GetExtension(name).ToLower();
                        if (extens == ".iso" || extens == ".cso" || extens == ".pbp")
                        {
                            _PreloadLength++;
                            Directory.CreateDirectory(_ConfigDefault.Param("PSPGame") + "\\" + Path.GetFileNameWithoutExtension(name));
                            File.Move(name, _ConfigDefault.Param("PSPGame") + "\\" + Path.GetFileNameWithoutExtension(name) + "\\" + Path.GetFileName(name));
                            _SyncGame.ReportProgress(_PreloadLength);
                        }
                    }

                    Int32 DataLength = 0;
                    _PreloadTitle = "Initialize....";
                    _PreloadLength = 0;
                    _PreloadMaxinium = Directory.GetDirectories(_ConfigDefault.Param("PSPGame")).Length - 1;
                    GameLibrary = new Item[_PreloadMaxinium + 1];
                    _SyncGame.ReportProgress(0);
                    foreach (String name in Directory.GetDirectories(_ConfigDefault.Param("PSPGame")))
                    {
                        if (PermissionDirectory(name))
                        {
                            foreach (String file in Directory.GetFiles(name))
                            {
                                _SyncGame.ReportProgress(_PreloadLength);
                                switch (Path.GetExtension(file).ToLower())
                                {
                                    case ".iso":
                                        PSPGame FileGame = new PSPGame(file);
                                        if (FileGame.Readable)
                                        {
                                            SfoFile GameSFO = new SfoFile();
                                            GameSFO.Load(FileGame.ReadAllByte("PSP_GAME\\PARAM.SFO"));
                                            _PreloadSubject = GameSFO.Param("TITLE");
                                            String DataDirectory = _ConfigStore + GameSFO.Param("DISC_ID") + "\\";

                                            GameLibrary[DataLength] = InitializeStore.GetLibrary(FileGame, DataDirectory, DataLength);
                                            DataLength++;
                                        }
                                        break;
                                    case ".pbp":
                                        //var psi = new ProcessStartInfo
                                        //{
                                        //    FileName = @"c:\work\test.exe",
                                        //    Arguments = @"param1 param2",
                                        //    UseShellExecute = false,
                                        //    RedirectStandardOutput = true,
                                        //};
                                        //var process = Process.Start(psi);
                                        //if (process.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds))
                                        //{
                                        //    var result = process.StandardOutput.ReadToEnd();
                                        //    Console.WriteLine(result);
                                        //}
                                        break;
                                    case ".cso":
                                        break;
                                }
                            }
                        }
                        _PreloadLength++;
                    }
                }
            }
            catch 
            {
                _SyncGame.CancelAsync();
            }
        }

        void _SyncGame_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            barPreload.Maximum = _PreloadMaxinium;
            barPreload.Value = e.ProgressPercentage;
            lblTitle.Content = _PreloadTitle;
            lblSubject.Content = _PreloadSubject;
        }

        void _SyncGame_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                throw new NotImplementedException();
            }
            else
            {
                _MainWindow.GetLibraryData(this.GameLibrary);
                GameLibrary = new Item[1];
                this.Viewed = false;
                this.Hide();
            }
        } 
        #endregion

    }

}

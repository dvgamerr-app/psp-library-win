using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace PSPlibrary
{
    /// <summary>
    /// Interaction logic for FolderDialog.xaml
    /// </summary>
    public partial class ConfigDialog : Window
    {
        FolderBrowserDialog _GameFolder = new FolderBrowserDialog();
        String _ConfigFolder = "";
        String _ConfigStore = "";
        SfoFile _ConfigDefault = new SfoFile();
        
        const String _ConfigName = "setting.cfg";
        const String _FOLDER_BROWSER = "Please Select Directory PSP Game.";

        public Boolean Reload { get; set; }
        public Boolean Viewed { get; set; }
        public ConfigDialog()
        {
            _ConfigFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
            _ConfigStore = _ConfigFolder + "PSPStore\\";

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _ConfigDefault.Load(File.ReadAllBytes(_ConfigFolder + _ConfigName));
            }
            catch
            {
                throw new Exception("Config file, Lost.");
            }

            _GameFolder.ShowNewFolderButton = false;
            txtBrowse.Text = _ConfigDefault.Param("PSPGame");
            if (_ConfigDefault.Param("PSPGame") == "") txtBrowse.Text = _FOLDER_BROWSER;
            if (txtBrowse.Text != _FOLDER_BROWSER) txtBrowse.Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80));
            else txtBrowse.Foreground = new SolidColorBrush(Color.FromRgb(190, 49, 49));    
        }


        private void ButtonCanceled_Click(object sender, RoutedEventArgs e)
        {
            this.Reload = false;
            this.Viewed = false;
            this.Hide();
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            _GameFolder.Description = "Select Folder PSP Game";
            if (Directory.Exists(_ConfigDefault.Param("PSPGame"))) _GameFolder.SelectedPath = _ConfigDefault.Param("PSPGame");
            if (_GameFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBrowse.Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80));
                txtBrowse.Text = _GameFolder.SelectedPath;
            }
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            this.Reload = false;
            if (Directory.Exists(txtBrowse.Text.Trim()) && _ConfigDefault.Param("PSPGame") != txtBrowse.Text.Trim())
            {
                this.Reload = true;
                _ConfigDefault.Param("PSPGame",txtBrowse.Text.Trim());
            }
            _ConfigDefault.SaveAs(_ConfigFolder + _ConfigName);

            this.Viewed = false;
            this.Hide();
        }

    }
}

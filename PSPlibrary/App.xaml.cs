using System;
using System.IO;
using System.Windows;

namespace PSPlibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void AppStartup(object sender, StartupEventArgs e)
        {
            try
            {
                if (e.Args.Length > 0)
                {
                    String _extenions = _extenions = Path.GetExtension(e.Args[0].ToLower());
                    if (_extenions == ".iso" || _extenions == ".pbp"/* || _extenions == ".jso" || _extenions == ".cso" || _extenions == ".dax"*/)
                    {
                        ViewerGame _Viewer = new ViewerGame(e.Args[0]);
                        _Viewer.Show();
                    }
                    else
                    {
                        MessageBox.Show("Cann't support this is extenions.", "Info:: PSP Library", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    //String args = "D:\\[PSP Game]\\#NEW\\Final Fanasy 3\\Final Fanasy 3.iso";
                    //ViewerGame _Viewer = new ViewerGame(args);
                    //_Viewer.Show();
                    MainWindow _WindowMain = new MainWindow();
                    _WindowMain.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning:: PSP Library", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
                Application.Current.Shutdown();
            }
        }
    }
}

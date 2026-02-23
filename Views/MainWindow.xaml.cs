using System.Windows;
using System.Windows.Input;
using ZenDesk.Helpers;

namespace ZenDesk.Views
{
    public partial class MainWindow : Window
    {
        private ZenDesk.Services.ContainerManager _manager;
        private ZenDesk.Services.DesktopWatcher _watcher;
        private ZenDesk.Services.FocusModeService _focusService;
        private ZenDesk.Services.ZenPeekService _zenPeekService;

        public MainWindow()
        {
            InitializeComponent();
            DwmHelper.EnableBackdrop(this, DwmHelper.BackdropType.MainWindow, true);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }
        }

        private void LoadContainers_Click(object sender, RoutedEventArgs e)
        {
            if (_manager == null)
            {
                _manager = new ZenDesk.Services.ContainerManager();
                _manager.Load();
                
                foreach(var model in _manager.Containers)
                {
                    var vm = new ZenDesk.ViewModels.ContainerViewModel(model);
                    var win = new ContainerWindow
                    {
                        DataContext = vm
                    };
                    win.Show();
                }

                _watcher = new ZenDesk.Services.DesktopWatcher(_manager);
                _watcher.Start();

                _focusService = new ZenDesk.Services.FocusModeService(this, _manager);
                _zenPeekService = new ZenDesk.Services.ZenPeekService(this);
            }
            else
            {
                // Unhide all windows if they were hidden using the Close 'X' button or Focus Mode
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is ContainerWindow cw)
                    {
                        cw.Visibility = Visibility.Visible;
                        Helpers.AnimationHelper.FadeIn(cw, 300);
                    }
                }
            }
        }

        private void Classify_Click(object sender, RoutedEventArgs e)
        {
            if (_manager != null)
            {
                var classifier = new ZenDesk.Services.FileClassifier(_manager);
                classifier.ClassifyDesktop();
            }
            else
            {
                MessageBox.Show("请先加载收纳盒！", "提示");
            }
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            _watcher?.Dispose();
            _focusService?.Dispose();
            _zenPeekService?.Dispose();
            Application.Current.Shutdown();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ExitApp_Click(sender, e);
        }
    }
}

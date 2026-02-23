using System.Windows;
using ZenDesk.Helpers;

namespace ZenDesk.Views
{
    public partial class OnboardingWindow : Window
    {
        public OnboardingWindow()
        {
            InitializeComponent();
            DwmHelper.EnableBackdrop(this, DwmHelper.BackdropType.TransientWindow, false); // Acrylic style

            this.Loaded += (s, e) =>
            {
                AnimationHelper.FadeIn(this, 1000); // Slow breathing fade in
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationHelper.FadeOut(this, 500, () =>
            {
                // Must show new window BEFORE closing the last one 
                // to prevent WPF from shutting down (if ShutdownMode = OnLastWindowClose)
                var mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close();
            });
        }
    }
}

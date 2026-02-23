using System;
using System.Windows;
using System.Windows.Input;
using ZenDesk.Helpers;

namespace ZenDesk.Views
{
    public partial class ContainerWindow : Window
    {
        private bool _isRolledUp = false;
        private double _originalHeight;

        public ContainerWindow()
        {
            InitializeComponent();
            DwmHelper.EnableBackdrop(this, DwmHelper.BackdropType.TransientWindow, true); // Acrylic
            
            this.Loaded += (s, e) => 
            {
                AnimationHelper.FadeIn(this, 300);
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (this.DataContext is ViewModels.ContainerViewModel vm)
            {
                if (e.Key == Key.Escape)
                {
                    vm.SearchText = string.Empty;
                }
                else if (e.Key == Key.Back && vm.SearchText.Length > 0)
                {
                    vm.SearchText = vm.SearchText.Substring(0, vm.SearchText.Length - 1);
                }
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            if (this.DataContext is ViewModels.ContainerViewModel vm)
            {
                // Only accept ascii letters and digits directly to search text, skip controls
                if (e.Text.Length == 1 && char.IsLetterOrDigit(e.Text[0]))
                {
                    vm.SearchText += e.Text;
                }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Roll-up effect
                if (!_isRolledUp)
                {
                    _originalHeight = this.Height;
                    this.Height = 36; // Only title bar height
                    _isRolledUp = true;
                }
                else
                {
                    this.Height = _originalHeight;
                    _isRolledUp = false;
                }
            }
            else if (e.ClickCount == 1)
            {
                this.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            // Minimize to system tray or visual hide
            AnimationHelper.FadeOut(this, 200, () => this.Hide());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Close actually hides the container or removes it, depending on implementation.
            // For now just hide
            AnimationHelper.FadeOut(this, 200, () => this.Hide());
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                {
                    if (this.DataContext is ViewModels.ContainerViewModel vm && vm.Model != null)
                    {
                        foreach (string file in files)
                        {
                            // In a real app, we might move the file or create a link.
                            // Here we just add it to the view model.
                            var entry = new Models.FileEntryModel(file);
                            vm.Model.Files.Add(entry);
                        }
                    }
                }
            }
        }

        private void FileItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((sender as FrameworkElement)?.DataContext is Models.FileEntryModel fileModel)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = fileModel.FilePath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开文件：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

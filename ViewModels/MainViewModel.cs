using CommunityToolkit.Mvvm.ComponentModel;

namespace ZenDesk.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _windowTitle = "ZenDesk";

        public MainViewModel()
        {
        }
    }
}

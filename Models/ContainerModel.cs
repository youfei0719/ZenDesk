using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ZenDesk.Models
{
    public partial class ContainerModel : ObservableObject
    {
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        [ObservableProperty]
        private string _name = "新收纳盒";

        [ObservableProperty]
        private double _x = 100;

        [ObservableProperty]
        private double _y = 100;

        [ObservableProperty]
        private double _width = 300;

        [ObservableProperty]
        private double _height = 400;

        [ObservableProperty]
        private string _colorTheme = "#40000000"; // 默认半透明深色

        [ObservableProperty]
        private bool _isLocked = false;

        [ObservableProperty]
        private string _mappedPath = string.Empty;

        public ObservableCollection<FileEntryModel> Files { get; set; } = new ObservableCollection<FileEntryModel>();
    }
}

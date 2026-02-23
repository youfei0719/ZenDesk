using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenDesk.Models;

namespace ZenDesk.ViewModels
{
    public partial class ContainerViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContainerModel _model;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public ICollectionView FilteredFiles { get; }

        public ContainerViewModel(ContainerModel model)
        {
            Model = model;
            FilteredFiles = CollectionViewSource.GetDefaultView(Model.Files);
            FilteredFiles.Filter = FileFilter;
        }

        partial void OnSearchTextChanged(string value)
        {
            FilteredFiles.Refresh();
        }

        private bool FileFilter(object item)
        {
            if (item is FileEntryModel file)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;
                
                return file.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        [RelayCommand]
        public void Close()
        {
            // Handled by View
        }
    }
}

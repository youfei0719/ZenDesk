using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ZenDesk.Helpers;

namespace ZenDesk.Models
{
    public partial class FileEntryModel : ObservableObject
    {
        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private string _fileName = string.Empty;

        [ObservableProperty]
        private string _fileExtension = string.Empty;

        // 图标可以直接绑定到系统的关联图标
        [ObservableProperty]
        private ImageSource _fileIcon; // Added ImageSource property

        [ObservableProperty]
        private DateTime _creationTime; // Added CreationTime property

        public FileEntryModel(string path)
        {
            FilePath = path;
            FileName = System.IO.Path.GetFileName(path);
            FileExtension = System.IO.Path.GetExtension(path);
            
            try 
            {
                CreationTime = File.GetCreationTime(path); // Set CreationTime
                FileIcon = IconHelper.GetIcon(path, true); // Load icon using IconHelper
            }
            catch 
            {
                CreationTime = DateTime.Now; // Fallback for CreationTime
                // FileIcon will remain null or default if loading fails
            }
        }
        
        public FileEntryModel() { } // for serializer
    }
}

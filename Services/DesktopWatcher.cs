using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ZenDesk.Models;

namespace ZenDesk.Services
{
    public class DesktopWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private readonly ContainerManager _containerManager;

        public DesktopWatcher(ContainerManager containerManager)
        {
            _containerManager = containerManager;
        }

        public void Start()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            _watcher = new FileSystemWatcher(desktopPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            _watcher.Created += OnFileCreated;
        }

        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Wait for file lock release in case it's still downloading/copying
            await Task.Delay(500);
            
            try 
            {
                if (File.Exists(e.FullPath))
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        ContainerModel inbox = null;
                        foreach(var c in _containerManager.Containers) {
                            if (c.Name.Equals("Inbox", StringComparison.OrdinalIgnoreCase)) {
                                inbox = c; break;
                            }
                        }

                        if (inbox != null)
                        {
                            // Avoid duplicates
                            bool exists = false;
                            foreach(var existing in inbox.Files) {
                                if (existing.FilePath.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase)) {
                                    exists = true; break;
                                }
                            }
                            if (!exists)
                            {
                                inbox.Files.Add(new FileEntryModel(e.FullPath));
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error capturing file into Inbox: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}

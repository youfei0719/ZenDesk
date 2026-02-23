using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ZenDesk.Models;

namespace ZenDesk.Services
{
    public class ContainerManager
    {
        private readonly string _savePath;
        public ObservableCollection<ContainerModel> Containers { get; private set; }

        public ContainerManager()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDir = Path.Combine(appData, "ZenDesk");
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }
            _savePath = Path.Combine(appDir, "containers.json");
            Containers = new ObservableCollection<ContainerModel>();
        }

        public void Load()
        {
            if (File.Exists(_savePath))
            {
                try
                {
                    string json = File.ReadAllText(_savePath);
                    var list = JsonSerializer.Deserialize<ObservableCollection<ContainerModel>>(json);
                    if (list != null)
                    {
                        Containers = list;
                    }
                }
                catch (Exception ex)
                {
                    // Handle load error gracefully
                    System.Diagnostics.Debug.WriteLine($"Failed to load containers: {ex.Message}");
                    Containers = new ObservableCollection<ContainerModel>();
                }
            }
            else
            {
                // Create default containers on first run
                Containers.Add(new ContainerModel { Name = "文档", X = 100, Y = 100 });
                Containers.Add(new ContainerModel { Name = "图片", X = 420, Y = 100 });
                Containers.Add(new ContainerModel { Name = "Inbox", X = 740, Y = 100 });
                Save();
            }

            // Sync Mapped Paths
            foreach (var container in Containers)
            {
                if (!string.IsNullOrEmpty(container.MappedPath) && Directory.Exists(container.MappedPath))
                {
                    container.Files.Clear();
                    foreach (var file in Directory.GetFiles(container.MappedPath))
                    {
                        container.Files.Add(new FileEntryModel(file));
                    }
                }
            }
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Containers, options);
                File.WriteAllText(_savePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save containers: {ex.Message}");
            }
        }

        public void AddContainer(ContainerModel container)
        {
            Containers.Add(container);
            Save();
        }

        public void RemoveContainer(ContainerModel container)
        {
            Containers.Remove(container);
            Save();
        }
    }
}

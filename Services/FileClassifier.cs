using System;
using System.Collections.Generic;
using System.IO;
using ZenDesk.Models;

namespace ZenDesk.Services
{
    public class FileClassifier
    {
        private readonly ContainerManager _containerManager;

        public FileClassifier(ContainerManager containerManager)
        {
            _containerManager = containerManager;
        }

        public string GetCategoryByExtension(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            
            var documentExts = new HashSet<string> { ".doc", ".docx", ".pdf", ".txt", ".xlsx", ".pptx", ".csv" };
            var imageExts = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp" };
            var videoExts = new HashSet<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
            
            if (documentExts.Contains(ext)) return "文档";
            if (imageExts.Contains(ext)) return "图片";
            if (videoExts.Contains(ext)) return "视频";

            return "Inbox"; // Default fallback
        }

        public void ClassifyDesktop()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var files = Directory.GetFiles(desktopPath);

            foreach (var file in files)
            {
                // Skip shortcuts to our own app or hidden links
                if (file.EndsWith(".lnk") && file.Contains("ZenDesk")) continue;
                if (new FileInfo(file).Attributes.HasFlag(FileAttributes.Hidden)) continue;

                string category = GetCategoryByExtension(file);
                
                // Find container or fallback to Inbox
                ContainerModel target = null;
                foreach (var container in _containerManager.Containers)
                {
                    if (container.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
                    {
                        target = container;
                        break;
                    }
                }

                if (target == null)
                {
                    foreach (var container in _containerManager.Containers)
                    {
                        if (container.Name.Equals("Inbox", StringComparison.OrdinalIgnoreCase))
                        {
                            target = container;
                            break;
                        }
                    }
                }

                if (target != null)
                {
                    // Move the file visually or physically. Here we do virtual move:
                    var entry = new FileEntryModel(file);
                    
                    // Do not add duplicates (based on file path)
                    bool exists = false;
                    foreach(var existing in target.Files) {
                         if (existing.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)) {
                             exists = true; break;
                         }
                    }
                    if (!exists)
                    {
                         target.Files.Add(entry);
                         // Optional: File.Move to the actual local sub-folder if we want it fully removed from windows desktop.
                    }
                }
            }
        }
    }
}

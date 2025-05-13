using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using System.Text.Json;

namespace Odin
{
    public class FolderTools
    {
        public static string GetFolders(IModel currentApp, string moduleName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }

            var folders = module.GetFolders().ToList();
            if (folders == null || folders.Count == 0)
            {
                return $"The module {module.Name} has no folders";
            }

            var folderStructure = GetFolderStructure(folders);
            var response = JsonSerializer.Serialize(folderStructure);
            return $"The module {module.Name} has folders: {response}";
        }

        public static IFolder? GetFolderFromPath(IModule module, FolderModel folder)
        {
            if (module == null || folder == null)
            {
                return null;
            }

            // Start searching from the root folders of the module
            var rootFolder = module.GetFolders().FirstOrDefault(f => string.Equals(f.Name, folder.Name, StringComparison.OrdinalIgnoreCase));
            if (rootFolder == null)
            {
                return null;
            }

            // Recursively find the target folder
            return FindFolderRecursive(rootFolder, folder.Folders);
        }

        private static IFolder? FindFolderRecursive(IFolder currentFolder, List<FolderModel> subFolders)
        {
            if (subFolders == null || subFolders.Count == 0)
            {
                return currentFolder;
            }

            var subFolder = subFolders.First();
            var nextFolder = currentFolder.GetFolders().FirstOrDefault(f => string.Equals(f.Name, subFolder.Name, StringComparison.OrdinalIgnoreCase));

            if (nextFolder == null)
            {
                return null;
            }

            return FindFolderRecursive(nextFolder, subFolder.Folders);
        }

        private static List<FolderModel> GetFolderStructure(List<IFolder> folders)
        {
            var folderNodes = new List<FolderModel>();

            foreach (var folder in folders)
            {
                var subFolders = folder.GetFolders().ToList();
                folderNodes.Add(new FolderModel(folder.Name, GetFolderStructure(subFolders)));
            }

            return folderNodes;
        }

    }
}

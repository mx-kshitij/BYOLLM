using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Microflows;
using System.Text.Json;

namespace Odin
{
    public class MicroflowTools
    {
        public static string GetMicroflows(IModel currentApp, string moduleName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            string response = "";
            if (module != null)
            {
                var microflows = module.GetDocuments().OfType<IMicroflow>().ToList();
                if (microflows == null || microflows.Count == 0)
                {
                    return $"The module {module.Name} has no microflows";
                }
                List<MicroflowModel> microflowModels = new List<MicroflowModel>();
                foreach (var item in microflows)
                {
                    MicroflowModel microflow = new MicroflowModel(item.Name);
                    microflowModels.Add(microflow);
                }
                response = JsonSerializer.Serialize(microflowModels);
                return $"The module {module.Name} has microflows : {response}";
            }
            return $"A module with name {moduleName} was not found";
        }

        public string createMicroflow(IModel currentApp, string moduleName, string microflowName, string? folderPath)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            if (module != null)
            {
                using var transaction = currentApp.StartTransaction("Create new microflow");
                IMicroflow newMicroflow = currentApp.Create<IMicroflow>();
                newMicroflow.Name = microflowName;

                if (!string.IsNullOrEmpty(folderPath))
                {
                    var folderModel = JsonSerializer.Deserialize<FolderModel>(folderPath);
                    var folder = folderModel != null ? FolderTools.GetFolderFromPath(module, folderModel) : null;

                    if (folder != null)
                    {
                        folder.AddDocument(newMicroflow);
                        transaction.Commit();
                        return $"The microflow {microflowName} was created in the module {moduleName} in the folder {folder.Name}";
                    }

                    module.AddDocument(newMicroflow);
                    transaction.Commit();
                    return $"The specified folder path does not exist in the module {moduleName}. Adding the microflow to the module root instead.";
                }
            }
            return $"A module with name {moduleName} was not found";
        }
    }
}

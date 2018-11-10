using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace CoLatex.Projects
{
    public class ProjectManager
    {
        private static string ProjectHomeDirectory = "./projects/";
        private IHubContext<ProjectHub> _hubContext;


        public ProjectManager(IHubContext<ProjectHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public FileListModel GetFileList(string project)
        {
            string projectDirectory = GetProjectDirectory(project);

            return new FileListModel
            {
                Files = Directory.GetFiles(projectDirectory, "*.*", SearchOption.AllDirectories)
                    .Select(path => GetFileModel(Path.GetRelativePath(projectDirectory, path))).ToList()
            };
        }

        private string GetProjectDirectory(string project)
        {
            return Path.Combine(ProjectHomeDirectory, project);
        }

        private FileModel GetFileModel(string path)
        {
            return new FileModel
            {
                Path = path,
                IsBinary = false,
                LiveEditable = true
            };
        }
    }
}
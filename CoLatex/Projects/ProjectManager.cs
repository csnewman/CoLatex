using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace CoLatex.Projects
{
    public class ProjectManager
    {
        private static string ProjectHomeDirectory = "./projects/";
        private IHubContext<ProjectHub> _hubContext;
        private MemoryCache _fileAccessCache;


        public ProjectManager(IHubContext<ProjectHub> hubContext)
        {
            _hubContext = hubContext;
            _fileAccessCache = new MemoryCache(new MemoryCacheOptions());
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

        private string GetFilePath(string project, string file)
        {
            return Path.Combine(GetProjectDirectory(project), file);
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

        public string GenerateFileAccessToken(string project, string file)
        {
            string token = Guid.NewGuid().ToString();
            _fileAccessCache.Set(token, GetFilePath(project, file), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(12))
            });
            return token;
        }

        public string GetResourcePath(string token)
        {
            return _fileAccessCache.Get(token) as string;
        }
    }
}
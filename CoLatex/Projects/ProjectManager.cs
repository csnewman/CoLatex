using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace CoLatex.Projects
{
    public class ProjectManager
    {
        private static string ProjectHomeDirectory = "./projects/";
        private static string BuildHomeDirectory = "./builds/";
        private IHubContext<ProjectHub> _hubContext;
        private MemoryCache _fileAccessCache;
        private MemoryCache _buildCache;


        public ProjectManager(IHubContext<ProjectHub> hubContext)
        {
            _hubContext = hubContext;
            _fileAccessCache = new MemoryCache(new MemoryCacheOptions());
            _buildCache = new MemoryCache(new MemoryCacheOptions());
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

        public string GetProjectDirectory(string project)
        {
            return Path.Combine(ProjectHomeDirectory, project);
        }

        public string GetBuildDirectory(string project)
        {
            return Path.Combine(BuildHomeDirectory, project);
        }

        public string GetFilePath(string project, string file)
        {
            return Path.Combine(GetProjectDirectory(project), file);
        }

        public string GetBuildFilePath(string project, string file)
        {
            return Path.Combine(GetBuildDirectory(project), file);
        }

        public FileModel GetFileModel(string path)
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

        public Task OnFileAdded(string project, string path)
        {
            return _hubContext.Clients.Group(project).SendAsync("FileAdded", GetFileModel(path));
        }

        public async Task<BuildInfo> GetBuildInfoAsync(string projectId)
        {
            if (_buildCache.TryGetValue(projectId, out BuildInfo info))
                return info;
            BuildInfo newInfo = new BuildInfo(this, projectId);
            _fileAccessCache.Set(projectId, newInfo, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(12))
            });
            return newInfo;
        }

        public void OnBuilt(BuildInfo info)
        {
            string pdftoken = null;

            if (info.State == BuildInfo.BuildState.Built)
            {
                pdftoken = Guid.NewGuid().ToString();
                _fileAccessCache.Set(pdftoken, GetBuildFilePath(info.ProjectId, "main.pdf"), new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(12))
                });
            }

            _hubContext.Clients.Group(info.ProjectId).SendAsync("ProjectBuild", new BuildInfoModel
            {
                ProjectId = info.ProjectId,
                LastLog = info.LastLog,
                State = info.State,
                PdfResourceToken = pdftoken
            }).Wait();
        }
    }
}
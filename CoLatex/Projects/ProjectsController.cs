using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoLatex.Authentication;
using CoLatex.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CoLatex.Projects
{
    [Produces("application/json")]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ProjectRepository _projectRepository;
        private ProjectManager _projectManager;

        public ProjectsController(ProjectRepository projectRepository, ProjectManager projectManager)
        {
            _projectRepository = projectRepository;
            _projectManager = projectManager;
        }

        [HttpGet("list")]
        public async Task<ProjectListResponseModel> GetProjectsAsync()
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            List<ProjectDbModel> dbModels = await _projectRepository.GetProjectsForUser(username);

            return new ProjectListResponseModel
            {
                Projects = dbModels.Select(model => new ProjectResponseModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    LastEdit = model.LastEdit,
                    Owner = model.Owner,
                    Collaborators = model.Collaborators
                }).ToList()
            };
        }

        [HttpPost("create")]
        public async Task<ProjectResponseModel> CreateProject([FromBody] CreateProjectModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            string id = Guid.NewGuid().ToString();

            ProjectDbModel dbModel = new ProjectDbModel
            {
                Id = id,
                Collaborators = new List<string>(),
                LastEdit = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Name = model.Name,
                Owner = username
            };
            await _projectRepository.AddProject(dbModel);

            return new ProjectResponseModel
            {
                Id = id,
                Name = model.Name,
                Collaborators = new List<string>(),
                LastEdit = dbModel.LastEdit,
                Owner = username
            };
        }

        [HttpGet("download-resource/{token}")]
        [AllowAnonymous]
        public async Task<FileResult> DownloadResourceAsync(string token)
        {
            return new PhysicalFileResult(Path.GetFullPath(_projectManager.GetResourcePath(token)),
                "application/octet-stream");
        }


        // Use FormData in ajax to encode upload model, not json
        [HttpPost("upload")]
        public async Task<UploadResponseModel> UploadResourceAsync([FromBody] UploadModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new UploadResponseModel
                {
                    Success = false
                };
            }

            string path = Path.GetFullPath(_projectManager.GetFilePath(model.ProjectId, model.Path));
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await model.File.CopyToAsync(fileStream);
            }

            await _projectManager.OnFileAdded(model.ProjectId, model.Path);

            return new UploadResponseModel
            {
                Success = true,
                File = _projectManager.GetFileModel(model.Path)
            };
        }

        [HttpPost("create-resource")]
        public async Task<CreateResponseModel> CreateResourceAsync([FromBody] CreateModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new CreateResponseModel
                {
                    Success = false
                };
            }

            string path = Path.GetFullPath(_projectManager.GetFilePath(model.ProjectId, model.Path));
            new FileStream(path, FileMode.Create).Dispose();

            await _projectManager.OnFileAdded(model.ProjectId, model.Path);

            return new CreateResponseModel
            {
                Success = true,
                File = _projectManager.GetFileModel(model.Path)
            };
        }
    }
}
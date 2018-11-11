using System;
using System.Buffers.Text;
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

        [HttpPost("info")]
        public async Task<GetProjectResponseModel> GetProjectAsync([FromBody] GetProjectModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new GetProjectResponseModel
                {
                    Success = false,
                    Error = GetProjectResponseModel.ErrorReason.Unauthorised
                };
            }

            return new GetProjectResponseModel
            {
                Success = true,
                Project = new ProjectResponseModel
                {
                    Id = dbModel.Id,
                    Name = dbModel.Name,
                    LastEdit = dbModel.LastEdit,
                    Owner = dbModel.Owner,
                    Collaborators = dbModel.Collaborators
                }
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

        [HttpPost("rename")]
        public async Task<RenameProjectResponseModel> RenameProject([FromBody] RenameProjectModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new RenameProjectResponseModel
                {
                    Success = false,
                    Error = RenameProjectResponseModel.ErrorReason.Unauthorised
                };
            }

            dbModel.Name = model.Name;
            await _projectRepository.UpdateProject(dbModel);

            return new RenameProjectResponseModel
            {
                Success = true
            };
        }

        [HttpPost("delete")]
        public async Task<DeleteProjectResponseModel> DeleteProject([FromBody] DeleteProjectModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new DeleteProjectResponseModel
                {
                    Success = false,
                    Error = DeleteProjectResponseModel.ErrorReason.Unauthorised
                };
            }

            await _projectRepository.DeleteProject(dbModel);

            return new DeleteProjectResponseModel
            {
                Success = true
            };
        }

        [HttpGet("download-resource/{token}")]
        [AllowAnonymous]
        public async Task<FileResult> DownloadResourceAsync(string token)
        {
            string path = Path.GetFullPath(_projectManager.GetResourcePath(token));
            return new PhysicalFileResult(path, path.EndsWith(".pdf") ? "application/pdf" : "application/octet-stream");
        }


        // Use FormData in ajax to encode upload model, not json
        [HttpPost("upload-resource")]
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

//            byte[] decodedBytes = Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(model.File)));
//            string decodedTxt = System.Text.Encoding.UTF8.GetString(decodedBytes);

            var logWriter = new System.IO.StreamWriter(path);
            logWriter.WriteLine(model.File);
            logWriter.Dispose();
//            using (FileStream fileStream = new FileStream(path, FileMode.Create))
//            {
//                await model.File.CopyToAsync(fileStream);
//            }

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

        [HttpPost("build")]
        public async Task<BuildProjectResponseModel> BuildProject([FromBody] BuildProjectModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new BuildProjectResponseModel
                {
                    Success = false
                };
            }

            (await _projectManager.GetBuildInfoAsync(model.ProjectId)).PerformBuild();

            return new BuildProjectResponseModel
            {
                Success = true
            };
        }


        [HttpPost("add-collaborator")]
        public async Task<AddCollaboratorResponseModel> AddCollaborator([FromBody] AddCollaboratorModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new AddCollaboratorResponseModel
                {
                    Success = false
                };
            }

            dbModel.Collaborators.Add(model.Username);
            await _projectRepository.UpdateProject(dbModel);

            return new AddCollaboratorResponseModel
            {
                Success = true
            };
        }

        [HttpPost("remove-collaborator")]
        public async Task<AddCollaboratorResponseModel> RemoveCollaborator([FromBody] AddCollaboratorModel model)
        {
            ClaimsPrincipal principal = HttpContext.User;
            string username = principal.FindFirstValue("username");

            ProjectDbModel dbModel = await _projectRepository.GetProject(model.ProjectId);

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                return new AddCollaboratorResponseModel
                {
                    Success = false
                };
            }

            dbModel.Collaborators.Remove(model.Username);
            await _projectRepository.UpdateProject(dbModel);

            return new AddCollaboratorResponseModel
            {
                Success = true
            };
        }
    }
}
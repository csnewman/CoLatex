using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoLatex.Authentication;
using CoLatex.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLatex.Projects
{
    [Produces("application/json")]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ProjectRepository _projectRepository;

        public ProjectsController(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
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
    }
}
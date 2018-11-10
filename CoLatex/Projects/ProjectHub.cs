using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoLatex.Database;
using Microsoft.AspNetCore.SignalR;

namespace CoLatex.Projects
{
    public class ProjectHub : Hub
    {
        private ProjectManager _projectManager;
        private ProjectRepository _projectRepository;

        public ProjectHub(ProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public async Task OpenProject(string projectId)
        {
            if (Context.Items.ContainsKey("project"))
                throw new InvalidOperationException("A project is already open");

            ProjectDbModel dbModel = await _projectRepository.GetProject(projectId);

            string username = Context.User.FindFirstValue("username");

            if (!(string.Equals(dbModel.Owner, username) ||
                  (dbModel.Collaborators != null && dbModel.Collaborators.Contains(username))))
            {
                throw new InvalidOperationException("Not allowed access to this project");
            }

            Context.Items.Add("project", projectId);
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);

            await Clients.Caller.SendAsync("FileList", _projectManager.GetFileList(projectId));
        }
    }
}
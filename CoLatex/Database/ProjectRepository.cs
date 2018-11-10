using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CoLatex.Database
{
    public class ProjectRepository
    {
        private DatabaseContext _databaseContext;

        public ProjectRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task<List<ProjectDbModel>> GetProjectsForUser(string username)
        {
            return _databaseContext.Projects.Find(project =>
                string.Equals(project.Owner, username) ||
                project.Collaborators != null && project.Collaborators.Contains(username)).ToListAsync();
        }

        public Task<ProjectDbModel> GetProject(string projectId)
        {
            return _databaseContext.Projects.Find(project => string.Equals(project.Id, projectId))
                .FirstOrDefaultAsync();
        }

        public Task AddProject(ProjectDbModel model)
        {
            return _databaseContext.Projects.InsertOneAsync(model);
        }
    }
}
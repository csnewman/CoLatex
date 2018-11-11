using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLatex.Projects
{
    public class BuildInfo
    {
        private ProjectManager _projectManager;
        public string ProjectId { get; private set; }

        public BuildState State { get; private set; }

        public string LastLog { get; private set; }

        public BuildInfo(ProjectManager projectManager, string projectId)
        {
            _projectManager = projectManager;
            ProjectId = projectId;
        }

        public void PerformBuild()
        {
            State = BuildState.Building;
            Task.Run(() =>
            {
                LastLog = "This needs implementing";
                State = BuildState.BuildFailed;
                _projectManager.OnBuilt(this);
            });
        }

        public enum BuildState
        {
            NotBuilt,
            Building,
            Built,
            BuildFailed
        }

    }
}
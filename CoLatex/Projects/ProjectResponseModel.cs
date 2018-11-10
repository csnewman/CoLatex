using System.Collections.Generic;

namespace CoLatex.Projects
{
    public class ProjectResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public IList<string> Collaborators { get; set; }
        public long LastEdit { get; set; }
    }
}
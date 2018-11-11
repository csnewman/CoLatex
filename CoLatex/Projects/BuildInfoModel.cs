namespace CoLatex.Projects
{
    public class BuildInfoModel
    {
        public string ProjectId { get; set; }

        public BuildInfo.BuildState State { get; set; }

        public string LastLog { get; set; }
    }
}
using Microsoft.AspNetCore.Http;

namespace CoLatex.Projects
{
    public class UploadModel
    {
        public string ProjectId { get; set; }
        public string Path { get; set; }
        public IFormFile File { get; set; }
    }
}
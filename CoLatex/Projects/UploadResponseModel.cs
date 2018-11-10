using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLatex.Projects
{
    public class UploadResponseModel
    {
        public bool Success { get; set; }
        public FileModel File { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoLatex.Controllers
{
    public class EditorController : Controller
    {
        public IActionResult Index(string project)
        {
            return View();
        }
    }
}
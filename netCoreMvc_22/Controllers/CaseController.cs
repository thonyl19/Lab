using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace netCoreMvc_22.Controllers
{
    public class CaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult exViewData()
        {
            return View();
        }
        

    }
}
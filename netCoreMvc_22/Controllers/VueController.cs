using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using netCoreMvc_22.Models;

namespace netCoreMvc_22.Controllers
{
    public class TestViewModel
    {
        //方字方塊最常見和string型別binding
        public string strMsg { get; set; }

    }
    public class VueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult case02()
        {
            return View();
        }
        public IActionResult case03()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult case03(TestViewModel vm)
        {
            ViewData["showMsg"] = vm.strMsg;
            
            return View(vm);
        }
    }
}

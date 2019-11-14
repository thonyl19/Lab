using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using netCoreMvc_22.Models;

namespace netCoreMvc_22.Controllers
{
    public class RTAController : Controller
    {
        public IActionResult Index()
        {
            //var x = this.HttpContext.Request.Query. Select(e=>new {e.Key);
            var nvc = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
            var x =  nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
            return RedirectToAction(nameof(RTA03),new RouteValueDictionary(x));
            // var x = this.Request.Query.ToDictionary<string,object>(k => k, k => (object)parsed[k]);
            // var parsed = HttpUtility.ParseQueryString(spliturl.queryString); 
            // Dictionary<string,object> querystringDic = parsed.AllKeys.ToDictionary(k => k, k => (object)parsed[k]); 

        


            //return RedirectToAction(nameof(RTA03),new {A=2});
        }

        public IActionResult RTA01()
        {
            return View();
        }

        public IActionResult RTA02()
        {
            return RedirectToAction(nameof(RTA01));
        }

        public IActionResult RTA03(string A)
        {
            ViewData["A"] = A;
            return View();
        }

    }
}

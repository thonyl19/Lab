using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace netCoreMvc_22
{
    public class B02
    {
        public class controller:Controller 
        {
            public virtual string Index()
            {
                return "This is my default action...";
            }

            // 因為 下面  Welcome(string name, int numTimes = 1) 的方法,會導致 MVC 出現 如下的
            //      The request matched multiple endpoints. Matches  
            // 所以必須得先把 此程式註解掉才能正常執行,特此備忘.
            // public string Welcome()
            // {
            //     return "This is the Welcome action method...";
            // }

            public string Welcome(string name, int numTimes = 1)
            {
                return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
            }
        }
    }

    public class B03
    {
        /// <summary>
        /// 因為 B03.Index 跟  B02.Index 的回傳值不一樣,無法用複寫的方式做成漸近式的範例,
        /// 所以只能另外建立
        /// </summary>
        public class controller:Controller
        {
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Welcome(string name, int numTimes = 1)
            {
                ViewData["Message"] = "Hello " + name;
                ViewData["NumTimes"] = numTimes;

                return View();
            }
        }
    }
}
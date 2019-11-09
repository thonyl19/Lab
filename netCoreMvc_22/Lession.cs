using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace netCoreMvc_22
{
    public class B02
    {
        public class controller:Controller 
        {
            public string Index()
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
}
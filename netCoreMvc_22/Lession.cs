using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using netCoreMvc_22.Models;

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
    public class B04
    {
        internal static void ConfigureServices_IocDBContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MovieContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("MovieContext")));
        }

        public class _Movie
        {
            public int Id { get; set; }
            public string Title { get; set; }

            [DataType(DataType.Date)]
            public DateTime ReleaseDate { get; set; }
            public string Genre { get; set; }
            public decimal Price { get; set; }
        }

        public static void Main_CreateDbIfNotExists(IWebHost host)
        {
            /*Microsoft.Extensions.DependencyInjection*/
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<MovieContext>();
                    context.Database.EnsureCreated();
                    
                    
                    // DB has been seeded
                    if (context.Movie.Any())  return;   

                    context.Movie.AddRange(
                        new Movie
                        {
                            Title = "When Harry Met Sally",
                            ReleaseDate = DateTime.Parse("1989-2-12"),
                            Genre = "Romantic Comedy",
                            Price = 7.99M
                        },

                        new Movie
                        {
                            Title = "Ghostbusters ",
                            ReleaseDate = DateTime.Parse("1984-3-13"),
                            Genre = "Comedy",
                            Price = 8.99M
                        },

                        new Movie
                        {
                            Title = "Ghostbusters 2",
                            ReleaseDate = DateTime.Parse("1986-2-23"),
                            Genre = "Comedy",
                            Price = 9.99M
                        },

                        new Movie
                        {
                            Title = "Rio Bravo",
                            ReleaseDate = DateTime.Parse("1959-4-15"),
                            Genre = "Western",
                            Price = 3.99M
                        }
                    );
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

    }

}
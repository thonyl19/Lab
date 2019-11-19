using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using netCoreMvc_22.Models;
using netCoreMvc_22.Controllers;
using System.Web;
using Microsoft.AspNetCore.Routing;

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

    /// <summary>
    /// 新增模型
    /// </summary>
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
            public virtual DateTime ReleaseDate { get; set; }
            public string Genre { get; set; }
            public virtual decimal Price { get; set; }
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

                    B08.initMove(context.Movie);
                    context.SaveChanges();
                    
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        /// <summary>
        /// 資料庫初始化填值程序 
        /// </summary>
        /// <param name="Movie"></param>
        public static void initMove(DbSet<Movie> Movie){
            Movie.AddRange(
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
        }

    }

    /// <summary>
    /// 控制器動作與檢視
    /// </summary>
    public class B06{
        public class _Movie:B04._Movie
        {
            [Display(Name = "Release Date")]
            [DataType(DataType.Date)]
            public override DateTime ReleaseDate { get; set; }

            [Column(TypeName = "decimal(18, 2)")]
            public override decimal Price { get; set; }
        }
    }

    /// <summary>
    /// 新增搜尋
    /// </summary>
    public class B07{

        public class MoviesController : _MoviesController
        {
            public MoviesController(MovieContext context) : base(context)
            {
            }

            public override async Task<IActionResult> Index()
            {
                
                var a = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
                var b1 = a.AllKeys.ToDictionary(k => k, k => a[k]);
                // var b2 = this.Request.Query.Keys.ToDictionary(k=>k,k=>this.Request.Query[k].ToString());
                var c1 = new RouteValueDictionary(b1);
                // var c2 = new RouteValueDictionary(b2);

                //B071
                //return RedirectToActionPermanent(nameof(Index_Search),c1);
                //return RedirectToAction(nameof(Index_Search),c1);
                
                //B072
                //return RedirectToAction(nameof(Index_Id));

                //B073
                return RedirectToAction(nameof(Index_GenreVM),c1);
            }


            /// <summary>
            /// B071 
            /// </summary>
            /// <param name="searchString"></param>
            /// <returns></returns>
            public async Task<IActionResult> Index_Search(string searchString)
            {
                var movies = from m in _context.Movie
                            select m;

                if (!String.IsNullOrEmpty(searchString))
                {
                    movies = movies.Where(s => s.Title.Contains(searchString));
                }

                return View("B071", await movies.ToListAsync());
            }


            /// <summary>
            /// B072 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public async Task<IActionResult> Index_Id(string id)
            {
                var movies = from m in _context.Movie
                            select m;

                if (!String.IsNullOrEmpty(id))
                {
                    movies = movies.Where(s => s.Title.Contains(id));
                }

                return View("Index", await movies.ToListAsync());
            }


            /// <summary>
            /// B073 - MovieGenreViewModel
            /// </summary>
            /// <param name="movieGenre"></param>
            /// <param name="searchString"></param>
            /// <returns></returns>
            public async Task<IActionResult> Index_GenreVM(string movieGenre, string searchString)
            {
                // Use LINQ to get list of genres.
                IQueryable<string> genreQuery = from m in _context.Movie
                                                orderby m.Genre
                                                select m.Genre;

                var movies = from m in _context.Movie
                            select m;

                if (!string.IsNullOrEmpty(searchString))
                {
                    movies = movies.Where(s => s.Title.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(movieGenre))
                {
                    movies = movies.Where(x => x.Genre == movieGenre);
                }

                var movieGenreVM = new MovieGenreViewModel
                {
                    Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                    Movies = await movies.ToListAsync()
                };

                return View("B073",movieGenreVM);
            }

            [HttpPost]
            public string Index(string searchString, bool notUsed)
            {
                return "From [HttpPost]Index: filter on " + searchString;
            }



        }
        
        public class _MovieGenreViewModel
        {
            public List<Movie> Movies { get; set; }
            public SelectList Genres { get; set; }
            public string MovieGenre { get; set; }
            public string SearchString { get; set; }
        }
    }

    /// <summary>
    /// 新增欄位
    /// </summary>
    public class B08{
        public class _Movie:B06._Movie
        {
            public string Rating { get; set; }
        }

        /// <summary>
        /// 資料庫初始化填值程序 
        /// </summary>
        /// <param name="Movie"></param>
        public static void initMove(DbSet<Movie> Movie){
            Movie.AddRange(
                new Movie
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.Parse("1989-2-12"),
                    Genre = "Romantic Comedy",
                    Rating = "R",
                    Price = 7.99M
                },

                new Movie
                {
                    Title = "Ghostbusters ",
                    ReleaseDate = DateTime.Parse("1984-3-13"),
                    Genre = "Comedy",
                    Rating = "R",
                    Price = 8.99M
                },

                new Movie
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23"),
                    Genre = "Comedy",
                    Rating = "R",
                    Price = 9.99M
                },

                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15"),
                    Genre = "Western",
                    Rating = "R",
                    Price = 3.99M
                }
            );
        }

        public class MoviesController : _MoviesController
        {
            public MoviesController(MovieContext context) : base(context)
            {
            }

            public override async Task<IActionResult> Index()
            {
                //B081
                return RedirectToAction(nameof(Index_GenreVM),this.routerValue);
            }

 

            /// <summary>
            /// B081 - Rating
            /// </summary>
            /// <param name="movieGenre"></param>
            /// <param name="searchString"></param>
            /// <returns></returns>
            public async Task<IActionResult> Index_GenreVM(string movieGenre, string searchString)
            {
                // Use LINQ to get list of genres.
                IQueryable<string> genreQuery = from m in _context.Movie
                                                orderby m.Genre
                                                select m.Genre;

                var movies = from m in _context.Movie
                            select m;

                if (!string.IsNullOrEmpty(searchString))
                {
                    movies = movies.Where(s => s.Title.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(movieGenre))
                {
                    movies = movies.Where(x => x.Genre == movieGenre);
                }

                var movieGenreVM = new MovieGenreViewModel
                {
                    Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                    Movies = await movies.ToListAsync()
                };

                return View("B081",movieGenreVM);
            }

  



        }
    }

    /// <summary>
    /// 新增驗證
    /// </summary>
    public class B09{
        /// <summary>
        /// 
        /// </summary>
        /// SPEC)因應驗証程序的處理,所以這一版的做法無法使用漸進模式
        public class _Movie 
        {
            public int Id { get; set; }

            [StringLength(60, MinimumLength = 3)]
            [Required]
            public string Title { get; set; }

            [Display(Name = "Release Date")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            public DateTime ReleaseDate { get; set; }

            [Range(1, 100)]
            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal Price { get; set; }

            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
            [Required]
            [StringLength(30)]
            public string Genre { get; set; }

            [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
            [StringLength(5)]
            [Required]
            public string Rating { get; set; }
        }
    }
}
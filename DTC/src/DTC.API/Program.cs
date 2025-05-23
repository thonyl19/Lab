using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DTC.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 確保 DB 目錄存在，並將連線字串轉為絕對路徑
var dbFolder = Path.Combine(AppContext.BaseDirectory, "DB");
if (!Directory.Exists(dbFolder))
{
    Directory.CreateDirectory(dbFolder);
}
var dbPath = Path.Combine(dbFolder, "todo.db");
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

// 添加服務到容器
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1",
        Description = "A simple Todo API built with .NET 6",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });
});

// 配置 DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

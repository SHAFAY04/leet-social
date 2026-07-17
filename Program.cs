using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITokensService, TokensService>();


var dbConnectionString=Environment.GetEnvironmentVariable("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>((options)=>options.UseNpgsql(dbConnectionString));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.MapGet("/hello", () =>
{

});

app.Run();

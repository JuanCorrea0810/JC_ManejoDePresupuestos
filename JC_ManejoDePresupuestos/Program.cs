using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings.Web;

public class Program
{
    public static IConfiguration Configuration { get; set; }
    private static void Main(string[] args)
    {
        var builderConfig = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json", optional: true)
        .AddEnvironmentVariables();

        Configuration = builderConfig.Build();

        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers().AddNewtonsoftJson().
                        AddJsonOptions(options =>
                        {
                            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                        });
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("MyDB"));
        });

        builder.Services.AddIdentity<NewIdentityUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredUniqueChars = 3;
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters + "*´!# /$";
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        });
        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddScoped<IRepositorioTipoCuentas, RepositorioTipoCuentas>();
        builder.Services.AddScoped<IRepositorioCuentas, RepositorioCuentas>();
        builder.Services.AddScoped<IRepositorioCategorias,RepositorioCategorias>();
        builder.Services.AddScoped<IRepositorioTransacciones, RepositorioTransacciones>();
        builder.Services.AddTransient<IGetUserInfo, GetUserInfo>();
        builder.Services.AddTransient<IServicioEmail, ServicioEmailElasticEmail>();



        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
        })
        .AddCookie(options =>
         {
             options.Cookie.Name = "AuthCookie";
             options.Cookie.SameSite = SameSiteMode.Strict;
             options.Cookie.HttpOnly = false;
             options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
             options.Cookie.Expiration = TimeSpan.FromDays(15);
             options.Events = new CookieAuthenticationEvents
             {
                 OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
                 OnRedirectToLogin = ctx =>
                 {
                     ctx.Response.Redirect("/Registro/LogIn?ReturnUrl=" +
                         UrlEncoder.Default.Encode(ctx.Request.Path));
                     return Task.CompletedTask;
                 }
             };
         }); ;

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = new PathString("/Registro/LogIn");         
        });

        builder.Services.AddAuthorization();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.Run();
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using WebApp.BLL;
using WebApp.BLL.Interfaces;
using WebApp.DAL.Data.Contexts;
using WebApp.DAL.Models;
using WebApp.PL.Helpers;
using WebApp.PL.Mapping;
using WebApp.PL.Settings;

namespace WebApp.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region Identity Services
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<CompanyContext>().AddDefaultTokenProviders();
            #endregion

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/SignIn";
            });

            #region Google Auth

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //})
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/SignIn";
            //    options.LogoutPath = "/Account/SignOut";
            //    options.AccessDeniedPath = "/Home/AccessDenied";
            //})
            //.AddGoogle(options =>
            //{
            //    options.ClientId = builder.Configuration["Auth:Google:ClientId"];
            //    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
            //});


            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            //    options.DefaultScheme = IdentityConstants.ApplicationScheme;
            //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

            //})
            //  .AddGoogle(options =>
            //  {
            //      options.ClientId = builder.Configuration["Auth:Google:ClientId"];
            //      options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
            //  });


            #endregion

            #region DbContext Services
            builder.Services.AddDbContext<CompanyContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion

            #region UnitOfWork Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region MailServices
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddScoped<IMailServices, MailServices>();
            #endregion

            #region AutoMapper Services
            builder.Services.AddAutoMapper(m => m.AddProfile(new DepartmentProfile()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new EmployeeProfile()));
            #endregion

            #region GoogleAuth Services
           
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Auth:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
            });

            #endregion



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
}

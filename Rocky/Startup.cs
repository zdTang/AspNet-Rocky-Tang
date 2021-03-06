using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocky.Data;
using Rocky.Examples.DI;
using Rocky.Utility;
using System;

namespace Rocky
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Do DI here !!!!
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(); // MVC
            // Add session
            services.AddHttpContextAccessor();

            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

            // Added IdentityUser
            // need packages:
            // Microsoft.AspNetCore.Identity.EntityFrameworkcore
            // Microsoft.AspNetCore.Identity.UI

            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            // As AddDefaultIdentity<>() has not provide IdentityRole
            // So that we need use AddIdentity()

            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()            //  switched from upside
              .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IEmailSender, EmailSender>();         //  Add email service
            //services.AddTransient<IEmailSender, EmailSenderTwo>();         //  Add email service

            //Test by myself
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0
            services.AddScoped<IMyDependency, MyDependency>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Enforce to use HTTPS
            //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();   // need it to work with IdentityUser
                                       // should put before UseAuthorization()

            app.UseAuthorization();
            // Add session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                // route to Razor pages
                endpoints.MapRazorPages();
                // route to MVC pages
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocky_DataAccess.Data;
using Rocky.Examples.DI;
using Rocky_Utility;
using System;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_DataAccess.Repository;
using Rocky_Utility.BrainTree;
using Rocky_DataAccess.Initializer;

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

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            // Retrive "BrainTree" section from "appsetting.json"
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            // Register Braintree class, make sure the Interface is public as it lies in different module
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

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
            //To migrate Db, create roles and Admin
            services.AddScoped<IMyDependency, MyDependency>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            // Add FaceBook authentification = 915462692687085
            // ToDo: is this a good place to save these keys ?????
            services.AddAuthentication().AddFacebook(Options =>
            {
                Options.AppId = "915462692687085";
                Options.AppSecret = "c7e7f58bf79500ebcaaa5745db67e3e6";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
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

            dbInitializer.Initalize();
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

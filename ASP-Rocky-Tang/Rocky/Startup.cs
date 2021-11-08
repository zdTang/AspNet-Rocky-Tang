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

        // can assess appsettings.json file to read Setup string such as DB connection string....
        public IConfiguration Configuration { get; }


        // it is a constructor here
        // using DI to inject configuration object
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;    // DI 
        }

   

        // This method gets called by the runtime. Use this method to add services to the container.
        // Do DI here !!!!
        // I switch the name to 'ConfigureXXXServices' format to make it a environment-specific
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">The handler to access all types currently registered with the DI system</param>
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Register static file browsing www root path
            //services.AddDirectoryBrowser();
            // Register MVC
            services.AddControllersWithViews();

            // Can use AddMvc() as well, it will add RazorPage Service as well
            //https://stackoverflow.com/questions/62251347/services-addcontrollerswithviews-vs-services-addmvc
            //services.AddMvc();

            // Register HttpContext so as we can use HttpContext
            services.AddHttpContextAccessor();
            // Add session
            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

            // Add DbContext
            // Here using 'Configuration' object to access 'appsettings.json' 
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
        /// <summary>
        ///    I switch the name to 'ConfigureXXX' format to make it a environment-specific
        /// </summary>
        /// <param name="app">An object passed by Runtime for adding Middleware</param>
        /// <param name="env">An object passed by runtime for reading Environment data</param>
        /// <param name="dbInitializer">User customized Object for seeding Database for the first run</param>
        /// 
        public void ConfigureDevelopment(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();     //  about HTTPS
            }
            // Enforce to use HTTPS
            //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            //app.UseDirectoryBrowser();  // Browse static files

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

        /// <summary>
        /// Reserved for Production Environment
        /// </summary>
        public void ConfigureProductionServices()
        {

        }




        /// <summary>
        /// Reserved for Production Environment
        /// </summary>
        public void ConfigureProduction()
        {

        }
       
    }
}

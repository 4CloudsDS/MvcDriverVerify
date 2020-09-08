using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcDriverVerify.Areas.Identity.Data;
using MvcDriverVerify.Models;

namespace MvcDriverVerify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<VerifyDriverContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("VerifyDriverContext")));

            //services.AddIdentity<SuperUser, IdentityRole>()
            //    .AddEntityFrameworkStores<VerifyDriverContext>()
            //    .AddDefaultTokenProviders();

            //services.AddAuthentication().AddTwitter(twitterOptions =>
            //{
            //    twitterOptions.ConsumerKey = "Twitter:ConsumerAPIKey";
            //    twitterOptions.ConsumerSecret = "Twitter:ConsumerSecret";
            //});

           

            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    //facebookOptions.AppId = "2375141766143418";
            //    //facebookOptions.AppSecret = "123456789";
            //});



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

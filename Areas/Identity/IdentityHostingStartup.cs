using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcDriverVerify.Areas.Identity.Data;
using MvcDriverVerify.Models;

[assembly: HostingStartup(typeof(MvcDriverVerify.Areas.Identity.IdentityHostingStartup))]
namespace MvcDriverVerify.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            NewMethod(builder);
        }

        private static IWebHostBuilder NewMethod(IWebHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<MvcDriverVerifyIdentityDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("MvcDriverVerifyIdentityDbContext")));

                services.AddIdentity<IdentityUser, IdentityRole>(options => {

                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<MvcDriverVerifyIdentityDbContext>()
                    .AddDefaultTokenProviders();

                services.AddAuthentication().AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "998016707209482";
                    facebookOptions.AppSecret = "09627564d552690d2789fbb41c194e56";
                });
                services.AddAuthentication().AddTwitter(twitteroptions =>
                {
                    twitteroptions.ConsumerKey = "998016707209482";
                    twitteroptions.ConsumerSecret = "09627564d552690d2789fbb41c194e56";
                });
                services.AddAuthentication().AddLinkedIn(options => 
                {
                        options.ClientId = "998016707209482";
                        options.ClientSecret = "09627564d552690d2789fbb41c194e56";
                        options.Events = new OAuthEvents() { OnRemoteFailure = loginFailureHandler => { var authProperties = options.StateDataFormat.Unprotect(loginFailureHandler.Request.Query["state"]);
                        loginFailureHandler.Response.Redirect("/Account/login");
                        loginFailureHandler.HandleResponse();
                        return Task.FromResult(0); } };
                });

                services.AddScoped<IUserStore<SuperUser>, UserStore<SuperUser>>();
                services.AddScoped<UserManager<SuperUser>>();
                services.AddScoped<DbContext, MvcDriverVerifyIdentityDbContext>();
                services.AddScoped<IUserValidator<SuperUser>, UserValidator<SuperUser>>();
                services.AddScoped<IPasswordValidator<SuperUser>, PasswordValidator<SuperUser>>();
                services.AddScoped<IPasswordHasher<SuperUser>, PasswordHasher<SuperUser>>();
                services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
                services.AddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
                // No interface for the error describer so we can add errors without rev'ing the interface
                services.AddScoped<IdentityErrorDescriber>();
                services.AddScoped<ISecurityStampValidator, SecurityStampValidator<SuperUser>>();
                services.AddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<SuperUser>>();
                services.AddScoped<IUserClaimsPrincipalFactory<SuperUser>, UserClaimsPrincipalFactory<SuperUser, IdentityRole>>();
                services.AddScoped<UserManager<SuperUser>>();
                services.AddScoped<SignInManager<SuperUser>>();
                services.AddScoped<RoleManager<IdentityRole>>();
                services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole>>();

                var identityBuilder = new IdentityBuilder(typeof(SuperUser), typeof(IdentityRole<string>), services);
                identityBuilder.AddTokenProvider("Default", typeof(DataProtectorTokenProvider<SuperUser>));

            });

        }
    }
}



using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpBlog.Core.Services;
using SharpBlog.Core.Services.Implementation;
using SharpBlog.Database;
using Microsoft.AspNetCore.Identity;
using System;
using SharpBlog.Client.Services;
using SharpBlog.Client.Services.Implementation;
using SharpBlog.Core.Dal;
using SharpBlog.Core.Dal.Implementation;

namespace SharpBlog.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews();

#if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.AddDbContext<BlogContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Connection")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;   

            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login/";
                    options.LogoutPath = "/Account/Logout/";
                    options.Cookie.Name = "sharpcookie";
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                });

            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IUserDal, UserDal>();
            services.AddScoped<IPostDal, PostDal>();
            services.AddScoped<ICommentDal, CommentDal>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddTransient<ISettingsService, SettingsService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BlogContext blogContext)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Blog/Error");
                app.UseHsts();
            }

            blogContext.Database.Migrate();

            var rewriteOptions = new RewriteOptions();

            if (Configuration.GetValue<bool>("forceSsl"))
            {
                rewriteOptions.AddRedirectToHttpsPermanent();
            }

            if (Configuration.GetValue<bool>("forceWwwPrefix"))
            {
                rewriteOptions.AddRedirectToWwwPermanent();
            }

            app.UseRewriter(rewriteOptions);
            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions());

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Blog}/{action=Index}");
                endpoints.MapControllerRoute(
                    name: "post",
                    pattern: "blog/post/{id}",
                    defaults: new { controller = "Blog", action = "Post" });
                endpoints.MapControllerRoute(
                    name: "postEdit",
                    pattern: "blog/editpost/{id?}",
                    defaults: new { controller = "Blog", action = "EditPost" });
                endpoints.MapControllerRoute(
                    name: "postEdit",
                    pattern: "blog/category/{name}",
                    defaults: new { controller = "Blog", action = "Category" });
            });
        }
    }
}

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

namespace SharpBlog.Client
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddDbContext<BlogContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("Connection")));

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;				
			});

			services
				.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Account/Login/";
					options.LogoutPath = "/Account/Logout/";
					options.Cookie.Name = "sharpcookie";

				});

			services.AddScoped<IHashService, HashService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<ICommentService, CommentService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

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
					pattern: "{controller=Home}/{action=Index}");
				endpoints.MapControllerRoute(
					name: "post",
					pattern: "blog/{id}",
					defaults: new { controller = "Blog", action = "Index" });
				endpoints.MapControllerRoute(
					name: "postEdit",
					pattern: "blog/editpost/{id?}",
					defaults: new { controller = "Blog", action = "EditPost" });
			});
		}
	}
}

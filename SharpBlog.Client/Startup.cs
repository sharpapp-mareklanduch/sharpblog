using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

			services.AddDbContext<BlogContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("WebioConnection")));

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

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}
		
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
			app.UseAuthentication();
			app.UseStaticFiles();
			app.UseCookiePolicy(new CookiePolicyOptions());

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}");
				routes.MapRoute(
					name: "post",
					template: "blog/{id}",
					defaults: new {controller = "Blog", action = "Index"});
				routes.MapRoute(
					name: "postEdit",
					template: "blog/editpost/{id?}",
					defaults: new {controller = "Blog", action = "EditPost"});
			});
		}
	}
}

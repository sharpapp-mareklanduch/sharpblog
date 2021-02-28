using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SharpBlog.Client.Services;
using System.Threading.Tasks;

namespace SharpBlog.Client.Attributes
{
    public class NewUserRedirection : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rs = context.HttpContext.RequestServices;
            var userService = rs.GetService<IUserService>();

            if (await userService.IsNewUserNeeded())
            {
                var controller = (Controller)context.Controller;
                context.Result = controller.RedirectToAction("Register", "Account");
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}

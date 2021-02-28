using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharpBlog.Common.Dal;
using System.Linq;
using System.Threading.Tasks;

namespace SharpBlog.Client
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IUserDal _userDal;

        public CustomCookieAuthenticationEvents(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var passwordChanged = true;
            var userPrincipal = context.Principal;

            var lastPasswordChangeDate = userPrincipal.Claims.FirstOrDefault(c => c.Type == "LastPasswordChangeDate")?.Value;
            if (!string.IsNullOrEmpty(lastPasswordChangeDate))
            {
                passwordChanged = await _userDal.ValidatePasswordChange(lastPasswordChangeDate);
            }

            if (passwordChanged)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}

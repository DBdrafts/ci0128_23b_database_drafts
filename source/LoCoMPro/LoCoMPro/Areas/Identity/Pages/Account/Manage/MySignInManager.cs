using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using LoCoMPro.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LoCoMPro.Areas.Identity.Pages.Account.Manage
{
    public class MySignInManager : SignInManager<User>
    {
        public MySignInManager(UserManager<User> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<User>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        // Overrides PasswordSignInAsync to FindbyEmail instead of FindByName
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password,
            bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}

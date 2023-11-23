using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsRaffles.Data.Models;
using System.Security.Claims;

namespace SportsRaffles.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;

        private readonly IReadOnlyDictionary<string, string> _claimsToSync = new Dictionary<string, string>()
         {
            { "image", "" },
         };

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager,RoleManager<UserRole> roleManager ,ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl != null) return LocalRedirect(returnUrl);
            else return LocalRedirect("/");
        }

        [HttpPost("login/{provider}")]
        public IActionResult Login(string provider, string returnUrl = null)
        {
            var redirectUrl = "https://" + Request.Host + "/loginCallback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [Route("loginCallback")]
        public async Task<IActionResult> LoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                
                return RedirectToPage("./", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {

                if (_claimsToSync.Count > 0)
                {
                    var user = await _userManager.FindByLoginAsync(info.LoginProvider,
                        info.ProviderKey);
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    bool refreshSignIn = false;

                    foreach (var addedClaim in _claimsToSync)
                    {
                        var userClaim = userClaims
                            .FirstOrDefault(c => c.Type == addedClaim.Key);

                        if (info.Principal.HasClaim(c => c.Type == addedClaim.Key))
                        {
                            var externalClaim = info.Principal.FindFirst(addedClaim.Key);
                            if(externalClaim is not null)
                            {
                                if (userClaim == null)
                                {
                                    await _userManager.AddClaimAsync(user,
                                        new Claim(addedClaim.Key, externalClaim.Value));
                                    refreshSignIn = true;
                                }
                                else if (userClaim.Value != externalClaim.Value)
                                {
                                    await _userManager
                                        .ReplaceClaimAsync(user, userClaim, externalClaim);
                                    if(addedClaim.Key == "image")
                                    {
                                        user.AvatarUri = externalClaim.Value;
                                        await _userManager.UpdateAsync(user);
                                    }
                                    refreshSignIn = true;
                                }
                            }
                        }
                        else if (userClaim == null)
                        {
                            await _userManager.AddClaimAsync(user, new Claim(addedClaim.Key,
                                addedClaim.Value));
                            refreshSignIn = true;
                        }
                    }

                    if (refreshSignIn)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                    }
                }

                return Redirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                //No Account -> Create
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    await CreateUser(returnUrl);
                }
                return Redirect(returnUrl);
            }
        }

        public async Task CreateUser(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return;
            }

                var user = CreateUser();
                user.UserName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    if (info.Principal.HasClaim(c => c.Type == "image"))
                    {
                        await _userManager.AddClaimAsync(user, info.Principal.FindFirst("image"));
                        user.AvatarUri = info.Principal.FindFirst("image")?.Value;
                    }

                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        //_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return;
                    }                  
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. ");
            }
        }
    }
}

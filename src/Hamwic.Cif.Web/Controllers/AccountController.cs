using Hamwic.Cif.Core.Entities;
using Hamwic.Cif.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace Hamwic.Cif.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET
        /// <summary>
        /// Captures the return url (if there is one) to populate on the LoginModel
        /// </summary>
        /// <param name="returnUrl">string: the returnUrl to redirect the user to when logged in</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);

                if (user == null)
                {
                    ModelState.AddModelError("Email", "Invalid login attempt.");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    Log.Information($"{model.EmailAddress} signed in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    Log.Warning("User account locked out.");
                    return RedirectToAction("Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
            return Content("Logged in");
        }

        // GET
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //GET
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            //clear the authentication token and re-direct to the login page
            await _signInManager.SignOutAsync();
            Log.Information("User logged out");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Login");
        }

        //GET
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return Content("Forgot Password");
        }

        //GET
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;
using WebApp.PL.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IMailServices mailServices;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMailServices mailServices)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailServices = mailServices;
        }
        #region SignUp
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user is null)
                {
                    user = await userManager.FindByEmailAsync(model.Email);
                    if (user is null)
                    {
                        user = new AppUser()
                        {
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Address = model.Address,
                            Email = model.Email,
                        };
                        var result = await userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                            return RedirectToAction(nameof(SignIn));
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Operation!");
            return View(model);
        }
        #endregion

        #region SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var flag = await userManager.CheckPasswordAsync(user, model.Password);
                    if (flag)
                    {
                        var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid SignIn");
            return View(model);
        }
        #endregion

        #region SignOut
        [HttpGet]
        public new async Task<IActionResult> SignOut()
        {
            //await signInManager.SignOutAsync();
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Sign out any external login cookie (used for Google, Facebook, etc.)
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Optionally sign out the generic "Cookies" scheme too
            return RedirectToAction(nameof(SignIn));

        }
        #endregion

        #region ForgetPassword
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ForgetPassword")]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action(action: "ResetPassword", controller: "Account", new { model.Email, token }, Request.Scheme);

                    var email = new Email()
                    {
                        To = model.Email,
                        Subject = $"Reset Password {user.UserName}",
                        Body = url
                    };
                    bool flag = mailServices.SendEmail(email);
                    if (flag)
                    {
                        return RedirectToAction(nameof(CheckInbox));
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Reset Password !");
            return View(model);
        }
        [HttpGet]
        public IActionResult CheckInbox()
        {
            return View();
        }
        #endregion

        #region ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;
                if (email is null || token is null) return BadRequest("Invalid Operation!");

                var user = await userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region Google Login
        public IActionResult GoogleLogin()
        {
            var prop = new AuthenticationProperties()
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            };

            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var cliams = result.Principal.Identities.FirstOrDefault().Claims.Select(cliam => new
            {
                cliam.Type,
                cliam.Value,
                cliam.Issuer,
                cliam.OriginalIssuer
            });

            return RedirectToAction("Index", "Home");
        }

        #endregion

    }
}

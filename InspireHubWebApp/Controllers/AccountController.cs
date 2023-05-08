using InspireHubWebApp.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            try
            {
                //kthimi i url ne home nese je e loguar
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                if (TempData["Error"] != null)
                {
                    ModelState.AddModelError(string.Empty, TempData["Error"].ToString());
                }
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                //AddError(ex);
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    ModelState.AddModelError("Username", "Username not found");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Training");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Username", "User locked");
                    return View(model);
                }
                else
                {
                    string message = "Invalid password";
                    ModelState.AddModelError("Password", message);
                    return View(model);
                }
                


                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        /*public async Task<IActionResult> Register()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser
                    {

                        UserName = "hello@inspirehub.info",
                        Email = "hello@inspirehub.info",
                        PhoneNumber = null
                    };

                    var userExist = await _userManager.FindByEmailAsync(user.Email);

                    if (userExist != null)
                    {
                        TempData["exists"] = "not null";
                        return View();
                    }

                    var result = await _userManager.CreateAsync(user, "123456");

                    if (!result.Succeeded)
                    {
                        return View();
                    }

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                }
                return View();
            }

            catch (Exception ex)
            {
                return View();
            }
        }*/
    }
}

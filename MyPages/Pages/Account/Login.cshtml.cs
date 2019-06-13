using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Entities;
using MyPages.Extensions;
using MyPages.Models;
using MyPages.Services;

namespace MyPages.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        public string ReturnUrl { get; private set; }

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        [BindProperty]
        public LoginUserViewModel UserModel { get; set; }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            User user;
            try
            {
                user = await _userService.Authenticate(UserModel.Username, UserModel.Password);
            }
            catch (ApplicationException)
            {
                ModelState.AddModelError("UserModel.Username", "Invalid username or password.");
                return Page();
            }

            if (user == null)
            {
                ModelState.AddModelError("UserModel.Username", "Invalid username or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(Url.GetLocalUrl(returnUrl));
        }
    }
}
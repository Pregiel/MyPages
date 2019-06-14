using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Extensions;
using MyPages.Models;
using MyPages.Services;

namespace MyPages.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        public string ReturnUrl { get; private set; }

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        [BindProperty]
        public RegisterUserViewModel UserModel { get; set; }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated || !ModelState.IsValid)
            {
                return Page();
            }

            if (await _userService.GetByUsername(UserModel.Username) != null)
            {
                ModelState.AddModelError("UserModel.Username", "Username already exist.");
                return Page();
            }
            try
            {
                await _userService.Create(UserModel.Username, UserModel.Password);
            }
            catch (ApplicationException)
            {
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserModel.Username)
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
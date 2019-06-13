using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyPages.Dtos;
using MyPages.Entities;
using MyPages.Helpers;
using MyPages.Models;
using MyPages.Services;

namespace MyPages.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public RegisterModel(DataContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RegisterUserViewModel UserModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _userService.GetByUsername(UserModel.Username) != null) {
                ModelState.AddModelError("UserModel.Username", "Username already exist.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _userService.Create(UserModel.Username, UserModel.Password);

            return RedirectToPage("./Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Entities;
using MyPages.Extensions;
using MyPages.Services;

namespace MyPages.Pages.Page
{
    public class DeleteModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public DeleteModel(IUserService userService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return RedirectToPage("/Index");

            try
            {
                var page = await _pageService.GetByIdWithAllParents(id);

                if (_pageService.CheckAccess(page, user))
                    await _pageService.Delete(id);
                else
                    return Unauthorized();
            }
            catch (ApplicationException)
            {
                return BadRequest();
            }

            return Page();
        }
    }
}
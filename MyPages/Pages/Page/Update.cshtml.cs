using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Entities;
using MyPages.Models;
using MyPages.Services;

namespace MyPages.Pages.Page
{
    public class UpdateModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public UpdateModel(IUserService userService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public Object Item;

        [BindProperty]
        public UpdatePageModel PageModel { get; set; }

        public string[] Colors = Helpers.Colors.ColorValues;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            try
            {
                var page = await _pageService.GetByIdWithAllParents(id);

                if (_pageService.CheckAccess(page, user))
                {
                    Item = page;
                    PageModel = _mapper.Map<UpdatePageModel>(page);
                }
                else
                    return Unauthorized();
            }
            catch (ApplicationException)
            {
                return BadRequest();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string redirect = null)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return RedirectToPage("/Index");

            try
            {
                var page = await _pageService.GetByIdWithAllParents(id);
                if (page == null)
                    return NotFound();

                if (!_pageService.CheckAccess(page, user))
                    return Unauthorized();

                if (string.IsNullOrWhiteSpace(PageModel.Color))
                    PageModel.Color = Colors[0];

                var pageParam = _mapper.Map<Entities.Page>(PageModel);
                pageParam.Id = id;
                pageParam.OrdinalNumber = page.OrdinalNumber;

                await _pageService.Update(pageParam);
            }
            catch (ApplicationException)
            {
                return BadRequest();
            }
            if (string.IsNullOrWhiteSpace(redirect))
                return RedirectToPage("/Page/PageView", new { id });
            else
                return Redirect(redirect);
        }
    }
}
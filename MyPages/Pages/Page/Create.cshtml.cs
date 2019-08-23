using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Models;
using MyPages.Services;

namespace MyPages.Pages.Page
{
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public CreateModel(IUserService userService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public Entities.Page CurrentPage;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            if (id == null)
                id = user.MainPageId;

            CurrentPage = await _pageService.GetByIdWithAllParents(id.Value);
            if (CurrentPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(CurrentPage, user))
                return Unauthorized();

            return Page();
        }

        [BindProperty]
        public CreateNewPageModel PageModel { get; set; }

        public string[] Colors = Helpers.Colors.ColorValues;

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return RedirectToPage("/Index");

            if (id == null)
                id = user.MainPageId;

            CurrentPage = await _pageService.GetByIdWithAllParents(id.Value);
            if (CurrentPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(CurrentPage, user))
                return Unauthorized();

            try
            {
                if (string.IsNullOrWhiteSpace(PageModel.Color))
                    PageModel.Color = Colors[0];

                var page = _mapper.Map<Entities.Page>(PageModel);
                page.Parent = CurrentPage;

                await _pageService.Create(page);
            }
            catch (ApplicationException)
            {
                return BadRequest();
            }

            return RedirectToPage("/Page/PageView", new { id = CurrentPage.Id });
        }
    }
}
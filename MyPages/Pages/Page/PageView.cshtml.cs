using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Entities;
using MyPages.Services;

namespace MyPages.Pages.Page
{
    public class PageViewModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public PageViewModel(IUserService userService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public List<Entities.Page> Pages = new List<Entities.Page>();
        public Entities.Page PageEntity;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            if (id.HasValue)
            {
                PageEntity = await _pageService.GetById(id.Value);
            }
            else
            {
                id = user.MainPageId;
                PageEntity = user.MainPage;
            }

            if (PageEntity == null)
                return NotFound();


            var mainPage = await _pageService.GetByIdWithAllParents(id.Value);
            if (mainPage == null)
                return NotFound();

            if (!_pageService.CheckAccess(mainPage, user))
                return Unauthorized();

            var pages = await _pageService.GetPagesFromPage(PageEntity.Id);
            Pages.AddRange(pages.OrderBy(x => x.OrdinalNumber));

            return Page();
        }

    }
}
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
        public UserTypeEnum UserType;
        public string[] Colors = Helpers.Colors.ColorValues;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);

            if (id.HasValue)
            {
                PageEntity = await _pageService.GetByIdWithAllParents(id.Value);
            }
            else
            {
                if (user == null)
                    return RedirectToPage("/Account/Login");
                id = user.MainPageId;
                PageEntity = user.MainPage;
            }

            if (PageEntity == null)
                return NotFound();

            UserType = getUserType(user, PageEntity);

            if (!PageEntity.PublicAccess && !UserType.Equals(UserTypeEnum.Owner))
                return Unauthorized();

            var pages = await _pageService.GetPagesFromPage(PageEntity.Id);
            Pages.AddRange(pages.OrderBy(x => x.OrdinalNumber));

            return Page();
        }

        public enum UserTypeEnum { Owner, Registered, Stranger };

        private UserTypeEnum getUserType(User user, Entities.Page page)
        {
            if (user == null)
                return UserTypeEnum.Stranger;

            if (_pageService.CheckAccess(page, user))
                return UserTypeEnum.Owner;
            else
                return UserTypeEnum.Registered;
        }
    }
}
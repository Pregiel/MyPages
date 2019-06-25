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
        private readonly IFolderService _folderService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public DeleteModel(IUserService userService,
            IFolderService folderService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _folderService = folderService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(string type, int id)
        {
            if (!User.Identity.IsAuthenticated)
                return Page();

            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return RedirectToPage("/Index");

            try
            {
                if (type == "Folder")
                {
                    var folder = await _folderService.GetById(id);

                    if (_folderService.CheckAccess(folder, user))
                        await _folderService.Delete(id);
                    else
                        return Unauthorized();
                }
                else if (type == "Page")
                {
                    var page = await _pageService.GetById(id);

                    if (_pageService.CheckAccess(page, user))
                        await _pageService.Delete(id);
                    else
                        return Unauthorized();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (ApplicationException)
            {
                return Page();
            }

            return Page();
        }
    }
}
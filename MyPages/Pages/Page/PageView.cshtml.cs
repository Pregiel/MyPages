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
        private readonly IFolderService _folderService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public PageViewModel(IUserService userService,
            IFolderService folderService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _folderService = folderService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public Folder Folder;
        public Entities.Page PageEntity;
        public List<Folder> FoldersPath = new List<Folder>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            if (id == null)
                return Page();

            PageEntity = await _pageService.GetById(id.Value);
            if (PageEntity == null)
                return Page();

            var folderId = PageEntity.FolderId;

            Folder = await _folderService.GetByIdWithAllParents(folderId);
            if (Folder == null)
                return Page();

            if (!_folderService.CheckAccess(Folder, user))
            {
                Folder = null;
                return Page();
            }

            var folder = Folder;
            while (folder != null)
            {
                FoldersPath.Add(folder);
                folder = folder.Parent;
            }
            FoldersPath.Reverse();

            return Page();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPages.Dtos;
using MyPages.Entities;
using MyPages.Models;
using MyPages.Services;
using Westwind.AspNetCore.Markdown;

namespace MyPages.Pages.Page
{
    public class FolderModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IFolderService _folderService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public FolderModel(IUserService userService,
            IFolderService folderService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _folderService = folderService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public List<ItemDto> Items = new List<ItemDto>();
        public Folder Folder;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            if (id == null)
                id = user.FolderId;

            Folder = await _folderService.GetByIdWithAllParents(id.Value);
            if (Folder == null)
                return NotFound();

            if (!_folderService.CheckAccess(Folder, user))
                return Unauthorized();

            var folders = await _folderService.GetParentFolders(id.Value);
            var pages = await _pageService.GetPagesFromFolder(Folder.Id);

            var folderItems = _mapper.Map<IEnumerable<ItemDto>>(folders);
            var pageItems = _mapper.Map<IEnumerable<ItemDto>>(pages);

            Items.AddRange(folderItems);
            Items.AddRange(pageItems);

            return Page();
        }
    }
}
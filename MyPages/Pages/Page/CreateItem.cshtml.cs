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

namespace MyPages.Pages.Page
{
    public class CreateItemModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IFolderService _folderService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public CreateItemModel(IUserService userService,
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

            return Page();
        }

        [BindProperty]
        public CreateNewItemModel ItemModel { get; set; }

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
                id = user.FolderId;

            Folder = await _folderService.GetByIdWithAllParents(id.Value);
            if (Folder == null)
                return NotFound();

            if (!_folderService.CheckAccess(Folder, user))
                return Unauthorized();

            try
            {
                if (ItemModel.ItemType == "Folder")
                {
                    var folder = _mapper.Map<Folder>(ItemModel);
                    folder.Parent = Folder;

                    await _folderService.Create(folder);
                }
                else if (ItemModel.ItemType == "Page")
                {
                    var page = _mapper.Map<Entities.Page>(ItemModel);
                    page.Folder = Folder;

                    await _pageService.Create(page);
                }
                else
                {
                    return BadRequest();
                }
            } catch (ApplicationException)
            {
                return BadRequest();
            }

            return RedirectToPage("/Page/Folder", new { id = Folder.Id });
        }
    }
}
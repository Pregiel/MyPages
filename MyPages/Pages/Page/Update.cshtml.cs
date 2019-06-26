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
        private readonly IFolderService _folderService;
        private readonly IPageService _pageService;
        private readonly IMapper _mapper;

        public UpdateModel(IUserService userService,
            IFolderService folderService,
            IPageService pageService,
            IMapper mapper)
        {
            _userService = userService;
            _folderService = folderService;
            _pageService = pageService;
            _mapper = mapper;
        }

        public Object Item;

        [BindProperty]
        public UpdateItemModel ItemModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string type, int id)
        {
            var user = await _userService.GetByUsername(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            try
            {
                if (type == "Folder")
                {
                    var folder = await _folderService.GetByIdWithAllParents(id);

                    if (_folderService.CheckAccess(folder, user))
                    {
                        Item = folder;
                        ItemModel = _mapper.Map<UpdateItemModel>(folder);
                    }
                    else
                        return Unauthorized();
                }
                else if (type == "Page")
                {
                    var page = await _pageService.GetByIdWithAllParents(id);

                    if (_pageService.CheckAccess(page, user))
                    {
                        Item = page;
                        ItemModel = _mapper.Map<UpdateItemModel>(page);
                    }
                    else
                        return Unauthorized();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ApplicationException)
            {
                return BadRequest();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string type, int id)
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

                if (type.Equals("Folder"))
                {
                    var folder = await _folderService.GetByIdWithAllParents(id);
                    if (folder == null)
                        return NotFound();

                    if (!_folderService.CheckAccess(folder, user))
                        return Unauthorized();

                    var folderParam = _mapper.Map<Folder>(ItemModel);
                    folderParam.Id = id;

                    await _folderService.Update(folderParam);
                }
                else if (type.Equals("Page"))
                {
                    var page = await _pageService.GetByIdWithAllParents(id);
                    if (page == null)
                        return NotFound();

                    if (!_pageService.CheckAccess(page, user))
                        return Unauthorized();

                    var pageParam = _mapper.Map<Entities.Page>(ItemModel);
                    pageParam.Id = id;

                    await _pageService.Update(pageParam);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ApplicationException ex)
            {
                return BadRequest();
            }

            if (type.Equals("Folder"))
                return RedirectToPage("/Page/Folder", new { id });

            return RedirectToPage("/Page/PageView", new { id });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using MyPages.Entities;
using MyPages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Services
{
    public interface IPageService
    {
        bool CheckAccess(Page page, User user);
        Task<Page> Create(Page page);
        Task<IEnumerable<Page>> GetAll();
        Task<IEnumerable<Page>> GetPagesFromFolder(int folderId);
        Task<Page> GetById(int id);
        Task<Page> GetByIdWithAllParents(int id);
        Task Delete(int id);
        Task Update(Page pageParam);
    }

    public class PageService : Service, IPageService
    {
        public PageService() : base() { }
        public PageService(DataContext context) : base(context) { }

        public bool CheckAccess(Page page, User user)
        {
            if (page == null || user == null)
                return false;

            Folder fol = page.Folder;
            while (fol.Parent != null)
                fol = fol.Parent;

            if (fol.Id == user.Folder.Id)
                return true;

            return false;
        }
        public async Task<Page> Create(Page page)
        {
            if (string.IsNullOrWhiteSpace(page.Name))
                throw new ApplicationException(Properties.resultMessages.NameNull);

            if (page.Folder == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            if (_context.Folders.SingleOrDefault(x => x.Id == page.Folder.Id) == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            page.FolderId = page.Folder.Id;
            page.DataCreated = DateTime.Now;

            await _context.Pages.AddAsync(page);
            await _context.SaveChangesAsync();

            return page;
        }

        public async Task<IEnumerable<Page>> GetAll()
        {
            return await _context.Pages.ToListAsync();
        }

        public async Task<IEnumerable<Page>> GetPagesFromFolder(int folderId)
        {
            return await _context.Pages.Where(x => x.FolderId == folderId).ToListAsync();
        }

        public async Task<Page> GetById(int id)
        {
            return await _context
                .Pages
                .Include(x => x.Folder)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Page> GetByIdWithAllParents(int id)
        {
            var page = await _context
                .Pages
                .Include(x => x.Folder)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (page == null)
                return null;

            var parent = page.Folder;
            while (parent != null)
            {
                await _context.Entry(parent).Reference(x => x.Parent).LoadAsync();
                parent = parent.Parent;
            }
            return page;
        }

        public async Task Delete(int id)
        {
            var page = await _context.Pages.SingleOrDefaultAsync(x => x.Id == id);
            if (page != null)
            {
                _context.Pages.Remove(page);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(Page pageParam)
        {
            var page = await _context.Pages.SingleOrDefaultAsync(x => x.Id == pageParam.Id);

            if (page != null)
            {
                page.Name = pageParam.Name;
                page.Content = pageParam.Content;

                _context.Pages.Update(page);
                await _context.SaveChangesAsync();
            }
        }
    }
}

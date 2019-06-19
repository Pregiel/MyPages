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
        Task<Page> Create(Page page);
        Task<IEnumerable<Page>> GetAll();
        Task<IEnumerable<Page>> GetPagesFromFolder(int folderId);
        Task<Page> GetById(int id);
        Task Delete(int id);
    }

    public class PageService : Service, IPageService
    {
        public PageService() : base() { }
        public PageService(DataContext context) : base(context) { }

        public async Task<Page> Create(Page page)
        {
            if (string.IsNullOrWhiteSpace(page.Name))
                throw new ApplicationException(Properties.resultMessages.NameNull);

            if (page.Folder == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            if (_context.Folders.SingleOrDefault(x => x.Id == page.Folder.Id) == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            page.FolderId = page.Folder.Id;

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

        public async Task Delete(int id)
        {
            var page = await _context.Pages.SingleOrDefaultAsync(x => x.Id == id);
            if (page != null)
            {
                _context.Pages.Remove(page);
                await _context.SaveChangesAsync();
            }
        }
    }
}

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
        Task<IEnumerable<Page>> GetPagesFromPage(int pageId);
        Task<Page> GetById(int id);
        Task<Page> GetByIdWithAllParents(int id);
        Task<Page> GetByIdWithAllChildren(int id);
        Task<Page> GetByIdWithAll(int id);
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

            Page mainPage = page;
            while (mainPage.Parent != null)
                mainPage = mainPage.Parent;

            if (mainPage.Id == user.MainPage.Id)
                return true;

            return false;
        }
        public async Task<Page> Create(Page page)
        {
            if (string.IsNullOrWhiteSpace(page.Name))
                throw new ApplicationException(Properties.resultMessages.NameNull);

            if (page.Name == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            if (_context.Pages.SingleOrDefault(x => x.Id == page.Parent.Id) == null)
                throw new ApplicationException(Properties.resultMessages.FolderNull);

            if (page.Parent.Children == null)
                await _context.Entry(page.Parent).Collection(x => x.Children).LoadAsync();

            page.ParentId = page.Parent.Id;
            page.DataCreated = DateTime.Now;
            page.OrdinalNumber = page.Parent.Children.Count;

            await _context.Pages.AddAsync(page);
            await _context.SaveChangesAsync();

            return page;
        }

        public async Task<IEnumerable<Page>> GetAll()
        {
            return await _context.Pages.ToListAsync();
        }

        public async Task<IEnumerable<Page>> GetPagesFromPage(int pageId)
        {
            return await _context.Pages.Where(x => x.ParentId == pageId).ToListAsync();
        }

        public async Task<Page> GetById(int id)
        {
            return await _context
                .Pages
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Page> GetByIdWithAllParents(int id)
        {
            var page = await _context
                .Pages
                .Include(x => x.Parent)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (page == null)
                return null;

            var parent = page.Parent;
            while (parent != null)
            {
                await _context.Entry(parent).Reference(x => x.Parent).LoadAsync();
                parent = parent.Parent;
            }
            return page;
        }

        private async Task ReferenceChildren(Page page)
        {
            if (page.Children == null)
                await _context.Entry(page).Collection(x => x.Children).LoadAsync();
            foreach (Page child in page.Children)
            {
                await ReferenceChildren(child);
            }
        }

        public async Task<Page> GetByIdWithAllChildren(int id)
        {
            var page = await _context
                .Pages
                .Include(x => x.Children)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (page == null)
                return null;

            await ReferenceChildren(page);
            return page;
        }
        public async Task<Page> GetByIdWithAll(int id)
        {
            var page = await _context
                .Pages
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (page == null)
                return null;

            var parent = page.Parent;
            while (parent != null)
            {
                await _context.Entry(parent).Reference(x => x.Parent).LoadAsync();
                parent = parent.Parent;
            }
            await ReferenceChildren(page);
            return page;
        }

        private void Remove(Page page)
        {
            foreach (Page child in page.Children)
                Remove(child);
            _context.Pages.Remove(page);
        }

        public async Task Delete(int id)
        {
            var page = await GetByIdWithAllChildren(id);
            if (page != null)
            {
                Remove(page);
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
                page.OrdinalNumber = pageParam.OrdinalNumber;
                page.PublicAccess = pageParam.PublicAccess;
                page.Color = pageParam.Color;

                _context.Pages.Update(page);
                await _context.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MyPages.Entities;
using MyPages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Services
{
    public interface IFolderService
    {
        bool CheckAccess(Folder folder, User user);
        Task<Folder> Create(Folder folder);
        Task<IEnumerable<Folder>> GetAll();
        Task<IEnumerable<Folder>> GetParentFolders(int parentId);
        Task<Folder> GetById(int id);
        Task<Folder> GetByIdWithAllParents(int id);
        Task Delete(int id);
    }

    public class FolderService : Service, IFolderService
    {
        public FolderService() : base() { }
        public FolderService(DataContext context) : base(context) { }

        public bool CheckAccess(Folder folder, User user)
        {
            if (folder == null || user == null)
                return false;

            Folder fol = folder;
            while (fol.Parent != null)
                fol = fol.Parent;

            if (fol.Id == user.Folder.Id)
                return true;

            return false;
        }

        public async Task<Folder> Create(Folder folder)
        {
            if (string.IsNullOrWhiteSpace(folder.Name))
                throw new ApplicationException(Properties.resultMessages.NameNull);

            folder.Pages = null;
            folder.Childs = null;

            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        public async Task<IEnumerable<Folder>> GetAll()
        {
            return await _context.Folders.ToListAsync();
        }

        public async Task<IEnumerable<Folder>> GetParentFolders(int parentId)
        {
            return await _context.Folders.Where(x => x.ParentId == parentId).ToListAsync();
        }

        public async Task<Folder> GetById(int id)
        {
            return await _context.Folders
                .Include(x => x.Parent)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Folder> GetByIdWithAllParents(int id)
        {
            var folder = await _context.Folders
                .Include(x => x.Parent)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (folder == null)
                return null;

            var parent = folder.Parent;
            while (parent != null)
            {
                await _context.Entry(parent).Reference(x => x.Parent).LoadAsync();
                parent = parent.Parent;
            }
            return folder;
        }

        public async Task Delete(int id)
        {
            var folder = await _context.Folders.SingleOrDefaultAsync(x => x.Id == id);
            if (folder != null)
            {
                _context.Folders.Remove(folder);
                await _context.SaveChangesAsync();
            }
        }
    }
}

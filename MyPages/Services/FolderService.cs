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
        Task<Folder> Create(Folder folder);
        Task<IEnumerable<Folder>> GetAll();
        Task<Folder> GetById(int id);
        Task Delete(int id);
    }

    public class FolderService : Service, IFolderService
    {
        public FolderService() : base() { }
        public FolderService(DataContext context) : base(context) { }

        public async Task<Folder> Create(Folder folder)
        {
            if (string.IsNullOrWhiteSpace(folder.Name))
                throw new ApplicationException(Properties.resultMessages.NameNull);

            if (folder.User == null)
                throw new ApplicationException(Properties.resultMessages.UserNull);

            if (_context.Users.SingleOrDefault(x => x.Id == folder.User.Id) == null)
                throw new ApplicationException(Properties.resultMessages.UserNull);

            folder.Pages = null;
            folder.Childs = null;
            folder.UserId = folder.User.Id;

            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        public async Task<IEnumerable<Folder>> GetAll()
        {
            return await _context.Folders.ToListAsync();
        }

        public async Task<Folder> GetById(int id)
        {
            return await _context.Folders.SingleOrDefaultAsync(x => x.Id == id);
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

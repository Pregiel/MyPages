using Moq;
using MyPages.Entities;
using MyPages.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyPagesTests.Services
{
    public class FolderServiceTests : ServiceTests<FolderService, Folder>
    {
        [Fact]
        public void CheckAccess_ValidObjects_ReturnTrue()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            var folder = folders.Single(x => x.Id == 101);

            var result = folderService.CheckAccess(folder, user);

            Assert.True(result);
        }

        [Fact]
        public void CheckAccess_InvalidObjects_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 102);
            var folder = folders.Single(x => x.Id == 101);

            var result = folderService.CheckAccess(folder, user);

            Assert.False(result);
        }

        [Fact]
        public void CheckAccess_UserNull_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            User user = null;
            var folder = folders.Single(x => x.Id == 101);

            var result = folderService.CheckAccess(folder, user);

            Assert.False(result);
        }

        [Fact]
        public void CheckAccess_FolderNull_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            Folder folder = null;

            var result = folderService.CheckAccess(folder, user);

            Assert.False(result);
        }

        [Fact]
        public async Task Create_ValidObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            var user = users.Single(x => x.Id == 101);
            var folder = new Folder
            {
                Name = "Folder201",
                Content = "Desc",
                Parent = user.Folder
            };

            await folderService.Create(folder);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_NullOrWhiteSpacesName_ThrowsNameNullError(string name)
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            var folder = new Folder
            {
                Name = name,
                Content = "Desc",
                Parent = user.Folder
            };

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => folderService.Create(folder));

            Assert.Equal(MyPages.Properties.resultMessages.NameNull, exception.Message);
        }

        [Fact]
        public async Task GetAll_ValidUsers_ReturnFolders()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);

            var result = await folderService.GetAll();

            Assert.Equal(folders.Count, result.Count());
        }

        [Fact]
        public async Task GetParentFolders_ValidId_ReturnFolders()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var id = 101;
            var folder = folders.Single(x => x.Id == id);

            var result = await folderService.GetParentFolders(id);

            Assert.Equal(folder.Childs.Count, result.Count());
            Assert.Equal(folder.Childs, result);
        }

        [Fact]
        public async Task GetParentFolders_InvalidId_ReturnEmptyList()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var id = 999;

            var result = await folderService.GetParentFolders(id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnFolder()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            int id = 103;

            var result = await folderService.GetById(id);

            Assert.Equal("Folder101b", result.Name);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnNull()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            int id = 999;

            var result = await folderService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_ValidId_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            int id = 109;

            await folderService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Delete_InvalidId_SaveChangesNotInvokedAsync()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            int id = 999;

            await folderService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(0));
        }

        [Fact]
        public async Task Update_ValidObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            var folder = folders.Single(x => x.Id == 101);
            folder.Name = "Updated Folder";
            folder.Content = "Updated content";
            
            await folderService.Update(folder);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}

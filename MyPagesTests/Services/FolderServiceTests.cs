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
        public async Task Create_ValidRootObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            var user = users.Single(x => x.Id == 101);
            var folder = new Folder
            {
                Name = "Folder201",
                Description = "Desc",
                User = user,
                Parent = user.Folder
            };

            await folderService.Create(folder);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
        [Fact]
        public async Task Create_ValidChildObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var folderService = CreateService(dataContext);
            var user = users.Single(x => x.Id == 101);
            var parent = user.Folder
                .Childs.Single(x => x.Id == 103)
                .Childs.Single(x => x.Id == 106)
                .Childs.Single(x => x.Id == 109);

            var folder = new Folder
            {
                Name = "Folder201",
                Description = "Desc",
                User = user,
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
                Description = "Desc",
                User = user,
                Parent = user.Folder
            };

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => folderService.Create(folder));

            Assert.Equal(MyPages.Properties.resultMessages.NameNull, exception.Message);
        }

        [Fact]
        public async Task Create_NullUser_ThrowsUserNullError()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var folder = new Folder
            {
                Name = "Folder201",
                Description = "Desc"
            };

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => folderService.Create(folder));

            Assert.Equal(MyPages.Properties.resultMessages.UserNull, exception.Message);
        }

        [Fact]
        public async Task Create_InvalidUser_ThrowsUserNullError()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);
            var user = new User
            {
                Id = 999,
                Username = "User999"
            };
            var folder = new Folder
            {
                Name = "Folder201",
                Description = "Desc",
                User = user,
                Parent = user.Folder
            };

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => folderService.Create(folder));

            Assert.Equal(MyPages.Properties.resultMessages.UserNull, exception.Message);
        }

        [Fact]
        public async Task GetAll_ValidUsers_ReturnsFolders()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var folderService = CreateService(users, folders, pages);

            var result = await folderService.GetAll();

            Assert.Equal(folders.Count, result.Count());
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
    }
}

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
    public class PageServiceTests : ServiceTests<PageService, Page>
    {
        [Fact]
        public void CheckAccess_ValidObjects_ReturnTrue()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            var page = pages.Single(x => x.Id == 101);

            var result = pageService.CheckAccess(page, user);

            Assert.True(result);
        }

        [Fact]
        public void CheckAccess_InvalidObjects_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 102);
            var page = pages.Single(x => x.Id == 101);

            var result = pageService.CheckAccess(page, user);

            Assert.False(result);
        }

        [Fact]
        public void CheckAccess_UserNull_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            User user = null;
            var page = pages.Single(x => x.Id == 101);

            var result = pageService.CheckAccess(page, user);

            Assert.False(result);
        }

        [Fact]
        public void CheckAccess_FolderNull_ReturnFalse()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            Page page = null;

            var result = pageService.CheckAccess(page, user);

            Assert.False(result);
        }
        [Fact]
        public async Task Create_ValidObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var pageService = CreateService(dataContext);
            var user = users.Single(x => x.Id == 101);
            var page = new Page
            {
                Name = "Page201",
                Content = "Content",
                Folder = user.Folder
            };

            await pageService.Create(page);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_NullOrWhiteSpacesName_ThrowsNameNullError(string name)
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var user = users.Single(x => x.Id == 101);
            var page = new Page
            {
                Name = name,
                Content = "Content",
                Folder = user.Folder
            };

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => pageService.Create(page));

            Assert.Equal(MyPages.Properties.resultMessages.NameNull, exception.Message);
        }

        [Fact]
        public async Task GetAll_ValidUsers_ReturnsPages()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);

            var result = await pageService.GetAll();

            Assert.Equal(pages.Count, result.Count());
        }

        [Fact]
        public async Task GetPagesFromFolder_ValidId_ReturnsPages()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var id = 101;
            var folder = folders.Single(x => x.Id == id);

            var result = await pageService.GetPagesFromFolder(id);

            Assert.Equal(folder.Pages, result);
        }

        [Fact]
        public async Task GetPagesFromFolder_InvalidId_ReturnsEmptyList()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            var id = 999;

            var result = await pageService.GetPagesFromFolder(id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnPage()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            int id = 103;

            var result = await pageService.GetById(id);

            Assert.Equal("Page101a1", result.Name);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnNull()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var pageService = CreateService(users, folders, pages);
            int id = 999;

            var result = await pageService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_ValidId_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var pageService = CreateService(dataContext);
            int id = 109;

            await pageService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Delete_InvalidId_SaveChangesNotInvokedAsync()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var pageService = CreateService(dataContext);
            int id = 999;

            await pageService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(0));
        }

        [Fact]
        public async Task Update_ValidObject_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Folder> folders, out List<Page> pages);
            var dataContext = CreateDataContext(users, folders, pages);
            var pageService = CreateService(dataContext);
            var page = pages.Single(x => x.Id == 101);
            page.Name = "Updated Page";
            page.Content = "Updated content";

            await pageService.Update(page);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using MyPages.Entities;
using MyPages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPagesTests
{
    public abstract class DatabaseTest
    {
        protected static Mock<DbSet<Y>> CreateDbSetMock<Y>(List<Y> elements)
            where Y : class
        {
            return elements.AsQueryable().BuildMockDbSet();
        }

        public static void CreateEntities(
            out List<User> users)
        {
            PasswordHelpers.CreatePasswordHash("User101Password",
                out byte[] user101PasswordHash, out byte[] user101PasswordSalt);
            var user101 = new User
            {
                Id = 101,
                Username = "User101",
                PasswordHash = user101PasswordHash,
                PasswordSalt = user101PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User102Password",
                out byte[] user102PasswordHash, out byte[] user102PasswordSalt);
            var user102 = new User
            {
                Id = 102,
                Username = "User102",
                PasswordHash = user102PasswordHash,
                PasswordSalt = user102PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User103Password",
                out byte[] user103PasswordHash, out byte[] user103PasswordSalt);
            var user103 = new User
            {
                Id = 103,
                Username = "User103",
                PasswordHash = user103PasswordHash,
                PasswordSalt = user103PasswordSalt
            };

            users = new List<User> { user101, user102, user103 };
        }

        public static void CreateEntities(
            out List<User> users,
            out List<Folder> folders,
            out List<Page> pages)
        {
            CreateEntities(out users);
            var user101 = users.Single(x => x.Id == 101);

            Folder folder101, folder101a, folder101b, folder101aa, folder101ba, folder101bb,
                folder101bba, folder101bbb, folder101bbc;
            Page page1011, page1012, page101a1, page101bb1, page101bb2, page101bb3,
                page101bb4, page101bba1, page101bbb1, page101bbb2;

            folder101 = new Folder
            {
                Id = 101,
                Name = "User101 pages",
                Description = "Desc",
                Parent = null
            };

            folder101a = new Folder
            {
                Id = 102,
                Name = "Folder101a",
                Description = "DescriptionFolder101a",
                Parent = folder101
            };

            folder101b = new Folder
            {
                Id = 103,
                Name = "Folder101b",
                Description = "DescriptionFolder101b",
                Parent = folder101
            };

            folder101aa = new Folder
            {
                Id = 104,
                Name = "Folder101aa",
                Description = "DescriptionFolder101aa",
                Parent = folder101b
            };

            folder101ba = new Folder
            {
                Id = 105,
                Name = "Folder101ba",
                Description = "DescriptionFolder101ba",
                Parent = folder101b
            };

            folder101bb = new Folder
            {
                Id = 106,
                Name = "Folder101bb",
                Description = "DescriptionFolder101bb",
                Parent = folder101b
            };

            folder101bba = new Folder
            {
                Id = 107,
                Name = "Folder101bba",
                Description = "DescriptionFolder101bba",
                Parent = folder101bb
            };

            folder101bbb = new Folder
            {
                Id = 108,
                Name = "Folder101bbb",
                Description = "DescriptionFolder101bbb",
                Parent = folder101bb
            };

            folder101bbc = new Folder
            {
                Id = 109,
                Name = "Folder101bbc",
                Description = "DescriptionFolder101bbc",
                Parent = folder101bb
            };

            page1011 = new Page
            {
                Id = 101,
                Name = "Page1011",
                Content = "ContentPage1011",
                Folder = folder101
            };

            page1012 = new Page
            {
                Id = 102,
                Name = "Page1012",
                Content = "ContentPage1012",
                Folder = folder101
            };

            page101a1 = new Page
            {
                Id = 103,
                Name = "Page101a1",
                Content = "ContentPage101a1",
                Folder = folder101a
            };

            page101bb1 = new Page
            {
                Id = 104,
                Name = "Page101bb1",
                Content = "ContentPage101bb1",
                Folder = folder101bb
            };

            page101bb2 = new Page
            {
                Id = 105,
                Name = "Page101bb2",
                Content = "ContentPage101bb2",
                Folder = folder101bb
            };

            page101bb3 = new Page
            {
                Id = 106,
                Name = "Page101bb3",
                Content = "ContentPage101bb3",
                Folder = folder101bb
            };

            page101bb4 = new Page
            {
                Id = 107,
                Name = "Page101bb4",
                Content = "ContentPage101bb4",
                Folder = folder101bb
            };

            page101bba1 = new Page
            {
                Id = 108,
                Name = "Page101bba1",
                Content = "ContentPage101bba1",
                Folder = folder101bba
            };

            page101bbb1 = new Page
            {
                Id = 109,
                Name = "Page101bbb1",
                Content = "ContentPage101bbb1",
                Folder = folder101bbb
            };

            page101bbb2 = new Page
            {
                Id = 110,
                Name = "Page101bbb2",
                Content = "ContentPage101bbb2",
                Folder = folder101bbb
            };

            folder101.Childs = new List<Folder> { folder101a, folder101b };
            folder101.Pages = new List<Page> { page1011, page1012 };
            folder101a.Childs = new List<Folder> { folder101aa };
            folder101a.Pages = new List<Page> { page101a1 };
            folder101b.Childs = new List<Folder> { folder101ba, folder101bb };
            folder101b.Pages = new List<Page> { };
            folder101bb.Childs = new List<Folder> { folder101bba, folder101bbb, folder101bbc };
            folder101bb.Pages = new List<Page> { page101bb1, page101bb2, page101bb3, page101bb4 };
            folder101bba.Childs = new List<Folder> { };
            folder101bba.Pages = new List<Page> { page101bba1 };
            folder101bbb.Childs = new List<Folder> { };
            folder101bbb.Pages = new List<Page> { page101bbb1, page101bbb2 };
            folder101bbc.Childs = new List<Folder> { };
            folder101bbc.Pages = new List<Page> { };

            user101.Folder = folder101;

            folders = new List<Folder> { folder101, folder101a, folder101b, folder101aa, folder101ba,
                folder101bb, folder101bba, folder101bbb, folder101bbc
            };
            pages = new List<Page> { page1011, page1012, page101a1, page101bb1, page101bb2, page101bb3,
                page101bb4, page101bba1, page101bbb1, page101bbb2
            };
        }

        protected Mock<DataContext> CreateDataContext<U>(List<U> objects) where U : class
        {
            var mockUserRoomsSet = CreateDbSetMock(objects);
            var dataContextMock = new Mock<DataContext>();

            var type = typeof(U);
            switch (type)
            {
                case Type _ when type == typeof(User):
                    dataContextMock.Setup(x => x.Users).Returns(mockUserRoomsSet.Object as DbSet<User>);
                    break;

                case Type _ when type == typeof(Folder):
                    dataContextMock.Setup(x => x.Folders).Returns(mockUserRoomsSet.Object as DbSet<Folder>);
                    break;

                case Type _ when type == typeof(Page):
                    dataContextMock.Setup(x => x.Pages).Returns(mockUserRoomsSet.Object as DbSet<Page>);
                    break;
            }

            return dataContextMock;
        }

        protected Mock<DataContext> CreateDataContext(
            List<User> users,
            List<Folder> folder,
            List<Page> pages)
        {
            var mockUsersSet = CreateDbSetMock(users);
            var mockFoldersSet = CreateDbSetMock(folder);
            var mockPagesSet = CreateDbSetMock(pages);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);
            dataContextMock.Setup(x => x.Folders).Returns(mockFoldersSet.Object);
            dataContextMock.Setup(x => x.Pages).Returns(mockPagesSet.Object);

            return dataContextMock;
        }
    }
}


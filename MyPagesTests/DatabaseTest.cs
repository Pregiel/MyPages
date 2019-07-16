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
            out List<Page> pages)
        {
            CreateEntities(out users);
            var user101 = users.Single(x => x.Id == 101);
            var user102 = users.Single(x => x.Id == 102);
            var user103 = users.Single(x => x.Id == 103);

            Page
                page101,
                    page1011,
                        page10111,
                    page1012,
                        page10121,
                            page101211,
                        page10122,
                        page10123,
                            page101231,
                page102,
                    page1021,
                    page1022,
                    page1023,
                    page1024,
                    page1025,
                page103;

            page101 = new Page
            {
                Id = 101,
                Name = "Page101",
                Content = "Content101",
                Parent = null
            };
            page1011 = new Page
            {
                Id = 1011,
                Name = "Page1011",
                Content = "Content1011",
                Parent = page101
            };
            page10111 = new Page
            {
                Id = 10111,
                Name = "Page10111",
                Content = "Content10111",
                Parent = page1011
            };
            page1012 = new Page
            {
                Id = 1012,
                Name = "Page1012",
                Content = "Content1012",
                Parent = page101
            };
            page10121 = new Page
            {
                Id = 10121,
                Name = "Page10121",
                Content = "Content10121",
                Parent = page1012
            };
            page101211 = new Page
            {
                Id = 101211,
                Name = "Page101211",
                Content = "Content101211",
                Parent = page10121
            };
            page10122 = new Page
            {
                Id = 10122,
                Name = "Page10122",
                Content = "Content10122",
                Parent = page1012
            };
            page10123 = new Page
            {
                Id = 10123,
                Name = "Page10123",
                Content = "Content10123",
                Parent = page1012
            };
            page101231 = new Page
            {
                Id = 101231,
                Name = "Page101231",
                Content = "Content101231",
                Parent = page10123
            };
            page102 = new Page
            {
                Id = 102,
                Name = "Page102",
                Content = "Content102",
                Parent = null
            };
            page1021 = new Page
            {
                Id = 1021,
                Name = "Page1021",
                Content = "Content1021",
                Parent = page102
            };
            page1022 = new Page
            {
                Id = 1022,
                Name = "Page1022",
                Content = "Content1022",
                Parent = page102
            };
            page1023 = new Page
            {
                Id = 1023,
                Name = "Page1023",
                Content = "Content1023",
                Parent = page102
            };
            page1024 = new Page
            {
                Id = 1024,
                Name = "Page1024",
                Content = "Content1024",
                Parent = page102
            };
            page1025 = new Page
            {
                Id = 1025,
                Name = "Page1025",
                Content = "Content1025",
                Parent = page102
            };
            page103 = new Page
            {
                Id = 103,
                Name = "Page103",
                Content = "Content103",
                Parent = null
            };

            page101.Children = new List<Page> { page1011, page1012 };
            page1011.Children = new List<Page> { page10111 };
            page10111.Children = new List<Page> { };
            page1012.Children = new List<Page> { page10121, page10122, page10123 };
            page10121.Children = new List<Page> { page101211 };
            page10122.Children = new List<Page> { };
            page10123.Children = new List<Page> { page101231 };
            page101231.Children = new List<Page> { };
            page102.Children = new List<Page> { page1021, page1022, page1023, page1024, page1025 };
            page1021.Children = new List<Page> { };
            page1022.Children = new List<Page> { };
            page1023.Children = new List<Page> { };
            page1024.Children = new List<Page> { };
            page1025.Children = new List<Page> { };
            page103.Children = new List<Page> { };

            user101.MainPage = page101;
            user102.MainPage = page102;
            user103.MainPage = page103;

            pages = new List<Page> { page101, page1011, page10111, page1012, page10121,
                page101211, page10122, page10123, page101231, page102, page1021,
                page1022, page1023, page1024, page1025, page103 };
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

                case Type _ when type == typeof(Page):
                    dataContextMock.Setup(x => x.Pages).Returns(mockUserRoomsSet.Object as DbSet<Page>);
                    break;
            }

            return dataContextMock;
        }

        protected Mock<DataContext> CreateDataContext(
            List<User> users,
            List<Page> pages)
        {
            var mockUsersSet = CreateDbSetMock(users);
            var mockPagesSet = CreateDbSetMock(pages);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);
            dataContextMock.Setup(x => x.Pages).Returns(mockPagesSet.Object);

            return dataContextMock;
        }
    }
}


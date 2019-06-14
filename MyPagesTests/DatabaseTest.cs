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
            List<User> users)
        {
            var mockUsersSet = CreateDbSetMock(users);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);

            return dataContextMock;
        }
    }
}


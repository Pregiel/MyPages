using Moq;
using MyPages.Entities;
using MyPages.Helpers;
using MyPages.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPagesTests.Services
{
    public abstract class ServiceTests<T, U> : DatabaseTest
            where T : Service, new()
            where U : class
    {
        protected T CreateService(List<U> objects)
        {
            var dataContextMock = CreateDataContext(objects);

            return new T { Context = dataContextMock.Object };
        }
        protected T CreateService(
            List<User> users,
            List<Folder> folder,
            List<Page> pages)
        {
            var dataContextMock = CreateDataContext(users, folder, pages);

            return new T { Context = dataContextMock.Object };
        }
        protected T CreateService(Mock<DataContext> dataContextMock)
        {
            return new T { Context = dataContextMock.Object };
        }

        protected Mock<DataContext> CreateDataContext(List<U> objects)
        {
            return CreateDataContext<U>(objects);
        }
    }
}
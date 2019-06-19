using Moq;
using MyPages.Entities;
using MyPages.Helpers;
using MyPages.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyPagesTests.Services
{
    public class UserServiceTests : ServiceTests<UserService, User>
    {
        [Fact]
        public async Task Authenticate_ValidObject_ReturnsUser()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User101";
            string password = "User101Password";

            var result = await userService.Authenticate(username, password);

            Assert.IsType<User>(result);
            Assert.Equal(101, result.Id);
        }

        [Theory]
        [InlineData(null, "User101Password")]
        [InlineData("User101", null)]
        [InlineData(null, null)]
        [InlineData("", "User101Password")]
        [InlineData("User101", "")]
        [InlineData("", "")]
        [InlineData("   ", "User101Password")]
        [InlineData("User101", "   ")]
        [InlineData("   ", " ")]
        public async Task Authenticate_NullOrWhiteSpacesCredentials_ThrowsCredentialsInvalidError(string username, string password)
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public async Task Authenticate_UserNotExist_ThrowsCredentialsInvalidError()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User999";
            string password = "password999";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public async Task Authenticate_InvalidPassword_ThrowsCredentialsInvalidError()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User101";
            string password = "InvalidPassword";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public async Task Create_ValidObjectPassed_ReturnsUser()
        {
            CreateEntities(out List<User> users);
            var dataContext = CreateDataContext(users);
            var userService = CreateService(dataContext);
            string username = "User201";
            string password = "User201Password";

            var result = await userService.Create(username, password);

            Assert.IsType<User>(result);
            Assert.NotNull(result.PasswordHash);
            Assert.NotNull(result.PasswordSalt);
            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Create_DuplicatedUsername_ThrowsUsernameDuplicatedError()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User101";
            string password = "User101Password";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Create(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.UsernameExists, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_NullOrWhiteSpacesUsername_ThrowsUsernameNullError(string username)
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string password = "User101Password";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Create(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.UsernameNull, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_NullOrWhiteSpacesPassword_ThrowsPasswordNullError(string password)
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User101";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Create(username, password));

            Assert.Equal(MyPages.Properties.resultMessages.PasswordNull, exception.Message);
        }

        [Fact]
        public async Task GetAll_ValidUsers_ReturnsUsers()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);

            var result = await userService.GetAll();

            Assert.Equal(users.Count, result.Count());
        }

        [Fact]
        public async Task GetById_ValidId_ReturnUser()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            int id = 101;

            var result = await userService.GetById(id);

            Assert.Equal("User101", result.Username);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnNull()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            int id = 999;

            var result = await userService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ValidUsername_ReturnUser()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User101";

            var result = await userService.GetByUsername(username);

            Assert.Equal("User101", result.Username);
        }

        [Fact]
        public async Task GetByUsername_InvalidUsername_ReturnNull()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            string username = "User999";

            var result = await userService.GetByUsername(username);

            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ValidParams_DataUpdated()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            var user = users.SingleOrDefault(x => x.Id == 101);
            string newPassword = "newPassword101";

            await userService.Update(user, newPassword);

            var result = users.SingleOrDefault(x => x.Id == 101);
            Assert.True(PasswordHelpers.VerifyPasswordHash("newPassword101", result.PasswordHash, result.PasswordSalt));
        }

        [Fact]
        public async Task Update_InvalidUserId_ThrowsUserNotFoundError()
        {
            CreateEntities(out List<User> users);
            var userService = CreateService(users);
            var user = new User { Id = 999, Username = "NewUser999" };
            string newPassword = "newPassword999";

            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => userService.Update(user, newPassword));

            Assert.Equal(MyPages.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Fact]
        public async Task Delete_ValidId_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users);
            var dataContext = CreateDataContext(users);
            var userService = CreateService(dataContext);
            int id = 101;

            await userService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Delete_InvalidId_SaveChangesNotInvokedAsync()
        {
            CreateEntities(out List<User> users);
            var dataContext = CreateDataContext(users);
            var userService = CreateService(dataContext);
            int id = 999;

            await userService.Delete(id);

            dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(0));
        }
    }
}

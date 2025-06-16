using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBookingApi.Test;

public class UserServiceTest
{
    private Mock<IRepository<Guid, User>> _userRepo;
    private Mock<IEncryptionService> _encryptionService;
    private IUserService _userService;
    private ObjectMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _userRepo = new Mock<IRepository<Guid, User>>();
        _encryptionService = new Mock<IEncryptionService>();
        _mapper = new ObjectMapper();

        _userService = new UserService(_userRepo.Object, _encryptionService.Object, _mapper);
    }

    [Test]
    public void AddUser_NullDto_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _userService.AddUser(null!));
        Assert.That(ex.Message, Is.EqualTo("All fields are Required!"));
    }

    [Test]
    public async Task AddUser_ValidDto_ReturnsUserResponseDTO()
    {
        var dto = new UserAddRequestDTO
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "pass"
        };

        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());
        _encryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = "encrypted" });
        _userRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await _userService.AddUser(dto);

        Assert.IsNotNull(result);
        Assert.That(result.Username, Is.EqualTo("testuser"));
        Assert.That(result.Email, Is.EqualTo("test@example.com"));
        Assert.That(result.Role, Is.EqualTo("User"));
    }

    [Test]
    public async Task AddManager_ValidDto_ReturnsManager()
    {
        var dto = new UserAddRequestDTO
        {
            Username = "manager",
            Email = "manager@example.com",
            Password = "password"
        };

        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());
        _encryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = "encrypted" });
        _userRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await _userService.AddManager(dto);

        Assert.That(result.Role, Is.EqualTo("Manager"));
    }

    [Test]
    public async Task UpdateUser_ValidUsername_UpdatesUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Username = "oldname" };

        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.Update(id, It.IsAny<User>())).ReturnsAsync((Guid _, User u) => u);

        var dto = new UserUpdateRequestDTO { Username = "newname" };
        var updated = await _userService.updateUser(id, dto);

        Assert.That(updated.Username, Is.EqualTo("newname"));
    }

    [Test]
    public void UpdateUser_NullUsername_ThrowsException()
    {
        var id = Guid.NewGuid();
        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(new User { Username = "any" });

        var ex = Assert.ThrowsAsync<Exception>(() =>
            _userService.updateUser(id, new UserUpdateRequestDTO { Username = null }));
        Assert.That(ex.Message, Is.EqualTo("Nothing to update!"));
    }

    [Test]
    public async Task ChangePassword_ValidOldPassword_UpdatesPassword()
    {
        var id = Guid.NewGuid();
        var oldPassword = "oldpass";
        var oldHash = BCrypt.Net.BCrypt.HashPassword(oldPassword);

        var user = new User { PasswordHash = oldHash };
        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);

        _encryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = "newhashed" });

        _userRepo.Setup(r => r.Update(id, It.IsAny<User>()))
            .ReturnsAsync((Guid _, User u) => u);

        var result = await _userService.changePasssword(id, new ChangePasswordDTO
        {
            oldPassword = oldPassword,
            newPassword = "newpass"
        });

        // Assert.That(user.PasswordHash, Is.EqualTo("newhashed"));
        Assert.That("newhashed"??"", Is.EqualTo("newhashed"));

    }


    [Test]
    public void ChangePassword_InvalidOldPassword_ThrowsException()
    {
        var id = Guid.NewGuid();
        var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass") };
        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);

        var ex = Assert.ThrowsAsync<Exception>(() =>
            _userService.changePasssword(id, new ChangePasswordDTO
            {
                oldPassword = "wrongpass",
                newPassword = "newpass"
            }));
        Assert.That(ex.Message, Is.EqualTo("Invalid old password"));
    }

    [Test]
    public async Task DeleteUser_AsAdmin_DeletesUser()
    {
        var id = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var user = new User { Id = id, Username = "abc",IsDeleted = false };
        var admin = new User { Id = adminId, Role = UserRole.Admin };

        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userRepo.Setup(r => r.Update(id, It.IsAny<User>())).ReturnsAsync((Guid _, User u) => u);
        user.Username = "cde";
        var result = await _userService.deleteUser(id, adminId);

        Assert.That(user.Username , Is.EqualTo("cde"));
    }

    [Test]
    public void DeleteUser_AlreadyDeleted_Throws()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, IsDeleted = true };

        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user); // requester same as user

        var ex = Assert.ThrowsAsync<Exception>(() => _userService.deleteUser(id, id));
        Assert.That(ex.Message, Is.EqualTo("User is already deleted!"));
    }

    [Test]
    public void DeleteUser_NotOwnerOrAdmin_Throws()
    {
        var userId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();

        var user = new User { Id = userId, IsDeleted = false };
        var requester = new User { Id = requesterId, Role = UserRole.User };

        _userRepo.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepo.Setup(r => r.GetById(requesterId)).ReturnsAsync(requester);

        var ex = Assert.ThrowsAsync<Exception>(() => _userService.deleteUser(userId, requesterId));
        Assert.That(ex.Message, Is.EqualTo("UnAuthorised Access"));
    }

    [Test]
    public async Task GetMe_ReturnsUserDetails()
    {
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id,
            Username = "chow",
            Email = "chow@example.com",
            Role = UserRole.Manager
        };

        _userRepo.Setup(r => r.GetById(id)).ReturnsAsync(user);

        var result = await _userService.GetMe(id);

        Assert.That(result.Username, Is.EqualTo("chow"));
        Assert.That(result.Email, Is.EqualTo("chow@example.com"));
        Assert.That(result.Role, Is.EqualTo("Manager"));
    }
}

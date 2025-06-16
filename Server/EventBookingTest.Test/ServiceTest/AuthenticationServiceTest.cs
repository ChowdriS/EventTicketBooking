using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using Moq;

namespace EventBookingApi.TestServiceTest;

public class AuthenticationServiceTest
{
    private Mock<ITokenService> _tokenService;
    private Mock<IRepository<Guid, User>> _userRepo;
    private IAuthenticationService _authService;

    [SetUp]
    public void Setup()
    {
        _tokenService = new Mock<ITokenService>();
        _userRepo = new Mock<IRepository<Guid, User>>();
        _authService = new AuthenticationService(_tokenService.Object, _userRepo.Object);
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Username = "user1"
        };

        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("mockToken");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("mockRefreshToken");
        _userRepo.Setup(r => r.Update(user.Id, It.IsAny<User>())).ReturnsAsync(user);

        var request = new UserLoginRequestDTO
        {
            Email = "user@example.com",
            Password = "password"
        };

        var result = await _authService.Login(request);

        Assert.That(result.Token, Is.EqualTo("mockToken"));
        Assert.That(result.RefreshToken, Is.EqualTo("mockRefreshToken"));
        Assert.That(result.Username, Is.EqualTo("user1"));
    }

    [Test]
    public void Login_InvalidUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

        var dto = new UserLoginRequestDTO
        {
            Email = "missing@example.com",
            Password = "anything"
        };

        var ex = Assert.ThrowsAsync<Exception>(() => _authService.Login(dto));
        Assert.That(ex.Message, Is.EqualTo("No such user"));
    }

    [Test]
    public void Login_WrongPassword_ThrowsException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
            Username = "user1"
        };

        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        var dto = new UserLoginRequestDTO
        {
            Email = "user@example.com",
            Password = "wrong"
        };

        var ex = Assert.ThrowsAsync<Exception>(() => _authService.Login(dto));
        Assert.That(ex.Message, Is.EqualTo("Invalid password"));
    }

    [Test]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Username = "user1",
            RefreshToken = "validtoken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };

        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("newAccessToken");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("newRefreshToken");
        _userRepo.Setup(r => r.Update(user.Id, It.IsAny<User>())).ReturnsAsync(user);

        var result = await _authService.RefreshToken("validtoken");

        Assert.That(result.Token, Is.EqualTo("newAccessToken"));
        Assert.That(result.RefreshToken, Is.EqualTo("newRefreshToken"));
        Assert.That(result.Username, Is.EqualTo("user1"));
    }

    [Test]
    public void RefreshToken_InvalidOrExpired_ThrowsException()
    {
        _userRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

        var ex = Assert.ThrowsAsync<Exception>(() => _authService.RefreshToken("invalid"));
        Assert.That(ex.Message, Is.EqualTo("Invalid or expired refresh token"));
    }

    [Test]
    public async Task Logout_ClearsRefreshToken()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            RefreshToken = "oldToken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        _userRepo.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepo.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync((Guid id, User u) => u);

        var result = await _authService.Logout(userId);

        Assert.IsTrue(result);
        Assert.That(user.RefreshToken, Is.Null);
        Assert.That(user.RefreshTokenExpiryTime, Is.EqualTo(DateTime.MinValue));
    }
}


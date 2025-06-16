using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventBookingApi.Model;
using EventBookingApi.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace EventBookingApi.Tests.Service
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private const string JwtKey = "ThisIsASecretKeyForTestingPurposesOnly123!";

        [SetUp]
        public void SetUp()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["Keys:JwtTokenKey"]).Returns(JwtKey);
            _tokenService = new TokenService(mockConfiguration.Object);
        }

        [Test]
        public void GenerateToken_ValidUser_ReturnsValidJwtToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = UserRole.Admin
            };

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.IsNotNull(jwtToken);
        }

        [Test]
        public void GenerateRefreshToken_ReturnsNonNullBase64String()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.IsNotNull(refreshToken);
            Assert.DoesNotThrow(() => Convert.FromBase64String(refreshToken));
            Assert.Greater(refreshToken.Length, 0);
        }
    }
}

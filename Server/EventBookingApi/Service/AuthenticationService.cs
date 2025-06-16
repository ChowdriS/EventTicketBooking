using System;
using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly IRepository<Guid, User> _userRepository;

    public AuthenticationService(ITokenService tokenService,
                                IRepository<Guid, User> userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }
    public async Task<UserLoginResponseDTO> Login(UserLoginRequestDTO user)
    {
        try
        {
            var allUsers = await _userRepository.GetAll();

            var existingUser = allUsers.FirstOrDefault(u =>
                string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase) && !u.IsDeleted);

            if (existingUser == null)
                throw new Exception("No such user");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash ?? "");
            if (!isPasswordValid)
                throw new Exception("Invalid password");

            var token = _tokenService.GenerateToken(existingUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            existingUser.RefreshToken = refreshToken;
            existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.Update(existingUser.Id, existingUser);

            return new UserLoginResponseDTO
            {
                Username = existingUser.Username ?? "",
                Email = existingUser.Email ?? "",
                Token = token,
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<UserLoginResponseDTO> RefreshToken(string refreshToken)
    {
        var users = await _userRepository.GetAll();
        var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken
                                        && u.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (user == null)
            throw new Exception("Invalid or expired refresh token");

        var newToken = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.Update(user.Id, user);

        return new UserLoginResponseDTO
        {
            Username = user.Username ?? "",
            Email = user.Email ?? "",
            Token = newToken,
            RefreshToken = newRefreshToken
        };
    }
    public async Task<bool> Logout(Guid Id)
    {
        var user = await _userRepository.GetById(Id);
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userRepository.Update(user.Id, user);
        return true;
    }

}

using System;
using EventBookingApi.Model;

namespace EventBookingApi.Interface;

public interface ITokenService
{
    public string GenerateToken(User user);
    public string GenerateRefreshToken();
}
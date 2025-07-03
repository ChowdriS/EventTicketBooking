using System;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;

public interface IUserService
{

    public Task<ICollection<UserAllResponseDTO>> GetAll();
    public Task<UserResponseDTO> AddUser(UserAddRequestDTO dto);
    public Task<UserResponseDTO> AddManager(UserAddRequestDTO dto);
    public Task<UserResponseDTO> AddAdmin(UserAddRequestDTO dto);
    public Task<UserResponseDTO> GetMe(Guid Id);
    public Task<UserResponseDTO> updateUser(Guid Id, UserUpdateRequestDTO dto);

    public Task<UserResponseDTO> changePasssword(Guid Id, ChangePasswordDTO dto);

    public Task<UserResponseDTO> deleteUser(Guid Id, Guid userId);
}

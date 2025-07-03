using System;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Service;

public class UserService : IUserService
{
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ObjectMapper _mapper;

    public UserService(IRepository<Guid, User> userRepository,
                        IEncryptionService encryptionService,
                        ObjectMapper mapper)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }

    private async Task<UserResponseDTO> Add(UserAddRequestDTO dto, UserRole role)
    {
        if (dto == null) throw new Exception("All fields are Required!");
        bool flag = false;
        try
        {
            var allUsers = await _userRepository.GetAll();
            var olduser = allUsers.FirstOrDefault(u => u.Email!.Equals(dto.Email));
            if (olduser != null) flag = true;
        }
        catch (Exception) { }
        if(flag == true)    throw new Exception("Email Already Taken!");
        var passwordhash = await _encryptionService.EncryptData(new EncryptModel
        {
            Data = dto.Password
        });
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordhash.EncryptedData,
            Role = role
        };

        user = await _userRepository.Add(user);
        return _mapper.UserResponseDTOMapper(user);
    }

    

    public async Task<UserResponseDTO> AddUser(UserAddRequestDTO dto)
    {
        return await Add(dto, UserRole.User);
    }

    public async Task<UserResponseDTO> AddManager(UserAddRequestDTO dto)
    {
        return await Add(dto, UserRole.Manager);
    }

    public async Task<UserResponseDTO> AddAdmin(UserAddRequestDTO dto)
    {
        return await Add(dto, UserRole.Admin);
    }
    public async Task<UserResponseDTO> updateUser(Guid Id, UserUpdateRequestDTO dto)
    {
        var user = await _userRepository.GetById(Id);
        if (dto.Username != null)
        {
            if (dto.Username.Equals(user.Username) == true)
            {
                throw new Exception("No Change Needed!");
            }
            user.Username = dto.Username;
            user.UpdatedAt = DateTime.UtcNow;
            user = await _userRepository.Update(Id, user);
        }
        else
        {
            throw new Exception("Nothing to update!");
        }
        return _mapper.UserResponseDTOMapper(user);
    }

    public async Task<UserResponseDTO> changePasssword(Guid Id, ChangePasswordDTO dto)
    {
        var user = await _userRepository.GetById(Id);
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.oldPassword,user.PasswordHash);
        if (!isPasswordValid)
            throw new Exception("Invalid old password");
        var passwordhash = await _encryptionService.EncryptData(new EncryptModel
        {
            Data = dto.newPassword
        });
        user.PasswordHash = passwordhash.EncryptedData;
        user.UpdatedAt = DateTime.UtcNow;
        user = await _userRepository.Update(Id, user);
        return _mapper.UserResponseDTOMapper(user);
    }

    public async Task<UserResponseDTO> GetMe(Guid Id)
    {
        var user = await _userRepository.GetById(Id);
        return _mapper.UserResponseDTOMapper(user);
    }
    public async Task<ICollection<UserAllResponseDTO>> GetAll()
    {
        var users = await _userRepository.GetAll();
        ICollection<UserAllResponseDTO> response = [];
        foreach (var item in users){
            response.Add(_mapper.UserALLResponseDTOMapper(item));
        }
        return response;
    }

    public async Task<UserResponseDTO> deleteUser(Guid Id, Guid userId)
    {
        var user = await _userRepository.GetById(Id);
        var requester = await _userRepository.GetById(userId);
        if (Id != userId && requester.Role != UserRole.Admin)
            throw new Exception("UnAuthorised Access");
        if (user.IsDeleted) throw new Exception("User is already deleted!");
        user.IsDeleted = true;
        user = await _userRepository.Update(Id, user);
        return _mapper.UserResponseDTOMapper(user);
    }
}

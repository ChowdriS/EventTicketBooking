using System.Security.Claims;
using System.Threading.Tasks;
using EventBookingApi.Context;
using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingApi.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IOtherFunctionalities _otherFuntionailities;

        public UserController(IAuthenticationService authenticationService,
                                IUserService userService,
                                IOtherFunctionalities otherFuntionailities)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _otherFuntionailities = otherFuntionailities;
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] UserAddRequestDTO dto)
        {
            try
            {
                var user = await _userService.AddAdmin(dto);
                return Ok(ApiResponse<object>.SuccessResponse("Admin succesfully added ",user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User creation is failed", new {ex.Message }));
            }
        }

        [HttpPost("manager")]
        public async Task<IActionResult> AddManager([FromBody] UserAddRequestDTO dto)
        {
            try
            {
                var user = await _userService.AddManager(dto);
                return Ok(ApiResponse<object>.SuccessResponse("Manager succesfully added ",user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User creation is failed", new {ex.Message }));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAddRequestDTO dto)
        {
            try
            {
                var user = await _userService.AddUser(dto);
                return Ok(ApiResponse<object>.SuccessResponse("User succesfully added ", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User creation is failed", new { ex.Message }));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var user_me = await _userService.GetMe(userId);
                return Ok(ApiResponse<object>.SuccessResponse("Your details are succesfully fetched!", user_me));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Getting your details operation failed!", new { ex.Message }));
            }
        }
        [HttpGet("all")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var users = await _userService.GetAll();
                return Ok(ApiResponse<object>.SuccessResponse("All User Details are succesfully fetched!", users));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("operation failed!", new { ex.Message }));
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update(UserUpdateRequestDTO dto)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var user = await _userService.updateUser(userId, dto);
                return Ok(ApiResponse<object>.SuccessResponse("User succesfully updated", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User updation is failed", new { ex.Message }));
            }
        }

        [HttpPut("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var user = await _userService.changePasssword(userId, dto);
                return Ok(ApiResponse<object>.SuccessResponse("Password succesfully Changed", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Password updation is failed", new { ex.Message }));
            }
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var user = await _userService.deleteUser(id,userId);
                return Ok(ApiResponse<object>.SuccessResponse("User is successfully deleted!", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User deletion is failed", new { ex.Message }));
            }
        }
    }
}

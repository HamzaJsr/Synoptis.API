
using Synoptis.API.DTOs;

namespace Synoptis.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterDTO dto);
        Task<AuthResultDTO> LoginAsync(LoginDTO dto);

        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO?> GetUserAsync(Guid userId);
        Task<UserResponseDTO> CreateUserByResponsableAsync(Guid responsableId, CreateUserDTO dto);
        Task<UserResponseDTO?> GetMeAsync(Guid userId);


    }
}
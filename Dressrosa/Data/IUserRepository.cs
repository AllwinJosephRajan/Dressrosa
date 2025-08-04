using Dressrosa.Model;
using Dressrosa.Core.Model;

namespace Dressrosa.Data
{
    public interface IUserRepository
    {
        Task<bool> CheckUserNameExistsAsync(string userName);
        Task<bool> CheckEmailExistsAsync(string emailAddress);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> Login(TokenRequest tokenRequest);
        Task<User> GetUserByEmailAsync(string emailAddress);
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(string userId);
    }
}

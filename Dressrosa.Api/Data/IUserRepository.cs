using Dressrosa.Api.Model;
using Dressrosa.API.Core.Model;

namespace Dressrosa.Api.Data
{
    public interface IUserRepository
    {
        Task<bool> CheckUserNameExistsAsync(string userName);
        Task<bool> CheckEmailExistsAsync(string emailAddress);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> Login(TokenRequest tokenRequest);
        Task<User> GetUserByEmailAsync(string emailAddress);
        Task<User> AddUserAsync(User user);
    }
}

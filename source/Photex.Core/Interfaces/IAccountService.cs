using System.Security.Claims;
using System.Threading.Tasks;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Requests;

namespace Photex.Core.Interfaces
{
    public interface IAccountService
    {
        Task<ClaimsIdentity> Authenticate(LoginRequest request);
        Task<UserModel> Register(RegistrationRequest request);
        Task<UserModel> GetUserData(long userId);
    }
}

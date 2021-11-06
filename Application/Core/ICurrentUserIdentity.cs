using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;

namespace Application.Core
{
    [ScopedService]
    public interface ICurrentUserIdentity
    {
        Task<User> GetCurrentUser();
    }
}
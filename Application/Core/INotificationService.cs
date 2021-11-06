using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;

namespace Application.Core
{
    [ScopedService]
    public interface INotificationService
    {
        Task SendMessage(string userId, string msgId, object message);
    }
}
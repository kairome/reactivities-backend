using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;

namespace Application.Core
{
    [ScopedService]
    public interface IChatService
    {
        Task SendMessage(string activityId, CommentDto comment);
    }
}
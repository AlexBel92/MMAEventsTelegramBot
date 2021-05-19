using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApiClients.Models;

namespace MMAEvents.ApiClients
{
    public interface IEventsApiClient
    {
        Task<ICollection<EventDTO>> GetEventsAsync(string eventName, System.DateTimeOffset? date, int? quantity, CancellationToken cancellationToken = default);
        Task<EventDTO> GetEventByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
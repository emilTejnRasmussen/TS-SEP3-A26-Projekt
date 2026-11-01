using WebApp.Client.DTO.Event;

namespace WebApp.Client.Services;

public interface IEventService
{
    Task<EventResponse> CreateAsync(CreateEventRequest request);
    Task<EventResponse> GetByIdAsync(long id);
    Task<List<EventResponse>> GetAllAsync();
}

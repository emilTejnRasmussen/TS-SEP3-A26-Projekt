using WebApp.Client.DTO.Event;

namespace WebApp.Client.Services;

public interface IEventService
{
    Task<EventResponse> CreateAsync(CreateEventRequest request);
    Task<List<EventResponse>> GetAllAsync();
}

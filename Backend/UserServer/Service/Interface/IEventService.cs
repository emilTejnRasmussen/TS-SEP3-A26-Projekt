using DTO.Event;

namespace Service.Interface;

public interface IEventService
{
    Task<EventResponse> CreateEventAsync(CreateEventRequest request);
    Task<EventResponse> GetEventByIdAsync(long id);
}
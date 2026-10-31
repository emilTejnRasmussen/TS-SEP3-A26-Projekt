using DTO.Event;

namespace GrpcClient;

public interface IEventGrpcClient
{
    Task<EventResponse> CreateEventAsync(CreateEventRequest request);

    Task<EventResponse> GetEventByIdAsync(long id);
    
    Task<List<EventResponse>> GetManyAsync();
}
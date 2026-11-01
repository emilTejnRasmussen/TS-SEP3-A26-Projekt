using DTO.Event;
using GrpcClient;
using Repository;
using Service.Interface;

namespace Service;

public class EventService(IEventGrpcClient eventClient, IUserRepository userRepository) : IEventService
{
    public Task<EventResponse> CreateEventAsync(CreateEventRequest request)
    {
        return eventClient.CreateEventAsync(request);
    }

    public async Task<EventResponse> GetEventByIdAsync(long id)
    {
        var response = await eventClient.GetEventByIdAsync(id);

        var hosts = new List<HostResponse>();

        foreach (var hostId in response.HostIds)
        {
            var user = await userRepository.GetSingleByIdAsync(hostId);

            hosts.Add(new HostResponse(user.Id, user.Name));
        }

        return response with { Hosts = hosts };
    }

    public Task<List<EventOverviewResponse>> GetManyAsync()
    {
        return eventClient.GetManyAsync();
    }
}
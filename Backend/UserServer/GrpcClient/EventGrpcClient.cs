using System.Globalization;
using DTO.Event;
using Proto = Kin.Events;

namespace GrpcClient;

public class EventGrpcClient(Proto.EventService.EventServiceClient client) : IEventGrpcClient
{
    public async Task<EventResponse> CreateEventAsync(CreateEventRequest request)
    {
        var grpcRequest = new Proto.CreateEventRequest
        {
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            LongDescription = request.LongDescription,
            Location = request.Location,
            MaxParticipants = request.MaxParticipants,
            DateTime = request.DateTime.ToString("yyyy-MM-dd'T'HH:mm:ss")
        };

        grpcRequest.HostIds.AddRange(request.HostIds);

        var response = await client.CreateEventAsync(grpcRequest);

        return new EventResponse(
            response.Id,
            response.Title,
            response.ShortDescription,
            response.LongDescription,
            response.Location,
            response.MaxParticipants,
            DateTime.Parse(response.DateTime, CultureInfo.InvariantCulture),
            [.. response.HostIds]
        );
    }

    public Task<EventResponse> GetEventByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<EventOverviewResponse>> GetManyAsync()
    {
        var response = await client.GetAllEventsAsync(new Proto.Empty());

        return response.Events
            .Select(e => new EventOverviewResponse(
                e.Id,
                e.Title,
                e.ShortDescription,
                e.Location,
                DateTime.Parse(e.DateTime, CultureInfo.InvariantCulture)
            ))
            .ToList();
    }
}
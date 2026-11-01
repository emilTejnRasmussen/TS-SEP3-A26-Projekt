namespace WebApp.Client.DTO.Event;

// TODO: Backendens EventResponse (Shared/DTO/Event) er stadig tom - ret felterne, når den er lavet.
public record EventResponse(
    long Id,
    string Title,
    string ShortDescription,
    string? LongDescription,
    string Location,
    int MaxParticipants,
    DateTime DateTime,
    string? Status,
    long HostId
);

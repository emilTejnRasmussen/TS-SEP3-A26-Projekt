namespace WebApp.Client.DTO.Event;

// TODO: Ret felterne, så de matcher backendens EventResponse, når den er lavet.
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

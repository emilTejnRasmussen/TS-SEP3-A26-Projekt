namespace DTO.Event;

public record EventOverviewResponse(
    long Id,
    string Title,
    string ShortDescription,
    string Location,
    DateTime DateTime
);
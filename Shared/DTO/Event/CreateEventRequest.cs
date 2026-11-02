using System.ComponentModel.DataAnnotations;

namespace DTO.Event;

// Klasse med set, så Blazor-formularer kan binde direkte til felterne.
// Felterne følger Contracts/proto/event.proto (CreateEventRequest).
public class CreateEventRequest
{
    [Required(ErrorMessage = "Titel er påkrævet")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Kort beskrivelse er påkrævet")]
    public string ShortDescription { get; set; } = "";

    // Tom streng i stedet for null: proto-strenge må ikke være null.
    public string LongDescription { get; set; } = "";

    [Required(ErrorMessage = "Placering er påkrævet")]
    public string Location { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Max deltagere skal være mindst 1")]
    public int MaxParticipants { get; set; }

    // Uden tidszone (Kind = Unspecified), fordi Event Serveren læser den som Java LocalDateTime.
    public DateTime DateTime { get; set; }

    public List<long> HostIds { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace WebApp.Client.DTO.Event;

// TODO: Backendens Event-DTO findes ikke endnu. Felterne er taget fra domænemodellen -
// ret navnene, så de matcher backendens CreateEventRequest, når den er lavet.
public class CreateEventRequest
{
    [Required(ErrorMessage = "Titel er påkrævet")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Kort beskrivelse er påkrævet")]
    public string ShortDescription { get; set; } = "";

    public string? LongDescription { get; set; }

    [Required(ErrorMessage = "Placering er påkrævet")]
    public string Location { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Max deltagere skal være mindst 1")]
    public int MaxParticipants { get; set; } = 10;

    public DateTime DateTime { get; set; } = DateTime.Now.Date.AddDays(1).AddHours(18);

    // TODO: Fjern, når login (JWT) er på plads - så finder backenden værten ud fra tokenet.
    [Range(1, long.MaxValue, ErrorMessage = "Vælg en vært")]
    public long HostId { get; set; }
}

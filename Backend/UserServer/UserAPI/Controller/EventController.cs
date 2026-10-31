using DTO.Event;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace UserAPI.Controller;

public class EventsController(IEventService eventService) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] CreateEventRequest request)
    {
        var response = await eventService.CreateEventAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{eventId:long}")]
    public async Task<IActionResult> GetEventById([FromRoute] long eventId)
    {
        var response = await eventService.GetEventByIdAsync(eventId);

        return Ok(response);
    }
}
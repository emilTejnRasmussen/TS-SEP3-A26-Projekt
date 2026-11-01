using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controller;

/// <summary>
/// Defines the shared API route prefix for all controllers.
/// Increment the API version only when introducing breaking changes
/// that cannot be handled while maintaining backward compatibility.
/// New endpoints, optional fields, and bug fixes do not require a new version.
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class ApiControllerBase : ControllerBase
{
}
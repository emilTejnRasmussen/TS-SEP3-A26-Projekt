using System.Text.Json;

namespace WebApp.Client.Services;

// Kastes når backenden svarer med en fejlstatus, så siderne kan vise beskeden.
public class ApiException(string message) : Exception(message)
{
    public static async Task ThrowIfFailedAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync();
        throw new ApiException(ExtractMessage(body) ??
            $"Backend svarede {(int)response.StatusCode} {response.ReasonPhrase}" +
            (string.IsNullOrWhiteSpace(body) ? "" : $": {body}"));
    }

    // Backendens GlobalExceptionHandler svarer med { "error": "..." }.
    private static string? ExtractMessage(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.ValueKind == JsonValueKind.Object &&
                   doc.RootElement.TryGetProperty("error", out var error)
                ? error.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

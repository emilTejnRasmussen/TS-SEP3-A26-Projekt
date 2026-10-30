namespace WebApp.Client.Services;

// Kastes når backenden svarer med en fejlstatus, så siderne kan vise beskeden.
public class ApiException(string message) : Exception(message)
{
    public static async Task ThrowIfFailedAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync();
        throw new ApiException(
            $"Backend svarede {(int)response.StatusCode} {response.ReasonPhrase}" +
            (string.IsNullOrWhiteSpace(body) ? "" : $": {body}"));
    }
}

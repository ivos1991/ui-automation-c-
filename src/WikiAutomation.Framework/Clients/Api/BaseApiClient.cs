using System.Net.Http.Headers;
using System.Text.Json;

namespace WikiAutomation.Framework.Clients.Api;

/// <summary>
/// Provides generic HTTP and JSON behavior for API clients.
/// </summary>
public abstract class BaseApiClient : IDisposable
{
    /// <summary>
    /// Initializes the base client with the concrete <see cref="HttpClient"/> instance used for requests.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the target API.</param>
    protected BaseApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Sends a GET request and parses the response body into a JSON document.
    /// </summary>
    /// <param name="requestUri">The relative request URI.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The parsed JSON document.</returns>
    protected async Task<JsonDocument> GetJsonDocumentAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(responseBody);
    }

    /// <summary>
    /// Disposes the underlying HTTP client owned by this API client instance.
    /// </summary>
    public void Dispose()
    {
        HttpClient.Dispose();
    }
}

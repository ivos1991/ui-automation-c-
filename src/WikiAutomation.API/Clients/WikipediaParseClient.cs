using System.Text.Json;
using WikiAutomation.API.Models;
using WikiAutomation.Framework.Clients.Api;

namespace WikiAutomation.API.Clients;

/// <summary>
/// Encapsulates direct calls to the MediaWiki Parse API.
/// </summary>
public sealed class WikipediaParseClient : BaseApiClient
{
    /// <summary>
    /// Initializes the API client with the configured HTTP transport.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for MediaWiki requests.</param>
    public WikipediaParseClient(HttpClient httpClient) : base(httpClient)
    {
    }

    /// <summary>
    /// Retrieves the logical sections for the target Wikipedia article.
    /// </summary>
    /// <param name="articleTitle">The Wikipedia article title.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The collection of article sections.</returns>
    public async Task<IReadOnlyCollection<WikipediaSection>> GetSectionsAsync(string articleTitle, CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"?action=parse&page={Uri.EscapeDataString(articleTitle)}&prop=sections&format=json&formatversion=2&origin=*";

        using var document = await GetJsonDocumentAsync(requestUri, cancellationToken);
        var sections = document.RootElement.GetProperty("parse").GetProperty("sections");

        return sections
            .EnumerateArray()
            .Select(section => new WikipediaSection(
                section.GetProperty("index").GetString() ?? string.Empty,
                section.GetProperty("line").GetString() ?? string.Empty,
                section.GetProperty("anchor").GetString() ?? string.Empty))
            .ToArray();
    }

    /// <summary>
    /// Retrieves the HTML fragment for a specific article section.
    /// </summary>
    /// <param name="articleTitle">The Wikipedia article title.</param>
    /// <param name="sectionIndex">The MediaWiki section index.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The raw HTML for the requested section.</returns>
    public async Task<string> GetSectionHtmlAsync(
        string articleTitle,
        string sectionIndex,
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"?action=parse&page={Uri.EscapeDataString(articleTitle)}&prop=text&section={Uri.EscapeDataString(sectionIndex)}&format=json&formatversion=2&origin=*";

        using var document = await GetJsonDocumentAsync(requestUri, cancellationToken);
        return document.RootElement.GetProperty("parse").GetProperty("text").GetString() ?? string.Empty;
    }
}

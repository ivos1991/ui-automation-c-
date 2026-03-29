using HtmlAgilityPack;
using WikiAutomation.API.Clients;

namespace WikiAutomation.API.Services;

/// <summary>
/// Provides domain-oriented API behavior for the Playwright Wikipedia article.
/// </summary>
public sealed class WikipediaArticleService : IDisposable
{
    private readonly WikipediaParseClient _client;

    /// <summary>
    /// Initializes the service with the MediaWiki client used for endpoint access.
    /// </summary>
    /// <param name="client">The MediaWiki Parse API client.</param>
    public WikipediaArticleService(WikipediaParseClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Retrieves the readable text for the Debugging features section.
    /// </summary>
    /// <param name="articleTitle">The Wikipedia article title.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The readable section text.</returns>
    public async Task<string> GetDebuggingFeaturesTextAsync(string articleTitle, CancellationToken cancellationToken = default)
    {
        var sections = await _client.GetSectionsAsync(articleTitle, cancellationToken);
        var targetSection = sections.FirstOrDefault(section =>
            section.Line.Equals("Debugging features", StringComparison.OrdinalIgnoreCase));

        if (targetSection is null)
        {
            throw new InvalidOperationException("Could not find the 'Debugging features' section through the MediaWiki Parse API.");
        }

        var sectionHtml = await _client.GetSectionHtmlAsync(articleTitle, targetSection.Index, cancellationToken);
        return ExtractReadableText(sectionHtml);
    }

    /// <summary>
    /// Converts MediaWiki section HTML into plain readable text for comparison in tests.
    /// </summary>
    /// <param name="sectionHtml">The raw HTML returned by the Parse API.</param>
    /// <returns>The cleaned readable text.</returns>
    public string ExtractReadableText(string sectionHtml)
    {
        if (string.IsNullOrWhiteSpace(sectionHtml))
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(sectionHtml);

        foreach (var node in document.DocumentNode.SelectNodes("//sup|//style|//script") ?? Enumerable.Empty<HtmlNode>())
        {
            node.Remove();
        }

        var lines = HtmlEntity.DeEntitize(document.DocumentNode.InnerText)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !line.StartsWith("^", StringComparison.Ordinal))
            .Where(line => !line.StartsWith("Debugging features", StringComparison.OrdinalIgnoreCase))
            .Where(line => !string.Equals(line, "[edit]", StringComparison.Ordinal))
            .ToArray();

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Disposes the owned MediaWiki client and its underlying HTTP resources.
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
    }
}

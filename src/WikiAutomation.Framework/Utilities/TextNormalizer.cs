using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WikiAutomation.Framework.Utilities;

/// <summary>
/// Normalizes raw UI and API text into a comparison-friendly word stream.
/// </summary>
public static partial class TextNormalizer
{
    /// <summary>
    /// Converts raw text into a lowercase normalized string without punctuation noise.
    /// </summary>
    /// <param name="rawText">The raw source text.</param>
    /// <returns>The normalized text.</returns>
    public static string Normalize(string? rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return string.Empty;
        }

        var htmlDecoded = HtmlEntity.DeEntitize(rawText);
        var withoutBracketedReferences = ReferencePattern().Replace(htmlDecoded, " ");
        var lettersAndDigitsOnly = NonWordPattern().Replace(withoutBracketedReferences.ToLowerInvariant(), " ");
        var singleSpaced = MultiWhitespacePattern().Replace(lettersAndDigitsOnly, " ").Trim();
        return singleSpaced;
    }

    /// <summary>
    /// Produces the distinct normalized word set for a raw text input.
    /// </summary>
    /// <param name="rawText">The raw source text.</param>
    /// <returns>The distinct normalized word set.</returns>
    public static IReadOnlyCollection<string> GetNormalizedWordSet(string? rawText)
    {
        var normalized = Normalize(rawText);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Array.Empty<string>();
        }

        return normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(word => word, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>
    /// Produces cleaned text lines while preserving the original line structure.
    /// </summary>
    /// <param name="rawText">The raw source text.</param>
    /// <returns>The normalized non-empty lines.</returns>
    public static IReadOnlyCollection<string> GetNormalizedLines(string? rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return Array.Empty<string>();
        }

        return HtmlEntity.DeEntitize(rawText)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => ReferencePattern().Replace(line, " "))
            .Select(line => MultiWhitespacePattern().Replace(line, " ").Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }

    [GeneratedRegex(@"\[[^\]]+\]", RegexOptions.Compiled)]
    private static partial Regex ReferencePattern();

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.Compiled)]
    private static partial Regex NonWordPattern();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex MultiWhitespacePattern();
}

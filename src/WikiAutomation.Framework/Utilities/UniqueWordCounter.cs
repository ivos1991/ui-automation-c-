namespace WikiAutomation.Framework.Utilities;

/// <summary>
/// Counts the distinct normalized words in a text block.
/// </summary>
public static class UniqueWordCounter
{
    /// <summary>
    /// Counts unique words after applying the shared text-normalization rules.
    /// </summary>
    /// <param name="rawText">The raw source text.</param>
    /// <returns>The count of unique normalized words.</returns>
    public static int CountUniqueWords(string? rawText)
    {
        return TextNormalizer.GetNormalizedWordSet(rawText).Count;
    }
}

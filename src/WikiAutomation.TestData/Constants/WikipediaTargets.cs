namespace WikiAutomation.TestData.Constants;

/// <summary>
/// Stores stable article and section labels used across tests and page-level helpers.
/// </summary>
public static class WikipediaTargets
{
    /// <summary>
    /// The visible article heading for the assignment target page.
    /// </summary>
    public const string PlaywrightArticleHeading = "Playwright (software)";

    /// <summary>
    /// The logical section name used for Task 1 extraction.
    /// </summary>
    public const string DebuggingFeaturesSection = "Debugging features";

    /// <summary>
    /// The Microsoft development tools label used for Task 2 navigation.
    /// </summary>
    public const string MicrosoftDevelopmentTools = "Microsoft development tools";

    /// <summary>
    /// The subsection label used for Task 2 validation.
    /// </summary>
    public const string TestingAndDebugging = "Testing and debugging";

    /// <summary>
    /// The stop headings used when isolating the visible Debugging features section from the article body.
    /// </summary>
    public static readonly IReadOnlyCollection<string> DebuggingFeaturesStopHeadings = new[]
    {
        "Reporters",
        "Usage Trends",
        "References",
        "Further reading",
        "External links"
    };
}

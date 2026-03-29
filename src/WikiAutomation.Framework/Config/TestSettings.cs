using System.Globalization;

namespace WikiAutomation.Framework.Config;

/// <summary>
/// Stores typed environment-driven configuration for the assignment project.
/// </summary>
public sealed class TestSettings
{
    public const string BrowserEvidenceOnFailure = "on_failure";

    public const string BrowserEvidenceAlways = "always";

    public string BaseUrl { get; init; } = GetEnvironmentValue("BASE_URL", "https://en.wikipedia.org");

    public string ArticleTitle { get; init; } = GetEnvironmentValue("ARTICLE_TITLE", "Playwright_(software)");

    public bool Headless { get; init; } = GetBoolean("HEADLESS", true);

    public float SlowMoMs { get; init; } = GetFloat("SLOW_MO_MS", 0);

    public int DefaultTimeoutMs { get; init; } = GetInteger("DEFAULT_TIMEOUT_MS", 15000);

    public int MaxLogMessageLength { get; init; } = GetInteger("MAX_LOG_MESSAGE_LENGTH", 10000);

    public string BrowserEvidenceMode { get; init; } = GetBrowserEvidenceMode();

    public string ArticleUrl => $"{BaseUrl.TrimEnd('/')}/wiki/{ArticleTitle}";

    public string MediaWikiApiUrl => $"{BaseUrl.TrimEnd('/')}/w/api.php";

    public string ProjectRoot => ResolveProjectRoot();

    public string ArtifactsRoot => Path.Combine(ProjectRoot, "artifacts");

    public string ScreenshotsDirectory => Path.Combine(ArtifactsRoot, "screenshots");

    public string TracesDirectory => Path.Combine(ArtifactsRoot, "traces");

    public string AllureResultsDirectory => Path.Combine(ArtifactsRoot, "allure-results");

    public bool CaptureBrowserArtifactsForEveryTest =>
        BrowserEvidenceMode.Equals(BrowserEvidenceAlways, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Loads the effective settings and creates the artifact directories required by the test run.
    /// </summary>
    /// <returns>A fully initialized settings instance.</returns>
    public static TestSettings Load()
    {
        var settings = new TestSettings();
        Directory.CreateDirectory(settings.ArtifactsRoot);
        Directory.CreateDirectory(settings.ScreenshotsDirectory);
        Directory.CreateDirectory(settings.TracesDirectory);
        Directory.CreateDirectory(settings.AllureResultsDirectory);
        return settings;
    }

    /// <summary>
    /// Resolves the project root so artifacts stay under the repository folder instead of under build output.
    /// </summary>
    /// <returns>The resolved project-root path.</returns>
    private static string ResolveProjectRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);

        while (directory is not null)
        {
            var solutionPath = Path.Combine(directory.FullName, "WikiAutomation.HomeAssignment.sln");
            if (File.Exists(solutionPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return currentDirectory;
    }

    /// <summary>
    /// Reads a string environment variable with a fallback value.
    /// </summary>
    /// <param name="key">The environment-variable name.</param>
    /// <param name="defaultValue">The default value when the variable is absent.</param>
    /// <returns>The resolved string value.</returns>
    private static string GetEnvironmentValue(string key, string defaultValue)
    {
        var rawValue = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(rawValue) ? defaultValue : rawValue.Trim();
    }

    /// <summary>
    /// Reads a boolean environment variable with a fallback value.
    /// </summary>
    /// <param name="key">The environment-variable name.</param>
    /// <param name="defaultValue">The default value when the variable is absent.</param>
    /// <returns>The resolved boolean value.</returns>
    private static bool GetBoolean(string key, bool defaultValue)
    {
        var rawValue = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return defaultValue;
        }

        return rawValue.Trim().Equals("1", StringComparison.OrdinalIgnoreCase)
            || rawValue.Trim().Equals("true", StringComparison.OrdinalIgnoreCase)
            || rawValue.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)
            || rawValue.Trim().Equals("on", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Reads an integer environment variable with a fallback value.
    /// </summary>
    /// <param name="key">The environment-variable name.</param>
    /// <param name="defaultValue">The default value when the variable is absent.</param>
    /// <returns>The resolved integer value.</returns>
    private static int GetInteger(string key, int defaultValue)
    {
        var rawValue = Environment.GetEnvironmentVariable(key);
        return int.TryParse(rawValue, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Reads a floating-point environment variable with a fallback value.
    /// </summary>
    /// <param name="key">The environment-variable name.</param>
    /// <param name="defaultValue">The default value when the variable is absent.</param>
    /// <returns>The resolved floating-point value.</returns>
    private static float GetFloat(string key, float defaultValue)
    {
        var rawValue = Environment.GetEnvironmentVariable(key);
        return float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : defaultValue;
    }

    private static string GetBrowserEvidenceMode()
    {
        var rawValue = GetEnvironmentValue("BROWSER_EVIDENCE_MODE", BrowserEvidenceOnFailure);
        return rawValue.Equals(BrowserEvidenceAlways, StringComparison.OrdinalIgnoreCase)
            ? BrowserEvidenceAlways
            : BrowserEvidenceOnFailure;
    }
}

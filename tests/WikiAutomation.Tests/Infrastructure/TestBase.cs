using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using WikiAutomation.API.Services;
using WikiAutomation.API.Clients;
using WikiAutomation.Framework.Config;
using WikiAutomation.Framework.Logging;
using WikiAutomation.Framework.Reporting;

namespace WikiAutomation.Tests.Infrastructure;

[AllureNUnit]
[AllureParentSuite("Wikipedia Home Assignment")]
/// <summary>
/// Provides shared configuration and API service construction for all test layers.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Loads the typed test settings once for the current process.
    /// </summary>
    protected static readonly TestSettings Settings = TestSettings.Load();

    /// <summary>
    /// Starts the shared test logging scope before each test so logs carry the same metadata in every layer.
    /// </summary>
    [SetUp]
    public void StartTestLogging()
    {
        FrameworkLogger.Configure(Settings.MaxLogMessageLength);
        FrameworkLogger.StartTest(TestContext.CurrentContext.Test.Name);
        FrameworkLogger.Info("Starting test execution.");
    }

    /// <summary>
    /// Creates the assignment's API service with the headers required by the MediaWiki Parse API.
    /// </summary>
    /// <returns>A configured API service instance for the current test.</returns>
    protected WikipediaArticleService CreateApiService()
    {
        FrameworkLogger.Info("Creating MediaWiki API service for the current test.");

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(Settings.MediaWikiApiUrl, UriKind.Absolute)
        };
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WikiAutomationHomeAssignment/1.0");
        httpClient.DefaultRequestHeaders.Add("Api-User-Agent", "WikiAutomationHomeAssignment/1.0");
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        var wikipediaParseClient = new WikipediaParseClient(httpClient);
        return new WikipediaArticleService(wikipediaParseClient);
    }

    /// <summary>
    /// Executes an asynchronous action with a scoped API service and disposes the service automatically.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    protected async Task UseApiServiceAsync(Func<WikipediaArticleService, Task> action)
    {
        using var apiService = CreateApiService();
        await action(apiService);
    }

    /// <summary>
    /// Executes an asynchronous function with a scoped API service and returns its result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <returns>The value returned by the supplied function.</returns>
    protected async Task<T> UseApiServiceAsync<T>(Func<WikipediaArticleService, Task<T>> action)
    {
        using var apiService = CreateApiService();
        return await action(apiService);
    }

    /// <summary>
    /// Finalizes the shared logging scope after each test and publishes the buffered logs to Allure.
    /// </summary>
    [TearDown]
    public void FinishTestLogging()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status switch
        {
            TestStatus.Passed => "passed",
            TestStatus.Failed => "failed",
            TestStatus.Skipped => "skipped",
            TestStatus.Inconclusive => "inconclusive",
            _ => "unknown"
        };

        FrameworkLogger.UpdateTestStatus(status);

        var runtime = FrameworkLogger.GetCurrentTestRuntime();
        if (runtime is not null)
        {
            FrameworkLogger.Info($"Finished test execution in {runtime.Value.TotalMilliseconds:F0} ms.");
        }

        AllureAttachmentService.AttachJsonObject(
            "test-execution-summary",
            new
            {
                testName = TestContext.CurrentContext.Test.Name,
                fullName = TestContext.CurrentContext.Test.FullName,
                categories = TestContext.CurrentContext.Test.Properties["Category"].Cast<object>().Select(value => value.ToString()).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray(),
                status,
                durationMs = runtime is null ? 0 : Math.Round(runtime.Value.TotalMilliseconds),
                message = TestContext.CurrentContext.Result.Message,
                stackTrace = TestContext.CurrentContext.Result.StackTrace
            });

        FrameworkLogger.ClearCurrentTest();
    }
}

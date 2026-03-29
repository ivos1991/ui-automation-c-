using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using WikiAutomation.Framework.Logging;
using WikiAutomation.Framework.Reporting;
using WikiAutomation.UI.Pages;

namespace WikiAutomation.Tests.Infrastructure;

/// <summary>
/// Owns Playwright browser lifecycle, per-test context setup, and configurable evidence capture.
/// </summary>
public abstract class PlaywrightTestBase : TestBase
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private string? _tracePath;

    protected IBrowserContext BrowserContext { get; private set; } = default!;

    protected IPage Page { get; private set; } = default!;

    /// <summary>
    /// Creates a page object for the target Wikipedia article.
    /// </summary>
    /// <returns>A page object bound to the current Playwright page and settings.</returns>
    protected WikipediaArticlePage ArticlePage()
    {
        return new WikipediaArticlePage(Page, Settings);
    }

    /// <summary>
    /// Opens the target Wikipedia article and returns the ready-to-use page object.
    /// </summary>
    /// <returns>The opened article page object.</returns>
    protected async Task<WikipediaArticlePage> OpenArticleAsync()
    {
        var articlePage = ArticlePage();
        await articlePage.OpenAsync();
        return articlePage;
    }

    /// <summary>
    /// Starts the shared Playwright browser for the current fixture class.
    /// </summary>
    [OneTimeSetUp]
    public async Task GlobalSetupAsync()
    {
        FrameworkLogger.Info($"Starting Playwright browser for fixture {GetType().Name}.");

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            SlowMo = Settings.SlowMoMs
        });
    }

    /// <summary>
    /// Creates a fresh browser context and page for the current test.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        _browser.Should().NotBeNull("the shared Playwright browser should be available before each UI test");
        FrameworkLogger.Info($"Creating a new browser context for test {TestContext.CurrentContext.Test.Name}.");

        BrowserContext = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });

        await BrowserContext.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        Page = await BrowserContext.NewPageAsync();
        Page.SetDefaultTimeout(Settings.DefaultTimeoutMs);
        Page.SetDefaultNavigationTimeout(Settings.DefaultTimeoutMs);

        _tracePath = Path.Combine(Settings.TracesDirectory, $"{TestContext.CurrentContext.Test.Name}.zip");
    }

    /// <summary>
    /// Captures failure evidence and disposes the per-test browser context.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        var shouldCaptureScreenshot =
            Page is not null &&
            (Settings.CaptureBrowserArtifactsForEveryTest || TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

        if (shouldCaptureScreenshot)
        {
            var page = Page!;

            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                FrameworkLogger.Error(
                    $"Test {TestContext.CurrentContext.Test.Name} failed with message: {TestContext.CurrentContext.Result.Message}");
                FrameworkLogger.Warn($"Capturing screenshot evidence for {TestContext.CurrentContext.Test.Name}.");
            }
            else
            {
                FrameworkLogger.Info($"Capturing screenshot evidence for passing test {TestContext.CurrentContext.Test.Name}.");
            }

            var screenshotPath = Path.Combine(
                Settings.ScreenshotsDirectory,
                $"{TestContext.CurrentContext.Test.Name}.png");

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            AllureAttachmentService.AttachFileIfExists(
                $"{TestContext.CurrentContext.Test.Name}-screenshot",
                screenshotPath,
                "image/png");
            FrameworkLogger.Info($"Attached screenshot evidence from {screenshotPath}.");
        }

        if (BrowserContext is not null)
        {
            FrameworkLogger.Info($"Stopping Playwright trace for test {TestContext.CurrentContext.Test.Name}.");

            await BrowserContext.Tracing.StopAsync(new TracingStopOptions
            {
                Path = _tracePath
            });

            if (!string.IsNullOrWhiteSpace(_tracePath) &&
                (Settings.CaptureBrowserArtifactsForEveryTest || TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed))
            {
                AllureAttachmentService.AttachFileIfExists(
                    $"{TestContext.CurrentContext.Test.Name}-trace",
                    _tracePath,
                    "application/zip");
                FrameworkLogger.Info($"Attached Playwright trace from {_tracePath}.");
            }

            await BrowserContext.CloseAsync();
        }
    }

    /// <summary>
    /// Closes the shared Playwright browser after all tests in the fixture finish.
    /// </summary>
    [OneTimeTearDown]
    public async Task GlobalTearDownAsync()
    {
        FrameworkLogger.Info($"Closing Playwright browser for fixture {GetType().Name}.");

        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }
}

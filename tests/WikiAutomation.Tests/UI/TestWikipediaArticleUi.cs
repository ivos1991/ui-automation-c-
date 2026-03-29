using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using NUnit.Framework;
using WikiAutomation.Framework.Logging;
using WikiAutomation.Framework.Reporting;
using static WikiAutomation.Tests.Assertions.AssertThat;
using WikiAutomation.Tests.Infrastructure;

namespace WikiAutomation.Tests.UI;

[TestFixture]
[Category("ui")]
[AllureParentSuite("Wikipedia Home Assignment")]
[AllureSuite("UI")]
[AllureSubSuite("Wikipedia Article")]
[AllureFeature("Wikipedia Article UI")]
/// <summary>
/// Contains browser-only coverage for the assignment's UI behaviors.
/// </summary>
public sealed class TestWikipediaArticleUi : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that the targeted technology names in the Microsoft development tools area are rendered as text links.
    /// </summary>
    [Test]
    [AllureStory("Task 2 - Microsoft development tools links")]
    public async Task MicrosoftDevelopmentTools_TestingAndDebuggingItems_ValidationExpectsTextLinksOnly()
    {
        FrameworkLogger.Info("Running UI link validation for the Microsoft development tools area.");

        var articlePage = await OpenArticleAsync();
        var results = await articlePage.GetTestingAndDebuggingLinkResultsAsync();
        var nonLinkedItems = results.Where(item => !item.HasTextLink).Select(item => item.Name).ToArray();

        AllureAttachmentService.AttachJsonObject(
            "microsoft-development-tools-testing-and-debugging-items",
            results);
        FrameworkLogger.Json("Microsoft development tools link results", results);

        That(
            nonLinkedItems,
            "every technology name under the Microsoft development tools > Testing and debugging subsection should be a text link")
            .IsEmpty();
    }

    /// <summary>
    /// Verifies that the Microsoft development tools validation fails clearly when executed on a page that does not contain the expected navbox.
    /// </summary>
    [Test]
    [AllureStory("Task 2 - Microsoft development tools links negative path")]
    public async Task MicrosoftDevelopmentTools_TestingAndDebuggingItems_OnWrongPageExpectsFailure()
    {
        FrameworkLogger.Info("Running negative UI link validation on a page without the Microsoft development tools navbox.");

        var articlePage = ArticlePage();
        await Page.GotoAsync(Settings.BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await ThrowsAsync<Exception>(
            () => articlePage.GetTestingAndDebuggingLinkResultsAsync(),
            "the validation should fail when the current page does not contain the Microsoft development tools navbox");
    }

    /// <summary>
    /// Verifies that switching the Wikipedia appearance setting to Dark applies the dark theme.
    /// </summary>
    [Test]
    [AllureStory("Task 3 - Dark mode")]
    public async Task AppearanceColorSetting_DarkModeSelectionExpectsAppliedDarkTheme()
    {
        FrameworkLogger.Info("Running UI theme-switch validation for Wikipedia dark mode.");

        var articlePage = await OpenArticleAsync();
        var switchResult = await articlePage.SwitchColorModeToDarkAsync();

        AllureAttachmentService.AttachJsonObject(
            "theme-switch-result",
            switchResult);
        FrameworkLogger.Json("Theme switch result", switchResult);

        That(
            switchResult.IsDarkThemeApplied,
            "choosing Dark in the Wikipedia appearance panel should change the page theme")
            .IsTrue();
    }

    /// <summary>
    /// Verifies that requesting a non-existent color mode fails clearly instead of silently passing.
    /// </summary>
    [Test]
    [AllureStory("Task 3 - Dark mode negative path")]
    public async Task AppearanceColorSetting_InvalidSelectionExpectsFailure()
    {
        FrameworkLogger.Info("Running negative UI theme-switch validation with an invalid color mode.");

        var articlePage = await OpenArticleAsync();

        await ThrowsAsync<InvalidOperationException>(
            () => articlePage.SwitchColorModeAsync("UltraDark"),
            "an unsupported color mode should fail explicitly in the appearance panel");
    }
}

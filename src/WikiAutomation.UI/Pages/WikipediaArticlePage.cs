using Microsoft.Playwright;
using WikiAutomation.Framework.Config;
using WikiAutomation.Framework.Logging;
using WikiAutomation.TestData.Constants;
using WikiAutomation.UI.Components;
using WikiAutomation.UI.Models;

namespace WikiAutomation.UI.Pages;

/// <summary>
/// Models the Wikipedia article page used in the assignment.
/// </summary>
public sealed class WikipediaArticlePage : BasePage
{
    private readonly ArticleSectionReader _articleSectionReader;
    private readonly DevelopmentToolsNavBox _developmentToolsNavBox;
    private readonly AppearancePanel _appearancePanel;

    /// <summary>
    /// Initializes the page object for the target article.
    /// </summary>
    /// <param name="page">The current Playwright page.</param>
    /// <param name="settings">The typed runtime settings.</param>
    public WikipediaArticlePage(IPage page, TestSettings settings) : base(page, settings)
    {
        _articleSectionReader = new ArticleSectionReader(page);
        _developmentToolsNavBox = new DevelopmentToolsNavBox(page);
        _appearancePanel = new AppearancePanel(page);
    }

    /// <summary>
    /// Opens the target article and waits for the main article heading.
    /// </summary>
    public async Task OpenAsync()
    {
        FrameworkLogger.Info("Opening the target Wikipedia article in the browser.");
        await NavigateAsync();
        await Ui.WaitForPageLoadAsync();
        await Ui.WaitForVisibleAsync(Page.GetByRole(AriaRole.Heading, new() { Name = WikipediaTargets.PlaywrightArticleHeading }));
    }

    /// <summary>
    /// Extracts the visible Debugging features section text from the live browser-rendered page.
    /// </summary>
    /// <returns>The visible section text captured from the UI.</returns>
    public async Task<string> GetDebuggingFeaturesTextAsync()
    {
        return await _articleSectionReader.ReadDebuggingFeaturesTextAsync();
    }

    /// <summary>
    /// Collects the leaf technology-link results from the Microsoft development tools area.
    /// </summary>
    /// <returns>The evaluated link results for the targeted technology names.</returns>
    public async Task<TechnologyLinkCheckResult[]> GetTestingAndDebuggingLinkResultsAsync()
    {
        return await _developmentToolsNavBox.ReadTestingAndDebuggingLinkResultsAsync();
    }

    /// <summary>
    /// Changes Wikipedia's appearance setting to Dark and returns the before/after theme state.
    /// </summary>
    /// <returns>The theme-switch result model.</returns>
    public async Task<ThemeSwitchResult> SwitchColorModeToDarkAsync()
    {
        return await _appearancePanel.SwitchColorModeToDarkAsync();
    }

    /// <summary>
    /// Changes Wikipedia's appearance setting to the requested visible option label.
    /// </summary>
    /// <param name="colorMode">The visible color mode label to apply.</param>
    /// <returns>The theme-switch result model.</returns>
    public async Task<ThemeSwitchResult> SwitchColorModeAsync(string colorMode)
    {
        return await _appearancePanel.SwitchColorModeAsync(colorMode);
    }
}

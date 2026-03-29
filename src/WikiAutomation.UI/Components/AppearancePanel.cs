using Microsoft.Playwright;
using WikiAutomation.Framework.Logging;
using WikiAutomation.UI.Models;

namespace WikiAutomation.UI.Components;

/// <summary>
/// Models the Wikipedia appearance controls used for theme switching.
/// </summary>
public sealed class AppearancePanel : BaseComponent
{
    /// <summary>
    /// Initializes the component with the current Playwright page.
    /// </summary>
    /// <param name="page">The active Playwright page.</param>
    public AppearancePanel(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Changes the color mode to Dark and returns the before/after state.
    /// </summary>
    /// <returns>The theme-switch result model.</returns>
    public async Task<ThemeSwitchResult> SwitchColorModeToDarkAsync()
    {
        return await SwitchColorModeAsync("Dark");
    }

    /// <summary>
    /// Changes the color mode to the requested option and returns the before/after state.
    /// </summary>
    /// <param name="colorMode">The visible color mode label to apply.</param>
    /// <returns>The theme-switch result model.</returns>
    public async Task<ThemeSwitchResult> SwitchColorModeAsync(string colorMode)
    {
        FrameworkLogger.Info($"Switching Wikipedia appearance setting to {colorMode}.");
        var beforeBackground = await GetBodyBackgroundColorAsync();

        if (!await IsVisibleAsync())
        {
            await OpenAsync();
        }

        var colorOption = Css($"label:has-text('{colorMode}'), button:has-text('{colorMode}')");
        if (!await Ui.IsVisibleAsync(colorOption))
        {
            throw new InvalidOperationException($"Could not find the '{colorMode}' color mode option in the Wikipedia appearance panel.");
        }

        await Ui.ClickAsync(colorOption);

        await Ui.WaitForFunctionAsync(
            @"(beforeBackground) => {
                const htmlClass = document.documentElement.className || '';
                const currentBackground = getComputedStyle(document.body).backgroundColor;
                return htmlClass.includes('skin-theme-clientpref-night') || currentBackground !== beforeBackground;
            }",
            beforeBackground);

        var htmlClass = await Ui.EvaluateAsync<string>("() => document.documentElement.className");
        var afterBackground = await GetBodyBackgroundColorAsync();

        return new ThemeSwitchResult(beforeBackground, afterBackground, htmlClass);
    }

    /// <summary>
    /// Checks whether the appearance panel is visible.
    /// </summary>
    /// <returns><c>true</c> when the appearance panel is visible; otherwise <c>false</c>.</returns>
    public async Task<bool> IsVisibleAsync()
    {
        var appearancePanel = Css("text=Color (beta)");
        return await Ui.IsVisibleAsync(appearancePanel);
    }

    /// <summary>
    /// Opens the appearance panel using the first visible supported trigger.
    /// </summary>
    public async Task OpenAsync()
    {
        // Wikipedia renders this control differently across layouts, so a small fallback list is safer here.
        await Ui.ClickFirstVisibleAsync(
            Button("Appearance"),
            Label("Appearance"),
            Css("[aria-label='Appearance']"),
            Css("button:has-text('Appearance')"));
    }

    /// <summary>
    /// Reads the computed background color for the page body.
    /// </summary>
    /// <returns>The computed CSS background color.</returns>
    private Task<string> GetBodyBackgroundColorAsync()
    {
        return Ui.EvaluateAsync<string>("() => getComputedStyle(document.body).backgroundColor");
    }
}

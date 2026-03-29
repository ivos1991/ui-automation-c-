using Microsoft.Playwright;
using WikiAutomation.Framework.Config;
using WikiAutomation.UI.Components;

namespace WikiAutomation.UI.Pages;

/// <summary>
/// Provides shared Playwright page behavior for article page objects.
/// </summary>
public abstract class BasePage : BaseComponent
{
    /// <summary>
    /// Initializes the base page with the current Playwright page and typed settings.
    /// </summary>
    /// <param name="page">The current Playwright page.</param>
    /// <param name="settings">The typed runtime settings.</param>
    protected BasePage(IPage page, TestSettings settings)
        : base(page)
    {
        Settings = settings;
    }

    protected TestSettings Settings { get; }

    /// <summary>
    /// Navigates to the configured Wikipedia article URL.
    /// </summary>
    /// <returns>A task that completes when the initial document load finishes.</returns>
    public virtual Task NavigateAsync()
    {
        return Page.GotoAsync(Settings.ArticleUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
    }
}

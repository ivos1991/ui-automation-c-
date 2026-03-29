using Microsoft.Playwright;
using WikiAutomation.UI.Actions;

namespace WikiAutomation.UI.Components;

/// <summary>
/// Provides shared Playwright locator and interaction helpers for reusable page components.
/// </summary>
public abstract class BaseComponent
{
    /// <summary>
    /// Initializes the component with the current Playwright page.
    /// </summary>
    /// <param name="page">The active Playwright page.</param>
    protected BaseComponent(IPage page)
    {
        Page = page;
        Ui = new UiActions(page);
    }

    /// <summary>
    /// Gets the current Playwright page used by the component.
    /// </summary>
    protected IPage Page { get; }

    /// <summary>
    /// Gets the shared UI action wrapper used by the component.
    /// </summary>
    protected UiActions Ui { get; }

    /// <summary>
    /// Returns the first locator matching the provided CSS selector.
    /// </summary>
    /// <param name="selector">The CSS selector.</param>
    protected ILocator Css(string selector) => Page.Locator(selector).First;

    /// <summary>
    /// Returns a locator by ARIA button name.
    /// </summary>
    /// <param name="name">The accessible button name.</param>
    protected ILocator Button(string name) => Page.GetByRole(AriaRole.Button, new() { Name = name });

    /// <summary>
    /// Returns a locator by label text.
    /// </summary>
    /// <param name="label">The accessible label text.</param>
    protected ILocator Label(string label) => Page.GetByLabel(label);

    /// <summary>
    /// Waits until the current page reaches DOMContentLoaded.
    /// </summary>
    protected Task WaitForDomReadyAsync() => Ui.WaitForPageLoadAsync();
}

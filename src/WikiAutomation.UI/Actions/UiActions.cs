using Microsoft.Playwright;

namespace WikiAutomation.UI.Actions;

/// <summary>
/// Wraps low-level Playwright interactions so pages and components depend on a shared UI action layer.
/// </summary>
public sealed class UiActions
{
    private readonly IPage _page;

    /// <summary>
    /// Initializes the UI action wrapper for the current Playwright page.
    /// </summary>
    /// <param name="page">The active Playwright page.</param>
    public UiActions(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Waits for the current page to reach the requested load state.
    /// </summary>
    /// <param name="state">The target load state.</param>
    public Task WaitForPageLoadAsync(LoadState state = LoadState.DOMContentLoaded)
    {
        return _page.WaitForLoadStateAsync(state);
    }

    /// <summary>
    /// Waits until the provided locator becomes visible.
    /// </summary>
    /// <param name="locator">The locator to wait for.</param>
    public Task WaitForVisibleAsync(ILocator locator)
    {
        return locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
    }

    /// <summary>
    /// Returns whether the provided locator is currently visible.
    /// </summary>
    /// <param name="locator">The locator to evaluate.</param>
    public Task<bool> IsVisibleAsync(ILocator locator)
    {
        return locator.IsVisibleAsync();
    }

    /// <summary>
    /// Clicks the provided locator after waiting for it to become visible.
    /// </summary>
    /// <param name="locator">The locator to click.</param>
    public async Task ClickAsync(ILocator locator)
    {
        await WaitForVisibleAsync(locator);
        await locator.ClickAsync();
    }

    /// <summary>
    /// Clicks the first visible locator from the provided candidates.
    /// </summary>
    /// <param name="candidates">The locators to evaluate in order.</param>
    public async Task ClickFirstVisibleAsync(params ILocator[] candidates)
    {
        foreach (var candidate in candidates)
        {
            var matchCount = await candidate.CountAsync();
            for (var index = 0; index < matchCount; index += 1)
            {
                var match = candidate.Nth(index);
                if (await match.IsVisibleAsync())
                {
                    await match.ClickAsync();
                    return;
                }
            }
        }

        throw new InvalidOperationException("Could not find a visible locator candidate to click.");
    }

    /// <summary>
    /// Evaluates a JavaScript function against the current page.
    /// </summary>
    /// <typeparam name="T">The expected result type.</typeparam>
    /// <param name="script">The JavaScript function body.</param>
    /// <param name="arg">The optional argument passed into the script.</param>
    /// <returns>The evaluated result.</returns>
    public Task<T> EvaluateAsync<T>(string script, object? arg = null)
    {
        return _page.EvaluateAsync<T>(script, arg);
    }

    /// <summary>
    /// Waits until the supplied page-level JavaScript condition returns true.
    /// </summary>
    /// <param name="script">The JavaScript predicate to evaluate.</param>
    /// <param name="arg">The optional argument passed into the predicate.</param>
    public Task WaitForFunctionAsync(string script, object? arg = null)
    {
        return _page.WaitForFunctionAsync(script, arg);
    }
}

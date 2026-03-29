using Microsoft.Playwright;
using WikiAutomation.Framework.Logging;
using WikiAutomation.TestData.Constants;
using WikiAutomation.UI.Models;

namespace WikiAutomation.UI.Components;

/// <summary>
/// Models the Microsoft development tools navigation box used for Task 2.
/// </summary>
public sealed class DevelopmentToolsNavBox : BaseComponent
{
    /// <summary>
    /// Initializes the component with the active Playwright page.
    /// </summary>
    /// <param name="page">The current Playwright page.</param>
    public DevelopmentToolsNavBox(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Collects the leaf technology-link results from the testing/debugging part of the navbox.
    /// </summary>
    /// <returns>The evaluated link results for the targeted technology names.</returns>
    public async Task<TechnologyLinkCheckResult[]> ReadTestingAndDebuggingLinkResultsAsync()
    {
        FrameworkLogger.Info("Reading Microsoft development tools navigation-box link results.");

        var navboxLabel = WikipediaTargets.MicrosoftDevelopmentTools;
        var subsectionLabel = WikipediaTargets.TestingAndDebugging;

        var script = $$"""
            ([navboxLabel, subsectionLabel]) => {
              const heading = [...document.querySelectorAll('th, caption, div')]
                .find(node => (node.textContent || '').trim() === navboxLabel);

              if (!heading) {
                throw new Error("Microsoft development tools section was not found.");
              }

              const navbox = heading.closest('table');
              if (!navbox) {
                throw new Error("Microsoft development tools navbox was not found.");
              }

              const normalize = (text) => (text || '').replace(/\s+/g, ' ').trim();
              const sectionLabel = [...navbox.querySelectorAll('*')]
                .find(node => normalize(node.textContent) === subsectionLabel);
              const contentRoot = sectionLabel
                ? sectionLabel.closest('tr')?.querySelector('td') || navbox
                : navbox;

              return [...contentRoot.querySelectorAll('li')]
                .filter(item => !item.querySelector(':scope > ul, :scope > ol'))
                .map(item => {
                  const fullText = (item.textContent || '').replace(/\s+/g, ' ').trim();
                  const directLink = item.querySelector(':scope > a, :scope > span > a');
                  const linkText = directLink ? (directLink.textContent || '').replace(/\s+/g, ' ').trim() : '';

                  return {
                    name: fullText,
                    hasTextLink: Boolean(directLink && linkText && fullText.includes(linkText))
                  };
                })
                .filter(item => item.name.length > 0);
            }
            """;

        // Only leaf list items are validated so grouped parent items do not create false negatives.
        return await Ui.EvaluateAsync<TechnologyLinkCheckResult[]>(script, new object[] { navboxLabel, subsectionLabel })
            ?? Array.Empty<TechnologyLinkCheckResult>();
    }
}

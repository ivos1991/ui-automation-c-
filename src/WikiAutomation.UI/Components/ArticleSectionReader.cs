using Microsoft.Playwright;
using WikiAutomation.Framework.Logging;
using WikiAutomation.TestData.Constants;

namespace WikiAutomation.UI.Components;

/// <summary>
/// Reads text content from article sections in the live browser-rendered page.
/// </summary>
public sealed class ArticleSectionReader : BaseComponent
{
    /// <summary>
    /// Initializes the section reader with the active Playwright page.
    /// </summary>
    /// <param name="page">The current Playwright page.</param>
    public ArticleSectionReader(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Extracts the visible text from the Debugging features section.
    /// </summary>
    /// <returns>The visible section text captured from the UI.</returns>
    public async Task<string> ReadDebuggingFeaturesTextAsync()
    {
        FrameworkLogger.Info("Reading the visible Debugging features section from the article.");

        var stopHeadingsJson = System.Text.Json.JsonSerializer.Serialize(WikipediaTargets.DebuggingFeaturesStopHeadings);
        var sectionName = WikipediaTargets.DebuggingFeaturesSection;

        var script = $$"""
            ([sectionName, stopHeadings]) => {
              const normalize = (text) => (text || '').replace(/\s+/g, ' ').trim();
              const contentRoot = document.querySelector('#bodyContent') || document.body;
              const lines = (contentRoot.innerText || '')
                .split('\n')
                .map(line => normalize(line))
                .filter(line => line.length > 0);

              const matchingIndexes = lines
                .map((line, index) => ({ line, index }))
                .filter(item => item.line.includes(sectionName))
                .map(item => item.index);

              const startIndex = matchingIndexes.length > 0
                ? matchingIndexes[matchingIndexes.length - 1]
                : -1;

              if (startIndex < 0) {
                throw new Error("Debugging features heading was not found.");
              }

              const parts = [];

              for (let index = startIndex + 1; index < lines.length; index += 1) {
                const line = lines[index];
                if (stopHeadings.some(heading => line.startsWith(heading))) {
                  break;
                }

                if (line === '[edit]') {
                  continue;
                }

                parts.push(line);
              }

              return parts.join('\n');
            }
            """;

        // The visible article text is more stable here than Wikipedia's internal section markup.
        return await Ui.EvaluateAsync<string>(script, new object[] { sectionName, WikipediaTargets.DebuggingFeaturesStopHeadings });
    }
}

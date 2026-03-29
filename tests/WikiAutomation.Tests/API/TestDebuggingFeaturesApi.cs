using NUnit.Framework;
using Allure.NUnit.Attributes;
using WikiAutomation.Framework.Logging;
using WikiAutomation.Framework.Reporting;
using WikiAutomation.Framework.Utilities;
using static WikiAutomation.Tests.Assertions.AssertThat;
using WikiAutomation.Tests.Infrastructure;

namespace WikiAutomation.Tests.API;

[TestFixture]
[Category("api")]
[AllureParentSuite("Wikipedia Home Assignment")]
[AllureSuite("API")]
[AllureSubSuite("Wikipedia Article")]
[AllureFeature("MediaWiki Parse API")]
/// <summary>
/// Contains API-only coverage for the assignment's MediaWiki Parse API behavior.
/// </summary>
public sealed class TestDebuggingFeaturesApi : TestBase
{
    /// <summary>
    /// Verifies that the MediaWiki Parse API returns usable readable content for the Debugging features section.
    /// </summary>
    [Test]
    [AllureStory("Task 1 - API extraction")]
    public async Task DebuggingFeatures_ParseApi_ExtractionExpectsReadableSectionText()
    {
        FrameworkLogger.Info("Running API-only validation for the Debugging features section.");

        var rawText = await UseApiServiceAsync(apiService =>
            apiService.GetDebuggingFeaturesTextAsync(Settings.ArticleTitle));
        var normalizedLines = TextNormalizer.GetNormalizedLines(rawText);
        var introLine = normalizedLines.FirstOrDefault();
        var featureItems = normalizedLines.Skip(1).ToArray();
        var featureText = string.Join(Environment.NewLine, featureItems);
        var normalizedFeatureText = TextNormalizer.Normalize(featureText);
        var uniqueWords = TextNormalizer.GetNormalizedWordSet(featureText);

        AllureAttachmentService.AttachJsonObject(
            "debugging-features-api-summary",
            new
            {
                introLine,
                featureItems,
                featureCount = featureItems.Length,
                uniqueWordCount = uniqueWords.Count,
                uniqueWords,
                normalizedText = normalizedFeatureText
            });

        That(
            normalizedFeatureText,
            "the MediaWiki Parse API should return content for the Debugging features section")
            .IsNotNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that requesting a non-existent article causes the API service to fail instead of returning misleading section content.
    /// </summary>
    [Test]
    [AllureStory("Task 1 - API extraction negative path")]
    public async Task DebuggingFeatures_ParseApi_RequestForMissingArticleExpectsFailure()
    {
        FrameworkLogger.Info("Running negative API validation for a missing Wikipedia article.");

        var missingArticleTitle = $"{Settings.ArticleTitle}-Missing-{Guid.NewGuid():N}";
        try
        {
            await UseApiServiceAsync(apiService => apiService.GetDebuggingFeaturesTextAsync(missingArticleTitle));
            Assert.Fail("A missing article should not produce a valid Debugging features section through the MediaWiki Parse API.");
        }
        catch (Exception exception)
        {
            AllureAttachmentService.AttachJsonObject(
                "debugging-features-api-missing-article",
                new
                {
                    requestedArticleTitle = missingArticleTitle,
                    exceptionType = exception.GetType().FullName,
                    exception.Message
                });
        }
    }
}

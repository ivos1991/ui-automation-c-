using Allure.NUnit.Attributes;
using NUnit.Framework;
using WikiAutomation.Framework.Logging;
using WikiAutomation.Framework.Reporting;
using WikiAutomation.Framework.Utilities;
using static WikiAutomation.Tests.Assertions.AssertThat;
using WikiAutomation.Tests.Infrastructure;

namespace WikiAutomation.Tests.E2E;

[TestFixture]
[Category("e2e")]
[AllureParentSuite("Wikipedia Home Assignment")]
[AllureSuite("E2E")]
[AllureSubSuite("Wikipedia Article")]
[AllureFeature("UI and API parity")]
/// <summary>
/// Contains the cross-layer comparison that validates UI and API alignment for the assignment.
/// </summary>
public sealed class TestDebuggingFeaturesVerticalSlice : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that the Debugging features section yields the same unique-word count through the UI path and the API path.
    /// </summary>
    [Test]
    [AllureStory("Task 1 - UI/API parity")]
    public async Task DebuggingFeatures_UiAndApiComparisonExpectsMatchingUniqueWordCount()
    {
        FrameworkLogger.Info("Running UI/API parity validation for the Debugging features section.");

        var articlePage = await OpenArticleAsync();
        var uiRawText = await articlePage.GetDebuggingFeaturesTextAsync();
        var apiRawText = await UseApiServiceAsync(apiService =>
            apiService.GetDebuggingFeaturesTextAsync(Settings.ArticleTitle));

        var uiLines = TextNormalizer.GetNormalizedLines(uiRawText);
        var apiLines = TextNormalizer.GetNormalizedLines(apiRawText);
        var uiFeatureItems = uiLines.Skip(1).ToArray();
        var apiFeatureItems = apiLines.Skip(1).ToArray();
        var uiFeatureText = string.Join(Environment.NewLine, uiFeatureItems);
        var apiFeatureText = string.Join(Environment.NewLine, apiFeatureItems);
        var normalizedUiFeatureText = TextNormalizer.Normalize(uiFeatureText);
        var normalizedApiFeatureText = TextNormalizer.Normalize(apiFeatureText);
        var uiWordSet = TextNormalizer.GetNormalizedWordSet(uiFeatureText);
        var apiWordSet = TextNormalizer.GetNormalizedWordSet(apiFeatureText);
        var missingInUi = apiFeatureItems.Except(uiFeatureItems, StringComparer.Ordinal).ToArray();
        var missingInApi = uiFeatureItems.Except(apiFeatureItems, StringComparer.Ordinal).ToArray();

        AllureAttachmentService.AttachJsonObject(
            "debugging-features-comparison",
            new
            {
                uiIntroLine = uiLines.FirstOrDefault(),
                apiIntroLine = apiLines.FirstOrDefault(),
                uiFeatureItems,
                apiFeatureItems,
                uiFeatureCount = uiFeatureItems.Length,
                apiFeatureCount = apiFeatureItems.Length,
                uiUniqueWordCount = uiWordSet.Count,
                apiUniqueWordCount = apiWordSet.Count,
                missingInUi,
                missingInApi,
                featuresMatch = missingInUi.Length == 0 && missingInApi.Length == 0,
                uniqueWordCountsMatch = uiWordSet.Count == apiWordSet.Count,
                normalizedUiText = normalizedUiFeatureText,
                normalizedApiText = normalizedApiFeatureText
            });

        That(
            UniqueWordCounter.CountUniqueWords(uiFeatureText),
            "the normalized UI and API versions of the Debugging features section should match")
            .IsEqualTo(UniqueWordCounter.CountUniqueWords(apiFeatureText));
    }

    /// <summary>
    /// Verifies that the parity comparison detects a unique-word mismatch when one side is intentionally tampered with.
    /// </summary>
    [Test]
    [AllureStory("Task 1 - UI/API parity negative path")]
    public async Task DebuggingFeatures_UiAndApiComparisonWithTamperedInputExpectsDifferentUniqueWordCount()
    {
        FrameworkLogger.Info("Running negative UI/API parity validation with intentionally tampered API content.");

        var articlePage = await OpenArticleAsync();
        var uiRawText = await articlePage.GetDebuggingFeaturesTextAsync();
        var apiRawText = await UseApiServiceAsync(apiService =>
            apiService.GetDebuggingFeaturesTextAsync(Settings.ArticleTitle));
        var tamperedApiText = $"{apiRawText}{Environment.NewLine}interviewonlyuniqueword";

        AllureAttachmentService.AttachText("debugging-features-api-tampered", tamperedApiText);
        FrameworkLogger.Info("Attached tampered API content for the negative parity scenario.");

        That(
            UniqueWordCounter.CountUniqueWords(uiRawText),
            "adding a synthetic unique word should change the parity count")
            .IsNotEqualTo(UniqueWordCounter.CountUniqueWords(tamperedApiText));
    }

}

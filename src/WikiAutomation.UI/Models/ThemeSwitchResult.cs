namespace WikiAutomation.UI.Models;

public sealed record ThemeSwitchResult(string BeforeBackground, string AfterBackground, string HtmlClassName)
{
    public bool IsDarkThemeApplied =>
        HtmlClassName.Contains("skin-theme-clientpref-night", StringComparison.OrdinalIgnoreCase)
        || !string.Equals(BeforeBackground, AfterBackground, StringComparison.Ordinal);
}

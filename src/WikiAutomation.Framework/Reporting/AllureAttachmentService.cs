using System.Text;
using Allure.Net.Commons;
using WikiAutomation.Framework.Serialization;

namespace WikiAutomation.Framework.Reporting;

/// <summary>
/// Wraps the low-level Allure attachment API in a small reusable helper layer.
/// </summary>
public static class AllureAttachmentService
{
    /// <summary>
    /// Attaches plain text content to the current Allure test entry.
    /// </summary>
    /// <param name="name">The attachment display name.</param>
    /// <param name="content">The plain text content.</param>
    public static void AttachText(string name, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        AllureApi.AddAttachment(name, "text/plain", bytes, ".txt");
    }

    /// <summary>
    /// Attaches JSON content to the current Allure test entry.
    /// </summary>
    /// <param name="name">The attachment display name.</param>
    /// <param name="content">The JSON content.</param>
    public static void AttachJson(string name, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        AllureApi.AddAttachment(name, "application/json", bytes, ".json");
    }

    /// <summary>
    /// Serializes an object and attaches it as JSON to the current Allure test entry.
    /// </summary>
    /// <param name="name">The attachment display name.</param>
    /// <param name="value">The object to serialize.</param>
    public static void AttachJsonObject(string name, object value)
    {
        AttachJson(name, JsonDefaults.SerializeCompact(value));
    }

    /// <summary>
    /// Attaches a file to Allure only when the file exists on disk.
    /// </summary>
    /// <param name="name">The attachment display name.</param>
    /// <param name="filePath">The full path to the file.</param>
    /// <param name="contentType">The MIME type used by Allure.</param>
    public static void AttachFileIfExists(string name, string filePath, string contentType)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        AllureApi.AddAttachment(name, contentType, filePath);
    }
}

using System.Text.Json;

namespace WikiAutomation.Framework.Serialization;

/// <summary>
/// Stores the project's shared JSON serialization settings.
/// </summary>
public static class JsonDefaults
{
    /// <summary>
    /// Compact JSON options for attachments and transport payloads.
    /// </summary>
    public static readonly JsonSerializerOptions Compact = new()
    {
        WriteIndented = false
    };

    /// <summary>
    /// Indented JSON options for console logging and interview-friendly output.
    /// </summary>
    public static readonly JsonSerializerOptions Indented = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Serializes a value with the project's compact JSON settings.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized JSON string.</returns>
    public static string SerializeCompact(object value)
    {
        return JsonSerializer.Serialize(value, Compact);
    }

    /// <summary>
    /// Serializes a value with the project's indented JSON settings.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized JSON string.</returns>
    public static string SerializeIndented(object value)
    {
        return JsonSerializer.Serialize(value, Indented);
    }
}

using FluentAssertions;
using System.Collections;

namespace WikiAutomation.Tests.Assertions;

/// <summary>
/// Provides a small assertion wrapper so the C# tests read closer to the Python project's assert_that style.
/// </summary>
public static class AssertThat
{
    /// <summary>
    /// Creates a fluent assertion wrapper for the supplied value.
    /// </summary>
    /// <typeparam name="T">The value type under test.</typeparam>
    /// <param name="actual">The actual value produced by the system under test.</param>
    /// <param name="because">An optional reason shown when the assertion fails.</param>
    /// <returns>A wrapper exposing readable assertion methods.</returns>
    public static AssertionSubject<T> That<T>(T actual, string? because = null)
    {
        return new AssertionSubject<T>(actual, because);
    }

    /// <summary>
    /// Verifies that an asynchronous action throws the expected exception type.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous action under test.</param>
    /// <param name="because">An optional reason shown when the assertion fails.</param>
    public static async Task ThrowsAsync<TException>(Func<Task> action, string? because = null)
        where TException : Exception
    {
        await action.Should().ThrowAsync<TException>(because);
    }
}

/// <summary>
/// Wraps a single assertion subject and exposes a compact set of interview-friendly assertion methods.
/// </summary>
/// <typeparam name="T">The value type under test.</typeparam>
public sealed class AssertionSubject<T>
{
    private readonly T _actual;
    private readonly string? _because;

    /// <summary>
    /// Initializes a new assertion subject.
    /// </summary>
    /// <param name="actual">The actual value produced by the system under test.</param>
    /// <param name="because">An optional reason shown when the assertion fails.</param>
    public AssertionSubject(T actual, string? because)
    {
        _actual = actual;
        _because = because;
    }

    /// <summary>
    /// Verifies that the subject equals the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    public void IsEqualTo(T expected)
    {
        _actual.Should().Be(expected, _because);
    }

    /// <summary>
    /// Verifies that the subject does not equal the unexpected value.
    /// </summary>
    /// <param name="unexpected">The value that should not match the actual value.</param>
    public void IsNotEqualTo(T unexpected)
    {
        _actual.Should().NotBe(unexpected, _because);
    }

    /// <summary>
    /// Verifies that the subject is true.
    /// </summary>
    public void IsTrue()
    {
        _actual.Should().BeOfType<bool>(_because);
        ((bool)(object)_actual!).Should().BeTrue(_because);
    }

    /// <summary>
    /// Verifies that the subject is false.
    /// </summary>
    public void IsFalse()
    {
        _actual.Should().BeOfType<bool>(_because);
        ((bool)(object)_actual!).Should().BeFalse(_because);
    }

    /// <summary>
    /// Verifies that the subject is empty.
    /// </summary>
    public void IsEmpty()
    {
        var items = ToObjectEnumerable();
        items.Should().BeEmpty(_because);
    }

    /// <summary>
    /// Verifies that the subject is not null, empty, or whitespace.
    /// </summary>
    public void IsNotNullOrWhiteSpace()
    {
        _actual.Should().BeAssignableTo<string>(_because);
        ((string)(object)_actual!).Should().NotBeNullOrWhiteSpace(_because);
    }

    /// <summary>
    /// Verifies that the subject has the expected item count.
    /// </summary>
    /// <param name="expectedCount">The expected number of items.</param>
    public void HasCount(int expectedCount)
    {
        var items = ToObjectEnumerable();
        items.Should().HaveCount(expectedCount, _because);
    }

    private IEnumerable<object?> ToObjectEnumerable()
    {
        _actual.Should().BeAssignableTo<IEnumerable>(_because);
        return ((IEnumerable)(object)_actual!).Cast<object?>();
    }
}

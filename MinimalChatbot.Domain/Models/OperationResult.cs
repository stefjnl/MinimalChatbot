namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents the result of an operation with success status and optional error information.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the result value when the operation is successful.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Gets the error message when the operation fails.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    /// <summary>
    /// Creates a failed operation result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

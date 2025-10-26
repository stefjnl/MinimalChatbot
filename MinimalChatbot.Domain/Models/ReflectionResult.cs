namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents the result of reflecting on a file operation to determine safety and approval requirements.
/// </summary>
public class ReflectionResult
{
    /// <summary>
    /// Gets the approval status for the operation.
    /// </summary>
    public ReflectionApprovalStatus ApprovalStatus { get; init; }

    /// <summary>
    /// Gets the reasoning for the approval decision.
    /// </summary>
    public string Reasoning { get; init; } = string.Empty;

    /// <summary>
    /// Gets additional context or suggestions for the user.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation is considered safe.
    /// </summary>
    public bool IsSafe => ApprovalStatus == ReflectionApprovalStatus.Approved;

    /// <summary>
    /// Creates a reflection result indicating the operation should be approved.
    /// </summary>
    /// <param name="reasoning">The reasoning for approval.</param>
    /// <param name="context">Additional context (optional).</param>
    /// <returns>An approved reflection result.</returns>
    public static ReflectionResult Approved(string reasoning, string? context = null) => new()
    {
        ApprovalStatus = ReflectionApprovalStatus.Approved,
        Reasoning = reasoning,
        Context = context
    };

    /// <summary>
    /// Creates a reflection result indicating the operation should be rejected.
    /// </summary>
    /// <param name="reasoning">The reasoning for rejection.</param>
    /// <param name="context">Additional context (optional).</param>
    /// <returns>A rejected reflection result.</returns>
    public static ReflectionResult Rejected(string reasoning, string? context = null) => new()
    {
        ApprovalStatus = ReflectionApprovalStatus.Rejected,
        Reasoning = reasoning,
        Context = context
    };

    /// <summary>
    /// Creates a reflection result indicating the operation requires user confirmation.
    /// </summary>
    /// <param name="reasoning">The reasoning for requiring confirmation.</param>
    /// <param name="context">Additional context (optional).</param>
    /// <returns>A confirmation-required reflection result.</returns>
    public static ReflectionResult RequiresConfirmation(string reasoning, string? context = null) => new()
    {
        ApprovalStatus = ReflectionApprovalStatus.RequiresConfirmation,
        Reasoning = reasoning,
        Context = context
    };
}

/// <summary>
/// Represents the approval status for a file operation after reflection.
/// </summary>
public enum ReflectionApprovalStatus
{
    /// <summary>
    /// The operation is approved and can proceed.
    /// </summary>
    Approved,

    /// <summary>
    /// The operation is rejected and should not proceed.
    /// </summary>
    Rejected,

    /// <summary>
    /// The operation requires user confirmation before proceeding.
    /// </summary>
    RequiresConfirmation
}

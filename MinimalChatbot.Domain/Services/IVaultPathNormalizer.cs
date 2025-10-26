namespace MinimalChatbot.Domain.Services;

/// <summary>
/// Interface for normalizing vault paths to ensure consistent handling across the application.
/// </summary>
public interface IVaultPathNormalizer
{
    /// <summary>
    /// Normalizes a vault path to ensure consistent format.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    string NormalizePath(string path);
}

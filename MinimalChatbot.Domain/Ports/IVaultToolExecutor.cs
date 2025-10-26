namespace MinimalChatbot.Domain.Ports
{
    using System.Threading;
    using System.Threading.Tasks;
    using MinimalChatbot.Domain.Models;

    /// <summary>
    /// Abstracts vault operations (append/patch/delete/create).
    /// Implementations execute file mutations against a backing store (e.g., disk, remote vault).
    /// </summary>
    public interface IVaultToolExecutor
    {
        /// <summary>
        /// Appends content to an existing file at the specified path.
        /// </summary>
        /// <param name="filePath">The normalized vault file path.</param>
        /// <param name="content">The content to append.</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="OperationResult{String}"/> describing the outcome.</returns>
        Task<OperationResult<string>> AppendAsync(string filePath, string content, CancellationToken ct = default);

        /// <summary>
        /// Applies a patch operation to the file at the specified path.
        /// </summary>
        /// <param name="filePath">The normalized vault file path.</param>
        /// <param name="content">The patch content or payload.</param>
        /// <param name="operation">The patch operation mode (e.g., "replace", "insert", "remove").</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="OperationResult{String}"/> describing the outcome.</returns>
        Task<OperationResult<string>> PatchAsync(string filePath, string content, string operation, CancellationToken ct = default);

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="filePath">The normalized vault file path.</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="OperationResult{Boolean}"/> describing the outcome.</returns>
        Task<OperationResult<bool>> DeleteAsync(string filePath, CancellationToken ct = default);

        /// <summary>
        /// Creates a new file with the given content at the specified path.
        /// </summary>
        /// <param name="filePath">The normalized vault file path.</param>
        /// <param name="content">The content to write to the new file.</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="OperationResult{String}"/> describing the outcome.</returns>
        Task<OperationResult<string>> CreateAsync(string filePath, string content, CancellationToken ct = default);
    }
}

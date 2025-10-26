using MinimalChatbot.Domain.Models;
using MinimalChatbot.Domain.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace MinimalChatbot.Infrastructure.Vault;

/// <summary>
/// A null implementation of IVaultToolExecutor that returns failure results when MCP is unavailable.
/// This allows the application to continue running without MCP functionality.
/// </summary>
public class NullVaultToolExecutor : IVaultToolExecutor
{
    /// <inheritdoc/>
    public Task<OperationResult<string>> AppendAsync(string filePath, string content, CancellationToken ct = default)
    {
        return Task.FromResult(OperationResult<string>.Failure("MCP server is not available. Vault operations are disabled."));
    }

    /// <inheritdoc/>
    public Task<OperationResult<string>> PatchAsync(string filePath, string content, string operation, CancellationToken ct = default)
    {
        return Task.FromResult(OperationResult<string>.Failure("MCP server is not available. Vault operations are disabled."));
    }

    /// <inheritdoc/>
    public Task<OperationResult<bool>> DeleteAsync(string filePath, CancellationToken ct = default)
    {
        return Task.FromResult(OperationResult<bool>.Failure("MCP server is not available. Vault operations are disabled."));
    }

    /// <inheritdoc/>
    public Task<OperationResult<string>> CreateAsync(string filePath, string content, CancellationToken ct = default)
    {
        return Task.FromResult(OperationResult<string>.Failure("MCP server is not available. Vault operations are disabled."));
    }
}

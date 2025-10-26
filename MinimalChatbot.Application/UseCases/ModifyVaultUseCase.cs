using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MinimalChatbot.Application.Services;
using MinimalChatbot.Domain.Models;
using MinimalChatbot.Domain.Ports;
using MinimalChatbot.Domain.Services;

namespace MinimalChatbot.Application.UseCases;

/// <summary>
/// Use case for modifying vault files through various operations.
/// </summary>
public class ModifyVaultUseCase
{
    private static readonly IReadOnlyDictionary<string, string> PatchOperationMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["modify"] = "replace",
        ["replace"] = "replace",
        ["write"] = "replace",
        ["update"] = "replace",
        ["overwrite"] = "replace",
        ["patch"] = "patch",
        ["insert"] = "insert",
        ["prepend"] = "prepend"
    };

    private readonly IVaultToolExecutor _executor;
    private readonly IVaultPathNormalizer _normalizer;
    private readonly IVaultIndexCache? _vaultIndexCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyVaultUseCase"/> class.
    /// </summary>
    /// <param name="executor">Executor for vault tool operations.</param>
    /// <param name="normalizer">Normalizer for vault paths.</param>
    public ModifyVaultUseCase(IVaultToolExecutor executor, IVaultPathNormalizer normalizer, IVaultIndexCache? vaultIndexCache = null)
    {
        _executor = executor;
        _normalizer = normalizer;
        _vaultIndexCache = vaultIndexCache;
    }

    /// <summary>
    /// Executes the modify vault use case.
    /// </summary>
    /// <param name="operation">The operation to perform (append, modify, patch, write, delete, create).</param>
    /// <param name="filePath">The file path to operate on.</param>
    /// <param name="content">The content for the operation.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<OperationResult<object>> ExecuteAsync(string operation, string filePath, string content, CancellationToken ct = default)
    {
        var normalizedPath = _normalizer.NormalizePath(filePath);

        var lowerOperation = operation.ToLowerInvariant();
        OperationResult<object> result;

        if (lowerOperation.Equals("append", StringComparison.Ordinal))
        {
            var appendResult = await _executor.AppendAsync(normalizedPath, content, ct).ConfigureAwait(false);
            result = appendResult.IsSuccess ? OperationResult<object>.Success(appendResult.Value!) : OperationResult<object>.Failure(appendResult.Error!);
        }
        else if (lowerOperation.Equals("delete", StringComparison.Ordinal))
        {
            var deleteResult = await _executor.DeleteAsync(normalizedPath, ct).ConfigureAwait(false);
            result = deleteResult.IsSuccess ? OperationResult<object>.Success(deleteResult.Value) : OperationResult<object>.Failure(deleteResult.Error!);
        }
        else if (lowerOperation.Equals("create", StringComparison.Ordinal))
        {
            var createResult = await _executor.CreateAsync(normalizedPath, content, ct).ConfigureAwait(false);
            result = createResult.IsSuccess ? OperationResult<object>.Success(createResult.Value!) : OperationResult<object>.Failure(createResult.Error!);
        }
        else if (PatchOperationMap.TryGetValue(lowerOperation, out var patchMode))
        {
            var patchResult = await _executor.PatchAsync(normalizedPath, content, patchMode, ct).ConfigureAwait(false);
            result = patchResult.IsSuccess ? OperationResult<object>.Success(patchResult.Value!) : OperationResult<object>.Failure(patchResult.Error!);
        }
        else
        {
            return OperationResult<object>.Failure($"Unsupported operation: {operation}");
        }

        if (result.IsSuccess)
        {
            _vaultIndexCache?.InvalidateAll();
        }

        return result;
    }
}

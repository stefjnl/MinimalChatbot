namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents a file operation that can be performed on the vault.
/// </summary>
public class FileOperation
{
    /// <summary>
    /// Gets the type of file operation.
    /// </summary>
    public FileOperationType OperationType { get; init; }

    /// <summary>
    /// Gets the target file path.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content for write operations (optional).
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the source path for move operations (optional).
    /// </summary>
    public string? SourcePath { get; init; }

    /// <summary>
    /// Gets additional metadata about the operation.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();

    /// <summary>
    /// Creates a read file operation.
    /// </summary>
    /// <param name="filePath">The file path to read.</param>
    /// <returns>A read file operation.</returns>
    public static FileOperation Read(string filePath) => new()
    {
        OperationType = FileOperationType.Read,
        FilePath = filePath
    };

    /// <summary>
    /// Creates a write file operation.
    /// </summary>
    /// <param name="filePath">The file path to write.</param>
    /// <param name="content">The content to write.</param>
    /// <returns>A write file operation.</returns>
    public static FileOperation Write(string filePath, string content) => new()
    {
        OperationType = FileOperationType.Write,
        FilePath = filePath,
        Content = content
    };

    /// <summary>
    /// Creates a delete file operation.
    /// </summary>
    /// <param name="filePath">The file path to delete.</param>
    /// <returns>A delete file operation.</returns>
    public static FileOperation Delete(string filePath) => new()
    {
        OperationType = FileOperationType.Delete,
        FilePath = filePath
    };

    /// <summary>
    /// Creates a move file operation.
    /// </summary>
    /// <param name="sourcePath">The source file path.</param>
    /// <param name="targetPath">The target file path.</param>
    /// <returns>A move file operation.</returns>
    public static FileOperation Move(string sourcePath, string targetPath) => new()
    {
        OperationType = FileOperationType.Move,
        FilePath = targetPath,
        SourcePath = sourcePath
    };

    /// <summary>
    /// Creates a list directory operation.
    /// </summary>
    /// <param name="directoryPath">The directory path to list.</param>
    /// <returns>A list directory operation.</returns>
    public static FileOperation ListDirectory(string directoryPath) => new()
    {
        OperationType = FileOperationType.ListDirectory,
        FilePath = directoryPath
    };
}

/// <summary>
/// Represents the type of file operation.
/// </summary>
public enum FileOperationType
{
    /// <summary>
    /// Read file contents.
    /// </summary>
    Read,

    /// <summary>
    /// Write content to a file.
    /// </summary>
    Write,

    /// <summary>
    /// Delete a file.
    /// </summary>
    Delete,

    /// <summary>
    /// Move or rename a file.
    /// </summary>
    Move,

    /// <summary>
    /// List directory contents.
    /// </summary>
    ListDirectory,

    /// <summary>
    /// Create a directory.
    /// </summary>
    CreateDirectory,

    /// <summary>
    /// Search for files.
    /// </summary>
    Search
}

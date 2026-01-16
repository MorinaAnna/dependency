using System;
using System.Collections.Generic;
using System.IO;
using DataReceiving;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;

namespace TextFileReceiver;

/// <summary>
/// The data receiver from text file.
/// </summary>
public class TextStreamReceiver : IDataReceiver
{
    private readonly string path;
    private readonly ILogger<TextStreamReceiver>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextStreamReceiver"/> class.
    /// </summary>
    /// <param name="path">The path to text file.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Throw if text reader is null or empty.</exception>
    public TextStreamReceiver(string? path, ILogger<TextStreamReceiver>? logger = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        this.path = path;
        this.logger = logger;
    }

    /// <summary>
    /// Receives lines from text reader.
    /// </summary>
    /// <returns>Strings.</returns>
    public IEnumerable<string> Receive()
    {
        try
        {
            return File.ReadLines(this.path);
        }
        catch (ArgumentNullException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (ArgumentException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (UnauthorizedAccessException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (DirectoryNotFoundException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (FileNotFoundException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (IOException ex)
        {
            // сюда попадут почти все проблемы чтения: “file in use”, ошибки диска и т.п.
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
        catch (ObjectDisposedException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            return Array.Empty<string>();
        }
    }
}

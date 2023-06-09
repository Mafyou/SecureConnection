﻿namespace SecureConnection.StructuredConcurrency;

/// <summary>
/// Extension methods for tasks.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Ignores cancellation of the source task.
    /// </summary>
    public static async Task IgnoreCancellation(this Task task)
    {
        task = task ?? throw new ArgumentNullException(nameof(task));
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }
}

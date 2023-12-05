using System.Threading.Tasks;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility methods related to <see langword="async"/> processing.</summary>
[PublicAPI]
public static class AsyncUtils {

  /// <summary>Synchronously awaits a task.</summary>
  /// <param name="task">The task to await.</param>
  public static void ResultOf(Task task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

  /// <summary>Synchronously awaits a task.</summary>
  /// <param name="task">The task to await.</param>
  /// <typeparam name="T">The return type for the task.</typeparam>
  /// <returns>The value returned by the task when it completes.</returns>
  public static T ResultOf<T>(Task<T> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

  /// <summary>Synchronously awaits a task.</summary>
  /// <param name="task">The task to await.</param>
  public static void ResultOf(ValueTask task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

  /// <summary>Synchronously awaits a task.</summary>
  /// <param name="task">The task to await.</param>
  /// <typeparam name="T">The return type for the task.</typeparam>
  /// <returns>The value returned by the task when it completes.</returns>
  public static T ResultOf<T>(ValueTask<T> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

}

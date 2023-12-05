using System;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility class for working with Unix time values (seconds since 1970-01-01T00:00:00).</summary>
[PublicAPI]
[Obsolete($"Use {nameof(DateTimeOffset)} instead.")]
public static class UnixTime {

  /// <summary>The epoch for Unix time values (1970-01-01T00:00:00Z).</summary>
  [Obsolete($"Use {nameof(DateTimeOffset)}.{nameof(DateTimeOffset.UnixEpoch)} instead.")]
  public static readonly DateTimeOffset Epoch = DateTimeOffset.UnixEpoch;

  /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
  /// <param name="value">The date/time to convert to a Unix time value.</param>
  /// <returns>The corresponding Unix time value.</returns>
  [Obsolete($"Use {nameof(DateTimeOffset)}.{nameof(DateTimeOffset.ToUnixTimeSeconds)} instead.")]
  public static long Convert(DateTimeOffset value) => value.ToUnixTimeSeconds();

  /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
  /// <param name="value">The date/time to convert to a Unix time value.</param>
  /// <returns>The corresponding Unix time value.</returns>
  [Obsolete($"Use {nameof(DateTimeOffset)}.{nameof(DateTimeOffset.ToUnixTimeSeconds)} instead.")]
  public static long? Convert(DateTimeOffset? value) => value?.ToUnixTimeSeconds();

  /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
  /// <param name="value">The Unix time value to convert to a date/time.</param>
  /// <returns>The corresponding date/time.</returns>
  [Obsolete($"Use {nameof(DateTimeOffset)}.{nameof(DateTimeOffset.FromUnixTimeSeconds)} instead.")]
  public static DateTimeOffset Convert(long value) => DateTimeOffset.FromUnixTimeSeconds(value);

  /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
  /// <param name="value">The Unix time value to convert to a date/time.</param>
  /// <returns>The corresponding date/time.</returns>
  [Obsolete($"Use {nameof(DateTimeOffset)}.{nameof(DateTimeOffset.FromUnixTimeSeconds)} instead.")]
  public static DateTimeOffset? Convert(long? value) => value.HasValue ? DateTimeOffset.FromUnixTimeSeconds(value.Value) : null;

}

using System;
using System.Text;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility methods related to text processing.</summary>
[PublicAPI]
public static class TextUtils {

  /// <summary>Decodes all the bytes in the specified span as a string, using the UTF-8 character set.</summary>
  /// <param name="bytes">A read-only byte span to decode to a Unicode string.</param>
  /// <returns>A string that contains the decoded bytes from the provided read-only span.</returns>
  [Obsolete("Call Encoding.UTF8.GetString() instead.")]
  public static string DecodeUtf8(ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes);

  /// <summary>Formats a string, including extra handling if it's multiline.</summary>
  /// <param name="text">The string to format. Trailing line breaks are discarded.</param>
  /// <param name="prefix">The prefix to use.</param>
  /// <param name="suffix">The suffix to use.</param>
  /// <param name="separator">The separator to use between lines.</param>
  /// <returns>The formatted version of <paramref name="text"/>.</returns>
  public static string FormatMultiLine(string text, string prefix = "<<", string suffix = ">>", string separator = "\n  ") {
    char[] newlines = { '\r', '\n' };
    text = text.Replace("\r\n", "\n").TrimEnd(newlines);
    var lines = text.Split(newlines);
    return lines.Length switch {
      0 => prefix + suffix,
      1 => prefix + lines[0] + suffix,
      _ => prefix + separator + string.Join(separator, lines) + "\n" + suffix
    };
  }

}

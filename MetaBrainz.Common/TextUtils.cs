using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility methods related to text processing.</summary>
[PublicAPI]
public static class TextUtils {

  private static readonly char[] NewLineCharacters = [
    '\r', '\n'
  ];

  /// <summary>Formats a string, including extra handling if it's multiline.</summary>
  /// <param name="text">The string to format. Trailing line breaks are discarded.</param>
  /// <param name="prefix">The prefix to use.</param>
  /// <param name="suffix">The suffix to use.</param>
  /// <param name="separator">The separator to use between lines.</param>
  /// <returns>The formatted version of <paramref name="text"/>.</returns>
  public static string FormatMultiLine(string text, string prefix = "<<", string suffix = ">>", string separator = "\n  ") {
    text = text.TrimEnd(TextUtils.NewLineCharacters).Replace("\r\n", "\n");
    var lines = text.Split(TextUtils.NewLineCharacters);
    return lines.Length switch {
      0 => prefix + suffix,
      1 => prefix + lines[0] + suffix,
      _ => prefix + separator + string.Join(separator, lines) + "\n" + suffix
    };
  }

}

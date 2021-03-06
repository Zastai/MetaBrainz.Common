using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility methods related to HTTP processing.</summary>
[PublicAPI]
public static class HttpUtils {

  /// <summary>Create a user agent header containing the name and version of the assembly containing a particular type.</summary>
  /// <typeparam name="T">The type to use to determine the assembly name and version.</typeparam>
  /// <returns>
  /// A user agent header containing the name and version of the assembly containing <typeparamref name="T"/>. If the assembly name
  /// cannot be obtained, <see cref="UnknownAssemblyName"/> will be used instead.
  /// </returns>
  public static ProductInfoHeaderValue CreateUserAgentHeader<T>() {
    var an = typeof(T).Assembly.GetName();
    return new ProductInfoHeaderValue(an.Name ?? HttpUtils.UnknownAssemblyName, an.Version?.ToString());
  }

  /// <summary>Gets the content encoding based on content headers.</summary>
  /// <param name="contentHeaders">The headers to get the information from.</param>
  /// <returns>
  /// The content encoding extracted from the headers, or "utf-8" as fallback if no explicit specification was found.
  /// </returns>
  public static string GetContentEncoding(HttpContentHeaders contentHeaders) {
    var characterSet = contentHeaders.ContentEncoding.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(characterSet)) {
      // Fall back on the charset portion of the content type.
      // FIXME: Should this check the media type?
      characterSet = contentHeaders.ContentType?.CharSet;
    }
    if (string.IsNullOrWhiteSpace(characterSet)) {
      characterSet = null;
    }
    return characterSet?.ToLowerInvariant() ?? "utf-8";
  }

  /// <summary>Gets the content of an HTTP response as a string.</summary>
  /// <param name="response">The response to process.</param>
  /// <returns>The content of <paramref name="response"/> as a string.</returns>
  public static string GetStringContent(HttpResponseMessage response)
    => AsyncUtils.ResultOf(HttpUtils.GetStringContentAsync(response));

  /// <summary>Gets the content of an HTTP response as a string.</summary>
  /// <param name="response">The response to process.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The content of <paramref name="response"/> as a string.</returns>
  public static async Task<string> GetStringContentAsync(HttpResponseMessage response,
                                                         CancellationToken cancellationToken = new()) {
    var content = response.Content;
    Debug.Print($"[{DateTime.UtcNow}] => RESPONSE ({content.Headers.ContentType}): {content.Headers.ContentLength} bytes");
#if NET
    var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    await using var _ = stream.ConfigureAwait(false);
#elif NETSTANDARD2_1_OR_GREATER
    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    await using var _ = stream.ConfigureAwait(false);
#else
    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
#if !NET
    if (stream is null) {
      return "";
    }
#endif
    var characterSet = HttpUtils.GetContentEncoding(content.Headers);
    using var sr = new StreamReader(stream, Encoding.GetEncoding(characterSet), false, 1024, true);
    // This is not (yet?) cancelable
    var text = await sr.ReadToEndAsync().ConfigureAwait(false);
    Debug.Print($"[{DateTime.UtcNow}] => RESPONSE TEXT: {TextUtils.FormatMultiLine(text)}");
    return text;
  }

  /// <summary>The name used by <see cref="CreateUserAgentHeader{T}"/> when no assembly name is available.</summary>
  public const string UnknownAssemblyName = "*Unknown Assembly*";

}

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetaBrainz.Common;

public static partial class HttpUtils {

  // These will be put in extension blocks once they work properly.

  #region extension(HttpContentHeaders)

  /// <summary>Gets the content encoding based on content headers.</summary>
  /// <param name="contentHeaders">The headers to get the information from.</param>
  /// <returns>
  /// The content encoding extracted from the headers (first from the <c>Content-Encoding</c> header, then from a <c>charset</c>
  /// specification as part of the <c>Content-Type</c> header), mapped to lower case; uses "utf-8" as fallback if no explicit
  /// specification was found.
  /// </returns>
  public static string GetContentEncoding(this HttpContentHeaders contentHeaders) => contentHeaders.GetContentEncoding("utf-8");

  /// <summary>Gets the content encoding based on content headers.</summary>
  /// <param name="defaultEncoding">
  /// The value to return when no encoding could be determined based on <paramref name="contentHeaders"/>.
  /// </param>
  /// <param name="contentHeaders">The headers to get the information from.</param>
  /// <returns>
  /// The content encoding extracted from the headers (first from the <c>Content-Encoding</c> header, then from a <c>charset</c>
  /// specification as part of the <c>Content-Type</c> header), mapped to lower case. If none could be determined,
  /// <paramref name="defaultEncoding"/> is returned (as is).
  /// </returns>
  [return: NotNullIfNotNull(nameof(defaultEncoding))]
  public static string? GetContentEncoding(this HttpContentHeaders contentHeaders, string? defaultEncoding) {
    var characterSet = contentHeaders.ContentEncoding.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(characterSet)) {
      // Fall back on the charset portion of the content type.
      // FIXME: Should this check the media type?
      characterSet = contentHeaders.ContentType?.CharSet;
    }
    if (string.IsNullOrWhiteSpace(characterSet)) {
      characterSet = null;
    }
    return characterSet?.ToLowerInvariant() ?? defaultEncoding;
  }

  #endregion

  #region extension(HttpResponseMessage)

  /// <summary>Checks a response to ensure it was successful.</summary>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <param name="response">The response to process.</param>
  /// <returns><paramref name="response"/>.</returns>
  /// <exception cref="HttpError">When the response did not have a successful status.</exception>
  public static async ValueTask<HttpResponseMessage> EnsureSuccessfulAsync(this HttpResponseMessage response,
                                                                           CancellationToken cancellationToken = new()) {
    if (response.IsSuccessStatusCode) {
      return response;
    }
    throw await HttpError.FromResponseAsync(response, cancellationToken);
  }

  /// <summary>Gets the content of an HTTP response as a string.</summary>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <param name="response">The response to process.</param>
  /// <returns>The content of <paramref name="response"/> as a string.</returns>
  public static async Task<string> GetStringContentAsync(this HttpResponseMessage response,
                                                         CancellationToken cancellationToken = new()) {
    var content = response.Content;
    var headers = content.Headers;
    HttpUtils.TraceSource.TraceEvent(TraceEventType.Verbose, 1, "RESPONSE ({0}): {1} bytes", headers.ContentType,
                                     headers.ContentLength);
    var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    await using var _ = stream.ConfigureAwait(false);
    using var sr = new StreamReader(stream, Encoding.GetEncoding(headers.GetContentEncoding()), false, 1024, true);
    var text = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    if (HttpUtils.TraceSource.Switch.ShouldTrace(TraceEventType.Verbose)) {
      HttpUtils.TraceSource.TraceEvent(TraceEventType.Verbose, 2, "RESPONSE TEXT: {0}", TextUtils.FormatMultiLine(text));
    }
    return text;
  }

  #endregion

}

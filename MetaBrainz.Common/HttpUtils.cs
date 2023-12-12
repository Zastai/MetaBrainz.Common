#define TRACE

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

  /// <summary>Checks a response to ensure it was successful.</summary>
  /// <param name="response">The response whose status should be checked.</param>
  /// <returns><paramref name="response"/>.</returns>
  /// <exception cref="HttpError">When the response did not have a successful status.</exception>
  public static HttpResponseMessage EnsureSuccessful(this HttpResponseMessage response)
    => AsyncUtils.ResultOf(response.EnsureSuccessfulAsync());

  /// <summary>Checks a response to ensure it was successful.</summary>
  /// <param name="response">The response whose status should be checked.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns><paramref name="response"/>.</returns>
  /// <exception cref="HttpError">When the response did not have a successful status.</exception>
  public static async ValueTask<HttpResponseMessage> EnsureSuccessfulAsync(this HttpResponseMessage response,
                                                                           CancellationToken cancellationToken = new()) {
    if (response.IsSuccessStatusCode) {
      return response;
    }
#if DEBUG
    // This also prints the contents.
    await response.GetStringContentAsync(cancellationToken);
#else
    await ValueTask.CompletedTask;
#endif
    throw new HttpError(response);
  }

  /// <summary>Gets the content encoding based on content headers.</summary>
  /// <param name="contentHeaders">The headers to get the information from.</param>
  /// <returns>
  /// The content encoding extracted from the headers, or "utf-8" as fallback if no explicit specification was found.
  /// </returns>
  public static string GetContentEncoding(this HttpContentHeaders contentHeaders) {
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
  public static string GetStringContent(this HttpResponseMessage response)
    => AsyncUtils.ResultOf(response.GetStringContentAsync());

  /// <summary>Gets the content of an HTTP response as a string.</summary>
  /// <param name="response">The response to process.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The content of <paramref name="response"/> as a string.</returns>
  public static async Task<string> GetStringContentAsync(this HttpResponseMessage response,
                                                         CancellationToken cancellationToken = new()) {
    var headers = response.Content.Headers;
    HttpUtils.TraceSource.TraceInformation("RESPONSE ({0}): {1} bytes", headers.ContentType, headers.ContentLength);
    var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    await using var _ = stream.ConfigureAwait(false);
    var characterSet = headers.GetContentEncoding();
    using var sr = new StreamReader(stream, Encoding.GetEncoding(characterSet), false, 1024, true);
#if NET6_0
    var text = await sr.ReadToEndAsync().ConfigureAwait(false);
#else
    var text = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
#endif
    if (HttpUtils.TraceSource.Switch.ShouldTrace(TraceEventType.Information)) {
      HttpUtils.TraceSource.TraceInformation("RESPONSE TEXT: {0}", TextUtils.FormatMultiLine(text));
    }
    return text;
  }

  /// <summary>The trace source (named 'MetaBrainz.Common.HttpUtils', with a switch of the same name) used by this class.</summary>
  public static readonly TraceSource TraceSource = new("MetaBrainz.Common.HttpUtils") {
    Switch = new SourceSwitch("MetaBrainz.Common.HttpUtils", "Off")
  };

  /// <summary>The name used by <see cref="CreateUserAgentHeader{T}"/> when no assembly name is available.</summary>
  public const string UnknownAssemblyName = "*Unknown Assembly*";

}

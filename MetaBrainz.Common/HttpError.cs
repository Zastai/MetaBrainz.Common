using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>An error reported by an HTTP response.</summary>
[PublicAPI]
public class HttpError : Exception {

  /// <summary>Creates a new HTTP error.</summary>
  /// <param name="status">The status code for the error.</param>
  /// <param name="reason">The reason phrase associated with the error.</param>
  /// <param name="version">The HTTP message version.</param>
  /// <param name="message">
  /// The message to use; if this is not specified or <see langword="null"/>, a message will be constructed based on
  /// <paramref name="status"/>, <paramref name="reason"/> and <paramref name="version"/>.
  /// </param>
  /// <param name="cause">The exception that caused this one, if any.</param>
  public HttpError(HttpStatusCode status, string? reason = null, Version? version = null, string? message = null,
                   Exception? cause = null) : base(HttpError.MessageFor(status, reason, version, message), cause) {
    this.Reason = reason;
    this.Status = status;
    this.Version = version;
  }

  /// <summary>The content (assumed to be text) of the error response, if available.</summary>
  public string? Content { get; private init; }

  /// <summary>The content headers of the error response, if available.</summary>
  public HttpContentHeaders? ContentHeaders { get; private init; }

  /// <summary>The reason phrase associated with the error.</summary>
  public string? Reason { get; }

  /// <summary>The headers of the request that provoked the error response, if available.</summary>
  public HttpRequestHeaders? RequestHeaders { get; private init; }

  /// <summary>The URI for the request that provoked the error response, if available.</summary>
  public Uri? RequestUri { get; private init; }

  /// <summary>The headers of the error response, if available.</summary>
  public HttpResponseHeaders? ResponseHeaders { get; private init; }

  /// <summary>The status code for the error.</summary>
  public HttpStatusCode Status { get; }

  /// <summary>The HTTP message version from the error response, if available.</summary>
  public Version? Version { get; private init; }

  /// <summary>Creates a new HTTP error based on an response message.</summary>
  /// <param name="response">The response.</param>
  /// <returns>A new HTTP error containing information taken from the response message.</returns>
  public static HttpError FromResponse(HttpResponseMessage response) => AsyncUtils.ResultOf(HttpError.FromResponseAsync(response));

  /// <summary>Creates a new HTTP error based on an response message.</summary>
  /// <param name="response">The response message that triggered the error.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>A new HTTP error containing information taken from the response message.</returns>
  public static async Task<HttpError> FromResponseAsync(HttpResponseMessage response,
                                                        CancellationToken cancellationToken = default) {
    return new HttpError(response.StatusCode, response.ReasonPhrase) {
      Content = await response.GetStringContentAsync(cancellationToken),
      ContentHeaders = HttpUtils.Copy(response.Content.Headers),
      RequestHeaders = HttpUtils.Copy(response.RequestMessage?.Headers),
      RequestUri = response.RequestMessage?.RequestUri,
      ResponseHeaders = HttpUtils.Copy(response.Headers),
      Version = response.Version,
    };
  }

  private static string MessageFor(HttpStatusCode status, string? reason, Version? version, string? message) {
    if (message is not null) {
      return message;
    }
    var sb = new StringBuilder();
    sb.Append("HTTP");
    if (version is not null) {
      sb.Append('/').Append(version);
    }
    sb.Append(' ').Append((int) status).Append(" (").Append(status).Append(')');
    if (reason is not null) {
      sb.Append(" '").Append(reason).Append('\'');
    }
    return sb.ToString();
  }

}

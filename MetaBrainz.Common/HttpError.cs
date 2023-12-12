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
  /// <param name="response">The response to take the status code and reason from.</param>
  [Obsolete($"Use {nameof(HttpError.FromResponse)} or {nameof(HttpError.FromResponseAsync)} instead.")]
  public HttpError(HttpResponseMessage response) : this(response.StatusCode, response.ReasonPhrase, response.Version) { }

  /// <summary>Creates a new HTTP error.</summary>
  /// <param name="status">The status code for the error.</param>
  /// <param name="reason">The reason phrase associated with the error.</param>
  /// <param name="cause">The exception that caused this one, if any.</param>
  public HttpError(HttpStatusCode status, string? reason, Exception? cause = null) : base(null, cause) {
    this.Reason = reason;
    this.Status = status;
  }

  /// <summary>Creates a new HTTP error.</summary>
  /// <param name="status">The status code for the error.</param>
  /// <param name="reason">The reason phrase associated with the error.</param>
  /// <param name="version">The HTTP message version.</param>
  /// <param name="cause">The exception that caused this one, if any.</param>
  public HttpError(HttpStatusCode status, string? reason, Version version, Exception? cause = null) : base(null, cause) {
    this.Reason = reason;
    this.Status = status;
    this.Version = version;
  }

  /// <summary>The content (assumed to be text) of the response that triggered the error, if available.</summary>
  public string? Content { get; private init; }

  /// <summary>The content headers of the response that triggered the error, if available.</summary>
  public HttpContentHeaders? ContentHeaders { get; private init; }

  /// <summary>Gets a textual representation of the HTTP error.</summary>
  /// <returns>A textual representation of the HTTP error.</returns>
  public override string Message {
    get {
      var sb = new StringBuilder();
      sb.Append("HTTP");
      if (this.Version is not null) {
        sb.Append('/').Append(this.Version);
      }
      sb.Append(' ').Append((int) this.Status).Append(" (").Append(this.Status).Append(')');
      if (this.Reason is not null) {
        sb.Append(" '").Append(this.Reason).Append('\'');
      }
      return sb.ToString();
    }
  }

  /// <summary>The reason phrase associated with the error.</summary>
  public string? Reason { get; }

  /// <summary>The headers of the response that triggered the error, if available.</summary>
  public HttpResponseHeaders? ResponseHeaders { get; private init; }

  /// <summary>The status code for the error.</summary>
  public HttpStatusCode Status { get; }

  /// <summary>The HTTP message version from the response that triggered the error, if available.</summary>
  public Version? Version { get; private init; }

  /// <summary>Creates a new HTTP error based on an response message.</summary>
  /// <param name="response">The response message that triggered the error.</param>
  /// <returns>A new HTTP error containing information taken from the response message.</returns>
  public static HttpError FromResponse(HttpResponseMessage response) => AsyncUtils.ResultOf(HttpError.FromResponseAsync(response));

  /// <summary>Creates a new HTTP error based on an response message.</summary>
  /// <param name="response">The response message that triggered the error.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>A new HTTP error containing information taken from the response message.</returns>
  public static async Task<HttpError> FromResponseAsync(HttpResponseMessage response,
                                                        CancellationToken cancellationToken = default) {
    // It's unfortunate that the headers are not easily copied (the classes do not have public constructors), so any changes to the
    // response after this method is called will be reflected in the error's properties.
    return new HttpError(response.StatusCode, response.ReasonPhrase) {
      Content = await response.GetStringContentAsync(cancellationToken),
      ContentHeaders = response.Content.Headers,
      ResponseHeaders = response.Headers,
      Version = response.Version,
    };
  }

}

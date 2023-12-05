using System;
using System.Net;
using System.Net.Http;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>An error reported by an HTTP response.</summary>
[PublicAPI]
public class HttpError : Exception {

  /// <summary>Creates a new HTTP error.</summary>
  /// <param name="response">The response to take the status code and reason from.</param>
  public HttpError(HttpResponseMessage response) : this(response.StatusCode, response.ReasonPhrase) { }

  /// <summary>Creates a new HTTP error.</summary>
  /// <param name="status">The status code for the error.</param>
  /// <param name="reason">The reason phrase associated with the error.</param>
  /// <param name="cause">The exception that caused this one, if any.</param>
  public HttpError(HttpStatusCode status, string? reason, Exception? cause = null) : base(null, cause) {
    this.Reason = reason;
    this.Status = status;
  }

  /// <summary>Gets a textual representation of the HTTP error.</summary>
  /// <returns>A string of the form <c>HTTP nnn/StatusName 'REASON'</c>.</returns>
  public override string Message
    => this.Reason is null ? $"HTTP {(int) this.Status}/{this.Status}" : $"HTTP {(int) this.Status}/{this.Status} '{this.Reason}'";

  /// <summary>The reason phrase associated with the error.</summary>
  public string? Reason { get; }

  /// <summary>The status code for the error.</summary>
  public HttpStatusCode Status { get; }

}

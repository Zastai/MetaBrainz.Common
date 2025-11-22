using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;

using JetBrains.Annotations;

namespace MetaBrainz.Common;

/// <summary>Utility methods related to HTTP processing.</summary>
[PublicAPI]
public static partial class HttpUtils {

  /// <summary>Creates a copy of a set of HTTP content headers.</summary>
  /// <param name="headers">The headers to copy.</param>
  /// <returns>A new set of HTTP content headers, with the same contents as the set provided.</returns>
  [return: NotNullIfNotNull(nameof(headers))]
  public static HttpContentHeaders? Copy(HttpContentHeaders? headers) {
    if (headers is null) {
      return null;
    }
    // There is no way to construct a copy of HTTP headers directly at the moment (see dotnet/runtime#95912).
    using var dummy = new ByteArrayContent([]);
    HttpUtils.Copy(headers, dummy.Headers);
    return dummy.Headers;
  }

  private static void Copy(HttpHeaders from, HttpHeaders to) {
    foreach (var (name, values) in from.NonValidated) {
      to.TryAddWithoutValidation(name, values);
    }
  }

  /// <summary>Creates a copy of a set of HTTP request headers.</summary>
  /// <param name="headers">The headers to copy.</param>
  /// <returns>A new set of HTTP request headers, with the same contents as the set provided.</returns>
  [return: NotNullIfNotNull(nameof(headers))]
  public static HttpRequestHeaders? Copy(HttpRequestHeaders? headers) {
    if (headers is null) {
      return null;
    }
    // There is no way to construct a copy of HTTP headers directly at the moment (see dotnet/runtime#95912).
    using var dummy = new HttpRequestMessage();
    HttpUtils.Copy(headers, dummy.Headers);
    return dummy.Headers;
  }

  /// <summary>Creates a copy of a set of HTTP response headers.</summary>
  /// <param name="headers">The headers to copy.</param>
  /// <returns>A new set of HTTP response headers, with the same contents as the set provided.</returns>
  [return: NotNullIfNotNull(nameof(headers))]
  public static HttpResponseHeaders? Copy(HttpResponseHeaders? headers) {
    if (headers is null) {
      return null;
    }
    // There is no way to construct a copy of HTTP headers directly at the moment (see dotnet/runtime#95912).
    using var dummy = new HttpResponseMessage();
    HttpUtils.Copy(headers, dummy.Headers);
    return dummy.Headers;
  }

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

  /// <summary>The trace source (named 'MetaBrainz.Common.HttpUtils') used by this class.</summary>
  public static readonly TraceSource TraceSource = new("MetaBrainz.Common.HttpUtils", SourceLevels.Off);

  /// <summary>The name used by <see cref="CreateUserAgentHeader{T}"/> when no assembly name is available.</summary>
  public const string UnknownAssemblyName = "*Unknown Assembly*";

}

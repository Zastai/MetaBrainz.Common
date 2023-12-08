# API Reference: MetaBrainz.Common

## Assembly Attributes

```cs
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]
```

## Namespace: MetaBrainz.Common

### Type: AsyncUtils

```cs
public static class AsyncUtils {

  public static void ResultOf(System.Threading.Tasks.Task task);

  public static void ResultOf(System.Threading.Tasks.ValueTask task);

  public static T ResultOf<T>(System.Threading.Tasks.Task<T> task);

  public static T ResultOf<T>(System.Threading.Tasks.ValueTask<T> task);

}
```

### Type: HttpError

```cs
public class HttpError : System.Exception {

  string Message {
    public override get;
  }

  string? Reason {
    public get;
  }

  System.Net.HttpStatusCode Status {
    public get;
  }

  public HttpError(System.Net.Http.HttpResponseMessage response);

  public HttpError(System.Net.HttpStatusCode status, string? reason, System.Exception? cause = null);

}
```

### Type: HttpUtils

```cs
public static class HttpUtils {

  public const string UnknownAssemblyName = "*Unknown Assembly*";

  public static System.Net.Http.Headers.ProductInfoHeaderValue CreateUserAgentHeader<T>();

  public static System.Net.Http.HttpResponseMessage EnsureSuccessful(this System.Net.Http.HttpResponseMessage response);

  public static System.Threading.Tasks.ValueTask<System.Net.Http.HttpResponseMessage> EnsureSuccessfulAsync(this System.Net.Http.HttpResponseMessage response, System.Threading.CancellationToken cancellationToken = default);

  public static string GetContentEncoding(this System.Net.Http.Headers.HttpContentHeaders contentHeaders);

  public static string GetStringContent(this System.Net.Http.HttpResponseMessage response);

  public static System.Threading.Tasks.Task<string> GetStringContentAsync(this System.Net.Http.HttpResponseMessage response, System.Threading.CancellationToken cancellationToken = default);

}
```

### Type: RateLimitInfo

```cs
public readonly struct RateLimitInfo {

  int? AllowedRequests {
    public get;
  }

  System.DateTimeOffset LastRequest {
    public get;
  }

  int? RemainingRequests {
    public get;
  }

  System.DateTimeOffset? ResetAt {
    public get;
  }

  int? ResetIn {
    public get;
  }

  public RateLimitInfo(System.Net.Http.Headers.HttpResponseHeaders headers);

}
```

### Type: TextUtils

```cs
public static class TextUtils {

  [System.ObsoleteAttribute("Call Encoding.UTF8.GetString() instead.")]
  public static string DecodeUtf8(System.ReadOnlySpan<byte> bytes);

  public static string FormatMultiLine(string text, string prefix = "<<", string suffix = ">>", string separator = "\n  ");

}
```

### Type: UnixTime

```cs
[System.ObsoleteAttribute("Use DateTimeOffset instead.")]
public static class UnixTime {

  [System.ObsoleteAttribute("Use DateTimeOffset.UnixEpoch instead.")]
  public static readonly System.DateTimeOffset Epoch;

  [System.ObsoleteAttribute("Use DateTimeOffset.ToUnixTimeSeconds instead.")]
  public static long Convert(System.DateTimeOffset value);

  [System.ObsoleteAttribute("Use DateTimeOffset.ToUnixTimeSeconds instead.")]
  public static long? Convert(System.DateTimeOffset? value);

  [System.ObsoleteAttribute("Use DateTimeOffset.FromUnixTimeSeconds instead.")]
  public static System.DateTimeOffset Convert(long value);

  [System.ObsoleteAttribute("Use DateTimeOffset.FromUnixTimeSeconds instead.")]
  public static System.DateTimeOffset? Convert(long? value);

}
```

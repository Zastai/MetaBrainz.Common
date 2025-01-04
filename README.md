# MetaBrainz.Common [![Build Status][CI-S]][CI-L] [![NuGet Package Version][NuGet-S]][NuGet-L]

General helper classes, for use by the other `MetaBrainz.*` packages.

[CI-S]: https://github.com/Zastai/MetaBrainz.Common/actions/workflows/build.yml/badge.svg
[CI-L]: https://github.com/Zastai/MetaBrainz.Common/actions/workflows/build.yml

[NuGet-S]: https://img.shields.io/nuget/v/MetaBrainz.Common
[NuGet-L]: https://www.nuget.org/packages/MetaBrainz.Common

## Debugging

The `HttpUtils` class provides a `TraceSource` that can be used to
configure debug output; its name is `MetaBrainz.Common.HttpUtils`.

### Configuration

#### In Code

In code, you can enable tracing like follows:

```cs
// Use the default switch, turning it on.
HttpUtils.TraceSource.Switch.Level = SourceLevels.All;

// Alternatively, use your own switch so multiple things can be
// enabled/disabled at the same time.
var mySwitch = new TraceSwitch("MyAppDebugSwitch", "All");
HttpUtils.TraceSource.Switch = mySwitch;

// By default, there is a single listener that writes trace events to
// the debug output (typically only seen in an IDE's debugger). You can
// add (and remove) listeners as desired.
var listener = new ConsoleTraceListener {
  Name = "MyAppConsole",
  TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId,
};
HttpUtils.TraceSource.Listeners.Clear();
HttpUtils.TraceSource.Listeners.Add(listener);
```

#### In Configuration

Your application can also be set up to read tracing configuration from
the application configuration file. To do so, the following needs to be
added to its startup code:

```cs
System.Diagnostics.TraceConfiguration.Register();
```

(Provided by the `System.Configuration.ConfigurationManager` package.)

The application config file can then have a `system.diagnostics` section
where sources, switches and listeners can be configured.

```xml
<configuration>
  <system.diagnostics>
    <sharedListeners>
      <add name="console" type="System.Diagnostics.ConsoleTraceListener" traceOutputOptions="DateTime,ProcessId" />
    </sharedListeners>
    <sources>
      <source name="MetaBrainz.Common.HttpUtils" switchName="MetaBrainz.Common.HttpUtils">
        <listeners>
          <add name="console" />
          <add name="http-log" type="System.Diagnostics.TextWriterTraceListener" initializeData="http-utils.log" />
        </listeners>
      </source>
    </sources>
    <switches>
      <add name="MetaBrainz.Common.HttpUtils" value="All" />
    </switches>
  </system.diagnostics>
</configuration>
```

## Release Notes

These are available [on GitHub][release-notes].

[release-notes]: https://github.com/Zastai/MetaBrainz.Common/releases

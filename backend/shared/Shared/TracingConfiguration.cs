using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Diagnostics.Metrics;
using System.Reflection;
using Confluent.Kafka.Extensions.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Class to configure OpenTelemetry tracing
public static class TracingConfiguration
{
    // Declare an ActivitySource for creating tracing activities
    private static readonly ActivitySource ActivitySource = new("MyCustomActivitySource");

    // Configure OpenTelemetry with custom settings and instrumentation
    public static void ConfigureOpenTelemetry(Assembly assembly)
    {
        // Retrieve the service name and version from the executing assembly metadata
        var serviceName = assembly.GetName().Name ?? "UnknownService";
        var serviceVersion = assembly.GetName().Version?.ToString() ?? "UnknownVersion";

        Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(
                // Set resource attributes including service name and version
                ResourceBuilder
                    .CreateDefault()
                    .AddService(serviceName, serviceVersion: serviceVersion)
                    .AddAttributes([new KeyValuePair<string, object>("environment", "development")]) // Additional attributes
                    .AddTelemetrySdk() // Add telemetry SDK information to the traces
                    .AddEnvironmentVariableDetector()
            )
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri("http://jaeger:4318/v1/traces");
                o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            })
            .Build();

        // Set up the tracer provider with various configurations
        Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(
                // Set resource attributes including service name and version
                ResourceBuilder
                    .CreateDefault()
                    .AddService(serviceName, serviceVersion: serviceVersion)
                    .AddAttributes([new KeyValuePair<string, object>("environment", "development")]) // Additional attributes
                    .AddTelemetrySdk() // Add telemetry SDK information to the traces
                    .AddEnvironmentVariableDetector()
            ) // Detect resource attributes from environment variables
            .AddSource(ActivitySource.Name) // Add the ActivitySource defined above
            .AddAspNetCoreInstrumentation(o =>
            {
                o.RecordException = true;
                o.Filter = context =>
                {
                    return context.Request.Path != "/hc";
                };
            }) // Add automatic instrumentation for ASP.NET Core
            .AddHttpClientInstrumentation(o =>
            {
                o.RecordException = true;
            }) // Add automatic instrumentation for HttpClient requests
            .AddConfluentKafkaInstrumentation()
            .AddSqlClientInstrumentation(o =>
            {
                o.RecordException = true;
                o.SetDbStatementForText = true;
            })
            .AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri("http://jaeger:4318/v1/traces");
                o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            })
            .Build(); // Build the tracer provider
    }

    // Method to start a new tracing activity with an optional activity kind
    public static Activity StartActivity(
        string activityName,
        ActivityKind kind = ActivityKind.Internal
    )
    {
        // Starts and returns a new activity if sampling allows it, otherwise returns null
        return ActivitySource.StartActivity(activityName, kind)!;
    }

    public static void LogException(this Activity activity, Exception exception)
    {
        activity.SetTag("error", true).AddException(exception);
    }
}

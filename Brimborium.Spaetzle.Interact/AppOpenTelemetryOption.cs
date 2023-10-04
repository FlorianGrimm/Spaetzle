using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using System.Diagnostics;
using System.Reflection;

namespace Brimborium.Spaetzle.Interact;

public class AppOpenTelemetryOption
{
    public string? ServiceName { get; set; } = null;
    public string? ServiceNamespace { get; set; } = null;
    public string? ServiceVersion { get; set; } = null;
    public bool AutoGenerateServiceInstanceId { get; set; } = true;
    public string? ServiceInstanceId { get; set; } = null;

    public bool EnableTracer { get; set; } = false;
    public AppOtlpExporterOptions TracerOptions { get; set; } = new AppOtlpExporterOptions();

    public bool EnableMeter { get; set; } = false;
    public AppOtlpExporterOptions MeterOptions { get; set; } = new AppOtlpExporterOptions();

    public bool EnableLog { get; set; } = false;
    public AppOtlpExporterOptions LogOptions { get; set; } = new AppOtlpExporterOptions();
}

public static class AppOpenTelemetryExtensions
{
    //public static IServiceCollection AddOpenTelemetryOption(
    //    this IServiceCollection services
    //    )
    //{
    //    services.AddOptions<OpenTelemetryOption>();
    //    return services;
    //}

    //public static IServiceCollection AddOpenTelemetryOption(
    //    this IServiceCollection services,
    //    Action<Microsoft.Extensions.Options.OptionsBuilder<OpenTelemetryOption>> configure
    //    )
    //{
    //    var optionBuilder = services.AddOptions<OpenTelemetryOption>();
    //    configure(optionBuilder);
    //    return services;
    //}

    //public static IServiceCollection AddOpenTelemetryOption(
    //    this IServiceCollection services,
    //    IConfiguration configuration
    //    )
    //{
    //    var optionBuilder = services.AddOptions<OpenTelemetryOption>();
    //    optionBuilder.Configure(option =>
    //    {
    //        configuration.Bind(option);
    //    });
    //    return services;
    //}

    //public static WebApplicationBuilder AddOpenTelemetryOption(
    //    this WebApplicationBuilder builder,
    //    string? sectionName=null
    //    )
    //{
    //    var optionBuilder = builder.Services.AddOptions<OpenTelemetryOption>();
    //    optionBuilder.Configure(option =>
    //    {
    //        var name = (string.IsNullOrEmpty(sectionName)) ? "OpenTelemetry" : sectionName;
    //        builder.Configuration.GetSection(name).Bind(option);
    //    });
    //    return builder;
    //}

    public static OpenTelemetryBuilder AddOpenTelemetry(
        this WebApplicationBuilder builder,
        AppOpenTelemetryOption? openTelemetryOption = default,
        string? sectionName = default,
        Action<AppOpenTelemetryOption, ResourceBuilder>? configureResource = default,
        Action<MeterProviderBuilder>? configureMeterProvider = default,
        Action<TracerProviderBuilder>? configureTracerProvider = default,
        Action<OpenTelemetryLoggerOptions>? configureLogger = default,
        Action<OtlpExporterOptions>? configureMeterOtlpExporter = default,
        Action<OtlpExporterOptions>? configureTraceOtlpExporter = default,
        Action<OtlpExporterOptions>? configureLogOtlpExporter = default
        )
    {
        if (openTelemetryOption is null)
        {
            var name = (string.IsNullOrEmpty(sectionName)) ? "OpenTelemetry" : sectionName;
            openTelemetryOption = new AppOpenTelemetryOption();
            builder.Configuration.GetSection(name).Bind(openTelemetryOption);
        }
        OpenTelemetryBuilder openTelemetryBuilder = builder.Services.AddOpenTelemetry();
        if (configureResource is not null)
        {
            openTelemetryBuilder.ConfigureResource(
                (resourceBuilder) => configureResource(openTelemetryOption, resourceBuilder));
        } else
        {
            if (!string.IsNullOrEmpty(openTelemetryOption.ServiceName))
            {
                openTelemetryBuilder.ConfigureResource((resourceBuilder) =>
                {
                    resourceBuilder.AddService(
                        serviceName: openTelemetryOption.ServiceName,
                        serviceNamespace: openTelemetryOption.ServiceNamespace,
                        serviceVersion: openTelemetryOption.ServiceVersion,
                        autoGenerateServiceInstanceId: openTelemetryOption.AutoGenerateServiceInstanceId,
                        serviceInstanceId: openTelemetryOption.ServiceInstanceId
                        );
                });
            }
        }

        if ((openTelemetryOption.EnableTracer)
            || (configureTracerProvider is not null))
        {
            openTelemetryBuilder.WithTracing((tracerProviderBuilder) =>
            {
                if (configureTracerProvider is not null)
                {
                    configureTracerProvider(tracerProviderBuilder);
                }
                tracerProviderBuilder.AddOtlpExporter(otlpExporterOptions =>
                {
                    CopyOtlpExporterOptions(openTelemetryOption.TracerOptions, otlpExporterOptions);
                    if (configureTraceOtlpExporter is not null)
                    {
                        configureTraceOtlpExporter(otlpExporterOptions);
                    }
                });
            });
        }

        if ((openTelemetryOption.EnableMeter)
            || (configureMeterProvider is not null))
        {
            openTelemetryBuilder.WithMetrics(meterProviderBuilder =>
            {
                if (configureMeterProvider is not null)
                {
                    configureMeterProvider(meterProviderBuilder);
                } else
                {
                    meterProviderBuilder.AddOtlpExporter(otlpExporterOptions =>
                    {
                        CopyOtlpExporterOptions(openTelemetryOption.MeterOptions, otlpExporterOptions);
                        if (configureMeterOtlpExporter is not null)
                        {
                            configureMeterOtlpExporter(otlpExporterOptions);
                        }
                    });
                }
            });
        }

        if (openTelemetryOption.EnableLog) {
            if (configureLogger is not null) {
                builder.Services.Configure(configureLogger);
            }
            builder.Logging.AddOpenTelemetry(openTelemetryLoggerOption => {
                openTelemetryLoggerOption.AddOtlpExporter((OtlpExporterOptions otlpExporterOptions) => {
                    CopyOtlpExporterOptions(openTelemetryOption.LogOptions, otlpExporterOptions);
                    if (configureLogOtlpExporter is not null)
                    {
                        configureLogOtlpExporter(otlpExporterOptions);
                    }
                });
            });
        }

        return openTelemetryBuilder;
    }

    public static void CopyOtlpExporterOptions(AppOtlpExporterOptions sourceOptions, OtlpExporterOptions targetOtlpExporterOptions)
    {
        if (sourceOptions.Protocol.HasValue)
        {
            targetOtlpExporterOptions.Protocol = sourceOptions.Protocol.Value;
        }

        if (sourceOptions.ExportProcessorType.HasValue)
        {
            targetOtlpExporterOptions.ExportProcessorType = sourceOptions.ExportProcessorType.Value;
        }

        if (sourceOptions.TimeoutMilliseconds.HasValue)
        {
            targetOtlpExporterOptions.TimeoutMilliseconds = sourceOptions.TimeoutMilliseconds.Value;
        }

        if (sourceOptions.Endpoint is not null)
        {
            targetOtlpExporterOptions.Endpoint = sourceOptions.Endpoint;
        }
        if (string.IsNullOrEmpty(sourceOptions.Headers))
        {
            targetOtlpExporterOptions.Headers = sourceOptions.Headers;
        }
    }
}


/// <summary>
/// OpenTelemetry Protocol (OTLP) exporter options.
/// OTEL_EXPORTER_OTLP_ENDPOINT, OTEL_EXPORTER_OTLP_HEADERS, OTEL_EXPORTER_OTLP_TIMEOUT, OTEL_EXPORTER_OTLP_PROTOCOL
/// environment variables are parsed during object construction.
/// </summary>
public class AppOtlpExporterOptions
{

    /// <summary>
    /// Initializes a new instance of the <see cref="OtlpExporterOptions"/> class.
    /// </summary>
    public AppOtlpExporterOptions()
    {
        this.BatchExportProcessorOptions = new();
    }

    /// <summary>
    /// Gets or sets the target to which the exporter is going to send telemetry.
    /// Must be a valid Uri with scheme (http or https) and host, and
    /// may contain a port and path. The default value is
    /// * http://localhost:4317 for <see cref="OtlpExportProtocol.Grpc"/>
    /// * http://localhost:4318 for <see cref="OtlpExportProtocol.HttpProtobuf"/>.
    /// </summary>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets optional headers for the connection. Refer to the <a href="https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/protocol/exporter.md#specifying-headers-via-environment-variables">
    /// specification</a> for information on the expected format for Headers.
    /// </summary>
    public string? Headers { get; set; }

    /// <summary>
    /// Gets or sets the max waiting time (in milliseconds) for the backend to process each batch. The default value is 10000.
    /// </summary>
    public int? TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the the OTLP transport protocol. Supported values: Grpc and HttpProtobuf.
    /// </summary>
    public OtlpExportProtocol? Protocol { get; set; }

    /// <summary>
    /// Gets or sets the export processor type to be used with the OpenTelemetry Protocol Exporter. The default value is <see cref="ExportProcessorType.Batch"/>.
    /// </summary>
    /// <remarks>Note: This only applies when exporting traces.</remarks>
    public ExportProcessorType? ExportProcessorType { get; set; }

    /// <summary>
    /// Gets or sets the BatchExportProcessor options. Ignored unless ExportProcessorType is Batch.
    /// </summary>
    /// <remarks>Note: This only applies when exporting traces.</remarks>
    public BatchExportProcessorOptions<Activity>? BatchExportProcessorOptions { get; set; }

    /// <summary>
    /// Gets or sets the factory function called to create the <see
    /// cref="HttpClient"/> instance that will be used at runtime to
    /// transmit telemetry over HTTP. The returned instance will be reused
    /// for all export invocations.
    /// </summary>
    /// <remarks>
    /// Notes:
    /// <list type="bullet">
    /// <item>This is only invoked for the <see
    /// cref="OtlpExportProtocol.HttpProtobuf"/> protocol.</item>
    /// <item>The default behavior when using the <see
    /// cref="OtlpTraceExporterHelperExtensions.AddOtlpExporter(TracerProviderBuilder,
    /// Action{OtlpExporterOptions})"/> extension is if an <a
    /// href="https://docs.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory">IHttpClientFactory</a>
    /// instance can be resolved through the application <see
    /// cref="IServiceProvider"/> then an <see cref="HttpClient"/> will be
    /// created through the factory with the name "OtlpTraceExporter"
    /// otherwise an <see cref="HttpClient"/> will be instantiated
    /// directly.</item>
    /// <item>The default behavior when using the <see
    /// cref="OtlpMetricExporterExtensions.AddOtlpExporter(MeterProviderBuilder,
    /// Action{OtlpExporterOptions})"/> extension is if an <a
    /// href="https://docs.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory">IHttpClientFactory</a>
    /// instance can be resolved through the application <see
    /// cref="IServiceProvider"/> then an <see cref="HttpClient"/> will be
    /// created through the factory with the name "OtlpMetricExporter"
    /// otherwise an <see cref="HttpClient"/> will be instantiated
    /// directly.</item>
    /// </list>
    /// </remarks>
    public Func<HttpClient>? HttpClientFactory { get; set; }
}

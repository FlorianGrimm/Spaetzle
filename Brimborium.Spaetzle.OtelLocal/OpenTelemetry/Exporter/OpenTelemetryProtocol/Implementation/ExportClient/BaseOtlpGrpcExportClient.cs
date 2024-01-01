// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using Grpc.Core;
using OpenTelemetry.Internal;
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
using Grpc.Net.Client;
#endif

namespace OpenTelemetry.Exporter.OpenTelemetryProtocol.Implementation.ExportClient;

/// <summary>Base class for sending OTLP export request over gRPC.</summary>
/// <typeparam name="TRequest">Type of export request.</typeparam>
internal abstract class BaseOtlpLocalExportClient<TRequest> : IExportClient<TRequest>
{
    protected BaseOtlpLocalExportClient(OtlpLocalExporterOptions options)
    {
        Guard.ThrowIfNull(options);
    }

    /// <inheritdoc/>
    public abstract bool SendExportRequest(TRequest request, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual bool Shutdown(int timeoutMilliseconds)
    {
        //if (this.Channel == null)
        //{
        //    return true;
        //}

        //if (timeoutMilliseconds == -1)
        //{
        //    this.Channel.ShutdownAsync().Wait();
        //    return true;
        //}
        //else
        //{
        //    return Task.WaitAny(new Task[] { this.Channel.ShutdownAsync(), Task.Delay(timeoutMilliseconds) }) == 0;
        //}
        return true;
    }
}

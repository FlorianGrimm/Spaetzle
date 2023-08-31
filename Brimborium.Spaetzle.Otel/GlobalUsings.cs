global using global::System;
global using global::System.Buffers;
global using global::System.Collections.Generic;
global using global::System.IO;
global using global::System.Linq;
global using global::System.Net.Http;
global using global::System.Text;
global using global::System.Text.RegularExpressions;
global using global::System.Threading;
global using global::System.Threading.Tasks;

global using global::OpenTelemetry.Proto.Collector;
global using global::OpenTelemetry.Proto.Collector.Logs.V1;
global using global::OpenTelemetry.Proto.Collector.Metrics.V1;
global using global::OpenTelemetry.Proto.Collector.Trace.V1;

global using global::Grpc.Core;

global using global::Microsoft.AspNetCore.Builder;
global using global::Microsoft.AspNetCore.Builder.Extensions;
global using global::Microsoft.AspNetCore.Hosting;
global using global::Microsoft.AspNetCore.Http;
global using global::Microsoft.AspNetCore.Http.HttpResults;

global using global::Microsoft.Extensions.DependencyInjection;
global using global::Microsoft.Extensions.Hosting;
global using global::Microsoft.Extensions.Logging;


global using global::Brimborium.Spaetzle.Otel.Services;
//
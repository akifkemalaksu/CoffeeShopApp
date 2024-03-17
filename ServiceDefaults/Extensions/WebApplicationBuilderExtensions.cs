using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
        {
            var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
            var otel = builder.Services.AddOpenTelemetry();

            otel.ConfigureResource(resource => resource
                .AddService(serviceName: builder.Environment.ApplicationName));

            otel.WithMetrics(metrics => metrics
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddOtlpExporter(configure =>
                {
                    configure.Endpoint = new Uri(tracingOtlpEndpoint);
                }));

            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                });
            });

            builder.Logging.AddOpenTelemetry(configure =>
            {
                configure.IncludeFormattedMessage = true;
                configure.IncludeScopes = true;
                configure.ParseStateValues = true;
                configure.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
            });

            return builder;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services;
        }
    }
}

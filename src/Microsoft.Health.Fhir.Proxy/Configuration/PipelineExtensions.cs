﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Health.Fhir.Proxy.Bindings;
using Microsoft.Health.Fhir.Proxy.Channels;
using Microsoft.Health.Fhir.Proxy.Filters;
using Microsoft.Health.Fhir.Proxy.Pipelines;
using Microsoft.Health.Fhir.Proxy.Security;
using System;
using System.Net.Http;

namespace Microsoft.Health.Fhir.Proxy.Configuration
{
    /// <summary>
    /// Helper extensions for pipelines
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// Uses application insights for logging.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instrumentationKey"></param>
        /// <param name="logLevel"></param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection UseAppInsightsLogging(this IServiceCollection services, string instrumentationKey, LogLevel logLevel)
        {
            services.AddLogging(builder =>
            {
                builder.AddFilter<ApplicationInsightsLoggerProvider>("", logLevel);
                builder.AddApplicationInsights(instrumentationKey);
            });

            return services;
        }

        /// <summary>
        /// Use application insights for telemetry.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instrumentationKey"></param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection UseTelemetry(this IServiceCollection services, string instrumentationKey)
        {
            services.Configure<TelemetryConfiguration>(options =>
            {
                options.InstrumentationKey = instrumentationKey;
            });
            services.AddScoped<TelemetryClient>();

            return services;
        }

        /// <summary>
        /// Use the authenticator for acquisition of access tokens from Azure AD.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection UseAuthenticator(this IServiceCollection services, Action<ServiceIdentityOptions> options)
        {
            services.AddScoped<IAuthenticator, Authenticator>();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Uses a Azure Function pipeline.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection UseAzureFunctionPipeline(this IServiceCollection services, Action<PipelineOptions> options)
        {
            services.AddScoped<IInputFilterCollection, InputFilterCollection>();
            services.AddScoped<IOutputFilterCollection, OutputFilterCollection>();
            services.AddScoped<IInputChannelCollection, InputChannelCollection>();
            services.AddScoped<IOutputChannelCollection, OutputChannelCollection>();
            services.AddScoped<IPipeline<HttpRequestData, HttpResponseData>, AzureFunctionPipeline>();
            services.Configure(options);
            return services;
        }



        /// <summary>
        /// Use a Web pipeline for Web services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection UseWebPipeline(this IServiceCollection services, Action<PipelineOptions> options)
        {
            services.AddScoped<IInputFilterCollection, InputFilterCollection>();
            services.AddScoped<IOutputFilterCollection, OutputFilterCollection>();
            services.AddScoped<IInputChannelCollection, InputChannelCollection>();
            services.AddScoped<IOutputChannelCollection, OutputChannelCollection>();
            services.AddScoped<IPipeline<HttpRequestMessage, HttpResponseMessage>, WebPipeline>();
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds an input filter.
        /// </summary>
        /// <typeparam name="TOptions">Type of options for input filter.</typeparam>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of input filter.</param>
        /// <param name="options">Options for input filter.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddInputFilter<TOptions>(this IServiceCollection services, Type type, Action<TOptions> options) where TOptions : class
        {
            services.Add(new ServiceDescriptor(typeof(IInputFilter), type, ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds an input filter.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of input filter.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddInputFilter(this IServiceCollection services, Type type)
        {
            services.Add(new ServiceDescriptor(typeof(IInputFilter), type, ServiceLifetime.Scoped));
            return services;
        }


        /// <summary>
        /// Add an output filter.
        /// </summary>
        /// <typeparam name="TOptions">Type of options for output filter.</typeparam>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of output filter.</param>
        /// <param name="options">Options for output filter.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddOutputFilter<TOptions>(this IServiceCollection services, Type type, Action<TOptions> options) where TOptions : class
        {
            services.Add(new ServiceDescriptor(typeof(IOutputFilter), type, ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds an output filter.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of output filter.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddOutputFilter(this IServiceCollection services, Type type)
        {
            services.Add(new ServiceDescriptor(typeof(IOutputFilter), type, ServiceLifetime.Scoped));
            return services;
        }

        /// <summary>
        /// Adds an input channel.
        /// </summary>
        /// <typeparam name="TOptions">Type of options for input channel.</typeparam>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of input channel.</param>
        /// <param name="options">Options for input channel.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddInputChannel<TOptions>(this IServiceCollection services, Type type, Action<TOptions> options) where TOptions : class
        {
            services.Add(new ServiceDescriptor(typeof(IInputChannel), type, ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Add an output channel.
        /// </summary>
        /// <typeparam name="TOptions">Type of options for output channel.</typeparam>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of output channel.</param>
        /// <param name="options">Options for output channel.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddOutputChannel<TOptions>(this IServiceCollection services, Type type, Action<TOptions> options) where TOptions : class
        {
            services.Add(new ServiceDescriptor(typeof(IOutputChannel), type, ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Add a binding
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of binding.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddBinding(this IServiceCollection services, Type type)
        {
            services.Add(new ServiceDescriptor(typeof(IBinding), type, ServiceLifetime.Scoped));
            return services;
        }

        /// <summary>
        /// Adds a binding.
        /// </summary>
        /// <typeparam name="TOptions">Type of options for binding.</typeparam>
        /// <param name="services">Services collection.</param>
        /// <param name="type">Type of binding.</param>
        /// <param name="options">Options for binding.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddBinding<TOptions>(this IServiceCollection services, Type type, Action<TOptions> options) where TOptions : class
        {
            services.Add(new ServiceDescriptor(typeof(IBinding), type, ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds a coupled binding.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddCoupledBinding(this IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IBinding), typeof(CoupledBinding), ServiceLifetime.Scoped));
            return services;
        }

        /// <summary>
        /// Adds a FHIR binding.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="options">Options for FHIR binding.</param>
        /// <returns>Services collection.</returns>
        public static IServiceCollection AddFhirBinding(this IServiceCollection services, Action<FhirBindingOptions> options)
        {
            services.Add(new ServiceDescriptor(typeof(IBinding), typeof(FhirBinding), ServiceLifetime.Scoped));
            services.Configure(options);
            return services;
        }

    }
}

﻿using Microsoft.Health.Fhir.Proxy.Pipelines;
using System;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Proxy.Filters
{
    /// <summary>
    /// IFilter interface to be implemented by filters.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Event signals the filter caught an error.
        /// </summary>
        event EventHandler<FilterErrorEventArgs> OnFilterError;

        /// <summary>
        /// Gets the unique id on the filter instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get the name of the filter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the filter operation.
        /// </summary>
        /// <param name="context">Context of the input for filter execution.</param>
        /// <returns>Context for input to next filter or output for http response.</returns>
        Task<OperationContext> ExecuteAsync(OperationContext context);
    }
}

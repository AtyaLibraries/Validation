// <copyright file="IValidator.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Models;

namespace Atya.Errors.Validation.Abstractions;

/// <summary>
/// Defines an asynchronous validator for a given type.
/// </summary>
/// <typeparam name="T">The type being validated.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValidationResult"/> representing the validation outcome.</returns>
    public ValueTask<ValidationResult> ValidateAsync(
        T instance,
        CancellationToken cancellationToken = default);
}

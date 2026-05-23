// <copyright file="ValidatorExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Extensions;

/// <summary>
/// Extension methods for validators.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Validates the provided instance using all validators and combines the results.
    /// </summary>
    /// <typeparam name="T">The validated type.</typeparam>
    /// <param name="validators">The validators.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A combined validation result.</returns>
    public static async ValueTask<ValidationResult> ValidateAllAsync<T>(
        this IEnumerable<IValidator<T>> validators,
        T instance,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(validators);

        var results = new List<ValidationResult>();

        foreach (var validator in validators)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.AgainstNull(validator);

            var result = await validator
                .ValidateAsync(instance, cancellationToken)
                .ConfigureAwait(false);

            Guard.AgainstNull(result);
            results.Add(result);
        }

        return ValidationResult.Combine(results);
    }

    /// <summary>
    /// Validates the provided instance and throws a validation exception when validation fails.
    /// </summary>
    /// <typeparam name="T">The validated type.</typeparam>
    /// <param name="validator">The validator.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="errorCode">The exception error code.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result when validation succeeds.</returns>
    public static async ValueTask<ValidationResult> ValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T instance,
        string message = "Validation failed.",
        string? errorCode = "validation.failed",
        IReadOnlyDictionary<string, object?>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(validator);
        cancellationToken.ThrowIfCancellationRequested();

        var result = await validator
            .ValidateAsync(instance, cancellationToken)
            .ConfigureAwait(false);

        Guard.AgainstNull(result);
        result.ThrowIfInvalid(message, errorCode, metadata);

        return result;
    }
}

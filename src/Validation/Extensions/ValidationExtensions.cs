// <copyright file="ValidationExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Exceptions;
using Atya.Errors.Validation.Internal;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Extensions;

/// <summary>
/// Extension methods for validation models.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Converts the result to a <see cref="ValidationException"/>.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="errorCode">The exception error code.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <returns>A <see cref="ValidationException"/>.</returns>
    public static ValidationException ToValidationException(
        this ValidationResult result,
        string message = "Validation failed.",
        string? errorCode = "validation.failed",
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        Guard.AgainstNull(result);
        Guard.AgainstNullOrWhiteSpace(message);

        if (result.IsValid)
        {
            throw new InvalidOperationException("Cannot create ValidationException from a valid validation result.");
        }

        var errors = result.Errors
            .Select(ValidationFailureConverter.ToExceptionItem)
            .ToArray();

        return new ValidationException(message, errors, errorCode, metadata);
    }

    /// <summary>
    /// Throws a <see cref="ValidationException"/> when the result is invalid.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="errorCode">The exception error code.</param>
    /// <param name="metadata">Optional metadata.</param>
    public static void ThrowIfInvalid(
        this ValidationResult result,
        string message = "Validation failed.",
        string? errorCode = "validation.failed",
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        Guard.AgainstNull(result);

        if (!result.IsValid)
        {
            throw result.ToValidationException(message, errorCode, metadata);
        }
    }

    /// <summary>
    /// Combines the current result with another result.
    /// </summary>
    /// <param name="left">The first result.</param>
    /// <param name="right">The second result.</param>
    /// <returns>A combined validation result.</returns>
    public static ValidationResult Merge(this ValidationResult left, ValidationResult right)
    {
        Guard.AgainstNull(left);
        Guard.AgainstNull(right);

        return ValidationResult.Combine(left, right);
    }
}

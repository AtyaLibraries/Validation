// <copyright file="ValidationFailureFactory.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Extensions;

/// <summary>
/// Factory helpers for creating common validation failures.
/// </summary>
public static class ValidationFailureFactory
{
    /// <summary>
    /// Creates a required-field validation failure.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="attemptedValue">The attempted value.</param>
    /// <returns>A <see cref="ValidationFailure"/>.</returns>
    public static ValidationFailure Required(string propertyName, object? attemptedValue = null)
    {
        Guard.AgainstNullOrWhiteSpace(propertyName);
        var normalizedPropertyName = propertyName.Trim();

        return ValidationFailure.Create(
            normalizedPropertyName,
            $"{normalizedPropertyName} is required.",
            "validation.required",
            attemptedValue);
    }

    /// <summary>
    /// Creates an invalid-value validation failure.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="message">The validation message.</param>
    /// <param name="attemptedValue">The attempted value.</param>
    /// <returns>A <see cref="ValidationFailure"/>.</returns>
    public static ValidationFailure Invalid(string propertyName, string message, object? attemptedValue = null)
    {
        return ValidationFailure.Create(
            propertyName,
            message,
            "validation.invalid",
            attemptedValue);
    }

    /// <summary>
    /// Creates an out-of-range validation failure.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="message">The validation message.</param>
    /// <param name="attemptedValue">The attempted value.</param>
    /// <returns>A <see cref="ValidationFailure"/>.</returns>
    public static ValidationFailure OutOfRange(string propertyName, string message, object? attemptedValue = null)
    {
        return ValidationFailure.Create(
            propertyName,
            message,
            "validation.out_of_range",
            attemptedValue);
    }

    /// <summary>
    /// Creates a validation failure with custom values.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="message">The validation message.</param>
    /// <param name="errorCode">The machine-readable validation error code.</param>
    /// <param name="attemptedValue">The attempted value.</param>
    /// <returns>A <see cref="ValidationFailure"/>.</returns>
    public static ValidationFailure Create(
        string propertyName,
        string message,
        string? errorCode = null,
        object? attemptedValue = null)
    {
        return ValidationFailure.Create(propertyName, message, errorCode, attemptedValue);
    }
}

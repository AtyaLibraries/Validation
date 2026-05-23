// <copyright file="ValidationFailure.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Models;

/// <summary>
/// Represents a single validation failure.
/// </summary>
public sealed record ValidationFailure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationFailure"/> class.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="message">The validation failure message.</param>
    /// <param name="errorCode">The optional machine-readable validation error code.</param>
    /// <param name="attemptedValue">The optional attempted value.</param>
    public ValidationFailure(
        string propertyName,
        string message,
        string? errorCode = null,
        object? attemptedValue = null)
    {
        Guard.AgainstNullOrWhiteSpace(propertyName);
        Guard.AgainstNullOrWhiteSpace(message);

        PropertyName = propertyName.Trim();
        Message = message.Trim();
        ErrorCode = string.IsNullOrWhiteSpace(errorCode) ? null : errorCode.Trim();
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Gets the related property or logical field name.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the validation failure message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the optional machine-readable validation error code.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the optional attempted value.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Creates a new <see cref="ValidationFailure"/> instance after validating input arguments.
    /// </summary>
    /// <param name="propertyName">The related property or logical field name.</param>
    /// <param name="message">The validation failure message.</param>
    /// <param name="errorCode">The optional machine-readable validation error code.</param>
    /// <param name="attemptedValue">The optional attempted value.</param>
    /// <returns>A new <see cref="ValidationFailure"/>.</returns>
    public static ValidationFailure Create(
        string propertyName,
        string message,
        string? errorCode = null,
        object? attemptedValue = null)
    {
        return new ValidationFailure(propertyName, message, errorCode, attemptedValue);
    }
}

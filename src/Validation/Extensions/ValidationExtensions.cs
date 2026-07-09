// <copyright file="ValidationExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Exceptions;
using Atya.Errors.Validation.Internal;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;
using Atya.Foundation.Results;

namespace Atya.Errors.Validation.Extensions;

/// <summary>
/// Extension methods for validation models.
/// </summary>
public static class ValidationExtensions
{
    private const string DefaultResultErrorCode = "atya.errors.validation.failed";
    private const string DefaultResultErrorMessage = "Validation failed.";

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

    /// <summary>
    /// Converts the validation failure to an <see cref="Error"/>.
    /// </summary>
    /// <param name="failure">The validation failure.</param>
    /// <returns>An <see cref="Error"/> with <see cref="ErrorKind.Validation"/>.</returns>
    public static Error ToError(this ValidationFailure failure)
    {
        Guard.AgainstNull(failure);

        return new Error(
            failure.ErrorCode ?? DefaultResultErrorCode,
            failure.Message,
            ErrorKind.Validation);
    }

    /// <summary>
    /// Converts the validation result to an untyped <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="errorCode">The error code used when the validation result is invalid.</param>
    /// <param name="message">The error message used when the validation result is invalid.</param>
    /// <returns>A successful result when validation succeeds; otherwise a validation failure result.</returns>
    public static Result ToResult(
        this ValidationResult result,
        string errorCode = DefaultResultErrorCode,
        string message = DefaultResultErrorMessage)
    {
        Guard.AgainstNull(result);
        Guard.AgainstNullOrWhiteSpace(errorCode);
        Guard.AgainstNullOrWhiteSpace(message);

        return result.IsValid
            ? Result.Success()
            : Result.Failure(errorCode, message, ErrorKind.Validation);
    }

    /// <summary>
    /// Converts the validation result to a typed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The success value type.</typeparam>
    /// <param name="result">The validation result.</param>
    /// <param name="value">The success value used when validation succeeds.</param>
    /// <param name="errorCode">The error code used when the validation result is invalid.</param>
    /// <param name="message">The error message used when the validation result is invalid.</param>
    /// <returns>A successful result with <paramref name="value"/> when validation succeeds; otherwise a validation failure result.</returns>
    public static Result<TValue> ToResultWithValue<TValue>(
        this ValidationResult result,
        TValue value,
        string errorCode = DefaultResultErrorCode,
        string message = DefaultResultErrorMessage)
    {
        Guard.AgainstNull(result);
        Guard.AgainstNullOrWhiteSpace(errorCode);
        Guard.AgainstNullOrWhiteSpace(message);

        return result.IsValid
            ? Result.Success(value)
            : Result.Failure<TValue>(errorCode, message, ErrorKind.Validation);
    }
}

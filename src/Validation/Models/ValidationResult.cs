// <copyright file="ValidationResult.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Models;

/// <summary>
/// Represents the outcome of a validation operation.
/// </summary>
public sealed class ValidationResult
{
    private static readonly ValidationResult s_successfulInstance = new ValidationResult(Array.Empty<ValidationFailure>());

    private ValidationResult(IReadOnlyList<ValidationFailure> errors)
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets a successful validation result.
    /// </summary>
    public static ValidationResult Success => s_successfulInstance;

    /// <summary>
    /// Gets the validation failures.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>
    /// Gets a value indicating whether validation succeeded.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Creates a validation result from a single failure.
    /// </summary>
    /// <param name="failure">The validation failure.</param>
    /// <returns>A failed <see cref="ValidationResult"/>.</returns>
    public static ValidationResult FromFailure(ValidationFailure failure)
    {
        Guard.AgainstNull(failure);

        return new ValidationResult(Array.AsReadOnly(new[] { failure }));
    }

    /// <summary>
    /// Creates a validation result from multiple failures.
    /// </summary>
    /// <param name="failures">The validation failures.</param>
    /// <returns>A validation result.</returns>
    public static ValidationResult FromFailures(IEnumerable<ValidationFailure> failures)
    {
        Guard.AgainstNull(failures);

        var items = failures.ToArray();

        if (items.Length == 0)
        {
            return Success;
        }

        if (items.Any(static failure => failure is null))
        {
            throw new ArgumentException("Validation failures collection cannot contain null items.", nameof(failures));
        }

        return new ValidationResult(Array.AsReadOnly(items));
    }

    /// <summary>
    /// Combines many validation results into a single result.
    /// </summary>
    /// <param name="results">The results to combine.</param>
    /// <returns>A combined validation result.</returns>
    public static ValidationResult Combine(IEnumerable<ValidationResult> results)
    {
        Guard.AgainstNull(results);

        var failures = new List<ValidationFailure>();

        foreach (var result in results)
        {
            Guard.AgainstNull(result);
            failures.AddRange(result.Errors);
        }

        return FromFailures(failures);
    }

    /// <summary>
    /// Combines many validation results into a single result.
    /// </summary>
    /// <param name="results">The results to combine.</param>
    /// <returns>A combined validation result.</returns>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        Guard.AgainstNull(results);
        return Combine((IEnumerable<ValidationResult>)results);
    }

    /// <summary>
    /// Creates a new failed result by appending the provided failure.
    /// </summary>
    /// <param name="failure">The failure to append.</param>
    /// <returns>A new validation result.</returns>
    public ValidationResult AddFailure(ValidationFailure failure)
    {
        Guard.AgainstNull(failure);

        if (IsValid)
        {
            return FromFailure(failure);
        }

        var failures = Errors.ToList();
        failures.Add(failure);

        return FromFailures(failures);
    }

    /// <summary>
    /// Creates a new result by appending the provided failures.
    /// </summary>
    /// <param name="failures">The failures to append.</param>
    /// <returns>A new validation result.</returns>
    public ValidationResult AddFailures(IEnumerable<ValidationFailure> failures)
    {
        Guard.AgainstNull(failures);

        var appendedFailures = failures.ToArray();
        if (appendedFailures.Any(static failure => failure is null))
        {
            throw new ArgumentException("Validation failures collection cannot contain null items.", nameof(failures));
        }

        if (appendedFailures.Length == 0)
        {
            return this;
        }

        var merged = Errors.ToList();
        merged.AddRange(appendedFailures);

        return FromFailures(merged);
    }
}

// <copyright file="CompositeValidator.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Validators;

/// <summary>
/// Combines multiple validators into one validator.
/// </summary>
/// <typeparam name="T">The validated type.</typeparam>
public sealed class CompositeValidator<T> : IValidator<T>
{
    private readonly IReadOnlyList<IValidator<T>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeValidator{T}"/> class.
    /// </summary>
    /// <param name="validators">The validators to compose.</param>
    public CompositeValidator(IEnumerable<IValidator<T>> validators)
    {
        var items = Guard.AgainstNull(validators).ToArray();

        if (items.Any(static validator => validator is null))
        {
            throw new ArgumentException("Validators collection cannot contain null items.", nameof(validators));
        }

        _validators = Array.AsReadOnly(items);
    }

    /// <summary>
    /// Validates the specified instance with all composed validators.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A combined validation result.</returns>
    public ValueTask<ValidationResult> ValidateAsync(
        T instance,
        CancellationToken cancellationToken = default)
    {
        return _validators.ValidateAllAsync(instance, cancellationToken);
    }
}

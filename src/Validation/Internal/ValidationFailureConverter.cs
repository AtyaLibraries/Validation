// <copyright file="ValidationFailureConverter.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Exceptions.Models;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Guards;

namespace Atya.Errors.Validation.Internal;

/// <summary>
/// Converts validation failures into exception payload items.
/// </summary>
internal static class ValidationFailureConverter
{
    /// <summary>
    /// Converts a validation failure into a validation exception item.
    /// </summary>
    /// <param name="failure">The validation failure to convert.</param>
    /// <returns>A converted validation exception item.</returns>
    internal static ValidationExceptionItem ToExceptionItem(ValidationFailure failure)
    {
        Guard.AgainstNull(failure);

        return new ValidationExceptionItem(
            failure.PropertyName,
            failure.Message,
            failure.ErrorCode,
            failure.AttemptedValue);
    }
}

// <copyright file="ValidationFailureTestData.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Models;
using Atya.Governance.Testing.Builders;

namespace Validation.UnitTests;

internal static class ValidationFailureTestData
{
    public static TestValidationFailure Build(
        string propertyName = "Name",
        string errorMessage = "Validation failed.",
        string? errorCode = null,
        object? attemptedValue = null)
    {
        return ValidationFailureBuilder
            .Create()
            .WithPropertyName(propertyName)
            .WithErrorMessage(errorMessage)
            .WithErrorCode(errorCode)
            .WithAttemptedValue(attemptedValue)
            .Build();
    }

    public static ValidationFailure Create(
        string propertyName = "Name",
        string errorMessage = "Validation failed.",
        string? errorCode = null,
        object? attemptedValue = null)
    {
        var failure = Build(propertyName, errorMessage, errorCode, attemptedValue);

        return ValidationFailure.Create(
            failure.PropertyName,
            failure.ErrorMessage,
            failure.ErrorCode,
            failure.AttemptedValue);
    }

    public static void ShouldMatch(this ValidationFailure actual, TestValidationFailure expected)
    {
        actual.PropertyName.Should().Be(expected.PropertyName);
        actual.Message.Should().Be(expected.ErrorMessage);
        actual.ErrorCode.Should().Be(expected.ErrorCode);
        actual.AttemptedValue.Should().Be(expected.AttemptedValue);
    }
}

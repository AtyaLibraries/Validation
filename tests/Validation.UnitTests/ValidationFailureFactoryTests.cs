// <copyright file="ValidationFailureFactoryTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Extensions;

namespace Validation.UnitTests;

public sealed class ValidationFailureFactoryTests
{
    [Fact]
    public void Required_Should_Create_Required_Failure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Email is required.",
            errorCode: "validation.required");

        var failure = ValidationFailureFactory.Required("Email");

        failure.ShouldMatch(expected);
    }

    [Fact]
    public void Required_Should_Trim_PropertyName_Before_Building_Message()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Email is required.",
            errorCode: "validation.required");

        var failure = ValidationFailureFactory.Required(" Email ");

        failure.ShouldMatch(expected);
    }

    [Fact]
    public void Required_Should_Throw_When_PropertyName_Is_Empty()
    {
        var act = () => ValidationFailureFactory.Required(" ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Invalid_Should_Create_Invalid_Failure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Email is invalid.",
            errorCode: "validation.invalid",
            attemptedValue: "bad");

        var failure = ValidationFailureFactory.Invalid("Email", "Email is invalid.", "bad");

        failure.ShouldMatch(expected);
    }

    [Fact]
    public void OutOfRange_Should_Create_OutOfRange_Failure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Age",
            errorMessage: "Age must be greater than zero.",
            errorCode: "validation.out_of_range",
            attemptedValue: 0);

        var failure = ValidationFailureFactory.OutOfRange("Age", "Age must be greater than zero.", 0);

        failure.ShouldMatch(expected);
    }

    [Fact]
    public void Create_Should_Create_Custom_Failure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Name",
            errorMessage: "Name is too short.",
            errorCode: "validation.length",
            attemptedValue: "a");

        var failure = ValidationFailureFactory.Create("Name", "Name is too short.", "validation.length", "a");

        failure.ShouldMatch(expected);
    }
}

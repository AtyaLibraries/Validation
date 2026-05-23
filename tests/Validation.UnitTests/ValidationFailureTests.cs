// <copyright file="ValidationFailureTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Models;

namespace Validation.UnitTests;

public sealed class ValidationFailureTests
{
    [Fact]
    public void Constructor_Should_Return_ValidationFailure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Email is required.",
            errorCode: "validation.required",
            attemptedValue: "abc");

        var result = new ValidationFailure("Email", "Email is required.", "validation.required", "abc");

        result.ShouldMatch(expected);
    }

    [Fact]
    public void Constructor_Should_Trim_Values_And_Normalize_Empty_ErrorCode()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Invalid email.",
            attemptedValue: "abc");

        var result = new ValidationFailure(" Email ", " Invalid email. ", " ", "abc");

        result.ShouldMatch(expected);
    }

    [Fact]
    public void Create_Should_Return_ValidationFailure()
    {
        var expected = ValidationFailureTestData.Build(
            propertyName: "Email",
            errorMessage: "Email is required.",
            errorCode: "validation.required",
            attemptedValue: "abc");

        var result = ValidationFailure.Create("Email", "Email is required.", "validation.required", "abc");

        result.ShouldMatch(expected);
    }

    [Fact]
    public void Constructor_Should_Throw_When_PropertyName_Is_Null()
    {
        var act = () => new ValidationFailure(null!, "Message");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_Throw_When_PropertyName_Is_Empty()
    {
        var act = () => new ValidationFailure(" ", "Message");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Should_Throw_When_Message_Is_Null()
    {
        var act = () => new ValidationFailure("Email", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_Throw_When_Message_Is_Empty()
    {
        var act = () => new ValidationFailure("Email", " ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Throw_When_PropertyName_Is_Empty()
    {
        var act = () => ValidationFailure.Create(" ", "Message");

        act.Should().Throw<ArgumentException>();
    }
}

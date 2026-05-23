// <copyright file="Program.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Validation.Validators;

namespace Validation.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}

[MemoryDiagnoser]
[RankColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ValidationResultBenchmarks
{
    private readonly ValidationFailure[] _failures =
    {
        ValidationFailure.Create("Name", "Name is required.", "validation.required"),
        ValidationFailure.Create("Email", "Email is invalid.", "validation.invalid", "bad"),
        ValidationFailure.Create("Age", "Age must be greater than zero.", "validation.out_of_range", 0),
        ValidationFailure.Create("TenantId", "Tenant is required.", "validation.required"),
    };

    private ValidationResult _singleFailureResult = ValidationResult.Success;
    private ValidationResult _secondFailureResult = ValidationResult.Success;

    [GlobalSetup]
    public void Setup()
    {
        _singleFailureResult = ValidationResult.FromFailure(_failures[0]);
        _secondFailureResult = ValidationResult.FromFailure(_failures[1]);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("result")]
    public ValidationResult FromSingleFailure()
    {
        return ValidationResult.FromFailure(_failures[0]);
    }

    [Benchmark]
    [BenchmarkCategory("result")]
    public ValidationResult FromMultipleFailures()
    {
        return ValidationResult.FromFailures(_failures);
    }

    [Benchmark]
    [BenchmarkCategory("result")]
    public ValidationResult CombineTwoResults()
    {
        return ValidationResult.Combine(_singleFailureResult, _secondFailureResult);
    }

    [Benchmark]
    [BenchmarkCategory("result")]
    public ValidationResult AppendFailure()
    {
        return _singleFailureResult.AddFailure(_failures[1]);
    }
}

[MemoryDiagnoser]
[RankColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ValidatorExecutionBenchmarks
{
    private readonly CreateCustomerCommand _validCommand = new CreateCustomerCommand("Ada Lovelace", "ada@example.com", 36);
    private readonly CreateCustomerCommand _invalidCommand = new CreateCustomerCommand(string.Empty, "not-an-email", 0);

    private IValidator<CreateCustomerCommand>[] _validators = Array.Empty<IValidator<CreateCustomerCommand>>();
    private CompositeValidator<CreateCustomerCommand> _compositeValidator = new CompositeValidator<CreateCustomerCommand>(
        Array.Empty<IValidator<CreateCustomerCommand>>());

    [GlobalSetup]
    public void Setup()
    {
        _validators =
        [
            new NameRequiredValidator(),
            new EmailFormatValidator(),
            new AgeRangeValidator(),
        ];

        _compositeValidator = new CompositeValidator<CreateCustomerCommand>(_validators);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("execution")]
    public async ValueTask<ValidationResult> ValidateAllValidCommand()
    {
        return await _validators.ValidateAllAsync(_validCommand);
    }

    [Benchmark]
    [BenchmarkCategory("execution")]
    public async ValueTask<ValidationResult> CompositeValidCommand()
    {
        return await _compositeValidator.ValidateAsync(_validCommand);
    }

    [Benchmark]
    [BenchmarkCategory("execution")]
    public async ValueTask<ValidationResult> ValidateAllInvalidCommand()
    {
        return await _validators.ValidateAllAsync(_invalidCommand);
    }

    [Benchmark]
    [BenchmarkCategory("execution")]
    public async ValueTask<ValidationResult> CompositeInvalidCommand()
    {
        return await _compositeValidator.ValidateAsync(_invalidCommand);
    }

    private sealed record CreateCustomerCommand(string Name, string Email, int Age);

    private sealed class NameRequiredValidator : IValidator<CreateCustomerCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CreateCustomerCommand instance,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return string.IsNullOrWhiteSpace(instance.Name)
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailure.Create("Name", "Name is required.", "validation.required", instance.Name)))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class EmailFormatValidator : IValidator<CreateCustomerCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CreateCustomerCommand instance,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return instance.Email.Contains('@', StringComparison.Ordinal)
                ? ValueTask.FromResult(ValidationResult.Success)
                : ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailure.Create("Email", "Email must contain '@'.", "validation.invalid", instance.Email)));
        }
    }

    private sealed class AgeRangeValidator : IValidator<CreateCustomerCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CreateCustomerCommand instance,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return instance.Age > 0
                ? ValueTask.FromResult(ValidationResult.Success)
                : ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailure.Create("Age", "Age must be greater than zero.", "validation.out_of_range", instance.Age)));
        }
    }
}

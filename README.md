# Validation

`Validation` is the repository for the `Atya.Errors.Validation` NuGet package.

| | |
| --- | --- |
| Repository | [https://github.com/AtyaLibraries/Validation](https://github.com/AtyaLibraries/Validation) |
| NuGet | `Atya.Errors.Validation` |
| License | MIT |

This package provides validation models, validator contracts, composition helpers,
conversion into `Atya.Errors.Exceptions.ValidationException`, and additive
conversion into `Atya.Foundation.Results`.

## Included APIs

- `IValidator<T>`
- `ValidationFailure`
- `ValidationResult`
- `ValidationFailureFactory`
- `ValidationExtensions`
- `ValidatorExtensions`
- `CompositeValidator<T>`

## Quick start

```csharp
using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Validation.Validators;
using Atya.Foundation.Results;

var validator = new CompositeValidator<CreateCustomerCommand>(
[
    new NameRequiredValidator(),
    new EmailFormatValidator(),
]);

var result = await validator.ValidateAsync(new CreateCustomerCommand("", "bad"));
Result commandResult = result.ToResult();

if (!result.IsValid)
{
    result.ThrowIfInvalid();
}
```

See `samples/Validation.Samples.Console` for a runnable example.

## Results integration

`ValidationFailure.ToError()` creates an `ErrorKind.Validation` error.
`ValidationResult.ToResult()` and `ValidationResult.ToResultWithValue(value)` convert
validation outcomes to untyped and typed Results. Invalid validation results use
`atya.errors.validation.failed` by default unless the caller supplies a specific
error code and message.

## Layout

```text
.
|-- src/Validation/
|-- tests/Validation.UnitTests/
|-- samples/Validation.Samples.Console/
|-- benchmarks/Validation.Benchmarks/
`-- .github/
```

## Build, test, pack

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test ./tests/Validation.UnitTests/Validation.UnitTests.csproj --configuration Release --no-build --collect "XPlat Code Coverage"
dotnet pack ./src/Validation/Validation.csproj --configuration Release --no-build --output artifacts/packages -p:EnablePackageValidation=true
```

Artifacts land in `artifacts/packages/`.

## Versioning

Versions are derived from git tags via [MinVer](https://github.com/adamralph/minver).
Merges to `master` publish stable NuGet packages through
`.github/workflows/publish-nuget.yml`, which creates the version tag and GitHub
Release after a successful publish.

## Compatibility

The package intentionally targets `net10.0`.

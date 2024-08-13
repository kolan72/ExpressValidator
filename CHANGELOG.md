## 0.0.16

- Support validation for nullable value types.
- Slightly improved performance by removing the restriction on the type of instance to validate in the `TypeValidatorBase<T>.Prevalidate` method.
- Make `_nullMessageProvider`, `_propName` and `_rule` `TypeValidatorBase<T>` class fields private instead of protected.
- Remove `IExpressPropertyValidatorBase` interface.
- Remove redundant `ExpressPropertyValidator` constructor.  


## 0.0.14

- Introduce `ExpressValidatorBuilder.AddFunc` method.
- Update to FluentValidation 11.9.1.
- Made the `BuilderWithPropValidator` constructor internal.
- Add `TObj` generic param to `IExpressPropertyValidator`, `ExpressPropertyValidator`.


## 0.0.10

- Support for adding a field for validation - introduced `ExpressValidatorBuilder.AddField` method.
- Made `IExpressPropertyValidatorBase` internal.
- Get rid of the `ExpressAsyncPropertyValidator`, `ExpressPropertyValidator` classes, rename `ExpressPropertyValidatorBase` to `ExpressPropertyValidator` and use it in sync and async scenarios.
- Remove the `BuilderWithPropValidator.PropertyValidator` property, make the `ExpressValidatorBuilder` property private.
- Move `IsAsync` property from `ExpressValidators` to `TypeValidators`.
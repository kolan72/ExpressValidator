using FluentValidation;
using NUnit.Framework;

namespace ExpressValidator.Tests
{
    /// <summary>
    /// Tests for <see cref="ExpressValidatorBuilder{TObj}.AddMember{T}"/>
    /// and <see cref="ExpressValidatorBuilder{TObj,TOptions}.AddMember{T}"/>.
    /// </summary>
    [TestFixture]
    public class ExpressValidatorBuilderAddMemberTests
    {
        // ── test doubles ────────────────────────────────────────────────────────

        private class SampleObj
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        /// <summary>Object with a public field (not a property).</summary>
        private class SampleObjWithField
        {
            public string Label;   // field, not a property
        }

        private class SampleOptions
        {
            public int MinAge { get; set; }
        }

        // ── ExpressValidatorBuilder<TObj> ────────────────────────────────────

        [Test]
        public void Should_ReturnValidResult_When_PropertyExpressionPassesValidation()
        {
            var obj = new SampleObj { Name = "Alice" };

            var result = new ExpressValidatorBuilder<SampleObj>()
                .AddMember(o => o.Name)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_PropertyExpressionFailsValidation()
        {
            var obj = new SampleObj { Name = string.Empty };

            var result = new ExpressValidatorBuilder<SampleObj>()
                .AddMember(o => o.Name)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UsePropertyName_When_PropertyValidationFails()
        {
            var obj = new SampleObj { Name = string.Empty };

            var result = new ExpressValidatorBuilder<SampleObj>()
                .AddMember(o => o.Name)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(SampleObj.Name)));
        }

        [Test]
        public void Should_ReturnValidResult_When_FieldExpressionPassesValidation()
        {
            var obj = new SampleObjWithField { Label = "hello" };

            var result = new ExpressValidatorBuilder<SampleObjWithField>()
                .AddMember(o => o.Label)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_FieldExpressionFailsValidation()
        {
            var obj = new SampleObjWithField { Label = string.Empty };

            var result = new ExpressValidatorBuilder<SampleObjWithField>()
                .AddMember(o => o.Label)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UseFieldName_When_FieldValidationFails()
        {
            var obj = new SampleObjWithField { Label = string.Empty };

            var result = new ExpressValidatorBuilder<SampleObjWithField>()
                .AddMember(o => o.Label)
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(SampleObjWithField.Label)));
        }

        [Test]
        public void Should_ThrowArgumentException_When_ExpressionIsNotMemberAccess()
        {
            // A method call is not a property or field — must throw.
            Assert.That(
                () => new ExpressValidatorBuilder<SampleObj>()
                          .AddMember(o => o.Name.ToUpper()),
                Throws.ArgumentException);
        }

        [Test]
        public void Should_ThrowArgumentException_When_ExpressionIsConstant()
        {
            Assert.That(
                () => new ExpressValidatorBuilder<SampleObj>()
                          .AddMember(_ => "constant"),
                Throws.ArgumentException);
        }

        [Test]
        public void Should_ValidateCorrectValue_When_PropertyHasMultipleInstances()
        {
            // Ensures the compiled accessor reads from the supplied instance, not a captured one.
            var passing = new SampleObj { Age = 18 };
            var failing = new SampleObj { Age = 0 };

            var validator = new ExpressValidatorBuilder<SampleObj>()
                .AddMember(o => o.Age)
                .WithValidation(r => r.GreaterThan(0))
                .Build();

            Assert.That(validator.Validate(passing).IsValid, Is.True);
            Assert.That(validator.Validate(failing).IsValid, Is.False);
        }

        [Test]
        public void Should_AccumulateAllFailures_When_MultipleAddMemberCallsAreChained()
        {
            var obj = new SampleObj { Name = string.Empty, Age = 0 };

            var result = new ExpressValidatorBuilder<SampleObj>()
                .AddMember(o => o.Name)
                .WithValidation(r => r.NotEmpty())
                .AddMember(o => o.Age)
                .WithValidation(r => r.GreaterThan(0))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors, Has.Count.EqualTo(2));
        }

        [Test]
        public void Should_StopAtFirstFailure_When_ValidationModeIsBreak()
        {
            var obj = new SampleObj { Name = string.Empty, Age = 0 };

            var result = new ExpressValidatorBuilder<SampleObj>(OnFirstPropertyValidatorFailed.Break)
                .AddMember(o => o.Name)
                .WithValidation(r => r.NotEmpty())
                .AddMember(o => o.Age)
                .WithValidation(r => r.GreaterThan(0))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors, Has.Count.EqualTo(1));
        }

        // ── ExpressValidatorBuilder<TObj, TOptions> ──────────────────────────

        [Test]
        public void Should_ReturnValidResult_When_TOptions_PropertyExpressionPassesValidation()
        {
            var obj = new SampleObj { Age = 21 };
            var options = new SampleOptions { MinAge = 18 };

            var result = new ExpressValidatorBuilder<SampleObj, SampleOptions>()
                .AddMember(o => o.Age)
                .WithValidation((opts, r) => r.GreaterThanOrEqualTo(opts.MinAge))
                .Build(options)
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_TOptions_PropertyExpressionFailsValidation()
        {
            var obj = new SampleObj { Age = 16 };
            var options = new SampleOptions { MinAge = 18 };

            var result = new ExpressValidatorBuilder<SampleObj, SampleOptions>()
                .AddMember(o => o.Age)
                .WithValidation((opts, r) => r.GreaterThanOrEqualTo(opts.MinAge))
                .Build(options)
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UsePropertyName_When_TOptions_PropertyValidationFails()
        {
            var obj = new SampleObj { Age = 16 };
            var options = new SampleOptions { MinAge = 18 };

            var result = new ExpressValidatorBuilder<SampleObj, SampleOptions>()
                .AddMember(o => o.Age)
                .WithValidation((opts, r) => r.GreaterThanOrEqualTo(opts.MinAge))
                .Build(options)
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(SampleObj.Age)));
        }

        [Test]
        public void Should_ReturnValidResult_When_TOptions_FieldExpressionPassesValidation()
        {
            var obj = new SampleObjWithField { Label = "valid" };
            var options = new SampleOptions();

            var result = new ExpressValidatorBuilder<SampleObjWithField, SampleOptions>()
                .AddMember(o => o.Label)
                .WithValidation((_, r) => r.NotEmpty())
                .Build(options)
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_TOptions_FieldExpressionFailsValidation()
        {
            var obj = new SampleObjWithField { Label = string.Empty };
            var options = new SampleOptions();

            var result = new ExpressValidatorBuilder<SampleObjWithField, SampleOptions>()
                .AddMember(o => o.Label)
                .WithValidation((_, r) => r.NotEmpty())
                .Build(options)
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_ThrowArgumentException_When_TOptions_ExpressionIsNotMemberAccess()
        {
            Assert.That(
                () => new ExpressValidatorBuilder<SampleObj, SampleOptions>()
                          .AddMember(o => o.Name.ToUpper()),
                Throws.ArgumentException);
        }

        [Test]
        public void Should_ApplyOptionsCorrectly_When_TOptions_DifferentOptionsProduceDifferentOutcomes()
        {
            var obj = new SampleObj { Age = 17 };

            var validator = new ExpressValidatorBuilder<SampleObj, SampleOptions>()
                .AddMember(o => o.Age)
                .WithValidation((opts, r) => r.GreaterThanOrEqualTo(opts.MinAge));

            // Same object, different options — outcome must differ.
            Assert.That(validator.Build(new SampleOptions { MinAge = 18 }).Validate(obj).IsValid, Is.False);
            Assert.That(validator.Build(new SampleOptions { MinAge = 16 }).Validate(obj).IsValid, Is.True);
        }
    }
}

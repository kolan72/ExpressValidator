using FluentValidation;
using NUnit.Framework;

namespace ExpressValidator.Tests
{
	/// <summary>
	/// Tests for <see cref="ExpressValidatorBuilder{TObj,TOptions}.AddProperty{T}"/>
	/// with indexed properties. These tests verify the branch where
	/// <c>MemberInfoParser.TryParse(func, MemberTypes.Property)</c> is false
	/// and <c>MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters)</c> is true.
	/// </summary>
	[TestFixture]
	public class ExpressValidatorBuilderAddIndexedPropertyTOptionsTests
	{
		[Test]
		public void Should_ReturnValidResult_When_IndexedValuePassesValidationWithOptions()
		{
			var obj = new ObjWithIntIndexer("hello", "world");
			var options = new ObjWithIntIndexerOptions { MinLength = 3 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_ReturnInvalidResult_When_IndexedValueFailsValidationWithOptions()
		{
			var obj = new ObjWithIntIndexer("hi", "world");
			var options = new ObjWithIntIndexerOptions { MinLength = 3 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.False);
		}

		[Test]
		public void Should_UseCorrectPropertyName_ForIndexedPropertyWithOptions()
		{
			var obj = new ObjWithIntIndexer("hi", "world");
			var options = new ObjWithIntIndexerOptions { MinLength = 3 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[index]"));
		}

		[Test]
		public void Should_DifferentIndexValues_ProduceDifferentValidationResultsWithOptions()
		{
			var passing = new ObjWithIntIndexer("validvalue", "short");
			var failing = new ObjWithIntIndexer("hi", "world");
			var options = new ObjWithIntIndexerOptions { MinLength = 5 };

			var validator = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(validator.Build(options).Validate(passing).IsValid, Is.True);
				Assert.That(validator.Build(options).Validate(failing).IsValid, Is.False);
			}
		}

		[Test]
		public void Should_CheckMaxLength_When_OptionsConfigured()
		{
			var obj = new ObjWithIntIndexer("tooooooooooolong", "short");
			var options = new ObjWithIntIndexerOptions { MaxLength = 5 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MaximumLength(opts.MaxLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.False);
		}

		[Test]
		public void Should_ReturnValidResult_When_ValueWithinMaxLength()
		{
			var obj = new ObjWithIntIndexer("ok", "longvalue");
			var options = new ObjWithIntIndexerOptions { MaxLength = 5 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MaximumLength(opts.MaxLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_CheckExpectedValue_WithEqualsValidation()
		{
			var obj = new ObjWithIntIndexer("expected", "other");
			var options = new ObjWithIntIndexerOptions { ExpectedValue = "expected" };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.Equal(opts.ExpectedValue))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_FailCheckExpectedValue_When_ValueMismatch()
		{
			var obj = new ObjWithIntIndexer("wrong", "other");
			var options = new ObjWithIntIndexerOptions { ExpectedValue = "expected" };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.Equal(opts.ExpectedValue))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.False);
		}

		[Test]
		public void Should_AccumulateAllFailures_When_MultipleIndexedPropertiesChainedWithOptions()
		{
			var obj = new ObjWithIntIndexer("hi", "bye");
			var options = new ObjWithIntIndexerOptions { MinLength = 4 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.AddProperty(x => x[1])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.Errors, Has.Count.EqualTo(2));
		}

		[Test]
		public void Should_StopAtFirstFailure_When_BreakModeEnabledAtIndexedChaining()
		{
			var obj = new ObjWithIntIndexer("hi", "bye");
			var options = new ObjWithIntIndexerOptions { MinLength = 4 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>(OnFirstPropertyValidatorFailed.Break)
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.AddProperty(x => x[1])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.Errors, Has.Count.EqualTo(1));
		}

		[Test]
		public void Should_HandleIndexerAccessOnDifferentIndices()
		{
			var obj = new ObjWithIntIndexer("first", "second", "third");
			var options = new ObjWithIntIndexerOptions { ExpectedValue = "second" };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[1])
				.WithValidation((opts, r) => r.Equal(opts.ExpectedValue))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_ReturnIBuilderWithPropValidator_When_IndexedPropertyAddedWithOptions()
		{
			var builder = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>();
			var result = builder.AddProperty(x => x[0]);

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void Should_BuildValidator_ReturnNotNull()
		{
			var options = new ObjWithIntIndexerOptions { MinLength = 3, MaxLength = 100 };

			var validator = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.Length(opts.MinLength, opts.MaxLength))
				.Build(options);

			Assert.That(validator, Is.Not.Null);
		}

		[Test]
		public void Should_NotThrow_When_ValidOptionsProvided()
		{
			var obj = new ObjWithIntIndexer("hello");
			var options = new ObjWithIntIndexerOptions { MinLength = 3, MaxLength = 100 };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength).MaximumLength(opts.MaxLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_ReturnErrorCountEqualToFailures_When_MixedOutcomesOnChainedIndexers()
		{
			var obj = new ObjWithIntIndexer("hello", "hi");
			var options = new ObjWithIntIndexerOptions { MinLength = 3, ExpectedValue = "hello" };

			var result = new ExpressValidatorBuilder<ObjWithIntIndexer, ObjWithIntIndexerOptions>()
				.AddProperty(x => x[0])
				.WithValidation((opts, r) => r.Equal(opts.ExpectedValue))
				.AddProperty(x => x[1])
				.WithValidation((opts, r) => r.MinimumLength(opts.MinLength))
				.Build(options)
				.Validate(obj);

			Assert.That(result.Errors, Has.Count.EqualTo(1));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[index]"));
		}
	}
}

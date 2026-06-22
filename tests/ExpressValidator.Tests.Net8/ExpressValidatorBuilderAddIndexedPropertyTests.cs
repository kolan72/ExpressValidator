using FluentValidation;
using NUnit.Framework;

namespace ExpressValidator.Tests.Net8
{
    /// <summary>
    /// Tests for <see cref="ExpressValidatorBuilder{TObj}.AddProperty{T}"/> with indexed properties.
    /// These tests verify the branch where <c>MemberInfoParser.TryParse(func, MemberTypes.Property)</c> is false
    /// and <c>MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters)</c> is true.
    /// </summary>
    [TestFixture]
    public class ExpressValidatorBuilderAddIndexedPropertyTests
    {
        [Test]
        public void Should_ReturnValidResult_When_IndexedValuePassesValidation()
        {
            var obj = new ObjWithSingleIndexer("hello", "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_IndexedValueFailsValidation()
        {
            var obj = new ObjWithSingleIndexer("", "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UseCorrectPropertyName_ForSingleParameterIndexer()
        {
            var obj = new ObjWithSingleIndexer("", "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[index]"));
        }

        [Test]
        public void Should_DifferentIndexValues_ProduceDifferentValidationResults()
        {
            var passing = new ObjWithSingleIndexer("valid");
            var failing = new ObjWithSingleIndexer("");

            var validator = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(validator.Validate(passing).IsValid, Is.True);
				Assert.That(validator.Validate(failing).IsValid, Is.False);
			}
		}

        [Test]
        public void Should_ValidateCorrectIndexElement_ByArrayAccess()
        {
            var obj = new ObjWithSingleIndexer("first", "second", "third");

            var result1 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.Equal("first"))
                .Build()
                .Validate(obj);

            var result2 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[1])
                .WithValidation(r => r.Equal("second"))
                .Build()
                .Validate(obj);

            var result3 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[2])
                .WithValidation(r => r.Equal("wrong"))
                .Build()
                .Validate(obj);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result1.IsValid, Is.True);
				Assert.That(result2.IsValid, Is.True);
				Assert.That(result3.IsValid, Is.False);
			}
		}

        [Test]
        public void Should_ReturnValidResult_When_StringIndexerValuePassesValidation()
        {
            var obj = new ObjWithStringIndexer();
            obj.Set("score", 95.0);

            var result = new ExpressValidatorBuilder<ObjWithStringIndexer>()
                .AddProperty(x => x["score"])
                .WithValidation(r => r.GreaterThan(50))
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_StringIndexerValueFailsValidation()
        {
            var obj = new ObjWithStringIndexer();
            obj.Set("score", 30.0);

            var result = new ExpressValidatorBuilder<ObjWithStringIndexer>()
                .AddProperty(x => x["score"])
                .WithValidation(r => r.GreaterThan(50))
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UseCorrectPropertyName_ForStringIndexer()
        {
            var obj = new ObjWithStringIndexer();
            obj.Set("score", 30.0);

            var result = new ExpressValidatorBuilder<ObjWithStringIndexer>()
                .AddProperty(x => x["score"])
                .WithValidation(r => r.GreaterThan(50))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[key]"));
        }

        [Test]
        public void Should_AccumulateAllFailures_When_MultipleIndexedPropertiesAreChained()
        {
            var obj = new ObjWithSingleIndexer("", "bad");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .AddProperty(x => x[1])
                .WithValidation(r => r.Equal("good"))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors, Has.Count.EqualTo(2));
        }

        [Test]
        public void Should_StopAtFirstFailure_When_ValidationModeIsBreak_AndIndexersAreChained()
        {
            var obj = new ObjWithSingleIndexer("", "bad");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>(OnFirstPropertyValidatorFailed.Break)
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .AddProperty(x => x[1])
                .WithValidation(r => r.Equal("good"))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors, Has.Count.EqualTo(1));
        }

        [Test]
        public void Should_HandleNullableIndexedValue_WhenIndexerReturnsNullable()
        {
            var obj = new ObjWithSingleIndexer("value");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0])
                .WithValidation(r => r.NotNull())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Should_CreateValidator_ReturnIBuilderWithPropValidator()
        {
            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddProperty(x => x[0]);

            Assert.That(result, Is.Not.Null);
        }
    }
}

using FluentValidation;

namespace ExpressValidator.Tests.Net8
{
    /// <summary>
    /// Tests for <see cref="ExpressValidatorBuilder{TObj}.AddMember{T}"/> with indexed properties.
    /// These tests verify the branch where <c>MemberInfoParser.TryParse(func, out MemberInfo)</c> is false
    /// and <c>MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters)</c> is true.
    /// </summary>
    [TestFixture]
    public class ExpressValidatorBuilderAddMemberIndexedPropertyTests
    {
        [Test]
        public void Should_ReturnValidResult_When_IndexedValuePassesValidation()
        {
            var obj = new ObjWithSingleIndexer("hello", "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
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
                .AddMember(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_UseCorrectPropertyName_ForIntIndexer()
        {
            var obj = new ObjWithSingleIndexer("", "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[index]"));
        }

        [Test]
        public void Should_UseCorrectPropertyName_ForStringIndexer()
        {
            var obj = new ObjWithStringIndexer();
            obj.Set("score", 30.0);

            var result = new ExpressValidatorBuilder<ObjWithStringIndexer>()
                .AddMember(x => x["score"])
                .WithValidation(r => r.GreaterThan(50))
                .Build()
                .Validate(obj);

            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("this[key]"));
        }

        [Test]
        public void Should_DifferentIndexValues_ProduceDifferentValidationResults()
        {
            var passing = new ObjWithSingleIndexer("valid");
            var failing = new ObjWithSingleIndexer("");

            var validator = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
                .WithValidation(r => r.NotEmpty())
                .Build();

            Assert.That(validator.Validate(passing).IsValid, Is.True);
            Assert.That(validator.Validate(failing).IsValid, Is.False);
        }

        [Test]
        public void Should_ValidateCorrectIndexElement_ByArrayAccess()
        {
            var obj = new ObjWithSingleIndexer("first", "second", "third");

            var result1 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
                .WithValidation(r => r.Equal("first"))
                .Build()
                .Validate(obj);

            var result2 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[1])
                .WithValidation(r => r.Equal("second"))
                .Build()
                .Validate(obj);

            var result3 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[2])
                .WithValidation(r => r.Equal("wrong"))
                .Build()
                .Validate(obj);

            Assert.That(result1.IsValid, Is.True);
            Assert.That(result2.IsValid, Is.True);
            Assert.That(result3.IsValid, Is.False);
        }

        [Test]
        public void Should_ReturnValidResult_When_StringIndexerValuePassesValidation()
        {
            var obj = new ObjWithStringIndexer();
            obj.Set("score", 95.0);

            var result = new ExpressValidatorBuilder<ObjWithStringIndexer>()
                .AddMember(x => x["score"])
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
                .AddMember(x => x["score"])
                .WithValidation(r => r.GreaterThan(50))
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_ReturnInvalidResult_When_IntIndexerValueIsNull()
        {
            var obj = new ObjWithSingleIndexer(null!, "world");

            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
                .WithValidation(r => r.NotNull())
                .Build()
                .Validate(obj);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Should_NotThrow_When_IndexerBoundsAreValid()
        {
            var obj = new ObjWithSingleIndexer("a", "b", "c");

            Assert.DoesNotThrow(() =>
            {
                new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                    .AddMember(x => x[0])
                    .WithValidation(r => r.NotEmpty())
                    .Build()
                    .Validate(obj);
            });
        }

        [Test]
        public void Should_CreateValidator_ReturnIBuilderWithPropValidator()
        {
            var result = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0]);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Should_AddMemberWithCompiledFunc_ForDuplicateIndexes()
        {
            var obj = new ObjWithSingleIndexer("pass", "fail");

            var validator1 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[0])
                .WithValidation(r => r.Equal("pass"))
                .Build();

            var validator2 = new ExpressValidatorBuilder<ObjWithSingleIndexer>()
                .AddMember(x => x[1])
                .WithValidation(r => r.Equal("pass"))
                .Build();

            Assert.That(validator1.Validate(obj).IsValid, Is.True);
            Assert.That(validator2.Validate(obj).IsValid, Is.False);
        }
    }
}

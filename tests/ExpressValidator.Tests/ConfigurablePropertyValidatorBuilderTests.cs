using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace ExpressValidator.Tests
{
	[TestFixture]
	public class ConfigurablePropertyValidatorBuilderTests
	{
		private class TestModel
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}

		private class NestedModel
		{
			public string Email { get; set; }
			public string Phone { get; set; }
		}

		private class TestOptions
		{
			public string CustomMessage { get; set; }
			public int MaxLength { get; set; }
		}

		[Test]
		public void Should_ReturnTrue_When_NestedValidatorPasses()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().EmailAddress()));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var validValue = new NestedModel { Email = "test@example.com" };

			// Act
			var result = validator.IsValid(context, validValue);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void Should_ReturnFalse_When_NestedValidatorFails()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().EmailAddress()));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "invalid-email" };

			// Act
			var result = validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public void Should_SetDefaultMessageTemplate_When_ValidationFails()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().WithMessage("Email is required")));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);
			var messageTemplate = validator.GetType()?
				.GetMethod("GetDefaultMessageTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
				.Invoke(validator, [""]) as string;

			// Assert
			Assert.That(messageTemplate, Is.Not.Null);
			Assert.That(messageTemplate, Does.Contain("Email is required"));
		}

		[Test]
		public void Should_UseCustomMessageTemplateFactory_When_Provided()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			builder.WithMessageTemplate((ctx, value, res) => $"Custom error: {res.Errors.Count} errors found");

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);
			var messageTemplate = validator.GetType()?
				.GetMethod("GetDefaultMessageTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
				.Invoke(validator, new object[] { "" }) as string;

			// Assert
			Assert.That(messageTemplate, Is.EqualTo("Custom error: 1 errors found"));
		}

		[Test]
		public void Should_AppendArgumentsToMessageFormatter_When_TemplateFactoryIsSet()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			var options = new TestOptions { CustomMessage = "TestMessage", MaxLength = 50 };
			builder.WithMessageTemplate((ctx, value, res) => "Error occurred")
				.WithTemplateArgument("CustomArg", o => o.CustomMessage)
				.WithTemplateArgument("MaxLengthArg", o => o.MaxLength);

			var validator = builder.Build(options);
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(context.MessageFormatter.PlaceholderValues.ContainsKey("CustomArg"), Is.True);
			Assert.That(context.MessageFormatter.PlaceholderValues["CustomArg"], Is.EqualTo("TestMessage"));
			Assert.That(context.MessageFormatter.PlaceholderValues.ContainsKey("MaxLengthArg"), Is.True);
			Assert.That(context.MessageFormatter.PlaceholderValues["MaxLengthArg"], Is.EqualTo(50));
		}

		[Test]
		public void Should_NotAppendArguments_When_TemplateFactoryIsNotSet()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			var options = new TestOptions { CustomMessage = "TestMessage" };
			var validator = builder.Build(options);
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);

			// Assert - No custom arguments should be added when template factory is not set
			Assert.That(context.MessageFormatter.PlaceholderValues.ContainsKey("CustomArg"), Is.False);
		}

		[Test]
		public void Should_JoinErrorMessages_When_NoCustomTemplateFactory()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().WithMessage("Error1"))
				.AddProperty(n => n.Phone)
				.WithValidation((o, v) => v.NotEmpty().WithMessage("Error2")));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "", Phone = "" };

			// Act
			validator.IsValid(context, invalidValue);
			var messageTemplate = validator.GetType()?
				.GetMethod("GetDefaultMessageTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
				.Invoke(validator, [""]) as string;

			// Assert
			Assert.That(messageTemplate, Does.Contain("Error1"));
			Assert.That(messageTemplate, Does.Contain("Error2"));
			Assert.That(messageTemplate, Does.Contain(";"));
		}

		[Test]
		public void Should_HaveCorrectName_Property()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			var validator = builder.Build(new TestOptions());

			// Act
			var name = validator.Name;

			// Assert
			Assert.That(name, Is.EqualTo("DynamicPropertyValidator"));
		}

		[Test]
		public void Should_HandleMultipleValidationRules_InNestedValidator()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().EmailAddress().MaximumLength(50)));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "a".PadRight(100, 'a') + "@test.com" };

			// Act
			var result = validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public void Should_PassValidationContext_ToTemplateFactory()
		{
			// Arrange
			ValidationContext<TestModel> capturedContext = null;
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			builder.WithMessageTemplate((ctx, value, res) =>
			{
				capturedContext = ctx;
				return "Error";
			});

			var testModel = new TestModel { Name = "Test", Age = 25 };
			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(testModel);
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(capturedContext, Is.Not.Null);
			Assert.That(capturedContext.InstanceToValidate, Is.EqualTo(testModel));
		}

		[Test]
		public void Should_PassPropertyValue_ToTemplateFactory()
		{
			// Arrange
			NestedModel capturedValue = null;
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			builder.WithMessageTemplate((ctx, value, res) =>
			{
				capturedValue = value;
				return "Error";
			});

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var testValue = new NestedModel { Email = "", Phone = "123" };

			// Act
			validator.IsValid(context, testValue);

			// Assert
			Assert.That(capturedValue, Is.Not.Null);
			Assert.That(capturedValue, Is.EqualTo(testValue));
			Assert.That(capturedValue.Phone, Is.EqualTo("123"));
		}

		[Test]
		public void Should_PassValidationResult_ToTemplateFactory()
		{
			// Arrange
			ValidationResult capturedResult = null;
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().WithMessage("EmailError"))
				.AddProperty(n => n.Phone)
				.WithValidation((o, v) => v.NotEmpty().WithMessage("PhoneError")));

			builder.WithMessageTemplate((ctx, value, res) =>
			{
				capturedResult = res;
				return "Error";
			});

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "", Phone = "" };

			// Act
			validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(capturedResult, Is.Not.Null);
			Assert.That(capturedResult.IsValid, Is.False);
			Assert.That(capturedResult.Errors.Count, Is.EqualTo(2));
		}

		[Test]
		public void Should_HandleNullValue_InNestedValidator()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotNull()));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());

			// Act
			var result = validator.IsValid(context, null);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public void Should_HandleEmptyConfiguration_InBuilder()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => { }); // Empty configuration

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var value = new NestedModel { Email = "test@example.com" };

			// Act
			var result = validator.IsValid(context, value);

			// Assert
			Assert.That(result, Is.True); // No validators means valid
		}

		[Test]
		public void Should_UseOptionsInNestedValidator()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, string, TestOptions>();
			builder.Configure(b => b
				.AddFunc(s => s, "value")
				.WithValidation((o, v) => v.MaximumLength(10)));

			var options = new TestOptions { MaxLength = 10 };
			var validator = builder.Build(options);
			var context = new ValidationContext<TestModel>(new TestModel());

			// Act
			var resultValid = validator.IsValid(context, "short");
			var resultInvalid = validator.IsValid(context, "this is a very long string");

			// Assert
			Assert.That(resultValid, Is.True);
			Assert.That(resultInvalid, Is.False);
		}

		[Test]
		public void Should_HandleMultipleAppendedArguments()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty()));

			var options = new TestOptions { CustomMessage = "Msg1", MaxLength = 100 };
			builder.WithMessageTemplate((ctx, value, res) => "Error")
				.WithTemplateArgument("Arg1", o => o.CustomMessage)
				.WithTemplateArgument("Arg2", o => o.MaxLength)
				.WithTemplateArgument("Arg3", o => "StaticValue");

			var validator = builder.Build(options);
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "" };

			// Act
			validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(context.MessageFormatter.PlaceholderValues.Count, Is.GreaterThanOrEqualTo(3));
			Assert.That(context.MessageFormatter.PlaceholderValues["Arg1"], Is.EqualTo("Msg1"));
			Assert.That(context.MessageFormatter.PlaceholderValues["Arg2"], Is.EqualTo(100));
			Assert.That(context.MessageFormatter.PlaceholderValues["Arg3"], Is.EqualTo("StaticValue"));
		}

		[Test]
		public void Should_ReturnTrue_When_AllNestedValidationsPass()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().EmailAddress())
				.AddProperty(n => n.Phone)
				.WithValidation((o, v) => v.NotEmpty().Matches(@"^\d{3}-\d{3}-\d{4}$")));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var validValue = new NestedModel { Email = "test@example.com", Phone = "123-456-7890" };

			// Act
			var result = validator.IsValid(context, validValue);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void Should_ReturnFalse_When_AnyNestedValidationFails()
		{
			// Arrange
			var builder = new ConfigurablePropertyValidatorBuilder<TestModel, NestedModel, TestOptions>();
			builder.Configure(b => b
				.AddProperty(n => n.Email)
				.WithValidation((o, v) => v.NotEmpty().EmailAddress())
				.AddProperty(n => n.Phone)
				.WithValidation((o, v) => v.NotEmpty().Matches(@"^\d{3}-\d{3}-\d{4}$")));

			var validator = builder.Build(new TestOptions());
			var context = new ValidationContext<TestModel>(new TestModel());
			var invalidValue = new NestedModel { Email = "test@example.com", Phone = "invalid" };

			// Act
			var result = validator.IsValid(context, invalidValue);

			// Assert
			Assert.That(result, Is.False);
		}

		#region PersonValidator Tests

		[Test]
		public void Should_Pass_When_PersonHasValidCatsCountAndId()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(),
					new Cat(),
					new Cat()
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.Errors, Is.Empty);
		}

		[Test]
		public void Should_Fail_When_CatsCount_ExceedsMaximum()
		{
			// Arrange - CatsCount option is 14, so 14 or more cats should fail
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat() // 14 cats
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.GreaterThan(0));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Cats"));
		}

		[Test]
		public void Should_Fail_When_CatsCount_IsBelowMinimum()
		{
			// Arrange - CatsMinimum option is 1, so 0 cats should fail
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>(), // Empty list
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.GreaterThan(0));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Cats"));
		}

		[Test]
		public void Should_Pass_When_CatsCount_IsAtMinimumBoundary()
		{
			// Arrange - CatsMinimum is 1, so exactly 1 cat should pass
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_Pass_When_CatsCount_IsJustBelowMaximum()
		{
			// Arrange - CatsCount is 14, so 13 cats should pass
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat() // 13 cats
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_HaveCorrectErrorMessage_When_CatsCount_IsInvalid()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>(), // 0 cats - below minimum
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var catsError = result.Errors[0];
			Assert.That(catsError.PropertyName, Is.EqualTo("Cats"));
			Assert.That(catsError.ErrorMessage, Does.Contain("must contain fewer than 14 items"));
			Assert.That(catsError.ErrorMessage, Does.Contain("greater than or equal 1 items"));
		}

		[Test]
		public void Should_IncludeTemplateArguments_InErrorMessage()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat() // 15 cats - exceeds maximum
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var catsError = result.Errors[0];
			Assert.That(catsError.PropertyName, Is.EqualTo("Cats"));
			Assert.That(catsError.ErrorMessage, Does.Contain("14")); // MaxElements
			Assert.That(catsError.ErrorMessage, Does.Contain("1"));  // MinElements
		}

		[Test]
		public void Should_Fail_When_Id_IsGreaterThanOne()
		{
			// Arrange - Id must be less than 1
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = 2
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var idError = System.Linq.Enumerable.FirstOrDefault(result.Errors, e => e.PropertyName == "Id");
			Assert.That(idError, Is.Not.Null);
		}

		[Test]
		public void Should_Fail_When_Id_IsEqualToOne()
		{
			// Arrange - Id must be less than 1 (not equal)
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = 1
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var idError = System.Linq.Enumerable.FirstOrDefault(result.Errors, e => e.PropertyName == "Id");
			Assert.That(idError, Is.Not.Null);
		}

		[Test]
		public void Should_Pass_When_Id_IsNegative()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = -5
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_Fail_When_Id_IsDefaultValue()
		{
			// Arrange - Default Id is 20, which fails validation
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() }
				// Id = 20 (default value)
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var idError = System.Linq.Enumerable.FirstOrDefault(result.Errors, e => e.PropertyName == "Id");
			Assert.That(idError, Is.Not.Null);
		}

		[Test]
		public void Should_Fail_When_BothCatsAndId_AreInvalid()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>(), // 0 cats - invalid
				Id = 20 // Greater than 1 - invalid
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Cats"), Is.True);
			Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Id"), Is.True);
		}

		[Test]
		public void Should_Fail_When_OnlyCats_IsInvalid()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>(), // 0 cats - invalid
				Id = 0 // Valid
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Cats"));
		}

		[Test]
		public void Should_Fail_When_OnlyId_IsInvalid()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat(), new Cat() }, // Valid
				Id = 10 // Invalid
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Id"));
		}

		[Test]
		public void Should_HandleNull_CatsCollection()
		{
			// Arrange
			var person = new Person
			{
				Cats = null,
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert - Null collection should fail validation
			Assert.That(result.IsValid, Is.False);
		}

		[Test]
		public void Should_ValidateMultipleTimes_WithDifferentInstances()
		{
			// Arrange
			var person1 = new Person { Cats = new System.Collections.Generic.List<Cat> { new Cat() }, Id = 0 };
			var person2 = new Person { Cats = new System.Collections.Generic.List<Cat>(), Id = 10 };
			var validator = new PersonValidator();

			// Act
			var result1 = validator.Validate(person1);
			var result2 = validator.Validate(person2);

			// Assert
			Assert.That(result1.IsValid, Is.True);
			Assert.That(result2.IsValid, Is.False);
			Assert.That(result2.Errors.Count, Is.EqualTo(2));
		}

		[Test]
		[TestCase(1, true)]   // Minimum valid
		[TestCase(2, true)]
		[TestCase(5, true)]
		[TestCase(10, true)]
		[TestCase(13, true)]  // Maximum valid (less than 14)
		[TestCase(0, false)]  // Below minimum
		[TestCase(14, false)] // At maximum (not less than)
		[TestCase(15, false)] // Above maximum
		[TestCase(20, false)]
		public void Should_ValidateCatsCount_WithVariousCounts(int catsCount, bool expectedValid)
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>(),
				Id = 0
			};

			for (int i = 0; i < catsCount; i++)
			{
				person.Cats.Add(new Cat());
			}

			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			if (expectedValid)
			{
				Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Cats"), Is.False,
					$"Expected {catsCount} cats to be valid, but validation failed");
			}
			else
			{
				Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Cats"), Is.True,
					$"Expected {catsCount} cats to be invalid, but validation passed");
			}
		}

		[Test]
		[TestCase(-10, true)]
		[TestCase(-1, true)]
		[TestCase(0, true)]
		[TestCase(1, false)]  // Equal to boundary
		[TestCase(2, false)]
		[TestCase(10, false)]
		[TestCase(20, false)]
		public void Should_ValidateId_WithVariousValues(int id, bool expectedValid)
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = id
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			if (expectedValid)
			{
				Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Id"), Is.False,
					$"Expected Id={id} to be valid, but validation failed");
			}
			else
			{
				Assert.That(System.Linq.Enumerable.Any(result.Errors, e => e.PropertyName == "Id"), Is.True,
					$"Expected Id={id} to be invalid, but validation passed");
			}
		}

		[Test]
		public void Should_UseSetExpressValidator_ForCatsValidation()
		{
			// Arrange - Verify that SetExpressValidator is working
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat() // 15 cats
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var error = System.Linq.Enumerable.FirstOrDefault(result.Errors, e => e.PropertyName == "Cats");
			Assert.That(error, Is.Not.Null);
			// Verify custom message template is used
			Assert.That(error.ErrorMessage, Does.Contain("Cats"));
		}

		[Test]
		public void Should_UseSetExpressValidator_ForIdValidation()
		{
			// Arrange - Verify that SetExpressValidator is working for Id
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() },
				Id = 5
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.False);
			var error = System.Linq.Enumerable.FirstOrDefault(result.Errors, e => e.PropertyName == "Id");
			Assert.That(error, Is.Not.Null);
		}

		[Test]
		public void Should_ValidateCatsCount_Property()
		{
			// Arrange - Testing that Count property is being validated
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat(), new Cat() }, // Count = 2
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_ValidatePerson_WithMinimumValidCats()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat> { new Cat() }, // Exactly 1 cat (minimum)
				Id = -1
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.Errors, Is.Empty);
		}

		[Test]
		public void Should_ValidatePerson_WithMaximumValidCats()
		{
			// Arrange
			var person = new Person
			{
				Cats = new System.Collections.Generic.List<Cat>
				{
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat(), new Cat(), new Cat(),
					new Cat(), new Cat(), new Cat() // 13 cats (maximum valid)
				},
				Id = 0
			};
			var validator = new PersonValidator();

			// Act
			var result = validator.Validate(person);

			// Assert
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.Errors, Is.Empty);
		}

		#endregion
	}
}

using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator.Tests
{
	internal class UtilitiesTests
	{
		private class TestClass
		{
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable S1144 // Unused private types or members should be removed

			public string Name { get; set; }

			public string this[int index] => $"Value at {index}";

			public string this[string key, int index] => $"Value at {key}, {index}";
#pragma warning restore S3459 // Unassigned members should be removed
#pragma warning restore S1144 // Unused private types or members should be removed

			public string Method() => "Method Result";
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionIsNull()
		{
			Expression<Func<TestClass, string>> expression = null;

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.False);
			Assert.That(parameters, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionIsPropertyAccess()
		{
			Expression<Func<TestClass, string>> expression = x => x.Name;

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.False);
			Assert.That(parameters, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionIsMethodCall()
		{
			Expression<Func<TestClass, string>> expression = x => x.Method();

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.False);
			Assert.That(parameters, Is.Null);
		}

		[Test]
		public void Should_ReturnTrueAndCorrectParameters_WhenExpressionIsSingleParameterIndexer()
		{
			Expression<Func<TestClass, string>> expression = x => x[0];

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.True);
			Assert.That(parameters, Is.Not.Null);
			Assert.That(parameters.Length, Is.EqualTo(1));
			Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
			Assert.That(parameters[0].Name, Is.EqualTo("index"));
		}

		[Test]
		public void Should_ReturnTrueAndCorrectParameters_WhenExpressionIsMultiParameterIndexer()
		{
			Expression<Func<TestClass, string>> expression = x => x["key", 1];

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.True);
			Assert.That(parameters, Is.Not.Null);
			Assert.That(parameters.Length, Is.EqualTo(2));
			Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
			Assert.That(parameters[0].Name, Is.EqualTo("key"));
			Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(int)));
			Assert.That(parameters[1].Name, Is.EqualTo("index"));
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsNotMethodCall()
		{
			Expression<Func<TestClass, int>> expression = _ => 5; // Constant expression

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.False);
			Assert.That(parameters, Is.Null);
		}

		[Test]
		public void Should_HandleStringIndexer()
		{
			Expression<Func<string, char>> expression = s => s[0]; // String indexer access

			var result = MemberInfoParser.TryParseMethodCallExpression(expression, out ParameterInfo[] parameters);

			Assert.That(result, Is.True);
			Assert.That(parameters, Is.Not.Null);
			Assert.That(parameters.Length, Is.EqualTo(1));
			Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
		}

		[Test]
		public void Should_PropertyInfoParser_TryParse_Work_Correctly()
		{
			var resParseS = MemberInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s.S, MemberTypes.Property, out MemberInfo propertyInfoS);
			Assert.That(resParseS, Is.True);
			Assert.That(propertyInfoS.Name, Is.EqualTo("S"));

			var resParseI = MemberInfoParser.TryParse<ObjWithTwoPublicProps, int>(s => s.I, MemberTypes.Property, out MemberInfo propertyInfoI);
			Assert.That(resParseI, Is.True);
			Assert.That(propertyInfoI.Name, Is.EqualTo("I"));

			var resParse = MemberInfoParser.TryParse<ObjWithTwoPublicProps, ObjWithTwoPublicProps>(s => s, MemberTypes.Property, out MemberInfo propertyInfo);
			Assert.That(resParse, Is.False);
		}

		[Test]
		public void Should_PropertyInfoParser_TryParse_For_MemberInfo_Work_Correctly()
		{
			var resParseS = MemberInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s.S, MemberTypes.Property, out MemberInfo memberInfoS);
			Assert.That(resParseS, Is.True);
			Assert.That(memberInfoS.Name, Is.EqualTo("S"));
			Assert.That(memberInfoS.MemberType, Is.EqualTo(MemberTypes.Property));

			var resParseI = MemberInfoParser.TryParse<ObjWithTwoPublicProps, int>(s => s.I, MemberTypes.Property, out MemberInfo memberInfoI);
			Assert.That(resParseI, Is.True);
			Assert.That(memberInfoI.Name, Is.EqualTo("I"));
			Assert.That(memberInfoS.MemberType, Is.EqualTo(MemberTypes.Property));

			var resParseField = MemberInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s._sField, MemberTypes.Field, out MemberInfo memberInfoField);
			Assert.That(resParseField, Is.True);
			Assert.That(memberInfoField.Name, Is.EqualTo("_sField"));
			Assert.That(memberInfoField.MemberType, Is.EqualTo(MemberTypes.Field));
		}

		[Test]
		public void Should_GetTypedValue_Work()
		{
			var objToTest = new ObjWithTwoPublicProps() { I = 1, S = "TestProp", _sField = "TestField" };

			_ = MemberInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s.S, MemberTypes.Property, out MemberInfo memberInfoS);
			Assert.That(memberInfoS.GetTypedValue<ObjWithTwoPublicProps, string>(objToTest), Is.EqualTo("TestProp"));

			_ = MemberInfoParser.TryParse<ObjWithTwoPublicProps, int>(s => s.I, MemberTypes.Property, out MemberInfo memberInfoI);
			Assert.That(memberInfoI.GetTypedValue<ObjWithTwoPublicProps, int>(objToTest), Is.EqualTo(1));

			_ = MemberInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s._sField, MemberTypes.Field, out MemberInfo memberInfoF);
			Assert.That(memberInfoF.GetTypedValue<ObjWithTwoPublicProps, string>(objToTest), Is.EqualTo("TestField"));
		}

		[Test]
		public void Should_ReturnFalse_ForNonNullableValueType_WhenCheckingIsValueNull()
		{
			Assert.That(TypeTraits<int>.CanBeNull, Is.False);
		}

		[Test]
		public void Should_ReturnTrue_ForNullableValueType_WhenCheckingIsValueNull_AndValueIsNull()
		{
			Assert.That(TypeTraits<int?>.CanBeNull, Is.True);
		}

		[Test]
		public void Should_ReturnTrue_ForReferenceType_WhenCheckingIsValueNull_AndValueIsNull()
		{
			Assert.That(TypeTraits<string>.CanBeNull, Is.True);
		}

		[Test]
		public void Should_TypeHelper_ReturnFalse_ForNonNullableValueType_WhenCheckingIsValueNull()
		{
			Assert.That(TypeHelper<int>.IsNull(0), Is.False);
		}

		[Test]
		public void Should_TypeHelper_ReturnTrue_ForNullableValueType_WhenCheckingIsValueNull_AndValueIsNull()
		{
			int? value = null;
			Assert.That(TypeHelper<int?>.IsNull(value), Is.True);
		}

		[Test]
		public void Should_TypeHelper_ReturnFalse_ForNullableValueType_WhenCheckingIsValueNull_AndValueIsNotNull()
		{
			int? value = 5;
			Assert.That(TypeHelper<int?>.IsNull(value), Is.False);
		}

		[Test]
		public void Should_TypeHelper_ReturnTrue_ForReferenceType_WhenCheckingIsValueNull_AndValueIsNull()
		{
			const string value = null;
			Assert.That(TypeHelper<string>.IsNull(value), Is.True);
		}

		[Test]
		public void Should_TypeHelper_ReturnFalse_ForReferenceType_WhenCheckingIsValueNull_AndValueIsNotNull()
		{
			const string value = "hello";
			Assert.That(TypeHelper<string>.IsNull(value), Is.False);
		}

		[Test]
		public void Should_ReturnTrue_WhenExpressionIsPublicField()
		{
			Expression<Func<ClassWithField, string>> getExpression = x => x.PublicField;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("PublicField"));
		}

		[Test]
		public void Should_ReturnTrue_WhenExpressionIsAutoImplementedProperty()
		{
			Expression<Func<ClassWithField, int?>> getExpression = x => x.NullableProperty;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo.Name, Is.EqualTo("NullableProperty"));
		}

		[Test]
		public void Should_ReturnTrue_WhenExpressionIsGetOnlyProperty()
		{
			Expression<Func<ClassWithField, string>> getExpression = x => x.GetOnlyProperty;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo.Name, Is.EqualTo("GetOnlyProperty"));
		}

		[Test]
		public void Should_ReturnTrue_WhenPropertyHasPrivateSetter()
		{
			Expression<Func<ClassWithField, int>> getExpression = x => x.PropertyWithPrivateSetter;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo.Name, Is.EqualTo("PropertyWithPrivateSetter"));
		}

		[Test]
		public void Should_HandleNestedMemberAccessInPropertyExpression()
		{
			Expression<Func<ClassWithField, int>> getExpression = x => x.NullableProperty.Value;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			// NullableProperty.Length accesses Nullable<int>.Length property which is a MethodInfo bound as MemberExpression
			Assert.That(result, Is.True);
			Assert.That(memberInfo.Name, Is.EqualTo("Value"));
		}

		[Test]
		public void Should_ReturnTrue_WhenPropertyIsFromOuterTestClass()
		{
			Expression<Func<TestClass, string>> getExpression = x => x.Name;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo.Name, Is.EqualTo("Name"));
		}

		[Test]
		public void Should_ExtractCorrectDeclaringType_ForMemberAccess()
		{
			Expression<Func<ClassWithField, string>> getExpression = x => x.PublicField;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.True);
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(ClassWithField)));
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsConstant()
		{
			Expression<Func<ClassWithField, int>> getExpression = _ => 42;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsMethodCall()
		{
			Expression<Func<ClassWithField, string>> getExpression = _ => typeof(int).ToString();

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsUnaryConversionOfConstant()
		{
			Expression<Func<ClassWithField, int?>> getExpression = _ => 42;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsArithmeticBinary()
		{
			Expression<Func<ClassWithField, int>> getExpression = _ => 1 + 2;

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsConditional()
		{
			Expression<Func<ClassWithField, object>> getExpression = _ => true ? "yes" : "no";

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		[Test]
		public void Should_ReturnFalse_WhenExpressionBodyIsNewObjectInstance()
		{
			Expression<Func<ClassWithField, DateTime>> getExpression = _ => new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local);

			var result = MemberInfoParser.TryParse(getExpression, out MemberInfo memberInfo);

			Assert.That(result, Is.False);
			Assert.That(memberInfo, Is.Null);
		}

		private class ClassWithField
		{
#pragma warning disable S3459 // Unassigned members should be removed
			public string PublicField;

			public int PropertyWithPrivateSetter { get; }

			public string GetOnlyProperty => "generated";

			public int? NullableProperty { get; }
#pragma warning restore S3459 // Unassigned members should be removed
		}
	}
}

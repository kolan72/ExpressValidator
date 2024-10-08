﻿using NUnit.Framework;
using System.Reflection;

namespace ExpressValidator.Tests
{
	internal class UtilitiesTests
	{
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
	}
}

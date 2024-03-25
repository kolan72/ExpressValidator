using NUnit.Framework;
using System.Reflection;

namespace ExpressValidator.Tests
{
	internal class UtilitiesTests
	{
		[Test]
		public void Should_PropertyInfoParser_TryParse_Work_Correctly()
		{
			var resParseS = PropertyInfoParser.TryParse<ObjWithTwoPublicProps, string>(s => s.S, out PropertyInfo propertyInfoS);
			Assert.That(resParseS, Is.True);
			Assert.That(propertyInfoS.Name, Is.EqualTo("S"));

			var resParseI = PropertyInfoParser.TryParse<ObjWithTwoPublicProps, int>(s => s.I, out PropertyInfo propertyInfoI);
			Assert.That(resParseI, Is.True);
			Assert.That(propertyInfoI.Name, Is.EqualTo("I"));

			var resParse = PropertyInfoParser.TryParse<ObjWithTwoPublicProps, ObjWithTwoPublicProps>(s => s, out PropertyInfo propertyInfo);
			Assert.That(resParse, Is.False);
		}
	}
}

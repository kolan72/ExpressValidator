using BenchmarkDotNet.Attributes;
using ExpressValidator;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Benchmark
{
	[MemoryDiagnoser]
	public class FuncFromPropertyInfoBenchmark
	{
        private readonly ObjWithTwoPublicProps obj = new ObjWithTwoPublicProps() { I = 1};
        private readonly Expression<Func<ObjWithTwoPublicProps, int>> _expression = (o) => o.I;

        [Benchmark]
        public int GetPropByGetPropertyFuncByExpression()
        {
            return PropertyInfoToFuncConverter.GetPropertyFuncByExpression<ObjWithTwoPublicProps, int>(GetMemberInfo())(obj);
        }

        [Benchmark]
        public int GetPropByGetPropertyFuncByReflection()
        {
            return PropertyInfoToFuncConverter.GetPropertyFuncByReflection<ObjWithTwoPublicProps, int>(GetMemberInfo())(obj);
        }

        [Benchmark]
        public int GetPropByGetPropertyFuncByMethodInfo()
        {
            return PropertyInfoToFuncConverter.GetPropertyFuncByMethodInfo<ObjWithTwoPublicProps, int>(GetMemberInfo())(obj);
        }

        private MemberInfo GetMemberInfo()
        {
            return (_expression.Body as MemberExpression)?.Member;
        }
    }

    public class ObjWithTwoPublicProps
    {
        public int I { get; set; }
        public string S { get; set; }
        public int PercentValue1 { get; set; }
        public int PercentValue2 { get; set; }
    }
}

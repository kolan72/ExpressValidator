using System;

namespace ExpressValidator
{
	public static class ApplyFuncs
	{
		public static Action<T2> Apply<T1, T2>(this Action<T1, T2> func, T1 t1)
					=> (t2) => func(t1, t2);
	}
}

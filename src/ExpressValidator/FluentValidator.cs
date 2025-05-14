using FluentValidation;
using System.Collections.Generic;

namespace ExpressValidator
{
	public class FluentValidator<T> : AbstractValidator<T>
	{
		internal FluentValidator(IEnumerable<AbstractValidator<T>> validators)
		{
			foreach (var v in validators)
			{
				Include(v);
			}
		}
	}
}

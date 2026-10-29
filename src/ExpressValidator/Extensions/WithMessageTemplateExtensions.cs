using System;
using System.Linq.Expressions;

namespace ExpressValidator
{
	public static class WithMessageTemplateExtensions
	{
		/// <summary>
		/// Appends a message template argument whose name is inferred from the member accessed in
		/// <paramref name="expression"/>: <c>o => o.CatsCount</c> registers an argument named
		/// <c>"CatsCount"</c> substituting <c>{CatsCount}</c> in the message template.
		/// </summary>
		/// <remarks>
		/// The expression must be a property or field access rooted in the options parameter
		/// (the leaf member name is used, so <c>o => o.Inner.City</c> registers <c>"City"</c>).
		/// Use the <c>WithTemplateArgument</c> overload taking an explicit <c>argName</c> when the
		/// placeholder name must differ from the member name.
		/// </remarks>
		/// <typeparam name="T">A type of validated object.</typeparam>
		/// <typeparam name="TProperty">A type of validated property.</typeparam>
		/// <typeparam name="TOptions">A type of validation options.</typeparam>
		/// <param name="builder">A builder returned by <c>WithMessageTemplate</c> or a previous <c>WithTemplateArgument</c> call.</param>
		/// <param name="expression">An expression accessing a member of <typeparamref name="TOptions"/>.</param>
		/// <returns><see cref="IConfiguredPropertyBuilder{T, TProperty, TOptions}"/></returns>
		public static IConfiguredPropertyBuilder<T, TProperty, TOptions> WithTemplateArgument<T, TProperty, TOptions>(
			this IWithMessageTemplate<T, TProperty, TOptions> builder,
			Expression<Func<TOptions, object>> expression)
		{
			return builder.WithTemplateArgument(MemberInfoParser.ParseArgumentName(expression), expression.Compile());
		}
	}
}
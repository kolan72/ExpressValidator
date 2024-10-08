using FluentValidation.Results;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	/// <summary>
	/// Defines a validator for an object.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	public interface IExpressValidator<TObj>
	{
		/// <summary>
		/// Validates the given object instance.
		/// </summary>
		/// <param name="obj">An object instance to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		ValidationResult Validate(TObj obj);

		/// <summary>
		/// Validates the given object instance asynchronously.
		/// </summary>
		/// <param name="obj">An object instance to validate.</param>
		/// <param name="token">A cancellation token to cancel validation.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		Task<ValidationResult> ValidateAsync(TObj obj, CancellationToken token = default);
	}
}

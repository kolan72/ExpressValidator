using ExpressValidator;
using Microsoft.Extensions.Options;
using Shared;

namespace ValidatorBuilderWithOptions
{
	public interface IGuessTheNumberService
	{
		(bool Result, string Message) ComplexGuess();
	}

	public class GuessTheNumberService : IGuessTheNumberService
	{
		private readonly ValidationParametersOptions _validateOptions;
		private readonly IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> _expressValidatorBuilder;

		private const string WIN_PHRASE = "The rules have changed in the middle of the game, but you still win!";
		private const string LOSE_PHRASE = "Sorry, the rules changed in the middle of the game.";

		public GuessTheNumberService(IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> expressValidatorBuilder,
			IOptions<ValidationParametersOptions> validateOptions)
		{
			_validateOptions = validateOptions.Value;
			_expressValidatorBuilder = expressValidatorBuilder;
		}
		public (bool Result, string Message) ComplexGuess()
		{
			var i = Random.Shared.Next(1, 11);
			var objToValidate = new ObjToValidate() { I = i };

			ChangeValidateOptions();

			var vr = _expressValidatorBuilder.Build(_validateOptions).Validate(objToValidate);
			if (vr.IsValid)
			{
				return (true, WIN_PHRASE + " " +
								$"You guessed {i} and it is correct because it's greater than {_validateOptions.IGreaterThanValue}.");
			}
			else
			{
				return (false, LOSE_PHRASE + " " +
					$"You have chosen {i} and it is wrong. " + vr.ToString());
			}
		}

		private void ChangeValidateOptions()
		{
			_validateOptions.IGreaterThanValue = Random.Shared.Next(2, 10);
		}
	}
}

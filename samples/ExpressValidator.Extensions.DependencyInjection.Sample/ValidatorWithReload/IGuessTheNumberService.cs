using ExpressValidator.Extensions.DependencyInjection;
using Shared;

namespace ValidatorWithReload
{
    public interface IGuessTheNumberService
    {
		(bool Result, string Message) GuessWithReload();
		Task<(bool Result, string Message)> GuessWithReloadAsync();
	}

	public class GuessTheNumberService : IGuessTheNumberService
	{
		private readonly IExpressValidatorWithReload<ObjToValidate> _expressValidatorWithReload;

		public GuessTheNumberService(IExpressValidatorWithReload<ObjToValidate> expressValidatorWithReload)
		{
			_expressValidatorWithReload = expressValidatorWithReload;
		}

		public (bool Result, string Message) GuessWithReload()
		{
			var i = Random.Shared.Next(1, 11);
			var objToValidate = new ObjToValidate() { I = i };
			var vr = _expressValidatorWithReload.Validate(objToValidate);
			if (vr.IsValid)
			{
				return (true, $"You guessed {i} and it is correct!");
			}
			else
			{
				return (false, $"You have chosen {i} and it is wrong. " + vr.ToString());
			}
		}

		public async Task<(bool Result, string Message)> GuessWithReloadAsync()
		{
			var i = Random.Shared.Next(1, 11);
			var objToValidate = new ObjToValidate() { I = i };
			var vr = await _expressValidatorWithReload.ValidateAsync(objToValidate).ConfigureAwait(false);
			if (vr.IsValid)
			{
				return (true, $"You guessed {i} and it is correct!");
			}
			else
			{
				return (false, $"You have chosen {i} and it is wrong. " + vr.ToString());
			}
		}
	}
}

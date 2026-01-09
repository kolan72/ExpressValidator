using ExpressValidator;
using Shared;

namespace QuickStart
{
	public interface IGuessTheNumberService
	{
		(bool Result, string Message) Guess();
	}

	public class GuessTheNumberService : IGuessTheNumberService
	{
		private readonly IExpressValidator<ObjToValidate> _expressValidator;

		public GuessTheNumberService(IExpressValidator<ObjToValidate> expressValidator)
		{
			_expressValidator = expressValidator;
		}

		public (bool Result, string Message) Guess()
		{
			var i = Random.Shared.Next(1, 11);
			var objToValidate = new ObjToValidate() { I = i };
			var vr = _expressValidator.Validate(objToValidate);
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
